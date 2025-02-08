using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;

public class SpellPanel : MonoBehaviour
{
    private SpellContainer spells;
    [SerializeField] SpellButton button;

    private void Start(){
        spells = GameManager.Instance.gameData.playerData.KnownSpells;
        SetIndex();
        Show();
     
    }

    private void SetIndex()
    {
        
            button.SetIndex(0);
        
       
    }

    private void Show()
    {
        
        

        
            button.Set(spells.slots[0]);
            
        
    }
}
