using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class goToNextLevel : MonoBehaviour
{
    public void GoBtnClick(){
        SceneManager.LoadScene(2);
    }

    public void ExitClick(){
        Application.Quit();
    }


}
