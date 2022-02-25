using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
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
            string obstacleName;
            string[] strArray = gameObject.name.Split(' ');
            obstacleName = strArray[0];
            if (obstacleName.Equals("ConeStop"))
            {
                GameManager.gm.looseLife();
            }
        }
    }

}