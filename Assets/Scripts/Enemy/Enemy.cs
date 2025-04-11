using System.Collections;

using UnityEngine;
using UnityEngine.Rendering.Universal;


public class Enemy :  EnemyAI{


    public string enemyName = "";

    [SerializeField] protected Sprite[] healthSprites;

    [SerializeField] protected SpriteRenderer healthMeter;

    [SerializeField] Color burnColor;
    [SerializeField] Color poisonColor;
    
    private SpriteRenderer renderer;

    private float spread = 1f;
    [SerializeField] GameObject[] potionDrops;
    [SerializeField] GameObject[] scrollDrops;
    [SerializeField] GameObject babySlime;
    private float _scrollDropChance = 0.1f;
    public float ScrollDropChance
    {
        get { return _scrollDropChance; }
        set {
                // if(GameManager.Instance.isDev){
                //     _scrollDropChance = Mathf.Clamp(value, 1, 1);  
                // }
                // else{
                   _scrollDropChance = Mathf.Clamp(value, 0.1f, 0.5f);  
                // }
             
             }
    }
    private float _potionDropChance = 0.3f;
     public float PotionDropChance
    {
        get { return _potionDropChance; }
        set { _potionDropChance = Mathf.Clamp(value, 0.2f, 0.7f); }
    }
    private int HealthInterval;
    private int currentHealthIndex = 0;

    
   
    
    


    
    void Awake(){
        damageGlow= GetComponentInChildren<Light2D>();
        damageGlow.intensity = 0;
    }

    void Start(){
        mapPanel = GameManager.Instance.mapPanel.GetComponent<MapPanel>();
        
        Health = maxHealth;
        Mana = maxMana;
        Body = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        behaviorTree = new BehaviorTree();
        lastMotionVector = Body.position.normalized;
        col = GetComponent<CapsuleCollider2D>();
        renderer = GetComponent<SpriteRenderer>();
        HealthInterval = (int) maxHealth/healthSprites.Length;

        InitializeBehaviorTree();
    }


    

    IEnumerator DestroyEnemy(){
        
        
        float dropChance = UnityEngine.Random.Range(0f,1f);

        Vector3 position = transform.position;
        position.x += spread * UnityEngine.Random.value - spread/2;
        position.y += spread * UnityEngine.Random.value - spread/2;
    
        if (dropChance <= PotionDropChance && dropChance > ScrollDropChance){
        
            int randomPotionIdx =  UnityEngine.Random.Range(0, potionDrops.Length);
            GameObject go =Instantiate(potionDrops[randomPotionIdx]);
            
            string key = go.GetComponent<PickUpPotion>().key;
            GameManager.Instance.hudController.wavePotions[key].totalSpawned++;

            
            go.transform.position = position;
            ResetPotionChance();
        }
        else{
            IncreasePotionChance();
        }

        if(GameManager.Instance.multiSpell){

            if(dropChance <= ScrollDropChance){

                int randomScrollIdx =  UnityEngine.Random.Range(0, scrollDrops.Length);
                GameObject go =Instantiate(scrollDrops[randomScrollIdx]);
                go.transform.position = position;
                string key = go.GetComponent<PickUpScroll>().key;

                GameManager.Instance.hudController.waveSpells[key].totalSpawned++;

                ResetScrollChance();

            }
            else{
                IncreaseScrollChance();
            }
        }         
        


       

        animator.SetBool("damage",true);
        
        GameManager.Instance.totalEnemies--;
        GameManager.Instance.enemiesDefeated++;

        
        GameManager.Instance.hudController.currentWave.enemiesDefeated++; 
        
        yield return StartCoroutine(mapPanel.RemoveMarker(gameObject)); 
      
        Destroy(gameObject);  
       

    }


