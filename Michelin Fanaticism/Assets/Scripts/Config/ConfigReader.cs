using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

using UnityEngine.Networking;
public class ConfigReader :  MonoBehaviour{
    public bool configReady = false;
    public LevelConfig configResult;
    public void GetConfig()
    {
       // StartCoroutine(Example());//开启协程运行函数
    }

    public  IEnumerator Load(int level, System.Action<bool> done) {
        string fileName = "Level" + level.ToString() + ".Config.json";
        string result = "";
        if(Application.platform == RuntimePlatform.WebGLPlayer){
            var  uri = new  System.Uri(Path.Combine(Application.streamingAssetsPath,fileName));
            Debug.Log(uri.ToString());
            Debug.Log("using web io");
            UnityWebRequest www = UnityWebRequest.Get(uri);
            yield return www.SendWebRequest();
    
            if(www.isNetworkError || www.isHttpError) {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log(www.downloadHandler.text);
                result = www.downloadHandler.text;
            }
        }else{
              string filePath = System.IO.Path.Combine(Application.streamingAssetsPath, fileName);
    
            Debug.Log("using local io");
             result = System.IO.File.ReadAllText(filePath);
        }
        Debug.Log("finish get config");
        configResult = JsonUtility.FromJson<LevelConfig>(result);
        Debug.Log(Newtonsoft.Json.JsonConvert.SerializeObject(configResult));
        configReady  = true;
        done(true);
       
    }
   
}