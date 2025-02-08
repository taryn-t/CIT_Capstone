using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PotionButton : MonoBehaviour
{
    [SerializeField] Image icon;
    [SerializeField] public Potion potion;

    int myIndex;

    void Start(){
        GameManager.Instance.potionButton = this;
       foreach(Transform transform in transform){
         icon = transform.gameObject.GetComponent<Image>();
         
       }
    }


    void Update(){
        if(potion!=null ){
            if( Input.GetKeyDown(KeyCode.E)){
                UsePotion();
            }
            
        }
    }
    public void SetIndex(int index){
        myIndex =index;
    }

    public void Set(Potion slot){
        transform.GetChild(0).GetComponent<Image>().sprite = slot.icon;
        
        potion = slot;
    }

    public void Clean(){
        transform.GetChild(0).GetComponent<Image>().sprite = null;
        transform.GetChild(0).gameObject.SetActive(false);
        
        potion = null;
    }

    public void UsePotion(){
        potion.potionEffect.OnApply(potion);   
    }



}