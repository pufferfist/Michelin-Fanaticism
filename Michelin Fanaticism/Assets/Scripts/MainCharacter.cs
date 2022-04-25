using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.XR;

public class MainCharacter : MonoBehaviour
{
    private Rigidbody rb;
    private GameState gameState;
    private Animator animator;

    public float hSpeed = 1;
    public int widthLimit = 5;
    public Boolean canJump = true;
    public float forwardSpeed = 15;

    
    private int checkBit;
    private float prePos = 0;
    private const int CHECKMID = 1; 
    private const int CHECKLEFT = 2; 
    private const int CHECKRIGHT = 4; 
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        rb.velocity = new Vector3(forwardSpeed, 0, 0);
        checkBit = CHECKLEFT| CHECKRIGHT;
    }
    public void SpeedUp(){
        if (forwardSpeed == 15)
        {
            forwardSpeed += 20;
            rb.velocity = new Vector3(forwardSpeed, 0, rb.velocity.z);
            animator.SetBool("isRun", true);
            animator.SetBool("isWalk", false);
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
        animator.SetBool("isWalk", true);
        animator.SetBool("isRun", false);
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
        yield return new WaitForSeconds(1f);
        rb.velocity = new Vector3(forwardSpeed, 0, rb.velocity.z);
        canJump = true;
        animator.SetBool("isJump", false);
        animator.SetFloat("yPosition", rb.position.y);
    }
    void Update()
    {
        
        var position = rb.position;
        checkArrival(position.z);
        prePos = position.z;
        animator.SetFloat("yPosition", position.y);
        switch (gameState)
        {
            case GameState.Playing:
                 // if (Input.GetKeyDown(KeyCode.W))
                 // {
                 //     if (forwardSpeed == 15)
                 //     {
                 //         forwardSpeed += 20;
                 //         rb.velocity = new Vector3(forwardSpeed, 0, rb.velocity.z);
                 //         animator.SetBool("isRun", true);
                 //         animator.SetBool("isWalk", false);
                 //         StartCoroutine(WaitandSlowDown());
                 //     }  
                 // }

                if (Input.GetKeyDown(KeyCode.Space))
                {
                    if (canJump==true)
                    {
                        animator.SetBool("isJump", true);
                        canJump = false;
                        rb.velocity = new Vector3(forwardSpeed, 7, rb.velocity.z); 
                        MainCharacter mainCharacter = GameManager.gm.character;
                        AudioSource[] audioSources = mainCharacter.GetComponents<AudioSource>();
                        AudioSource audioSource = audioSources[5];
                        audioSource.Play();
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
                transform.LookAt(transform.position + new Vector3(rb.velocity.x,0,rb.velocity.z));
                

                break;
            default:
                break;
        }
        animator.SetFloat("speed", rb.velocity.magnitude);
    }


    public void changeState(GameState state)
    {
        switch (state)
        {
            case GameState.Playing:
                rb.velocity = new Vector3(forwardSpeed,0,0);
                animator.SetBool("isWalk", true);
                animator.SetBool("isJump", false);
                animator.SetBool("isRun", false);
                animator.SetFloat("yPosition", rb.position.y);
                break;
            default:
                animator.SetBool("isWalk", false);
                animator.SetBool("isJump", false);
                animator.SetBool("isRun", false);
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