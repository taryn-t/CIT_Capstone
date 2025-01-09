using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Item")]
public class Item : ScriptableObject
{
    public string Name;
    public Sprite Icon;
    public bool Stackable;
    public bool Edible;
    public bool Drinkable;
    public int HungerRestored;
    public int ThirstRestored;
    public float Damage;
    public ToolAction onAction;
    
}
