


using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Data/Potions/Effects/Invisibility")]
public class Invisibility : PotionEffect
{
    
    public override bool OnApply(Potion potion)
    {
        if(potion == null){
            return false;
        }
        base.OnApply(potion);
        
        Color newColor = new Color(225,225,25);
        Color oldColor = GameManager.Instance.player.GetComponentInChildren<SpriteRenderer>().color;
        

        RunEffect(oldColor, newColor, potion.duration);

        

        return true;
    }

    private async void RunEffect(Color oldColor, Color newColor, int duration){
        
        int durMilli = duration * 1000;

        GameManager.Instance.GetPlayer().visible = false;
        GameManager.Instance.player.GetComponentInChildren<SpriteRenderer>().color = newColor;
        
        await Task.Delay(durMilli);

        GameManager.Instance.GetPlayer().visible = true;
        GameManager.Instance.player.GetComponentInChildren<SpriteRenderer>().color = oldColor;
       
        
    }
}
