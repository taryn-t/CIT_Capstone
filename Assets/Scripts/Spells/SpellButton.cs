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
        foreach(Transform transform in transform){
            transform.gameObject.GetComponent<Image>().sprite = slot.spell.Icon;
            break;
        }
        
        spell = slot.spell;
    }

    public void Clean(){
        foreach(Transform transform in transform){
            transform.gameObject.GetComponent<Image>().sprite = null;
            transform.gameObject.SetActive(false);
            break;
        }
        spell = null;
    }

    public void SelectSpell(){
        
        
        GameManager.Instance.SetSpell(gameObject);
            
       
    }



}