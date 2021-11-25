using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum CanvasType
{
	Start,
	Option
}
public enum OptionType
{
	KeyButton,
	SoundButton,
	VideoButton,
}
public enum KeyOptionType
{
	None = -1,

	Qkey,
	Ekey,
}
public class Option : MonoBehaviour
{
	//ĵ����
	public List<GameObject> Canvas;
	//�ɼǹ�ư
	public List<Button> buttons;
	//�ɼǱ׷�
	public List<GameObject> options;
	//������ȯ Ű����
	public List<Button> KeySetting;
	public List<Text> texts;
	ColorBlock color;
	ColorBlock origincolor;
	KeyOptionType keyOptionType = KeyOptionType.None;
	
	private void Awake()
	{
		color = origincolor = buttons[1].colors;
		//��ư Ŭ��(Ȱ��ȭ)�� �� ����
		color.normalColor = new Color(132f, 132f, 132f, 71f) / 255f;

		buttons[0].colors = color;
		Canvas[(int)CanvasType.Option].gameObject.SetActive(false);
		options[(int)OptionType.SoundButton].gameObject.SetActive(false);
		options[(int)OptionType.VideoButton].gameObject.SetActive(false);
	}
	public void EnableStartCanvas()
	{
		Canvas[(int)CanvasType.Start].gameObject.SetActive(false);
		Canvas[(int)CanvasType.Option].gameObject.SetActive(true);
	}
	//Ʈ��� ���� ������ ���� �޽��� �ȴ��� ������ ����
	private void Chage_Color(bool flag, Button obj)
	{
		if (flag)
		{
			obj.colors = color;
		}
		else obj.colors = origincolor;
	}

	public void SettingView(int type)
	{
		for (int i = 0; i < options.Count; ++i)
		{
			switch ((OptionType)type)
			{
				case OptionType.KeyButton:
					if (i == (int)OptionType.KeyButton)
					{
						options[i].gameObject.SetActive(true);
						Chage_Color(true, buttons[i]);
					}
					else
					{
						options[i].gameObject.SetActive(false);
						Chage_Color(false, buttons[i]);
					}
					break;
				case OptionType.SoundButton:
					if (i == (int)OptionType.SoundButton)
					{
						options[i].gameObject.SetActive(true);
						Chage_Color(true, buttons[i]);
					}
					else
					{
						options[i].gameObject.SetActive(false);
						Chage_Color(false, buttons[i]);
					}
					break;
				case OptionType.VideoButton:
					if (i == (int)OptionType.VideoButton)
					{
						options[i].gameObject.SetActive(true);
						Chage_Color(true, buttons[i]);
					}
					else
					{
						options[i].gameObject.SetActive(false);
						Chage_Color(false, buttons[i]);
					}
					break;
			}

		}
	}
	void OnGUI()
	{
		if (RecordInput && Input.anyKeyDown)
		{
			Event e = Event.current;

			if (null != e)
			{
				if (e.isKey)
				{
					switch (keyOptionType)
					{
						case KeyOptionType.Qkey:
							{
								Text text = texts[(int)KeyOptionType.Qkey];
								string temp = text.text;
								if(e.keyCode.ToString()==texts[(int)KeyOptionType.Ekey].text)
								{
									text.text = temp;
									RecordInput = false;
									return;
								}
								text.text = e.keyCode.ToString();
								RecordInput = false;
							}
							break;
						case KeyOptionType.Ekey:
							{
								Text text = texts[(int)KeyOptionType.Ekey];
								string temp = text.text;
								if (e.keyCode.ToString() == texts[(int)KeyOptionType.Qkey].text)
								{
									text.text = temp;
									RecordInput = false;
									return;
								}
								text.text = e.keyCode.ToString();
								RecordInput = false;
							}
							break;
					}
					return;
				}
			}
		}
	}
	bool RecordInput = false;
	public void ChangeKey(int type)
	{
		RecordInput = true;
		keyOptionType = (KeyOptionType)type;
	}
}
