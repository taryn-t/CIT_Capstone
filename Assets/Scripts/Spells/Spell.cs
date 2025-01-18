
using System.Collections.Generic;
using UnityEngine;

public enum ElementType{
    Fire,
    Water,
    Air,
    Earth
}


public class Spell : MonoBehaviour
{
    List<SpellElement> spellElements;

    public virtual void Hit(){
        Destroy(gameObject);
    }
    public virtual bool CanBeHit(List<ResourceNodeType> canBeHit){
        return true;
    }

    public virtual ResourceNodeType GetResourceNodeType(){
        return ResourceNodeType.Undefined;
    }
}

public class SpellElement{
    ElementType element;

    float baseDamage;
    int areaOfImpact;
    float speedImpact;
    bool spreads;
    int range;

    

}