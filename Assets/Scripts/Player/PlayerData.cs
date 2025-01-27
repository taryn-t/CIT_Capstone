using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class PlayerData 
{
    public int Health;
    public int Mana;
    public SpellContainer KnownSpells;

    public Vector3Int lastPosition;

    public PlayerData(Vector3Int center, SpellContainer spells){
        Health = 100;
        Mana = 100;
        lastPosition = center;
        KnownSpells = spells;
    }
    

}
