using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : PlayerController
{
    private float runSpeed = 2f;
 
    public bool moving;
    public Vector2 moveInput = Vector2.zero;



    void Start()
    {
        GameManager.Instance.playerMovement = this;
        InitializeMovement();
    }
    void Update(){
       

        // if(Input.GetKeyDown(KeyCode.U)){
        //      Body.AddForce(Vector2.up * 1000f);
        // }
        HandleMovementAnimation();
    }
    void FixedUpdate()
    {
       if(GameManager.Instance.regenerating ){
            Freeze();
        }
        else if(GameManager.Instance.instructionsUI.GetComponent<Instructions>().open){

            Freeze();
        }
        else if(frozen){
            UnFreeze();
        }

        
        if(!frozen){
            MovePlayer();
        }
        
    }


    public void HandleMovementAnimation(){
        
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        
        moveInput = new Vector2(horizontal,vertical);
       
        animator.SetFloat("horizontal",horizontal);
        animator.SetFloat("vertical",vertical);
        moving = horizontal != 0 || vertical != 0;
        animator.SetBool("moving",moving);

        if(moving){
            lastMotionVector = moveInput.normalized;
            animator.SetFloat("lastHorizontal",lastMotionVector.x);
            animator.SetFloat("lastVertical",lastMotionVector.y);
        }
        
    }

    public void MovePlayer(){
        
        if(!GameManager.Instance.GetPlayer().slow){
           Body.velocity = moveInput * moveSpeed/Time.deltaTime;   
        }
        else{

            Body.velocity = moveInput * (moveSpeed*0.5f)/Time.deltaTime;  
        }
        
        
          
        
    } 

     private void InitializeMovement(){
        Body = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
    }
   

}
