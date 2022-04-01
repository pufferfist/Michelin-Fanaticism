using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ingredients : MonoBehaviour
{
    
    private void OnTriggerEnter(Collider other)
    {
        var player = other.GetComponent<MainCharacter>();

        if (player)
        {
            string ingredientName;
            string[] strArray = gameObject.name.Split(' ');
            ingredientName = strArray[0];
            bool collected = GameManager.gm.canPickUp(ingredientName);
            if (collected)
            {
                    Debug.Log("Play Sound.");
                    GetComponent<AudioSource>().Play();
            }
            gameObject.SetActive(!collected);
        }
    }

}