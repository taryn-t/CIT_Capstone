using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Player")]
public class PlayerData : ScriptableObject
{
    public int Health = 100;
    public int Hunger = 0;
    public int Thirst = 0;
    public int Stamina = 100;
    
}
