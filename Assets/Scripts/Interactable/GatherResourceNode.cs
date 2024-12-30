using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ResourceNodeType{
    Undefined,
    Tree,
    Stump,
    Ore
}


[CreateAssetMenu(menuName = "Data/Tool action/Gather Resource Node")]
public class GatherResourceNode : ToolAction
{
    [SerializeField] float sizeInteractableArea = 2;
    [SerializeField] List<ResourceNodeType> canHitNodesOfType;
    
     public override bool OnApply(Vector2 worldPoint){
        
        
        Collider2D[] colliders = Physics2D.OverlapCircleAll(worldPoint,sizeInteractableArea);
        
        foreach(Collider2D c in colliders){
           
                ToolHit hit = c.gameObject.GetComponent<ToolHit>();
                if(hit !=null){
                    if(hit.CanBeHit(canHitNodesOfType)==true){
                        hit.Hit(); 
                    }
                    return true;
                }

                if(hit.GetResourceNodeType() == ResourceNodeType.Tree){
                    break;
                }
            
            
        }
        return false;
    }
}
