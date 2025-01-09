using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Walker
{
    public Vector2 Position;
    public Vector2 Direction;
    public float ChanceToChange;

    public Walker(Vector2 pos, Vector2 dir, float chanceToChange){
        Position = pos;
        Direction = dir;
        ChanceToChange = chanceToChange;
    }
    

}
