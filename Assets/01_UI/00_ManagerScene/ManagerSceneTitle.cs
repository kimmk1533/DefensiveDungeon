using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ManagerSceneTitle : MonoBehaviour
{
	public void End()
	{
		MainSceneLoader.Instance.End();
	}
}
