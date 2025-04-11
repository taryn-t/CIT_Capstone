using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VenomPillar : MonoBehaviour
{
     // Start is called before the first frame update
    Animator anim;
    int tick = default;
    public string caster;
    private List<GameObject> affectedBodies = new List<GameObject>();
    public float damage = 1f;
    bool damaging = false;
   void Start()
    {
        anim = GetComponent<Animator>();
        StartCoroutine( DestroyObject());
    }

    // Update is called once per frame
    
       IEnumerator DamageCollisions(GameObject go){


        if (go == null) yield return null;
                       
              
        yield return StartCoroutine(DamageCoroutine( go)) ;
                


    }

    IEnumerator DamageCoroutine(GameObject go){
        damaging = true;
        

        if(go == null){
            yield return null;
        }
        
        Vector2 direction = (transform.position- go.transform.position).normalized;

               
        if(caster == "Enemy"){
            if(go.TryGetComponent<PlayerController>(out var player))
            {
                player.TakeDamage(damage,0,direction,SpellEffect.Poison);
            }     
        }
        else{
            if(go.TryGetComponent<Enemy>(out var enemy))
            {
                enemy.TakeDamage(damage,0,direction,SpellEffect.Poison);
            }     
        }          
        yield return new WaitForSeconds(1f);
        
        damaging = false;
    }
    // Update is called once per frame
    void Update()
    {
        // if(tick >=60){
        //     Destroy(gameObject,0.5f);
        // }
        // tick++;
        //  if(anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f){
        //     StopCoroutine("DamageCollisions");
        //     StopCoroutine("DamageCoroutine");
        //     Destroy(gameObject);
        // }
    }
      IEnumerator DestroyObject(){

        yield return new WaitForSeconds(5f);
        StopCoroutine("DamageCollisions");
            StopCoroutine("DamageCoroutine");
        Destroy(gameObject);
        
    }

      private void OnTriggerStay2D(Collider2D other)
    {
        if(!other.gameObject.CompareTag(caster) && !damaging){
            GameObject go = other.gameObject;
           

             StartCoroutine(DamageCollisions(go));
        }
        
    }

    void OnDestroy()
    {
        StopAllCoroutines();
    }
}
