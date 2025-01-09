using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolHit : MonoBehaviour
{
    
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
 
