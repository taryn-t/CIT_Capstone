using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using UnityEngine.UIElements;
public class PickUpPotion : MonoBehaviour
{
    // Start is called before the first frame update

    Transform player;
    [SerializeField] float speed = 0.2f;
    [SerializeField] float pickUpDistance = 0.01f;
    [SerializeField] float ttl = 90f;
    [SerializeField] public Potion potion;
    
    void Start()
    {
        
        player = GameManager.Instance.player.transform;

       
    }




    // Update is called once per frame
    void Update()
    {
        ttl -= Time.deltaTime;
        if(ttl<=0){Destroy(gameObject);}
         StartCoroutine(PickUp());
    }

     IEnumerator PickUp()
    {

        float distance = Vector3.Distance(transform.position, player.position);

        if(distance > pickUpDistance){
            yield return null;
        }

        yield return new WaitForSeconds(1f);

        transform.position = Vector3.MoveTowards( 
            transform.position,
            player.position,
            speed*Time.deltaTime
        );

        if(distance < 0.1f){
            
            if(GameManager.Instance.potionButton.potion == null){
                Debug.Log("Adding potion");
                GameManager.Instance.potionButton.Set(potion);
                Destroy(gameObject);
            }
            
            
        }
       
        
    }


}
