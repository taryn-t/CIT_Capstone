using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;

public class SpellPanel : MonoBehaviour
{
    private SpellContainer spells;
    [SerializeField] List<SpellButton> buttons;

    private void Start(){
        spells = GameManager.Instance.gameData.playerData.KnownSpells;
        SetIndex();
        Show();
     
    }

    private void SetIndex()
    {
        for (int i =0; i<spells.slots.Count; i++){
            buttons[i].SetIndex(i);
        }
       
    }

    private void Show()
    {
        
        

        for(int i = 0; i<spells.slots.Count; i++){
        
            
            if(spells.slots[i].spell ==null){
                buttons[i].Clean();
            }else{
                buttons[i].Set(spells.slots[i]);
            }
        }
    }
}
