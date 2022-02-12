using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Play_button_controller : MonoBehaviour
{
    public void PlayBtnClick(){
        SceneManager.LoadScene(1);
    }
}
