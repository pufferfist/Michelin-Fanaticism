using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCharacter : MonoBehaviour
{
    private Rigidbody rb;
    public float speed = 25;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();

    }

    // Update is called once per frame
    void Update()
    {


        var h = Input.GetAxis("Horizontal");
//        rb.AddForce(new Vector3(0, 0, -h) * speed);
        rb.velocity = new Vector3(rb.velocity.x,rb.velocity.y,-h*speed);
    }
}