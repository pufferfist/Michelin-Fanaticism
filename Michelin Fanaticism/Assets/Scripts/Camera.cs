using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{
    public Transform CameraTransform;
    public Vector3 distance;
    // Use this for initialization
    void Start()
    {
        distance = transform.position - CameraTransform.position;

    }

    // Update is called once per frame
    void Update()
    {

        transform.position =new Vector3(CameraTransform.position.x + distance.x - 2,CameraTransform.position.y + distance.y, 0);
        //transform.position = CameraTransform.position + distance;

    }
}