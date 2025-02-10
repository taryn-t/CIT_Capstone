


using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class HeartsContainer : MonoBehaviour
{
    
    [SerializeField] List<Sprite> heartSprites;
    [SerializeField] TMP_Text healthLabel;
    [SerializeField] List<Heart> hearts;
    public Heart currentHeart;

    public int currentHeartIndex;
    public int currentSpriteIndex;
    public float heartHealthInterval; //total amount per heart
    public float spriteInterval;
    public float maxPlayerHealth;

    void Start(){

        currentSpriteIndex=0;
        GameManager.Instance.heartsContainer = this;
        currentHeartIndex = 0;
        currentHeart = transform.GetChild(0).transform.GetChild(0).gameObject.GetComponent<Heart>();
        maxPlayerHealth = GameManager.Instance.GetPlayer().maxHealth;
        heartHealthInterval = GameManager.Instance.GetPlayer().Health/hearts.Count;
        spriteInterval = heartHealthInterval/heartSprites.Count;

        float min = GameManager.Instance.GetPlayer().Health - heartHealthInterval;
        float max = GameManager.Instance.GetPlayer().Health;

        foreach(Transform child in transform.GetChild(0).transform){
            Debug.Log(child.gameObject.name);
            child.gameObject.GetComponent<Heart>().maxHeartHealth = max;
            max -= heartHealthInterval;

            child.gameObject.GetComponent<Heart>().minHeartHealth = min;
            min -= heartHealthInterval;

            child.gameObject.GetComponent<Heart>().SetSprite(heartSprites[currentSpriteIndex]);
        }
        
        healthLabel.text = $"{maxPlayerHealth}/{maxPlayerHealth}";

    }

    void Update(){

        CheckForHeartChange(GameManager.Instance.GetPlayer().Health);

        CheckForSpriteChange(GameManager.Instance.GetPlayer().Health);


    }

    public void CheckForSpriteChange(float currentHealth){
 
        float difference = currentHeart.maxHeartHealth - currentHealth;
        
        int idx = (int) (difference / spriteInterval);
        
        if(idx != currentSpriteIndex){
            currentSpriteIndex = idx;
            currentHeart.SetSprite(heartSprites[currentSpriteIndex]);
        }
        
          healthLabel.text = $"{currentHealth}/{maxPlayerHealth}";
    }

    public void CheckForHeartChange(float currentHealth){
        float difference = maxPlayerHealth - currentHealth;

        int idx = (int) (difference / heartHealthInterval);
        
        if(idx != currentHeartIndex){

            int heartIndexDifference = idx - currentHeartIndex;
            
            if(heartIndexDifference > 1 ){
                for(int i = currentHeartIndex-heartIndexDifference; i < currentHeartIndex; i++){
                    hearts[i].SetSprite(heartSprites.Last());
                }
            }
            else if(heartIndexDifference < -1){
                 for(int i = currentHeartIndex-heartIndexDifference; i < currentHeartIndex; i++){
                    hearts[i].SetSprite(heartSprites.First());
                }
            }


            currentHeartIndex = idx;
            currentHeart = hearts[currentHeartIndex];
        }
    }
    




}