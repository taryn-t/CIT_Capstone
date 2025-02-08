using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Heart : MonoBehaviour
{
    public Image icon;
    public float maxHeartHealth; //max player health for current heart
    public float minHeartHealth; //minimum player health for current heart
   public float lastHealth;


    void Start(){

        lastHealth = maxHeartHealth;

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