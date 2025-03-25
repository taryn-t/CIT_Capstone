


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
    public int manaPerHeart; //total amount per heart
    public float manaSpriteInterval;
    public int maxPlayerMana;

    void Start(){

        currentManaSpriteIndex=0;
        GameManager.Instance.manaContainer = this;
        currentManaIndex = 0;
        currentMana = transform.GetChild(0).transform.GetChild(0).gameObject.GetComponent<Mana>();
        maxPlayerMana = (int)GameManager.Instance.GetPlayer().maxMana;
        manaPerHeart = maxPlayerMana/5;
        manaSpriteInterval = manaPerHeart/manaSprites.Count;

        float min = GameManager.Instance.GetPlayer().Mana - manaPerHeart;
        float max = GameManager.Instance.GetPlayer().Mana;

        foreach(Transform child in transform.GetChild(0).transform){
            
            child.gameObject.GetComponent<Mana>().maxManaAmount = max;
            max -= manaPerHeart;

            child.gameObject.GetComponent<Mana>().minManaAmount = min;
            min -= manaPerHeart;

            child.gameObject.GetComponent<Mana>().SetSprite(manaSprites[currentManaSpriteIndex]);
        }
        
        manaLabel.text = $"{maxPlayerMana}/{maxPlayerMana}";

    }

    void FixedUpdate(){
        

         UpdateManaDisplay(GameManager.Instance.GetPlayer().Mana);
        maxPlayerMana = (int)GameManager.Instance.GetPlayer().maxMana;
        manaLabel.text = $"{GameManager.Instance.GetPlayer().Mana}/{maxPlayerMana}";


    }


      void UpdateManaDisplay(float currentMana)
    {

        for (int i = 0; i < manaSprites.Count; i++)
        {
            int reversedIndex = (manaSprites.Count - 1) - i; 
            int heartStart = manaPerHeart * (i + 1);
            int heartEnd = manaPerHeart * i;

            if (currentMana >= heartStart)
            {
                 mana[reversedIndex].SetSprite(manaSprites[4]); // Full heart
            }
            else if (currentMana <= heartEnd)
            {
                mana[reversedIndex].SetSprite(manaSprites[0]); // Empty heart
            }
            else
            {
                int index = Mathf.FloorToInt((currentMana - heartEnd) / (manaPerHeart / 5));
                mana[reversedIndex].SetSprite(manaSprites[index]);
            }
        }
    }
   




}