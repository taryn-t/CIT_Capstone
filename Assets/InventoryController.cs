using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    public GameObject panel;


    void Start(){
        
        // panel.SetActive(false);
    }
    private void Update(){
        
        if(Input.GetKeyDown(KeyCode.Tab)){
             panel.SetActive(!panel.activeInHierarchy);
        }
    }

    public void Init(){
        panel = GameObject.FindGameObjectWithTag("Inventory");
        foreach(Transform transform in panel.transform){
            transform.gameObject.SetActive(true);
        }
        panel.SetActive(false);
    }
}
