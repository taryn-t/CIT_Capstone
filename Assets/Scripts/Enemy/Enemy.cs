using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class Enemy : EnemyAI{


    public float Health
    {
        get { return _health; }
        set { _health = Mathf.Clamp(value, 0, 52); }
    }

    private float _health = 52;
    private float maxHealth;
     public float Mana
    {
        get { return _mana; }
        set { _mana = Mathf.Clamp(value, 0, 52); }
    }

    private float _mana = 100;

    public float speed = 2f;
    [SerializeField] protected Sprite[] healthSprites;

    [SerializeField] protected SpriteRenderer healthMeter;
    float startTime;

    [SerializeField] Color burnColor;
    [SerializeField] Color poisonColor;
    
    private Color endColor;
    private Color startColor;
    private SpriteRenderer renderer;
    Collider2D[] results ={};
    Light2D damageGlow;

    ContactFilter2D cf;
    
   CancellationTokenSource damageCancellation;
    private int dropCount = 1;
    private float spread = 1f;
    [SerializeField] GameObject[] potionDrops;
    [SerializeField] GameObject[] scrollDrops;
    private float _scrollDropChance = 0.01f;
     public float ScrollDropChance
    {
        get { return _scrollDropChance; }
        set { _scrollDropChance = Mathf.Clamp(value, 0.01f, 0.1f); }
    }
    private float _potionDropChance = 0.3f;
     public float PotionDropChance
    {
        get { return _potionDropChance; }
        set { _potionDropChance = Mathf.Clamp(value, 0.3f, 0.5f); }
    }
    private int HealthInterval;
    private int currentHealthIndex = 0;



    void Awake(){
        damageGlow= GetComponentInChildren<Light2D>();
        damageGlow.intensity = 0;
    }

    void Start(){
        maxHealth = Health;
        Body = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        behaviorTree = new BehaviorTree();
        lastMotionVector = Body.position.normalized;
        col = GetComponent<CapsuleCollider2D>();
        renderer = GetComponent<SpriteRenderer>();
        startColor = renderer.color;
        HealthInterval = (int) maxHealth/healthSprites.Length;

        cf = new ContactFilter2D();
        InitializeBehaviorTree();
        
    }
      private void OnDestroy()
    {
        
        damageCancellation?.Cancel();
    }


    void DestroyEnemy(){
        

        float dropChance = UnityEngine.Random.Range(0f,1f);

        Vector3 position = transform.position;
        position.x += spread * UnityEngine.Random.value - spread/2;
        position.y += spread * UnityEngine.Random.value - spread/2;
        Debug.Log(dropChance);

         if (dropChance <= PotionDropChance && dropChance > ScrollDropChance){
            
            int randomPotionIdx =  UnityEngine.Random.Range(0, potionDrops.Length);
            GameObject go =Instantiate(potionDrops[randomPotionIdx]);
            go.transform.position = position;
            ResetPotionChance();
        }
        else{
            IncreasePotionChance();
        }

        if(dropChance <= ScrollDropChance){

            int randomScrollIdx =  UnityEngine.Random.Range(0, scrollDrops.Length);
            GameObject go =Instantiate(scrollDrops[randomScrollIdx]);
            go.transform.position = position;
            ResetScrollChance();

        }
        else{
            IncreaseScrollChance();
        }


       

        animator.SetBool("damage",true);
        
        GameManager.Instance.totalEnemies--;
        
        Destroy(gameObject);  

    }

    void ResetPotionChance()
    {
        PotionDropChance = 0.1f;
    }
    void ResetScrollChance()
    {
        ScrollDropChance = 0.01f;
    }
     void IncreasePotionChance()
    {
        PotionDropChance += 0.02f;
    }
    void IncreaseScrollChance()
    {
        ScrollDropChance += 0.01f;
    }

    void Update()
    {

        

        if(Health==0){
            DestroyEnemy();
        }

        
        CheckForHealthChange(Health); 
        
         

        
        
       
    }
    void FixedUpdate(){
        
         behaviorTree.Tick();

         Movement();
         
    }



    public async void  TakeDamage(int damage, float knockback, Vector2 direction, SpellEffect spellEffect){
        damageCancellation = new CancellationTokenSource();
        try
        {
            damaged = true;
            Health -= damage;
            DamageEffect(spellEffect);

            if(!animator.GetBool("damage")){
                animator.SetBool("damage",true);
                damageGlow.intensity = 1f;
            }
            
            // if(damaged && Health % healthSprites.Length-1 == 0 && Health > 0){ 
            //     healthIndex++;
            //     healthMeter.sprite = healthSprites[healthIndex];
            // }
            startTime = Time.time;
            
            await Task.Delay(1000,damageCancellation.Token);
            
            animator.SetBool("damage",false); 
            damageGlow.intensity = 0;
            damaged=false;
                
            
           await Task.Delay(500,damageCancellation.Token);
            
        }
        catch
        {
            
            return;
        }
        finally
        {
            if(damageCancellation != null){
                damageCancellation.Dispose();
                damageCancellation = null;
            }
            
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

    
     public void CheckForHealthChange(float currentHealth){
        float difference = maxHealth - currentHealth;

        int idx = (int) (difference / HealthInterval);
        
           if(idx != currentHealthIndex && idx < healthSprites.Length){
            currentHealthIndex = idx;
            healthMeter.sprite  = healthSprites[currentHealthIndex];
        }
    }
 

 

  
}