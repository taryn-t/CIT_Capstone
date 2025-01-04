using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu( menuName = "Data/Resources/Tiles")]
public class ResourceTiles : ScriptableObject
{
    public List<ResourceTile> data = new();
}
