using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Play_button_controller : MonoBehaviour
{
    public GameObject selectPanel;

    public void PlayBtnClick(){
        selectPanel.SetActive(true);
    }

    public void SelectLevel(){
        selectPanel.SetActive(false);
        int index = 1;
        switch(UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.name){
            case "Level1Btn":
                        index = 1;
                        break;
            case "Level2Btn":
                        index = 2;
                        break;
            case "Level3Btn":
                        index = 3;
                        break;
            case "Level4Btn":
                        index = 4;
                        break;
            case "Level5Btn":
                        index = 5;
                        break;
            case "Level6Btn":
                        index = 6;
                        break;
                    default:
                        break;
        }
        GameObject configReader = GameObject.FindGameObjectsWithTag("ConfigReader")[0];
        GameObject.DontDestroyOnLoad(configReader);
        ConfigReader cr = configReader.GetComponent<ConfigReader>();
        //上边创建完以后，先不要读配置，先选关，选完关以后，根据选的关来配置.
        StartCoroutine(cr.Load(index, done => {
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
