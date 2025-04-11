
using UnityEngine;

using System.Collections;

public class CastedSpell: MonoBehaviour
{
     float speed = 1f;

    public Vector2 direction;
    public Quaternion rotation;

    private float lifeTime = 3f;
    private int tick = default;
    public int damage;
    public float knockback;

    public SpellEffect effect;

    public string caster;
    Rigidbody2D body;
    public Spell spell;
    int index = default;
    SpriteRenderer renderer;
    public Animator casterAnimator;
    public bool colliding = false;
    public Vector2 targetPosition;
    public Vector2 startPosition;
    public float range = 10f;
    private Vector2 previousPosition;
    private float totalDistanceTraveled = 0f;
    void Start(){
        
         
         transform.rotation = rotation;
         body = GetComponent<Rigidbody2D>();
         renderer = GetComponent<SpriteRenderer>();
         renderer.sprite = spell.frames[0];

         startPosition = body.position;
        
        direction = (targetPosition-body.position).normalized;    
        previousPosition = body.position;

        StartCoroutine(SmoothMovement( ));
    }
    public void Update()
    {
       
    }

    public virtual void FixedUpdate()
    {
        float distanceThisFrame = Vector2.Distance(body.position, previousPosition);
        totalDistanceTraveled += distanceThisFrame;

        // Update previous position
        previousPosition = body.position;

        transform.rotation = rotation;
        
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        
        colliding = true;
        if(!collision.gameObject.CompareTag(caster)){
            StopCoroutine("SmoothMovement");
            Character character = collision.gameObject.GetComponent<Character>();
        
            if(character!=null ){
                
                character.TakeDamage(damage,knockback,direction, spell.spellEffect); 
            }
            HitEffect();
        
        }
        
            
        
         
        
        
    }

     IEnumerator  SmoothMovement( )
    {   
        bool finish = false;

         if(caster == "Player"){
            GameManager.Instance.GetPlayer().lastMotionVector = direction;
        }
                
            
        while(!finish)
        {
            body.AddForce(direction * speed , ForceMode2D.Impulse);

            yield return new WaitForFixedUpdate();

            if(totalDistanceTraveled >= range){
                break;
            }
            
            finish = IsRigidbodyNear(targetPosition, body);
            
            
        } 
        
         
           
        HitEffect();
        
}

    bool IsRigidbodyNear(Vector2 position, Rigidbody2D rb, float tolerance = 0.1f)
    {
        return Mathf.Abs(rb.position.x - position.x) <= tolerance &&
            Mathf.Abs(rb.position.y - position.y) <= tolerance;
    }

    public void HitEffect(){
        GameObject go;
        switch(effect) 
        {
        case SpellEffect.Explode:
            
            go = Instantiate(GameManager.Instance.explosionGO, transform.position, Quaternion.identity, GameManager.Instance.SpellsGO.transform);
            go.GetComponent<Explosion>().caster = caster;
            break;

        case SpellEffect.Poison:
            go =Instantiate(GameManager.Instance.venomPillarGO, transform.position, Quaternion.identity, GameManager.Instance.SpellsGO.transform);
            go.GetComponent<VenomPillar>().caster = caster;
            break;
        case SpellEffect.Gravity:
            go =Instantiate(GameManager.Instance.blackHoleGO, transform.position, Quaternion.identity, GameManager.Instance.SpellsGO.transform);
            go.GetComponent<BlackHole>().caster = caster;
            break;    

        default:
           break;
        }

        Destroy(gameObject);
    }


    // void OnTriggerEnter(Collider collision)
    // {
    //     Debug.Log("Spell");
       
    //     if(collision.gameObject.CompareTag("Enemy"))
    //     {
    //         Enemy enemy = collision.gameObject.GetComponent<Enemy>();
    //         CastedSpell spell = collision.gameObject.GetComponent<CastedSpell>();
    //         enemy.TakeDamage(spell.damage,spell.knockback,spell.direction,spell.effect);
    //         Debug.Log(enemy.Health);
    //         Destroy(collision.gameObject);
    //     }
        
    //     if(collision.gameObject.CompareTag("Player"))
    //     {
    //         PlayerController player = collision.gameObject.GetComponent<PlayerController>();
    //         CastedSpell spell = collision.gameObject.GetComponent<CastedSpell>();
    //         player.TakeDamage(spell.damage,spell.knockback,spell.direction,spell.effect);
    //         Debug.Log(player.Health);
    //         Destroy(collision.gameObject);
    //     }
    // }
}
   