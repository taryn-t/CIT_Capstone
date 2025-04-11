using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHole : MonoBehaviour
{
        // Start is called before the first frame update
    Animator anim;
    int tick = default;
    public string caster;
    public float gravityRadius;
    public float gravityPull = 10f;
    private List<Rigidbody2D> affectedBodies = new List<Rigidbody2D>();
    public float damage = 2f;
    bool damaging = false;
    
    void Start()
    {
        anim = GetComponent<Animator>();
        gravityRadius = GetComponent<CircleCollider2D>().radius;

        StartCoroutine(DestroyObject());
    }




    IEnumerator DestroyObject(){

        yield return new WaitForSeconds(5f);
        StopCoroutine("DamageCollisions");
        Destroy(gameObject);
        
    }


    void FixedUpdate()
    {
         foreach (Rigidbody2D rb in affectedBodies)
        {
            if (rb == null) continue;

           
        }

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, gravityRadius);


        foreach (Collider2D hit in hits)
        {
            if(hit.gameObject.CompareTag(caster)){break;}
            Rigidbody2D rb = hit.attachedRigidbody;

            try{
                Vector2 direction = (transform.position - rb.transform.position).normalized;
                rb.AddForce(direction * 100f , ForceMode2D.Force);
            }
            catch{

            }
        
            

              
        }
    }
    IEnumerator DamageCollisions(GameObject go){
       

        if (go == null) yield return null;
            
        yield return StartCoroutine(DamageCoroutine( go)) ;
                        
         
        
    }

    IEnumerator DamageCoroutine(GameObject go){
        damaging = true;
        
        if(go == null){
            yield return null;
        }
        else{
            Vector2 direction = (transform.position- go.transform.position).normalized;

            Debug.Log(caster);

            if(caster == "Enemy"){
                if(go.TryGetComponent<PlayerController>(out var player))
                {
                    player.TakeDamage(damage,0,direction,SpellEffect.Gravity);
                }     
            }
            else{
                if(go.TryGetComponent<Enemy>(out var enemy))
                {
                    enemy.TakeDamage(damage,0,direction,SpellEffect.Gravity);
                }     
            }       
            yield return new WaitForSeconds(0.75f);
            damaging = false;
        }
        
      
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if(!other.gameObject.CompareTag(caster) && !damaging){
            GameObject rb = other.gameObject;
      
            StartCoroutine(DamageCollisions(rb));
        }
        
        
    }

        void OnDestroy()
    {
        StopAllCoroutines();
    }
    
}
