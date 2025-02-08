using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PotionSlot{
    public Potion potion;
   
}

[CreateAssetMenu(menuName = "Data/Potions/PotionContainer")]
public class PotionContainer : ScriptableObject
{
    public List<PotionSlot> slots;

    public void Add(Potion potion){
        PotionSlot potionSlot = slots.Find(x=>x.potion == null);
        if(potionSlot != null){
            potionSlot.potion = potion;
        }
    }
}
