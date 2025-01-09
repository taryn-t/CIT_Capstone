using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    
    void Start()
    {
        GetComponent<InventoryController>().Init();
        GameManager.Instance.SetPlayer(this.gameObject);
    }

    void Update()
    {

    }
   
   
}