using System;

using UnityEngine;


[Serializable]
public class EventData 
{
    public string event_id;
    public string test_id;
    public string wave_id;
    public string eventType;
    public string eventTime;
    public string details;
    public string Stringify() 

    {

        return JsonUtility.ToJson(this);

    }

    public static TestingData Parse(string json)

    {

        return JsonUtility.FromJson<TestingData>(json);

    }


    public EventData(WaveData wave, string type, string details ){

        event_id =Guid.NewGuid().ToString("N");
        wave_id = wave.wave_id;

        test_id = wave.test_id;

        eventType = type;

        eventTime = DateTime.Now.ToString("HH:mm:ss");

        this.details = details;
    }
    

}

