using System;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class Test 
{
    public TestingData testData;
    public List<WaveData> waveData;
    public List<SpellData> spellData;
    public List<PotionData> potionData;
    public List<EventData> eventData;

    public string Stringify() 

    {

        return JsonUtility.ToJson(this);

    }

    public static TestingData Parse(string json)

    {

        return JsonUtility.FromJson<TestingData>(json);

    }



    public void NewTester(bool proceduralGeneration, bool spellProgression){
        testData = new TestingData(proceduralGeneration, spellProgression);
        waveData = new ();
        spellData = new();
        potionData = new();
        eventData = new();
        
    }

}

