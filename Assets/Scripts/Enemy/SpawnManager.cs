using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SpawnManager : Singleton<SpawnManager>
{
	#region 내부 프로퍼티
	#region 매니저
	private EnemyManager M_Enemy => EnemyManager.Instance;
	#endregion
	#endregion

	#region 외부 함수
	public void Start_BattleStage(in List<StageEnemy_TableExcel> stageEnemyData, bool isBoss)
	{
		for (int i = 0; i < stageEnemyData.Count; i++)
		{
			E_Direction dir = (E_Direction)stageEnemyData[i].SponPosition - 1;

			StartCoroutine(Spawn(dir, stageEnemyData[i], i == stageEnemyData.Count - 1, isBoss));
		}
	}
	#endregion

	#region 코루틴
	IEnumerator Spawn(E_Direction dir, SpawnData data, bool isLast, bool isBoss)
	{
		if (data.AppearSpeed > 0)
			yield return new WaitForSeconds(data.AppearSpeed);

		int code = data.Enemy_Code;
		Enemy enemy;

		for (int i = 0; i < data.Create_num - 1; ++i)
		{
			enemy = M_Enemy.SpawnEnemy(dir, code);
			enemy.gameObject.SetActive(true);

			yield return new WaitForSeconds(data.CreateSpeed);
		}

		enemy = M_Enemy.SpawnEnemy(dir, code);
		enemy.gameObject.SetActive(true);

		if (isLast && !isBoss)
			StageInfoManager.Instance.ChangeSkipButtonActive(true);
	}
	#endregion

	[Serializable]
	public struct SpawnData
	{
		public int SpawnPosition;
		public int Create_num;
		public int Enemy_Code;
		public float AppearSpeed;
		public float CreateSpeed;

		public static implicit operator StageEnemy_TableExcel(SpawnData data)
		{
			StageEnemy_TableExcel stageEnemy = new StageEnemy_TableExcel();
			stageEnemy.SponPosition = data.SpawnPosition;
			stageEnemy.Create_num = data.Create_num;
			stageEnemy.Emeny_Code = data.Enemy_Code;
			stageEnemy.AppearSpeed = data.AppearSpeed;
			stageEnemy.CreateSpeed = data.CreateSpeed;
			return stageEnemy;
		}
		public static implicit operator SpawnData(StageEnemy_TableExcel stageEnemy)
		{
			SpawnData data = new SpawnData();
			data.SpawnPosition = stageEnemy.SponPosition;
			data.Create_num = stageEnemy.Create_num;
			data.Enemy_Code = stageEnemy.Emeny_Code;
			data.AppearSpeed = stageEnemy.AppearSpeed;
			data.CreateSpeed = stageEnemy.CreateSpeed;
			return data;
		}
	}
}
