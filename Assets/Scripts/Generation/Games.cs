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
    public int daysPlayed;

    public Map map;

    public PlayerData playerData;

    


    public GameData(string name, Map map, DateTime date, Vector3Int center, SpellContainer spells ){

        this.name = name;
        this.map = map;
        playerData = new PlayerData(center, spells);

        lastSaved = date.ToString("g");
        gameTime = 28800f;

        daysPlayed = 1;
    }
    


}