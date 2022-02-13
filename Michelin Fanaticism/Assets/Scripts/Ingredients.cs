using System;
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
            string ingredientName;
            string[] strArray = gameObject.name.Split(' ');
            ingredientName = strArray[0];
            // gameObject.SetActive(!GameManager.gm.canPickUp(ingredientName));
            Boolean colleced = GameManager.gm.canPickUpWithThreeMenusTwoPanels(ingredientName);
            gameObject.SetActive(!colleced);
        }
    }

}