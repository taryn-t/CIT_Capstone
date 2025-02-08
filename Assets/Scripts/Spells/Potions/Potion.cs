using UnityEngine;

[CreateAssetMenu(menuName = "Data/Potions/Potion")]
public class Potion : ScriptableObject
{
    public string Name;
    public Sprite icon;
    public PotionEffect potionEffect;
    public float effectStrength; 
    public int  duration; //in seconds - 0 if instant

}

