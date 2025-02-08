using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;

public class PotionPanel : MonoBehaviour
{
    private PotionContainer potions;
    [SerializeField] PotionButton button;

    private void Start(){
        potions = GameManager.Instance.availablePotions;
        SetIndex();
        
        button.Set(potions.slots[0].potion);
    }

    private void SetIndex()
    {
        button.SetIndex(0);
       
    }

}
