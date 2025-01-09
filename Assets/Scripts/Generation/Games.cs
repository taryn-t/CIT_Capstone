using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu( menuName = "Data/Game")]
public class Games : ScriptableObject
{
    public List<GameData> data = new();

}


[Serializable]

public class GameData{
    public string name;
    public string lastSaved;
    public float gameTime;

    public Map map;

    public PlayerData playerData;

    


    public GameData(string name, Map map, DateTime date, Vector3 center ){

        this.name = name;
        this.map = map;
        playerData = new PlayerData(center);

        lastSaved = date.ToString("g");
        gameTime = 28800f;
    }
    


}