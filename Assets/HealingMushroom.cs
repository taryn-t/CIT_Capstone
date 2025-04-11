using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingMushroom : WorldItem
{
   
    public float healAmount = 25f;

    // Update is called once per frame
    void Update()
    {

        if(messageBox.activeSelf && Input.GetKeyDown(KeyCode.E)){
            PickUp();
        }
    }

    public override void PickUp()
    {
        StartCoroutine(GameManager.Instance.hudController.ShowPopupMessage($"Mushroom +{healAmount} health"));
        GameManager.Instance.GetPlayer().Health += healAmount;

         GameManager.Instance.hudController.currentWave.mushroomsUsed++;
        
        base.PickUp();
    }
    void OnDestroy()
    {
        StopAllCoroutines();
    }

}
