using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu( menuName = "Data/Maps")]
public class Maps : ScriptableObject
{
    public List<Map> maps = new();
}
