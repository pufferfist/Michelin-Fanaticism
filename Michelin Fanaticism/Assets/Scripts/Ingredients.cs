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
            
            //gameObject.SetActive(canPickUp(gameObject.name));
            print(gameObject.name);
            gameObject.SetActive(false);
            
        }
    }

}