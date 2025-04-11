


using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Potions/Effects/Speed Boost")]
public class SpeedBoost : PotionEffect
{
    
    public override bool OnApply(Potion potion)
    {
        if(potion == null){
            return false;
        }
        
        float oldSpeed = GameManager.Instance.playerMovement.moveSpeed;
        float newSpeed = oldSpeed *2;
        
        base.OnApply(potion);
        
        GameManager.Instance.player.GetComponent<MonoBehaviour>().StartCoroutine(RunEffect(newSpeed,oldSpeed,potion.duration));
        GameManager.Instance.hudController.wavePotions["Speed"].totalUsed++;
        

        return true;
    }

    private IEnumerator RunEffect(float newSpeed, float oldSpeed, int duration){
        
        GameManager.Instance.playerMovement.moveSpeed = newSpeed;
        SetStatusUI();
        GameManager.Instance.GetPlayer().potionActive = true;

        yield return new WaitForSeconds(duration);
         GameManager.Instance.GetPlayer().potionActive = false;
        GameManager.Instance.playerMovement.moveSpeed = oldSpeed;
        CleanStatusUI();

       
    }

}
