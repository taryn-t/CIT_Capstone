using System;
using UnityEngine;

[CreateAssetMenu( menuName = "Data/Spell")]
public class Spell : ScriptableObject
{
    [SerializeField] public string Name;
    [SerializeField] public int damage;
    [SerializeField] public float knockback;
    [SerializeField] public int manaCost;
    [SerializeField] public SpellEffect spellEffect;
    [SerializeField] public Sprite Icon;
    [SerializeField] public Sprite[] frames;
    [SerializeField] public float range = 10f;

}



[Serializable]
public enum SpellEffect{
    Explode,
    Burn,
    Poison,
    Slow,
    Gravity,
    None
}