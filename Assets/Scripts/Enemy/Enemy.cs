using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

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

    ContactFilter2D cf;
    

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
        Destroy(this);
    }
     void Update()
    {
        
        
       
    }
    void FixedUpdate(){

         behaviorTree.Tick(this);

         Movement();
         
    }



    public async void TakeDamage(int damage, float knockback, Vector2 direction, SpellEffect spellEffect){
        damaged = true;
        Health -= damage;
        DamageEffect(spellEffect);
        
        GetComponent<Rigidbody2D>().AddForce(direction*knockback);

        if(Health % healthSprites.Length == 0){
            healthIndex++;
            healthMeter.sprite = healthSprites[healthIndex];
       }
       startTime = Time.time;

       await DamageColor();

       damaged=true;

       await Task.Yield();
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

    public async Task DamageColor(){
        
        int duration = 5;
        float t1 = Mathf.PingPong(Time.time - startTime, 1) / duration;
        float t2 = (Mathf.Cos( ( (Time.time - startTime) + duration ) * Mathf.PI / duration ) + 1 ) * 0.5f;

    // Use t2 instead of t1 if you want smoother interpolation
    
        
        renderer.color = Color.Lerp(startColor, endColor, t2);
        
        await Task.Delay(1500);
        


        
    }

 

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Spell");
        //  if(collision.gameObject.CompareTag("Enemy"))
        // {
        //     Enemy enemy = collision.gameObject.GetComponent<Enemy>();
        //     CastedSpell spell = collision.gameObject.GetComponent<CastedSpell>();
        //     enemy.TakeDamage(spell.damage,spell.knockback,spell.direction,spell.effect);
        //     Debug.Log(enemy.Health);
        //     Destroy(collision.gameObject);
        // }
        
        // if(collision.gameObject.CompareTag("Player"))
        // {
        //     PlayerController player = collision.gameObject.GetComponent<PlayerController>();
        //     CastedSpell spell = collision.gameObject.GetComponent<CastedSpell>();
        //     player.TakeDamage(spell.damage,spell.knockback,spell.direction,spell.effect);
        //     Debug.Log(player.Health);
        //     Destroy(collision.gameObject);
        // }
    }
}