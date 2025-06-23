
using System.Collections.Generic;
using UnityEngine;

public class PotionPanel : MonoBehaviour
{
    private PotionContainer potions;

    [SerializeField] PotionButton button;
    [SerializeField] List<PotionButton> buttons;

    private void Start(){
        potions = GameManager.Instance.availablePotions;
        GameManager.Instance.potionButtons = buttons;
        
        
    }


}
