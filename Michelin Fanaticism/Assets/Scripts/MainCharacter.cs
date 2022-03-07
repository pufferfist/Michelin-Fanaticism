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
                var position = rb.position;
                if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    GameManager.gm.switchTrack(true);//used for analytic, don't delete!
                    rb.position = new Vector3(position.x, position.y, Mathf.Clamp(position.z + 5, -5, 5));
                }
                if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    GameManager.gm.switchTrack(false);//used for analytic, don't delete!
                    rb.position = new Vector3(position.x, position.y, Mathf.Clamp(position.z - 5, -5, 5));
                }

                break;
            default:
                break;
        }
    }

    public void changeState(GameState state)
    {
        switch (state)
        {
            case GameState.Playing:
                rb.velocity = new Vector3(forwardSpeed,0,0);
                break;
            default:
                rb.velocity = Vector3.zero;
                break;
        }
        this.gameState = state;
    }
}