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
    

    void Start(){
         Body = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        behaviorTree = new BehaviorTree();
        lastMotionVector = Body.position.normalized;
        collider = GetComponent<CapsuleCollider2D>();
        
        InitializeBehaviorTree();
    }

    void DestroyEnemy(){
        Destroy(this);
    }
     void Update()
    {
        if(Health==0){
            DestroyEnemy();
        }

       
    }
    void FixedUpdate(){
         behaviorTree.Tick();
         Movement();
    }

    public void TakeDamage(int damage, float knockback, Vector2 direction, SpellEffect spellEffect){
        Health -= damage;
        DamageEffect(spellEffect);
        
        GetComponent<Rigidbody2D>().AddForce(direction*knockback);
    }

    public void DamageEffect(SpellEffect spellEffect){
        switch(spellEffect) 
        {
        case SpellEffect.Burn:
    
            Invoke("Burn",5);
            break;

        case SpellEffect.Poison:

            Invoke("Poison",5);
            break;

        default:

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