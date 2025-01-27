using System;
using UnityEditor.Animations;
using UnityEngine;

[CreateAssetMenu( menuName = "Data/Spell")]
public class Spell : ScriptableObject
{
    [SerializeField] public string Name;
    [SerializeField] public int damage;
    [SerializeField] public float knockback;
    [SerializeField] public SpellEffect spellEffect;
    [SerializeField] public AnimatorController animator;
    [SerializeField] public Sprite Icon;

}



[Serializable]
public enum SpellEffect{
    Burn,
    Poison,
    Slow
}