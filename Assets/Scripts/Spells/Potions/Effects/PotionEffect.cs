using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionEffect : ScriptableObject
{
    
    public virtual bool OnApply(Potion potion){
        
        GameManager.Instance.potionButton.Clean();

        return true;
    }

  
}
