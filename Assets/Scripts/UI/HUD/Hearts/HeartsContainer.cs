


using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HeartsContainer : MonoBehaviour
{
    
    [SerializeField] List<Sprite> heartSprites;
    [SerializeField] TMP_Text healthLabel;
    [SerializeField] List<Heart> hearts;
    public Heart currentHeart;

    public int currentHeartIndex;
    public int currentSpriteIndex;
    public int healthPerHeart; //total amount per heart
    public float spriteInterval;
    public int maxPlayerHealth;

    void Start(){

        currentSpriteIndex=0;
        GameManager.Instance.heartsContainer = this;
        currentHeartIndex = 0;
        currentHeart = transform.GetChild(0).transform.GetChild(0).gameObject.GetComponent<Heart>();
         SetInterval();

        float min = GameManager.Instance.GetPlayer().Health - healthPerHeart;
        float max = GameManager.Instance.GetPlayer().Health;

        foreach(Transform child in transform.GetChild(0).transform){
            
            child.gameObject.GetComponent<Heart>().maxHeartHealth = max;
            max -= healthPerHeart;

            child.gameObject.GetComponent<Heart>().minHeartHealth = min;
            min -= healthPerHeart;

            child.gameObject.GetComponent<Heart>().SetSprite(heartSprites[currentSpriteIndex]);
        }
        
        healthLabel.text = $"{maxPlayerHealth}/{maxPlayerHealth}";

    }
    public void SetInterval(){
        maxPlayerHealth = (int)GameManager.Instance.GetPlayer().maxHealth;
        healthPerHeart = maxPlayerHealth / 5;
        spriteInterval = healthPerHeart/heartSprites.Count;
    }

    void FixedUpdate(){

        UpdateHealthDisplay(GameManager.Instance.GetPlayer().Health);

        maxPlayerHealth = (int)GameManager.Instance.GetPlayer().maxHealth;

        healthLabel.text = $"{(int)GameManager.Instance.GetPlayer().Health}/{maxPlayerHealth}";
    }

     void UpdateHealthDisplay(float currentHealth)
    {

        for (int i = 0; i < heartSprites.Count; i++)
        {
            int reversedIndex = Mathf.Clamp((hearts.Count - 1) - i, 0, hearts.Count-1 ); 

            int heartStart = healthPerHeart * (i + 1);
            int heartEnd = healthPerHeart * i;

            if (currentHealth >= heartStart)
            {
                 hearts[reversedIndex].SetSprite(heartSprites[4]); // Full heart
            }
            else if (currentHealth <= heartEnd)
            {
                hearts[reversedIndex].SetSprite(heartSprites[0]); // Empty heart
            }
            else
            {
                int index = Mathf.FloorToInt((currentHealth - heartEnd) / (healthPerHeart / 5));
                index = Mathf.Clamp(index,0,heartSprites.Count-1);
                hearts[reversedIndex].SetSprite(heartSprites[index]);
            }
        }
    }
   





}