

using UnityEngine;

public class CastedSpell: MonoBehaviour
{
    private float speed = 1.0f;

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
    CapsuleCollider2D collider;

    void Start(){
         direction = GameManager.Instance.playerMovement.GetComponent<PlayerMovement>().lastMotionVector;
         transform.rotation = rotation;
         body = GetComponent<Rigidbody2D>();
         renderer = GetComponent<SpriteRenderer>();
         renderer.sprite = spell.frames[0];
         collider = GetComponent<CapsuleCollider2D>();

    }  

    void FixedUpdate()
    {

        if(tick <=60){
            if(tick%spell.frames.Length==0){
                index++;
                if(index< spell.frames.Length){
                     renderer.sprite = spell.frames[index];
                }
            }
            // collider.offset -=  direction;
          body.AddForce(speed * Time.deltaTime * direction);
          
          transform.rotation = rotation;
        }
        else{
            Destroy(gameObject);
        }

        tick++;
        
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Spell");
        if(collision.gameObject.CompareTag(caster)){
            return;
        }
        
         if(collision.gameObject.CompareTag("Enemy"))
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            
            enemy.TakeDamage(damage,knockback,direction,effect);
            Debug.Log(enemy.Health);

            if(enemy.Health==0){
                Destroy(collision.gameObject);
                
            }
            Destroy(gameObject,1f);
        }
        
        if(collision.gameObject.CompareTag("Player"))
        {
            PlayerController player = GameManager.Instance.player.GetComponent<PlayerController>();
            
            player.TakeDamage(damage,knockback,direction,effect);
           

            
            Destroy(gameObject,1f);
        }
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
   