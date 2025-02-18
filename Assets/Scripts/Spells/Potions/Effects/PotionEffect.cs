using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionEffect : ScriptableObject
{
    [SerializeField] public Color effectColor;
    [SerializeField] public string label;

    public virtual bool OnApply(Potion potion){
        
        GameManager.Instance.potionButton.Clean();

        return true;
    }

    public virtual void SetStatusUI(){
        GameManager.Instance.statusUI.SetStatus(effectColor,label);
    }

    public virtual void CleanStatusUI(){
        GameManager.Instance.statusUI.CleanStatus();
    }

  
}
