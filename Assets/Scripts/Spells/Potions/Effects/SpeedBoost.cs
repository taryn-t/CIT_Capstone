
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Data/Potions/Effects/Speed Boost")]
public class SpeedBoost : PotionEffect
{
    
    public override bool OnApply(Potion potion)
    {
        if(potion == null){
            return false;
        }
        
        float oldSpeed = GameManager.Instance.playerMovement.moveSpeed;
        float newSpeed = oldSpeed + (oldSpeed*potion.effectStrength);
        
        base.OnApply(potion);
        
        RunEffect(newSpeed,oldSpeed,potion.duration);

        

        return true;
    }

    private async void RunEffect(float newSpeed, float oldSpeed, int duration){
        
        int durMilli = duration * 1000;
        GameManager.Instance.playerMovement.moveSpeed = newSpeed;
        SetStatusUI();

        await Task.Delay(durMilli);

        GameManager.Instance.playerMovement.moveSpeed = oldSpeed;
        CleanStatusUI();

       
    }

}
