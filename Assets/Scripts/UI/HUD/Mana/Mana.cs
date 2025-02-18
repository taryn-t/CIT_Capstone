using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Mana : MonoBehaviour
{
    public Image icon;
    public float maxManaAmount; //max player health for current heart
    public float minManaAmount; //minimum player health for current heart
   public float lastMana;


    void Start(){

        lastMana = maxManaAmount;

        if(icon == null){
            foreach(Transform transform in transform){
                icon = transform.gameObject.GetComponent<Image>();
                break;
            }
        }
      
    }


    public void SetSprite(Sprite sprite){
        
        
            icon.overrideSprite  = sprite;
        
        
        
    }






}