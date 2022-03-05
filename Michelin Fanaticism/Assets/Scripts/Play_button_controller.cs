using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Play_button_controller : MonoBehaviour
{
    public GameObject selectPanel;
    public GameObject startPanel;
    public void PlayBtnClick(){
        startPanel.SetActive(false);
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
        loadLevel(index);
    }

    public void nextLevel()
    {
        int currentLevel = GameManager.gm.currentLevel;
        if (currentLevel==4)
        {
            SceneManager.LoadScene(0);
        }
        else
        {
            loadLevel(currentLevel+1);
        }
    }

    public void goBack()
    {
        Debug.Log("go back");
        SceneManager.LoadScene(0);
    }

    public void retry()
    {
        loadLevel(GameManager.gm.currentLevel);
    }

    private void loadLevel(int level)
    {
        GameObject configReader = GameObject.FindGameObjectsWithTag("ConfigReader")[0];
        GameObject.DontDestroyOnLoad(configReader);
        ConfigReader cr = configReader.GetComponent<ConfigReader>();
        //上边创建完以后，先不要读配置，先选关，选完关以后，根据选的关来配置.
        StartCoroutine(cr.Load(level, done => {
            if(done != null) {
                if(done) {
                    // 读取配置文件成功
                    SceneManager.LoadScene(1);
                } else {

                }
            }
        }));
    }
}