      IEnumerator DestroyEnemySlime(){
        
        System.Random rand = new System.Random();

        int babyAmount = rand.Next(2,4);

        Vector3 position = transform.position;
        position.x += spread * UnityEngine.Random.value - spread/2;
        position.y += spread * UnityEngine.Random.value - spread/2;
   

        for(int i = 0; i< babyAmount; i++){
            GameObject go = Instantiate(babySlime);
            go.transform.position = position;
            GameManager.Instance.totalEnemies++;
        }
            
        
        


       

        animator.SetBool("damage",true);
        
        GameManager.Instance.totalEnemies--;
        GameManager.Instance.hudController.currentWave.enemiesDefeated++; 

        
        

        yield return StartCoroutine(mapPanel.RemoveMarker(gameObject)); 

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

        
        

        if(Health==0 && !dead){

            dead=true;
            StartCoroutine(GameManager.Instance.hudController.ShowPopupMessage($"{enemyName} Defeated"));
            if(!parent){
               StartCoroutine(DestroyEnemy()); 
         
            }else{
                StartCoroutine(DestroyEnemySlime());
                
            }

            
            
        }

        
        CheckForHealthChange(Health); 
        
         
        CastAttackCone();
        
       
    }
    void FixedUpdate(){


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
            behaviorTree.Tick();

             Movement(); 
        }
        

        
        if(mapPanel.open){
            mapPanel.SetMarker(gameObject, mapMarker);
        }

        
        if( !manaRegenerating && Mana < maxMana){
            StartCoroutine(ManaRegen());
        }
    }



// private void OnDrawGizmosSelected()
//     {
//         float angleRange = Mathf.PI / 3;      // 60-degree cone
//         float distance = 5f;
//         int rayCount = 10;

//         if (lastMotionVector == Vector2.zero) return; // Prevent division errors

//         Gizmos.color = Color.red;
//         Vector2 origin = transform.position;

//         // Calculate the base direction angle from the normalized vector
//         float directionAngle = Mathf.Atan2(lastMotionVector.y, lastMotionVector.x); // Get angle in radians

//         // Calculate angle bounds
//         float halfAngle = angleRange * Mathf.Deg2Rad / 2f;
//         float startAngle = directionAngle - halfAngle;
//         float endAngle = directionAngle + halfAngle;

//         for (int i = 0; i < rayCount; i++)
//         {
//             float angle = Mathf.Lerp(startAngle, endAngle, (float)i / (rayCount - 1));
//             Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

//             Gizmos.DrawRay(origin, direction * distance);
//         }
//     }

  
    void CastAttackCone()
    {

       if (lastMotionVector == Vector2.zero ) return; 

        if(!GameManager.Instance.GetPlayer().visible){
            playerInRayAttack = false;
            return;
        }
        
        float angleRange = 60f;  
        int rayCount = 10;
        Vector2 origin = transform.position;

        float directionAngle = Mathf.Atan2(lastMotionVector.y, lastMotionVector.x);

        float halfAngle = angleRange * Mathf.Deg2Rad / 2f;
        float startAngle = directionAngle - halfAngle;
        float endAngle = directionAngle + halfAngle;
        int collisions =0;

        for (int i = 0; i < rayCount; i++)
        {
            float angle = Mathf.Lerp(startAngle, endAngle, (float)i / (rayCount - 1));
            Vector2 rayDirection = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

            // Perform the actual raycast
            RaycastHit2D hit = Physics2D.Raycast(origin, rayDirection, attackRange, playerLayerMask);

            // Debugging: Draw the rays in Scene view
            Debug.DrawRay(origin, rayDirection * attackRange, Color.red, 0.1f);

            if (hit.collider != null)
            {
                collisions++;
            }

        }

        if (collisions > 0)
        {
            playerInRayAttack = true;
        }else{
                playerInRayAttack = false;
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

    void OnCollisionEnter2D(Collision2D collision){

        if(collision.gameObject.CompareTag("Player")){
            PlayerController player = GameManager.Instance.player.GetComponent<PlayerController>();
            Vector2 direction = Vector2.zero;
            SpellEffect effect = babySlime ? SpellEffect.Poison : SpellEffect.None;

            int damage = babySlime ? 8 : 5;
            player.TakeDamage(damage,2,direction, effect);
        }
    }
 

 

  
}