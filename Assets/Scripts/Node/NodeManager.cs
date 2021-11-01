using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeManager : Singleton<NodeManager>
{
	public delegate void NodeEventHandler();
	// 노드 회전 종료시 호출되는 이벤트
	public event NodeEventHandler m_RotateEndEvent;

	// 회전할 각도
	[SerializeField]
	protected float m_RotateAngle;
	// 회전에 걸리는 시간
	[SerializeField]
	protected float m_Duration;
	// 회전의 중심점
	[SerializeField, ReadOnly]
	protected Transform m_Center;

	// [타입(안, 밖)][방향(북, 동, 남, 서)]
	// 회전시킬 노드 리스트
	protected Dictionary<E_NodeType, Dictionary<E_Direction, List<Node>>> m_NodeList;
	// 노드 부모 (동서남북)
	protected Dictionary<E_NodeType, Dictionary<E_Direction, Transform>> m_NodeParentList;

	// 선택 노드
	protected Node m_SelectedNode;
	// 회전 기준 노드 (회전을 시작할 때 선택한 노드)
	protected Node m_RotationStandardNode;
	// 회전 여부
	[SerializeField]
	protected bool m_IsRotating;
	// 타워 바라볼 방향 회전용 오브젝트
	protected GameObject m_LookingDir;
	// 메인 카메라
	protected Camera m_Camera;

	#region 내부 프로퍼티
	// 선택 노드의 타입
	protected E_NodeType SelectedNodeType => m_SelectedNode.m_NodeType;
	protected E_Direction SelectedNodeDir => m_SelectedNode.m_Direction;
	protected E_NodeType StandardNodeType => m_RotationStandardNode.m_NodeType;
	// 현재 카메라
	protected Camera MainCamera
	{
		get
		{
			if (null == m_Camera)
			{
				m_Camera = Camera.main;
			}

			return m_Camera;
		}
	}
	#endregion

	private void Awake()
	{
		Initialize();
	}

	protected void Initialize()
	{
		// 회전 중심점 설정
		m_Center = transform.Find("Center");
		if (null == m_Center)
		{
			GameObject center = new GameObject("Center");
			center.transform.SetParent(transform);
			m_Center = center.transform;
		}

		// 초기화
		m_NodeList = new Dictionary<E_NodeType, Dictionary<E_Direction, List<Node>>>();//new List<Node>[(int)E_NodeType.Max][];
		m_NodeParentList = new Dictionary<E_NodeType, Dictionary<E_Direction, Transform>>();//new Transform[(int)E_NodeType.Max][];

		// 타입별 (안, 밖)
		for (E_NodeType i = 0; i < E_NodeType.Max; ++i)
		{
			m_NodeList[i] = new Dictionary<E_Direction, List<Node>>();
			m_NodeParentList[i] = new Dictionary<E_Direction, Transform>();

			// 방향별
			for (E_Direction j = 0; j < E_Direction.Max; ++j)
			{
				m_NodeList[i][j] = new List<Node>();
				m_NodeParentList[i][j] = transform.Find(i.ToString()).Find(j.ToString());

				m_NodeParentList[i][j].GetComponentsInChildren<Node>(m_NodeList[i][j]);

				foreach (var item in m_NodeList[i][j])
				{
					item.m_NodeType = i;
					item.m_Direction = j;
					item.Initialize();
				}
			}
		}

		m_LookingDir = new GameObject();
		m_LookingDir.transform.SetParent(transform);
		m_LookingDir.SetActive(false);
	}

	private void Update()
	{
		// 선택 노드 검색
		MouseProcess();

		// 회전
		RotateProcess();
	}

	// 선택 노드 검색
	protected void MouseProcess()
	{
		// 마우스 포인터가 UI위에 없을 때
		if (false == UnityEngine.EventSystems.EventSystem.current?.IsPointerOverGameObject())
		{
			// 좌클릭 시
			if (Input.GetMouseButtonDown(0))
			{
				// 클릭한 곳으로 레이캐스트
				Ray ray = MainCamera.ScreenPointToRay(Input.mousePosition);
				float maxDistance = MainCamera.farClipPlane;
				int layerMask = LayerMask.GetMask("Node");

				RaycastHit hit;

				// 레이캐스트 성공 시
				if (Physics.Raycast(ray, out hit, maxDistance, layerMask))
				{
					// 선택 노드 설정
					SelectNode(hit.transform.GetComponent<Node>());
				}
				// 레이캐스트 실패 시
				else
				{
					// 선택 노드 제거
					SelectNode(null);
				}
			}
		}
	}
	#region MouseProcess
	// 아웃라인 업데이트
	protected void UpdateOutline(bool active)
	{
		// 예외 처리 (선택한 노드가 없을 때)
		if (null == m_SelectedNode)
			return;

		// 아웃라인 설정할 노드 리스트
		Dictionary<E_Direction, List<Node>> nodes = m_NodeList[SelectedNodeType];

		// 아웃라인 on, off 설정
		for (E_Direction i = E_Direction.None + 1; i < E_Direction.Max; ++i)
		{
			for (int j = 0; j < nodes[i].Count; ++j)
			{
				nodes[i][j].Outline.SetActive(active);
			}
		}
	}
	// 노드 선택
	protected void SelectNode(Node node)
	{
		// 예외 처리 (마왕 노드 선택)
		if (null != node?.m_Devil)
			return;

		// 선택 노드 아웃라인 제거
		UpdateOutline(false);

		// 선택 노드 변경
		m_SelectedNode = node;

		// 선택 노드 아웃라인 생성
		UpdateOutline(true);
	}
	#endregion

	// 회전
	protected void RotateProcess()
	{
		// 마우스 회전 검사
		Rotate_Mouse();
		// 키보드 회전 검사
		Rotate_Keyboard();
	}
	#region RotateProcess
	// 마우스 회전
	protected void Rotate_Mouse()
	{
		// 예외 처리 (이미 회전 중인 경우)
		if (m_IsRotating)
			return;

		// 예외 처리 (선택한 노드가 없을 때)
		if (null == m_SelectedNode)
			return;

		// 마우스 포인터가 UI위에 없을 때
		if (false == UnityEngine.EventSystems.EventSystem.current?.IsPointerOverGameObject())
		{
			// 좌클릭시
			if (Input.GetMouseButtonUp(0))
			{
				// 클릭한 곳으로 레이캐스트
				Ray ray = MainCamera.ScreenPointToRay(Input.mousePosition);
				float maxDistance = MainCamera.farClipPlane;
				int layerMask = LayerMask.GetMask("Node", "NodeRotate");

				RaycastHit hit;

				// 레이캐스트 성공 시
				if (Physics.Raycast(ray, out hit, maxDistance, layerMask))
				{
					// 방향 계산
					E_Direction from = SelectedNodeDir;
					E_Direction to = (E_Direction)Enum.Parse(typeof(E_Direction), hit.transform.parent.name);

					// 시계 방향 회전
					if ((int)to == (int)(from + 1) % (int)E_Direction.Max)
					{
						CWRotate();
					}
					// 반시계 방향 회전
					else if ((int)to == (int)(from + (int)E_Direction.Max - 1) % (int)E_Direction.Max)
					{
						CCWRotate();
					}
				}
				// 레이캐스트 실패 시
				else
				{
					// 선택 노드 제거
					SelectNode(null);
				}
			}
		}
	}
	// 키보드 회전
	protected void Rotate_Keyboard()
	{
		// 예외 처리 (이미 회전 중인 경우)
		if (m_IsRotating)
			return;

		// 예외 처리 (선택한 노드가 없을 때)
		if (null == m_SelectedNode)
			return;

		// Q키를 누른 경우
		if (Input.GetKeyDown(KeyCode.Q))
		{
			// 반시계 방향 회전
			CCWRotate();
			return;
		}

		// E키를 누른 경우
		if (Input.GetKeyDown(KeyCode.E))
		{
			// 시계 방향 회전
			CWRotate();
		}
	}
	// 매 프레임 회전
	// 참고 출처: http://devkorea.co.kr/bbs/board.php?bo_table=m03_qna&wr_id=95809
	protected IEnumerator RotateNode(bool clockwise = true)
	{
		// 회전 여부 설정
		m_IsRotating = true;
		// 회전 기준 노드 설정
		m_RotationStandardNode = m_SelectedNode;

		// 방향별 노드 부모
		Dictionary<E_Direction, Transform> node_parent_by_dir = m_NodeParentList[StandardNodeType];

		// 총 경과한 시간
		float time = 0f;
		// 회전할 방향
		int Dir = clockwise ? 1 : -1;
		// 회전할 각도
		float angle;

		// 타워 타겟 업데이트
		UpdateTowerTarget();

		// 예외 처리 (회전에 걸리는 시간이 0이하인 경우)
		if (m_Duration <= 0f)
		{
			// 회전할 각도 계산
			angle = Dir * m_RotateAngle;
			// 회전할 각도만큼 즉시 회전
			for (E_Direction i = E_Direction.None + 1; i < E_Direction.Max; ++i)
			{
				for (int j = 0; j < node_parent_by_dir[i].childCount; ++j)
				{
					node_parent_by_dir[i].GetChild(j).RotateAround(m_Center.transform.position, Vector3.up, angle);
				}
			}
			// 타워가 바라볼 방향도 회전
			UpdateTowerLookingDir(angle);

			// 노드 업데이트
			UpdateNode(clockwise);

			// 타워 공격 여부 설정
			UpdateTowerAttack(true);

			// 회전 여부 설정
			m_IsRotating = false;

			// 회전 종료 이벤트 호출
			m_RotateEndEvent?.Invoke();

			// 회전 종료
			yield break;
		}

		// 타워 공격 여부 설정
		UpdateTowerAttack(false);

		// 위로 이동
		for (E_Direction i = E_Direction.None + 1; i < E_Direction.Max; ++i)
		{
			for (int j = 0; j < node_parent_by_dir[i].childCount; ++j)
			{
				node_parent_by_dir[i].GetChild(j).Translate(Vector3.up * 3f);
			}
		}

		// 정해진 시간동안 회전
		while (time < m_Duration)
		{
			// 회전할 각도 계산
			angle = Dir * m_RotateAngle * Time.deltaTime / m_Duration;
			// 회전
			for (E_Direction i = E_Direction.None + 1; i < E_Direction.Max; ++i)
			{
				for (int j = 0; j < node_parent_by_dir[i].childCount; ++j)
				{
					node_parent_by_dir[i].GetChild(j).RotateAround(m_Center.transform.position, Vector3.up, angle);
				}
			}
			// 타워가 바라볼 방향도 회전
			UpdateTowerLookingDir(angle);

			// 총 경과 시간 증가
			time += Time.deltaTime;

			// 다음 프레임까지 대기
			yield return null;
		}

		// 회전할 각도 계산
		angle = Dir * m_RotateAngle * (m_Duration - time) / m_Duration;
		// 오차 범위 수정 (원하는 시간 보다 +된 만큼 반대방향으로 회전)
		for (E_Direction i = E_Direction.None + 1; i < E_Direction.Max; ++i)
		{
			for (int j = 0; j < node_parent_by_dir[i].childCount; ++j)
			{
				node_parent_by_dir[i].GetChild(j).RotateAround(m_Center.transform.position, Vector3.up, angle);
			}
		}
		// 타워가 바라볼 방향도 오차 범위 수정
		UpdateTowerLookingDir(angle);

		// 아래로 이동
		for (E_Direction i = E_Direction.None + 1; i < E_Direction.Max; ++i)
		{
			for (int j = 0; j < node_parent_by_dir[i].childCount; ++j)
			{
				node_parent_by_dir[i].GetChild(j).Translate(Vector3.down * 3f);
			}
		}

		#region 인스펙터 정리용
#if UNITY_EDITOR
		for (E_Direction i = E_Direction.None + 1; i < E_Direction.Max; ++i)
		{
			for (int j = 0; j < node_parent_by_dir[i].childCount; ++j)
			{
				Vector3 pos = node_parent_by_dir[i].GetChild(j).position;

				pos.x = Mathf.Round(pos.x * 10000f) * 0.0001f;
				pos.y = Mathf.Round(pos.y * 10000f) * 0.0001f;
				pos.z = Mathf.Round(pos.z * 10000f) * 0.0001f;

				node_parent_by_dir[i].GetChild(j).position = pos;
			}
		}
#endif
		#endregion

		// 노드 업데이트
		UpdateNode(clockwise);

		// 타워 공격 여부 설정
		UpdateTowerAttack(true);

		// 회전 여부 설정
		m_IsRotating = false;

		// 회전 종료 이벤트 호출
		m_RotateEndEvent?.Invoke();
	}
	// 노드 업데이트
	protected void UpdateNode(bool clockwise = true)
	{
		// 시계 방향
		if (clockwise)
		{
			SwapList(E_Direction.North, E_Direction.East);
			SwapList(E_Direction.South, E_Direction.West);
			SwapList(E_Direction.North, E_Direction.South);
		}
		// 반시계 방향
		else
		{
			SwapList(E_Direction.North, E_Direction.West);
			SwapList(E_Direction.South, E_Direction.East);
			SwapList(E_Direction.North, E_Direction.South);
		}
	}
	// 리스트 교환
	protected void SwapList(E_Direction first, E_Direction second)
	{
		List<Node> FirstList = m_NodeList[StandardNodeType][first];
		List<Node> SecondList = m_NodeList[StandardNodeType][second];
		List<Node> TempList = new List<Node>(FirstList);

		Transform First_T = m_NodeParentList[StandardNodeType][first];
		Transform Second_T = m_NodeParentList[StandardNodeType][second];
		List<Transform> Temp_T = new List<Transform>();

		int FirstCount = First_T.childCount;
		int SecondCount = Second_T.childCount;

		// 1 -> Temp
		for (int i = 0; i < FirstCount; ++i)
		{
			Temp_T.Add(First_T.GetChild(i));
		}
		// 2 -> 1
		FirstList.Clear();
		FirstList.AddRange(SecondList);
		for (int i = 0; i < SecondCount; ++i)
		{
			Second_T.GetChild(0).SetParent(First_T);
		}
		// Temp -> 2
		SecondList.Clear();
		SecondList.AddRange(TempList);
		for (int i = 0; i < FirstCount; ++i)
		{
			Temp_T[i].SetParent(Second_T);
		}

		// 정보 업데이트
		foreach (var item in FirstList)
		{
			// 노드 방향 업데이트
			item.m_Direction = first;

			// 타워 방향 업데이트
			if (null != item.m_Tower)
				item.m_Tower.Direction = first;
		}
		foreach (var item in SecondList)
		{
			// 노드 방향 업데이트
			item.m_Direction = second;

			// 타워 방향 업데이트
			if (null != item.m_Tower)
				item.m_Tower.Direction = second;
		}
	}
	// 타워 바라볼 방향 업데이트
	protected void UpdateTowerLookingDir(float angle)
	{
		// 타워 바라볼 방향 업데이트
		for (E_Direction i = 0; i < E_Direction.Max; ++i)
		{
			foreach (var item in m_NodeList[StandardNodeType][i])
			{
				if (null != item.m_Tower)
				{
					m_LookingDir.transform.position = item.m_Tower.m_TowerInfo.LookingDir;
					m_LookingDir.transform.RotateAround(m_Center.transform.position, Vector3.up, angle);
					item.m_Tower.m_TowerInfo.LookingDir = m_LookingDir.transform.position;
				}
			}
		}
	}
	// 타워 타겟 업데이트
	protected void UpdateTowerTarget()
	{
		for (E_Direction i = 0; i < E_Direction.Max; ++i)
		{
			foreach (var item in m_NodeList[StandardNodeType][i])
			{
				if (null != item.m_Tower)
				{
					item.m_Tower.m_Target = null;
					item.m_Tower.m_TowerInfo.ShouldFindTarget = true;
				}
			}
		}
	}
	// 타워 공격 여부 업데이트
	protected void UpdateTowerAttack(bool flag)
	{
		// 타워 공격 여부 업데이트
		for (E_Direction i = 0; i < E_Direction.Max; ++i)
		{
			foreach (var item in m_NodeList[StandardNodeType][i])
			{
				if (null != item.m_Tower)
				{
					item.m_Tower.CanAttack = flag;
				}
			}
		}
	}
	#endregion

	public List<Node> GetNodeList(E_Direction dir)
	{
		List<Node> result = new List<Node>();
		result.AddRange(m_NodeList[E_NodeType.Inner][dir]);
		result.AddRange(m_NodeList[E_NodeType.Outer][dir]);
		return result;
	}

	// 시계 방향 회전
	protected void CWRotate()
	{
		StartCoroutine(RotateNode());
	}
	// 반시계 방향 회전
	protected void CCWRotate()
	{
		StartCoroutine(RotateNode(false));
	}
}

public enum E_NodeType
{
	None = -1,

	Inner,
	Outer,

	Max
}