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
    private string goTag = "PotionButton";
    public string key = "";
    
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

        yield return new WaitForSeconds(0.5f);

        transform.position = Vector3.MoveTowards( 
            transform.position,
            player.position,
            speed*Time.deltaTime
        );

        if(distance < 0.3f){
            
            foreach(PotionButton pb in GameManager.Instance.potionButtons){

                if(pb.potion == potion){
                    GameManager.Instance.soundEffectController.PlayPositiveSound();
                    pb.Set(potion);
                
                    GameManager.Instance.hudController.wavePotions[key].pickedUp++;
                    StartCoroutine(GameManager.Instance.hudController.ShowPopupMessage($"Picked Up {potion.Name} Potion"));
                    
                    Destroy(gameObject);
                }
                
            }
            
         
            
            
        }
       
        
    }


}
