using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SpellButton : MonoBehaviour
{
    [SerializeField] Image icon;
    [SerializeField] public Spell spell;

    int myIndex;

    void Start(){
       foreach(Transform transform in transform){
         icon = transform.gameObject.GetComponent<Image>();
         break;
       }
    }


    void Update(){
        if(Input.inputString == (myIndex+1).ToString()){
            SelectSpell();
        }
    }
    public void SetIndex(int index){
        myIndex =index;
    }

    public void Set(SpellSlot slot){

        if(!transform.GetChild(0).gameObject.activeSelf){
            transform.GetChild(0).gameObject.SetActive(true);
        }
        
        
        transform.GetChild(0).gameObject.GetComponent<Image>().sprite = slot.spell.Icon;
        
        
        spell = slot.spell;
    }

    public void Clean(){
        transform.GetChild(0).GetComponent<Image>().sprite = null;
        transform.GetChild(0).gameObject.SetActive(false);
        spell = null;
    }

    public void SelectSpell(){
        
        
        GameManager.Instance.SetSpell(gameObject);
            
       
    }



}