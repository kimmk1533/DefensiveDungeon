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
    //캔버스
    public List<GameObject> Canvas;
    //옵션버튼
    public List<Button> buttons;
    //옵션그룹
    public List<GameObject> options;
    //방향전환 키세팅
    public List<Button> KeySetting;
    public List<Text> texts;
    protected List<KeyCode> keyCode;
    protected OptionManager M_Option => OptionManager.Instance;

    Button m_current_button;

    ColorBlock color;
    ColorBlock origincolor;
    KeyOptionType keyOptionType = KeyOptionType.None;
    bool ActiveMainCanvas = false;
    bool RecordInput = false;

    private void Awake()
    {
        //color = origincolor = buttons[1].colors;
        ////버튼 클릭(활성화)시 색 변경
        //color.normalColor = new Color(132f, 132f, 132f, 71f) / 255f;
        //buttons[0].colors = color;

        m_current_button = buttons[0];
        __OnSelectButton(0);

        Canvas[(int)CanvasType.Option].gameObject.SetActive(false);
        options[(int)OptionType.SoundButton].gameObject.SetActive(false);
        options[(int)OptionType.VideoButton].gameObject.SetActive(false);

        M_Option.SetKeyCode(KeyOptionType.Qkey, KeyCode.Q);
        M_Option.SetKeyCode(KeyOptionType.Ekey, KeyCode.E);
        M_Option.SetKeyCode(KeyOptionType.Skill1, KeyCode.Z);
        M_Option.SetKeyCode(KeyOptionType.Skill2, KeyCode.X);
        M_Option.SetKeyCode(KeyOptionType.LevelUp, KeyCode.F);
        M_Option.SetKeyCode(KeyOptionType.ReShop, KeyCode.D);
        M_Option.SetKeyCode(KeyOptionType.Synerge_North, KeyCode.Alpha1);
        M_Option.SetKeyCode(KeyOptionType.Synerge_East, KeyCode.Alpha2);
        M_Option.SetKeyCode(KeyOptionType.Synerge_South, KeyCode.Alpha3);
        M_Option.SetKeyCode(KeyOptionType.Synerge_West, KeyCode.Alpha4);
        M_Option.SetKeyCode(KeyOptionType.ActiveShop, KeyCode.Space);
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

    public void ConnectSkill()
    {

    }
    public void __OnSelectButton(int index)
    {
        m_current_button.targetGraphic.color = m_current_button.colors.normalColor;
        m_current_button = buttons[index];
        m_current_button.targetGraphic.color = m_current_button.colors.selectedColor;
    }
}
