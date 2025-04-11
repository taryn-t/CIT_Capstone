



using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Potions/Effects/Invisibility")]
public class Invisibility : PotionEffect
{
    
    public override bool OnApply(Potion potion)
    {
        if(potion == null){
            return false;
        }
        
        base.OnApply(potion);
        
        Color newColor = new Vector4(225,225,50);
        Color oldColor = GameManager.Instance.player.GetComponentInChildren<SpriteRenderer>().color;
        
        GameManager.Instance.player.GetComponent<MonoBehaviour>().StartCoroutine(RunEffect(oldColor, newColor, potion.duration));

        GameManager.Instance.hudController.wavePotions["Invisibility"].totalUsed++;

        return true;
    }

    private IEnumerator RunEffect(Color oldColor, Color newColor, int duration){
        
        GameManager.Instance.GetPlayer().potionActive = true;

        GameManager.Instance.GetPlayer().visible = false;
        GameManager.Instance.player.GetComponentInChildren<SpriteRenderer>().color = newColor;

        SetStatusUI();
        
        yield return new WaitForSeconds(duration);
         GameManager.Instance.GetPlayer().potionActive = false;

        GameManager.Instance.GetPlayer().visible = true;
        GameManager.Instance.player.GetComponentInChildren<SpriteRenderer>().color = oldColor;
        CleanStatusUI();
       
        
    }
}
