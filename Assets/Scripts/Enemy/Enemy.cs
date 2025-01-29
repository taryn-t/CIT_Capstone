using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Enemy : EnemyAI{


    public float Health
    {
        get { return _health; }
        set { _health = Mathf.Clamp(value, 0, 100); }
    }

    private float _health = 100;

     public float Mana
    {
        get { return _mana; }
        set { _mana = Mathf.Clamp(value, 0, 100); }
    }

    private float _mana = 100;

    public float speed = 2f;
    [SerializeField] protected Sprite[] healthSprites;

    [SerializeField] protected SpriteRenderer healthMeter;
    private int healthIndex = default;
    float startTime;

    [SerializeField] Color burnColor;
    [SerializeField] Color poisonColor;
    private Color endColor;
    private Color startColor;
    private SpriteRenderer renderer;
    Collider2D[] results ={};
    Light2D damageGlow;

    ContactFilter2D cf;
    
    void Awake(){
        damageGlow= GetComponentInChildren<Light2D>();
        damageGlow.intensity = 0;
    }

    void Start(){
        
         Body = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        behaviorTree = new BehaviorTree();
        lastMotionVector = Body.position.normalized;
        collider = GetComponent<CapsuleCollider2D>();
        renderer = GetComponent<SpriteRenderer>();
        startColor = renderer.color;


        cf = new ContactFilter2D();
        InitializeBehaviorTree();
        
    }

    void DestroyEnemy(){
        animator.SetBool("damage",true);
        Destroy(gameObject,1f);  
    }
     void Update()
    {
        if(Health==0){
            DestroyEnemy();
        }

         

        
        
       
    }
    void FixedUpdate(){
        
         behaviorTree.Tick(this);

         Movement();
         
    }



    public async void  TakeDamage(int damage, float knockback, Vector2 direction, SpellEffect spellEffect){
        cancellationTokenSource = new CancellationTokenSource();
        try
        {
            damaged = true;
            Health -= damage;
            DamageEffect(spellEffect);

            if(!animator.GetBool("damage")){
                animator.SetBool("damage",true);
                damageGlow.intensity = 1f;
            }
            
            GetComponent<Rigidbody2D>().AddForce(direction*knockback);

            // if(damaged && Health % healthSprites.Length-1 == 0 && Health > 0){ 
            //     healthIndex++;
            //     healthMeter.sprite = healthSprites[healthIndex];
            // }
            startTime = Time.time;
            
            await Task.Delay(1000,cancellationTokenSource.Token);
            
            animator.SetBool("damage",false); 
            damageGlow.intensity = 0;
            damaged=false;
                
            
            await Task.Yield();
            
        }
        catch
        {
            
            return;
        }
        finally
        {
            cancellationTokenSource.Dispose();
            cancellationTokenSource = null;
        }
        
    }

    public void DamageEffect(SpellEffect spellEffect){
        switch(spellEffect) 
        {
        case SpellEffect.Burn:
            endColor = burnColor;
            Invoke("Burn",5);
            break;

        case SpellEffect.Poison:
            endColor = poisonColor;
            Invoke("Poison",5);
            break;

        default:
            endColor = renderer.color;
           break;
        }
    }

    public void Burn(){
        Health -= 2;
    }
   public void Poison(){
        Health -= 2;
    }

 

 

  
}