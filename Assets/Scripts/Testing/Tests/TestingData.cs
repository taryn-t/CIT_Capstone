using System;

using UnityEngine;


[Serializable]
public class TestingData 
{
    public string test_id;
    public string testDateTime;

    public float wavesPerMinute;
    public float deathsPerMinute;
    public float testDuration;

    public int totalDeaths;
    public int wavesCompleted;
    public bool spellProgression;
    public bool proceduralGeneration;


    public string Stringify() 

    {

        return JsonUtility.ToJson(this);

    }

    public static TestingData Parse(string json)

    {

        return JsonUtility.FromJson<TestingData>(json);

    }



    public TestingData(bool proceduralGeneration, bool spellProgression){
        test_id = Guid.NewGuid().ToString("N");
        testDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        wavesPerMinute =0;
        deathsPerMinute=0;
        testDuration=0;
        this.spellProgression = spellProgression;
        this.proceduralGeneration = proceduralGeneration;
        
    }

}

