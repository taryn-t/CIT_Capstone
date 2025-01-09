using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class PlayerData 
{
    public int Health;
    public int Hunger;
    public int Thirst;
    public int Stamina;
    public Vector3 lastPosition;

    public PlayerData(Vector3 center){
        Health = 100;
        Hunger = 0;
        Thirst = 0;
        Stamina = 100;
        lastPosition = center;
    }
    

}
