using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float moveSpeed = 0.05f;
    private float runSpeed = 2f;
    private Animator animator;
    public Animator Animator{
        get{return animator;}
        set{animator = value;}
    }
    public bool moving;
    private Rigidbody2D body;
    public Rigidbody2D Body{
        get{return body;}
        set{body=value;}
    }
    public Vector2 moveInput = Vector2.zero;

    public Vector2 lastMotionVector;
    private PlayerData playerData;



    void Start()
    {
        GameManager.Instance.playerMovement = gameObject;
        InitializePlayerData();
        InitializeMovement();
    }
    void Update(){
        HandleMovementAnimation();
    }
    void FixedUpdate()
    {
       
        
         MovePlayer();
    }


    public void HandleMovementAnimation(){

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        
        moveInput = new Vector2(horizontal,vertical);
       
        Animator.SetFloat("horizontal",horizontal);
        Animator.SetFloat("vertical",vertical);
        moving = horizontal != 0 || vertical != 0;
        Animator.SetBool("moving",moving);

        if(moving){
            lastMotionVector = moveInput.normalized;
            Animator.SetFloat("lastHorizontal",lastMotionVector.x);
            Animator.SetFloat("lastVertical",lastMotionVector.y);
        }
        
    }

    public void MovePlayer(){
   
        Body.velocity = moveInput * moveSpeed/Time.deltaTime;  
          
        
    }

     private void InitializeMovement(){
        Body = GetComponent<Rigidbody2D>();
        Animator = GetComponentInChildren<Animator>();
    }
    private void InitializePlayerData(){
        playerData = GameManager.Instance.GetGameData().playerData;
    }

}
