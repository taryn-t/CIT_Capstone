using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SpellSlot{
    public Spell spell;
   
}

[CreateAssetMenu(menuName = "Data/Spells/SpellContainer")]
public class SpellContainer : ScriptableObject
{
    public List<SpellSlot> slots;

    public void Add(Spell spell){
        SpellSlot spellSlot = slots.Find(x=>x.spell == null);
        if(spellSlot != null){
            spellSlot.spell = spell;
        }
    }
}
