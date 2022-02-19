using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ConfigReader {
    public static T LoadJsonFromFile<T>(int level) where T : class {
        if (!File.Exists(Application.dataPath + "/StreamingAssets/LevelConfig_"+level.ToString()+".json")) {
            Debug.LogError("配置文件路径不正确");
            return null;
        }

        StreamReader sr = new StreamReader(Application.dataPath + "/StreamingAssets/LevelConfig_"+level.ToString()+".json");
        if (sr == null) {
            return null;
        }
        string json = sr.ReadToEnd();

        if (json.Length > 0) {
            return JsonUtility.FromJson<T>(json);
        }
        return null;
    }
}