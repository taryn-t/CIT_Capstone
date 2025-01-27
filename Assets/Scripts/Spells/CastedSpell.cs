

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



    void Start(){
         direction = GameManager.Instance.playerMovement.GetComponent<PlayerMovement>().lastMotionVector;
         transform.rotation = rotation;
    }  

    void FixedUpdate()
    {
        if(tick <=60){
          transform.Translate(speed * Time.deltaTime * direction);  
          transform.rotation = rotation;
        }
        else{
            Destroy(gameObject);
        }

        tick++;
        
    }

    void OnTriggerEnter(Collider collision)
    {
        if(collision.gameObject.tag == "Enemy"){
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();

            enemy.TakeDamage(damage,knockback,direction,effect);
        }
        
        if(collision.gameObject.tag == "Player"){
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();

            player.TakeDamage(damage,knockback,direction,effect);
        }
    }
}