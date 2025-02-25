


using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class ManaContainer : MonoBehaviour
{
    
    [SerializeField] List<Sprite> manaSprites;
    [SerializeField] TMP_Text manaLabel;
    [SerializeField] List<Mana> mana;
    public Mana currentMana;

    public int currentManaIndex;
    public int currentManaSpriteIndex;
    public float manaInterval; //total amount per heart
    public float manaSpriteInterval;
    public float maxPlayerMana;

    void Start(){

        currentManaSpriteIndex=0;
        GameManager.Instance.manaContainer = this;
        currentManaIndex = 0;
        currentMana = transform.GetChild(0).transform.GetChild(0).gameObject.GetComponent<Mana>();
        maxPlayerMana = GameManager.Instance.GetPlayer().maxMana;
        manaInterval = GameManager.Instance.GetPlayer().Mana/(mana.Count+1);
        manaSpriteInterval = manaInterval/manaSprites.Count;

        float min = GameManager.Instance.GetPlayer().Mana - manaInterval;
        float max = GameManager.Instance.GetPlayer().Mana;

        foreach(Transform child in transform.GetChild(0).transform){
            
            child.gameObject.GetComponent<Mana>().maxManaAmount = max;
            max -= manaInterval;

            child.gameObject.GetComponent<Mana>().minManaAmount = min;
            min -= manaInterval;

            child.gameObject.GetComponent<Mana>().SetSprite(manaSprites[currentManaSpriteIndex]);
        }
        
        manaLabel.text = $"{maxPlayerMana}/{maxPlayerMana}";

    }

    void FixedUpdate(){

        CheckForManaChange(GameManager.Instance.GetPlayer().Mana);

        CheckForManaSpriteChange(GameManager.Instance.GetPlayer().Mana);


    }

    public void CheckForManaSpriteChange(float currentManaAmount){
        if(currentManaAmount >= currentMana.maxManaAmount){
            mana[currentManaIndex-1].SetSprite(manaSprites[0]);
            // currentMana.SetSprite(manaSprites[0]);
        }
        else if (currentManaAmount <= currentMana.minManaAmount){
            mana[currentManaIndex+1].SetSprite(manaSprites.Last());
            // currentMana.SetSprite(manaSprites.Last());
        }
        else{
            float difference = currentMana.maxManaAmount - currentManaAmount;
        
            int difIndex = (int) (difference / manaSpriteInterval);
            int idx = Mathf.Clamp(difIndex, 0, manaSprites.Count-1);
            if(idx != currentManaSpriteIndex){
                currentManaSpriteIndex = idx;
                currentMana.SetSprite(manaSprites[currentManaSpriteIndex]);
            } 
        }
       
        
          manaLabel.text = $"{currentManaAmount}/{maxPlayerMana}";
    }

    public void CheckForManaChange(float currentManaAmount){
        float difference = maxPlayerMana - currentManaAmount;

        int difIndex = (int) (difference / manaInterval);
        
        int idx = Mathf.Clamp(difIndex, 0, mana.Count-1);

        if(idx != currentManaIndex  ){

            int manaIndexDifference = idx - currentManaIndex;
            if(manaIndexDifference > 1 ){
                for(int i = currentManaIndex-manaIndexDifference; i < currentManaIndex; i++){
                    mana[i].SetSprite(manaSprites.Last());
                }
            }
            else if(manaIndexDifference < -1){
                 for(int i = currentManaIndex-manaIndexDifference; i < currentManaIndex; i++){
                    mana[i].SetSprite(manaSprites.First());
                }
            }


            currentManaIndex = idx;
            currentMana = mana[currentManaIndex];
        }
    }
    




}