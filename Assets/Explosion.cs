using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    // Start is called before the first frame update
    public Animator anim;
    int tick = default;
    public string caster;
    
   private List<GameObject> affectedBodies = new List<GameObject>();
    public float damage = 12f;
    bool damaging = false;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    
     IEnumerator DamageCollisions(GameObject go){


        if (go == null) yield return null;
        
        
                    
              
        yield return StartCoroutine(DamageCoroutine( go)) ;
                
        
        
    }

    IEnumerator DamageCoroutine(GameObject go){
        damaging = true;
        Vector2 direction = (transform.position- go.transform.position).normalized;

               
                        
        if(go.TryGetComponent<Character>(out var charc))
        {
             charc.TakeDamage(damage,0,direction);
        }    
        yield return new WaitForSeconds(0.5f);
        damaging = false;
    }

    // Update is called once per frame
    void Update()
    {
        // if(tick >=60){
        //     Destroy(gameObject,0.5f);
        // }
        // tick++;
         if(anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f){
            StopCoroutine("DamageCoroutine");
            StopCoroutine("DamageCollisions");
            Destroy(gameObject);
        }
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
