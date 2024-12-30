using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolAction : ScriptableObject
{
    
    public virtual bool OnApply(Vector2 worldPoint){
        Debug.LogWarning("OnApply not implemented");
        return true;
    }
    // public virtual bool OnApplyToTileMap(Vector3Int gridPosition, TileMapReadController tileMapReadController, Item item){
    //     Debug.LogWarning("OnApplyToTileMap is not implemented");
    //     return true;

    // }
    public virtual bool OnEat( Item item, PlayerController player){
        Debug.LogWarning("OnApplyToTileMap is not implemented");
        return true;

    }
}
