using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buff : MonoBehaviour
{
    private void Start()
    { 
        MeshCollider meshCollider = gameObject.GetComponent<MeshCollider>();
        meshCollider.convex = true;
        meshCollider.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        var player = other.GetComponent<MainCharacter>();

        if (player)
        {
            string buffName;
            string[] strArray = gameObject.name.Split(' ');
            buffName = strArray[0];
            if (buffName.Equals("Heart"))
            {
                GameManager.gm.addLife();
                gameObject.SetActive(false);
            }
            else if (buffName.Equals("SpeedUp"))
            {
                GameManager.gm.speedChange(5);
                gameObject.SetActive(false);
            }
            else if (buffName.Equals("SpeedDown"))
            {
                GameManager.gm.speedChange(-5);
                gameObject.SetActive(false);
            }
        }
    }

}