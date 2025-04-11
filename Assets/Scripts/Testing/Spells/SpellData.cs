using System;

using UnityEngine;


[Serializable]
public class SpellData 
{
    public string spell_id;
    public string test_id;
    public string wave_id;

    public string spellType;
    public int pickedUp = default;
    public int totalSpawned = default;
    public int damageDone = default;
    public int damageReceived = default;

    public string Stringify() 

    {

        return JsonUtility.ToJson(this);

    }

    public static TestingData Parse(string json)

    {

        return JsonUtility.FromJson<TestingData>(json);

    }


    public SpellData(WaveData wave, string type ){

        spell_id = Guid.NewGuid().ToString("N");
        wave_id =wave.wave_id;

        test_id = wave.test_id;

        spellType = type;
        
    }
    

}

