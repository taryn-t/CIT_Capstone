

using System;
using UnityEngine;
using UnityEngine.Tilemaps;


[Serializable]
public class ResourceTile{

    public GameObject resourcePrefab;
    public TileBase tile;

    public ResourceNodeType resourceType;
}