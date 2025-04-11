using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FairyJar : WorldItem
{


    // Start is called before the first frame update
    void Start()
    {
        messageBox.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

        if(messageBox.activeSelf && Input.GetKeyDown(KeyCode.E)){
            PickUp();
        }
    }


    public override void PickUp()
    {
        GameManager.Instance.hudController.UpdateFairyJar();
        
        base.PickUp();
    }
}
