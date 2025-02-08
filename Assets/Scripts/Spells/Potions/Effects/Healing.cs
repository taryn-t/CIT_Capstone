using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Data/Potions/Effects/Healing")]
public class Healing : PotionEffect
{


    public override bool OnApply(Potion potion)
    {
        if(potion == null || GameManager.Instance.GetPlayer().Health == 100){
            return false;
        }

        base.OnApply(potion);

        GameManager.Instance.GetPlayer().Health += potion.effectStrength;

                
        return true;
    }
}
