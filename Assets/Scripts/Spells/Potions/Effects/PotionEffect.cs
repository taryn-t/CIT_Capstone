using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionEffect : ScriptableObject
{
    [SerializeField] public Color effectColor;
    [SerializeField] public string label;
    public PotionButton potionButton;
    

    public virtual bool OnApply(Potion potion){
        foreach(PotionButton pb in GameManager.Instance.potionButtons){
            if(pb.potion == potion){
                potionButton = pb;
                potionButton.Clean();
                GameManager.Instance.hudController.currentWave.potionsUsed++; 
                return true;
            }
        }
        
        return false;
    }

    public virtual void SetStatusUI(){
        GameManager.Instance.statusUI.SetStatus(effectColor,label);
    }

    public virtual void CleanStatusUI(){
        GameManager.Instance.statusUI.CleanStatus();
    }

  
}
