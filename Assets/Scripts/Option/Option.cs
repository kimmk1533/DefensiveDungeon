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
    None = -1,

    KeyButton,
    SoundButton,
    VideoButton,
    Exit,

    Max
}
public enum KeyOptionType
{
    None = -1,

    Qkey,
    Ekey,
    Skill1,
    Skill2,
    ActiveShop,
    ReShop,
    LevelUp,
    Synerge_North,
    Synerge_East,
    Synerge_South,
    Synerge_West,

    Max
}
public class Option : MonoBehaviour
{
    //옵션버튼
    [SerializeField] List<Button> buttons;
    //옵션그룹
    public List<GameObject> options;
    //방향전환 키세팅
    public List<Button> KeySetting;
    public List<Text> texts;
    protected List<KeyCode> keyCode;
    protected OptionManager M_Option => OptionManager.Instance;

    Button m_current_button;

    KeyOptionType keyOptionType = KeyOptionType.None;
    bool RecordInput = false;

    private void Awake()
    {
        m_current_button = buttons[0];
        __OnSelectButton(0);

        options[(int)OptionType.SoundButton].gameObject.SetActive(false);
        options[(int)OptionType.VideoButton].gameObject.SetActive(false);
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
                    }
                    else
                    {
                        options[i].gameObject.SetActive(false);
                    }
                    break;
                case OptionType.SoundButton:
                    if (i == (int)OptionType.SoundButton)
                    {
                        options[i].gameObject.SetActive(true);
                    }
                    else
                    {
                        options[i].gameObject.SetActive(false);
                    }
                    break;
                case OptionType.VideoButton:
                    if (i == (int)OptionType.VideoButton)
                    {
                        options[i].gameObject.SetActive(true);
                    }
                    else
                    {
                        options[i].gameObject.SetActive(false);
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

                    M_Option.ClearKeyCode(keyOptionType);
                    M_Option.SetKeyCode(keyOptionType, M_Option.GetKeyCode((KeyOptionType)i));

                    M_Option.ClearKeyCode((KeyOptionType)i);
                    M_Option.SetKeyCode((KeyOptionType)i, keycode);
                    return;
                }
            }

            texts[(int)keyOptionType].text = e.keyCode.ToString();
            M_Option.ClearKeyCode(keyOptionType);
            M_Option.SetKeyCode(keyOptionType, e.keyCode);
        }
    }
    public void ChangeKey(int type)
    {
        RecordInput = true;
        keyOptionType = (KeyOptionType)type;
    }

    public void __OnSelectButton(int index)
    {
        m_current_button.targetGraphic.color = m_current_button.colors.normalColor;
        m_current_button = buttons[index];
        m_current_button.targetGraphic.color = m_current_button.colors.selectedColor;
    }
}
