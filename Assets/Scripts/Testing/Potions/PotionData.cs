using System;

using UnityEngine;


[Serializable]
public class PotionData 
{
    public string potion_id;
    public string test_id;
    public string wave_id;

    public string potionType;
    public int pickedUp = default;
    public int totalSpawned = default;
    public int totalUsed = default;

    public string Stringify() 

    {

        return JsonUtility.ToJson(this);

    }

    public static TestingData Parse(string json)

    {

        return JsonUtility.FromJson<TestingData>(json);

    }


    public PotionData(WaveData wave, string type ){

        potion_id = Guid.NewGuid().ToString("N");
        wave_id = wave.wave_id;

        test_id = wave.test_id;

        potionType = type;
        
    }
    

}

