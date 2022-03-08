using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.XR;

public class MainCharacter : MonoBehaviour
{
    private Rigidbody rb;
    public float speed = 0.1f;
    public float forwardSpeed = 15;
    private GameState gameState;
    private int target = 0;
    private int checkBit;
    private float prePos = 0;
    private const int CHECKMID = 1; 

    private const int CHECKLEFT = 2; 

    private const int CHECKRIGHT = 4; 
    private bool begin = true;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = new Vector3(forwardSpeed, 0, 0);
        checkBit = CHECKLEFT| CHECKRIGHT;
    }

    // Update is called once per frame
    void checkArrival(float z){
        // check for mid arrival
        if((checkBit & CHECKMID) >0){
            if(prePos * z<=0){
                  rb.velocity = new Vector3(forwardSpeed, 0, 0);
                  checkBit = CHECKLEFT| CHECKRIGHT;
            }
        }
         // check for left arrival
        if((checkBit & CHECKLEFT) >0){
             if((prePos-5) * (z-5)<=0){
                  rb.velocity = new Vector3(forwardSpeed, 0, 0);
                  checkBit = CHECKMID;
            }
        }
        // check for right arrival
        if((checkBit & CHECKRIGHT) >0){
             if((prePos+5) * (z+5)<=0){
                  rb.velocity = new Vector3(forwardSpeed, 0, 0);
                  checkBit = CHECKMID;
            }
        }
    }
    void Update()
    {
        
        var position = rb.position;
        checkArrival(position.z);
        prePos = position.z;
        switch (gameState)
        {
            case GameState.Playing:
                if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    GameManager.gm.switchTrack(true);//used for analytic, don't delete!
                    rb.velocity = new Vector3(forwardSpeed, 0, 20);
                    //rb.position = new Vector3(position.x, position.y, Mathf.Clamp(position.z + 5, -5, 5));
                }
                if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    GameManager.gm.switchTrack(false);//used for analytic, don't delete!
                    rb.velocity = new Vector3(forwardSpeed, 0, -20);
                    //rb.position = new Vector3(position.x, position.y, Mathf.Clamp(position.z - 5, -5, 5));
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