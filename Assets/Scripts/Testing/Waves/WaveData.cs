using System;

using UnityEngine;


[Serializable]
public class WaveData 
{
    public string wave_id;
    public string test_id;
    public int waveNumber;

    public float timeComplete;
    public int totalEnemies = default;
    public int enemiesDefeated= default;
    public int damageDone= default;
    public int damageReceived= default;
    public int healingDone= default;

    public int potionsUsed= default;
    public int mushroomsUsed= default;
    public string seedUsed;
    public bool completed = false;

    public string Stringify() 

    {

        return JsonUtility.ToJson(this);

    }

    public static TestingData Parse(string json)

    {

        return JsonUtility.FromJson<TestingData>(json);

    }


    public WaveData(string test_id, int number, string seed){
        wave_id = Guid.NewGuid().ToString("N");
        this.test_id = test_id;
        waveNumber = number;
        seedUsed =seed;
        completed = false;
    }
    

}

