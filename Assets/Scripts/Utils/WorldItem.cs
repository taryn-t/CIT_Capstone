using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldItem : MonoBehaviour
{
    [SerializeField] public GameObject messageBox;
   
    void Start()
    {
        messageBox.SetActive(false);
    }

    // Update is called once per frame


    public virtual void PickUp(){
        GameManager.Instance.soundEffectController.PlayPositiveSound();
        Destroy(gameObject);
    }
      private void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.CompareTag("Player")){
            messageBox.SetActive(true);
        }
        
    }
        private void OnCollisionExit2D(Collision2D other)
    {
        if(messageBox.activeSelf){
          
            messageBox.SetActive(false);
        
        }
    }
}
