using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ingredients : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        var player = other.GetComponent<MainCharacter>();

        if (player)
        {          
            print(gameObject.name);
            string[] strArray = gameObject.name.Split('(');
            print(strArray[0]);
            gameObject.SetActive(GameManager.gm.canPickUp(strArray[0]));
            //gameObject.SetActive(false);
            print(GameManager.gm.canPickUp(strArray[0]));

        }
    }

}