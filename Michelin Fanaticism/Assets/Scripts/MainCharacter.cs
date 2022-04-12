using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.XR;

public class MainCharacter : MonoBehaviour
{
    private Rigidbody rb;
    public float hSpeed = 20;
    public int widthLimit = 5;
    public Boolean canJump=true;
    
    public float forwardSpeed = 15;
    private GameState gameState;
    
    private int checkBit;
    private float prePos = 0;
    private const int CHECKMID = 1; 

    private const int CHECKLEFT = 2; 

    private const int CHECKRIGHT = 4; 
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = new Vector3(forwardSpeed, 0, 0);
        checkBit = CHECKLEFT| CHECKRIGHT;
    }
    public void SpeedUp(){
        if (forwardSpeed == 15)
        {
            forwardSpeed += 20;
            rb.velocity = new Vector3(forwardSpeed, 0, rb.velocity.z);
            StartCoroutine(WaitandSlowDown());
        }
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
             if((prePos-widthLimit) * (z-widthLimit)<=0){
                  rb.velocity = new Vector3(forwardSpeed, 0, 0);
                  checkBit = CHECKMID;
            }
        }
        // check for right arrival
        if((checkBit & CHECKRIGHT) >0){
             if((prePos+widthLimit) * (z+widthLimit)<=0){
                  rb.velocity = new Vector3(forwardSpeed, 0, 0);
                  checkBit = CHECKMID;
            }
        }
    }
    IEnumerator WaitandSlowDown()
    {
        yield return new WaitForSeconds(4f);
        forwardSpeed -= 20;
        rb.velocity = new Vector3(forwardSpeed, -10, rb.velocity.z);
        /*
        for (int i = 0; i < 40; i++)
        {
            yield return new WaitForSeconds(0.5f);
            if (forwardSpeed > 15)
            {
                forwardSpeed -= 1;
                rb.velocity = new Vector3(forwardSpeed, 0, rb.velocity.z);
            }
        }*/
    }
    IEnumerator WaitandJumpDown()
    {
        yield return new WaitForSeconds(1.5f);
        rb.velocity = new Vector3(forwardSpeed, 0, rb.velocity.z);
        canJump = true;
    }
    void Update()
    {
        
        var position = rb.position;
        checkArrival(position.z);
        prePos = position.z;
        switch (gameState)
        {
            case GameState.Playing:
                if (Input.GetKeyDown(KeyCode.W))
                {
                    if (forwardSpeed == 15)
                    {
                        forwardSpeed += 20;
                        rb.velocity = new Vector3(forwardSpeed, 0, rb.velocity.z);
                        StartCoroutine(WaitandSlowDown());
                    }
                    
                }
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    if (canJump==true)
                    {
                        canJump = false;
                        rb.velocity = new Vector3(forwardSpeed, 7, rb.velocity.z); 
                        StartCoroutine(WaitandJumpDown());
                    }
                }
                if (Input.GetKeyDown(KeyCode.A))
                {
                    if(position.z<widthLimit){
                         GameManager.gm.switchTrack(true);//used for analytic, don't delete!
                        rb.velocity = new Vector3(forwardSpeed, 0, hSpeed);
                    }
                   
                    //rb.position = new Vector3(position.x, position.y, Mathf.Clamp(position.z + 5, -5, 5));
                }
                if (Input.GetKeyDown(KeyCode.D))
                {
                    if(position.z>-1*widthLimit)
                    {     
                        GameManager.gm.switchTrack(false);//used for analytic, don't delete!
                        rb.velocity = new Vector3(forwardSpeed, 0, -1* hSpeed);
                    }
                    //rb.position = new Vector3(position.x, position.y, Mathf.Clamp(position.z - 5, -5, 5));
                }

                rb.position = new Vector3(position.x, position.y, Mathf.Clamp(position.z, -5, 5));


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

    public void changeSpeed(int delta)
    {
        forwardSpeed += delta;
        forwardSpeed = Math.Max(forwardSpeed, 5);
        rb.velocity = new Vector3(forwardSpeed, 0, 0);
    }
}