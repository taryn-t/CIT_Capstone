

using UnityEngine;
using System.Threading;
using System.Threading.Tasks;

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
    CapsuleCollider2D collider;
    public Animator casterAnimator;
    private CancellationTokenSource cancellation;
    public Rigidbody2D target;

    void Start(){
        
         
         transform.rotation = rotation;
         body = GetComponent<Rigidbody2D>();
         renderer = GetComponent<SpriteRenderer>();
         renderer.sprite = spell.frames[0];
         collider = GetComponent<CapsuleCollider2D>();

         if(caster =="Enemy"){
            SmoothMovement( );
         }else{
            direction = GameManager.Instance.playerMovement.GetComponent<PlayerMovement>().lastMotionVector;
         }
        
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

            if(caster== "Player"){
                  // collider.offset -=  direction;
                 body.AddForce(direction * speed , ForceMode2D.Impulse);
            }
          
          
          transform.rotation = rotation;
        }
        else{
            Destroy(gameObject);
        }

        tick++;
        
    }
    private void OnDestroy()
    {
        cancellation?.Cancel();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        
        if(collision.gameObject.CompareTag(caster)){
            return;
        }
        
         if(collision.gameObject.CompareTag("Enemy"))
        {
            cancellation = new CancellationTokenSource();
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            
            enemy.TakeDamage(damage,knockback,direction,effect);

        }
        
        if(collision.gameObject.CompareTag("Player"))
        {
            PlayerController player = GameManager.Instance.player.GetComponent<PlayerController>();
            
            player.TakeDamage(damage,knockback,direction,effect);

        }
        
          Destroy(gameObject);  
        
        
    }

     async void SmoothMovement( )
    {   
         cancellation = new CancellationTokenSource();

        try
        {
    
            Vector3 direction =(target.position-body.position).normalized; 
        
          
                while(!Mathf.Approximately(Vector2.Distance(body.position, target.position), 0f))
                    {
                           
                         body.AddForce(direction * speed , ForceMode2D.Impulse);

                        await Task.Delay(20, cancellation.Token);
                        
                    }
            
           
           
        }
        catch
        {
            
            return;
        }
        finally
        {
            cancellation.Dispose();
            cancellation = null;
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
   