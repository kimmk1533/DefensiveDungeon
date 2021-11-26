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
	Exit,
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
	public List<KeyCode> keyCode;
	protected OptionManager M_Option => OptionManager.Instance;

	ColorBlock color;
	ColorBlock origincolor;
	KeyOptionType keyOptionType = KeyOptionType.None;
	bool ActiveMainCanvas = false;
	bool RecordInput = false;

	private void Awake()
	{
		color = origincolor = buttons[1].colors;
		//��ư Ŭ��(Ȱ��ȭ)�� �� ����
		color.normalColor = new Color(132f, 132f, 132f, 71f) / 255f;

		buttons[0].colors = color;
		Canvas[(int)CanvasType.Option].gameObject.SetActive(false);
		options[(int)OptionType.SoundButton].gameObject.SetActive(false);
		options[(int)OptionType.VideoButton].gameObject.SetActive(false);

		M_Option.SetKeyCode(KeyOptionType.Qkey,KeyCode.Q);
		M_Option.SetKeyCode(KeyOptionType.Ekey, KeyCode.E);
	}
	public void EnableStartCanvas()
	{
		if (!ActiveMainCanvas)
		{
			Canvas[(int)CanvasType.Start].gameObject.SetActive(false);
			Canvas[(int)CanvasType.Option].gameObject.SetActive(true);
		}
		else
		{
			Canvas[(int)CanvasType.Start].gameObject.SetActive(true);
			Canvas[(int)CanvasType.Option].gameObject.SetActive(false);
		}
		ActiveMainCanvas = !ActiveMainCanvas;
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
		if (!RecordInput)
			return;

		Event e = Event.current;
		if (null != e && Input.anyKeyDown)
		{
			if (!e.isKey)
				return;

			KeyCode keycode = M_Option.GetKeyCode(keyOptionType);

			if (keycode == e.keyCode)
			{
				return;
			}
			string text = texts[(int)keyOptionType].text;

			RecordInput = false;

			for (int i = 0; i < texts.Count; ++i)
			{
				if (e.keyCode.ToString() == texts[i].text)
				{
					texts[(int)keyOptionType].text = texts[i].text;
					texts[i].text = text;

					M_Option.SetKeyCode(keyOptionType, M_Option.GetKeyCode((KeyOptionType)i));
					M_Option.SetKeyCode((KeyOptionType)i, keycode);
					return;
				}
			}

			texts[(int)keyOptionType].text = e.keyCode.ToString();
			M_Option.SetKeyCode(keyOptionType, e.keyCode);
		}
	}
	public void ChangeKey(int type)
	{
		RecordInput = true;
		keyOptionType = (KeyOptionType)type;
	}

}
