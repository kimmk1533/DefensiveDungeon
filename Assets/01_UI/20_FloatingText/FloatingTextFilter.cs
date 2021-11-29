using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public struct FloatingTextFilter
{
	E_PostionType m_PositionType;
	Vector3 m_Position;
	float m_Time;
	Color m_Color;
	Color32 m_OutlineColor;
	float m_OutlineWidth;
	Vector2 m_SizeDelta;
	Vector3 m_Scale;

	public E_PostionType postionType { get => m_PositionType; set => m_PositionType = value; }
	public Vector2 sizeDelta { get => m_SizeDelta; set => m_SizeDelta = value; }
	public Vector3 scale { get => m_Scale; set => m_Scale = value; }
	public Vector3 position { get => m_Position; set => m_Position = value; }
	public float time { get => m_Time; set => m_Time = value; }
	public Color color { get => m_Color; set => m_Color = value; }
	public Color outlineColor { get => m_OutlineColor; set => m_OutlineColor = value; }
	public float outlineWidth { get => m_OutlineWidth; set => m_OutlineWidth = value; }

	public enum E_PostionType
	{
		World,
		WorldToScreen,
		ViewToScreen,
	}
}
