using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.XR;

public class MainCharacter : MonoBehaviour
{
    private Rigidbody rb;
    public float speed = 25;
    public float forwardSpeed = 15;
    private GameState gameState;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = new Vector3(forwardSpeed, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        switch (gameState)
        {
            case GameState.Playing:
                //var h = Input.GetAxis("Horizontal");
                //var velocity = rb.velocity;
                var position = rb.position;
                if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    rb.position = new Vector3(position.x, position.y, Mathf.Clamp(position.z + 7, -7, 7));
                }
                if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    rb.position = new Vector3(position.x, position.y, Mathf.Clamp(position.z - 7, -7, 7));
                }
                //rb.velocity = new Vector3(velocity.x,velocity.y,-h*speed);
                //rb.position = new Vector3 (position.x, position.y, Mathf.Clamp(position.z ,-14, 14));

                break;
            default:
                break;
        }
    }

    public void changeState(GameState state)
    {
        this.gameState = state;
    }
}