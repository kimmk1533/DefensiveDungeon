using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct ScreenSetting
{
    public ScreenSettingData m_Data;

    public ScreenSetting(ScreenSettingData data)
    {
        m_Data = data;
    }
}
[System.Serializable]
public struct AudioSettings
{
    public List<AudioSettingData> m_Data;

    public AudioSettings(List<AudioSettingData> data)
    {
        m_Data = data;
    }
}
[System.Serializable]
public struct KeySettings
{
    public List<KeySettingData> m_Data;

    public KeySettings(List<KeySettingData> data)
    {
        m_Data = data;
    }
}

[System.Serializable]
public struct SettingArgs
{
    public ScreenSetting screen_data;
    public AudioSettings audio_data;
    public KeySettings key_data;
}

public class SettingManager : Singleton<SettingManager>
{
    // 설정 변경 윈도우에서 확인 버튼이 눌릴 경우에 셋팅값이 변경됨
    // 셋팅 값(m file setting)에 변경이 생긴 경우 호출됩니다
    // current setting 을 args 로 통체로 넘김
    public delegate void OnSettingChangeHandler(SettingArgs args);
    public event OnSettingChangeHandler OnSettingChnagedEvent;

    [Space(10)]
    [SerializeField] string m_screen_directoryPath;      // 유저 셋팅의 저장된 directory 경로
    [SerializeField] string m_audio_directoryPath;       // 유저 셋팅의 저장된 directory 경로
    [SerializeField] string m_key_directoryPath;         // 유저 셋팅의 저장된 directory 경로

    [Space(10)]
    [SerializeField] string m_screen_filename;      // ex ) ScreenSetting.json
    [SerializeField] string m_audio_filename;       // ex ) ScreenSetting.json
    [SerializeField] string m_key_filename;         // ex ) ScreenSetting.json

    [Space(10)]
    [SerializeField] SettingArgs m_Initial_Setting;  // 초기 셋팅
    [SerializeField] SettingArgs m_File_Setting;     // 유저 셋팅
    [SerializeField] SettingArgs m_Current_Setting;  // 현제 셋팅

    Canvas m_Canvas = null;
    CanvasScaler m_CanvasScaler = null;

    public SettingArgs GetCurrentSetting()
    {
        return m_Current_Setting;
    }

    public void Awake()
    {
        Initialize();
    }

    public void OnApplicationQuit()
    {
        SaveFileSetting();
    }
    public void Initialize()
    {
        // new
        AllocData();

        // 파일을 읽어 값을 셋팅
        LoadInitSetting();
        LoadFileSetting();
        // 게임 관리 값을 셋팅합니다
        SetCurrentSetting();

        InitCanvas();
    }

    // 멤버변수를 할당
    private void AllocData()
    {
        m_Initial_Setting = new SettingArgs();
        m_File_Setting = new SettingArgs();
        m_Current_Setting = new SettingArgs();       
    }

    private void InitCanvas()
    {
        if (m_Canvas == null)
            m_Canvas = GameObject.FindWithTag("MainCanvas").GetComponent<Canvas>();
        if (m_CanvasScaler == null)
            m_CanvasScaler = m_Canvas.GetComponent<CanvasScaler>();
    }

    // scriptable object 에서 정보를 모두 읽어
    // 각 setting class 의 생성자 호출 및 init setting 값 초기화   
    private void LoadInitSetting()
    {
        // Load ScriptableObject( Screen_Setting )
        SO_ScreenSetting Temp_Screen = Resources.Load<SO_ScreenSetting>("Screen");
        ScreenSettingData Initial_Screen = Temp_Screen.m_Data;

        // Load ScriptableObject( Audio_Setting )        
        SO_AudioSetting[] Temp_Audio = Resources.LoadAll<SO_AudioSetting>("");
        List<AudioSettingData> Initial_Audio = new List<AudioSettingData>();
        for (int i = 0; i < Temp_Audio.Length; ++i)
        {
            Initial_Audio.Add(Temp_Audio[i].m_Data);
        }
        Initial_Audio.Sort((AudioSettingData x, AudioSettingData y) => { return x.CompareTo(y); });

        // Load ScriptableObject( Key_Setting )
        SO_KeySetting[] Temp_Key = Resources.LoadAll<SO_KeySetting>("");
        List<KeySettingData> Initial_Key = new List<KeySettingData>();
        for (int i = 0; i < Temp_Key.Length; ++i)
        {
            Initial_Key.Add(Temp_Key[i].m_Data);
        }
        Initial_Key.Sort((KeySettingData x, KeySettingData y) => { return x.CompareTo(y); });

        // load 된 정보를 멤버변수에 셋팅
        m_Initial_Setting.screen_data = new ScreenSetting(Initial_Screen);
        m_Initial_Setting.audio_data = new AudioSettings(Initial_Audio);
        m_Initial_Setting.key_data = new KeySettings(Initial_Key);
    }

    // JSON 파일을 모두 읽어
    // 유저거 과거에 셋팅한 값으로 셋팅
    private void LoadFileSetting()
    {
        // load from json
        ScreenSetting? loaded_screensetting_data = JsonUtilityEx.LoadFromJson<ScreenSetting>(m_screen_directoryPath, m_screen_filename);
        AudioSettings? loaded_audiosetting_data = JsonUtilityEx.LoadFromJson<AudioSettings>(m_audio_directoryPath, m_audio_filename);
        KeySettings? loaded_keysetting_data = JsonUtilityEx.LoadFromJson<KeySettings>(m_key_directoryPath, m_key_filename);

        // 항상 존재해야 한다고 가정하고 작성함
        if (null == loaded_screensetting_data || null == loaded_audiosetting_data || null == loaded_keysetting_data)
        {
            Debug.LogError("ScreenSetting load file 없음");
        }

        // 불러온 값 멤버 변수에 셋팅
        m_File_Setting.screen_data = loaded_screensetting_data.Value;
        m_File_Setting.audio_data = loaded_audiosetting_data.Value;
        m_File_Setting.key_data = loaded_keysetting_data.Value;
    }

    private void SetCurrentSetting()
    {
        // file setting 이 항상 불러와 진다고 가정하고 작성함
        // file setting 그대로 복사하기
        m_Current_Setting = m_File_Setting;
    }

    // 변경 시 마다 매번 업데이트 해줄것
    private void SetFileSetting()
    {
        // current setting 그대로 복사하기
        m_File_Setting = m_Current_Setting;
    }

    // file setting 값을 저장한다
    private void SaveFileSetting()
    {
        // 데이터를 최신화 하고
        SetFileSetting();

        // Json 데이터 저장
        JsonUtilityEx.SaveToJson<ScreenSetting>(m_File_Setting.screen_data, m_screen_directoryPath, m_screen_filename);
        JsonUtilityEx.SaveToJson<AudioSettings>(m_File_Setting.audio_data, m_audio_directoryPath, m_audio_filename);
        JsonUtilityEx.SaveToJson<KeySettings>(m_File_Setting.key_data, m_key_directoryPath, m_key_filename);
    }

#if UNITY_EDITOR
    private void Update()
    {        
        OnSettingChnagedEvent?.Invoke(m_Current_Setting);
    }
#endif
}
