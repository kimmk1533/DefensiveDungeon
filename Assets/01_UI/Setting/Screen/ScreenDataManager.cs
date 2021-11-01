using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

static class JsonUtilityEx
{
    public static void SaveToJson<T>(T setting, string directory_path, string filename) where T : struct
    {
        string path = "";

        // Modify Path
        if (directory_path.Length == 0)
        {
            path = Path.Combine(Application.dataPath, filename);
        }
        else
        {
            path = Path.Combine(Application.dataPath, directory_path, filename);
        }

        // Save Data
        File.WriteAllText(path, JsonUtility.ToJson(setting, true));
    }

    public static T? LoadFromJson<T>(string directory_path, string filename) where T : struct
    {
        T ret_setting = new T();
        // Modify Path
        string path = "";
        if (directory_path.Length == 0)
        {
            path = Path.Combine(Application.dataPath, filename);
        }
        else
        {
            path = Path.Combine(Application.dataPath, directory_path, filename);
        }

        if (false == File.Exists(path))
        {
            Debug.LogWarningFormat("{0} : {1}", "JsonUtilityEx.LoadFromJson()", "로드할 파일이 없습니다");
            return null;
        }
        ret_setting = JsonUtility.FromJson<T>(File.ReadAllText(path));
        return ret_setting;
    }
}

static class ScreenDataUtility
{
    public static Vector2 GetResoultion(ScreenSetting setting)
    {
        string enum2string = setting.m_Data.m_Resolution.ToString(); // _1920x1080
        enum2string = enum2string.Substring(1);     // 1920x1080
        string[] s_resolution = enum2string.Split('x'); // { 1920, 1080 }

        int width = int.Parse(s_resolution[0]);
        int height = int.Parse(s_resolution[1]);

        return new Vector2(width, height);
    }

    public static ScreenSetting SetResolution(ScreenSetting setting, int resoultion)
    {
        ScreenSetting ret_setting = new ScreenSetting();
        string enum2string = setting.m_Data.m_Resolution.ToString(); // _1920x1080
        enum2string = enum2string.Substring(1); // 1920x1080
        string[] s_resolution = enum2string.Split('x'); // { 1920, 1080 }

        int width = int.Parse(s_resolution[0]);
        int height = int.Parse(s_resolution[1]);              
        Screen.SetResolution(width, height, false);

        return ret_setting;
    }  
}

static class AudioDataUtility
{
    
}

static class KeyDataUtility
{

}




public class ScreenDataManager : MonoBehaviour
{

}
