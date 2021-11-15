using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingTextPool : ObjectPool<FloatingTextPool, FloatingText>
{
	#region ���� �Լ�
	public override void __Initialize()
	{
		base.__Initialize();

		FloatingText origin = transform.Find("DamageText").GetComponent<FloatingText>();

		AddPool("DamageText", origin, transform);

		origin.gameObject.SetActive(false);
	}
	#endregion
}
