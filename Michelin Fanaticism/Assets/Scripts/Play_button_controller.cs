using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Play_button_controller : MonoBehaviour
{
    public void PlayBtnClick(){
        GameObject  configReader = GameObject.FindGameObjectsWithTag("ConfigReader")[0];
        GameObject.DontDestroyOnLoad(configReader);
        ConfigReader cr = configReader.GetComponent<ConfigReader>();
        StartCoroutine(cr.Load(1, done => {
            if(done != null) {
                if(done) {
                    // 读取配置文件成功
                    SceneManager.LoadScene(1);
                } else {
                    
                }
            }	
        }));
        //cr.GetConfig();
    }
}
