

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
public class TestingManager : MonoBehaviour
{
    public bool submitToDB = false;


    [SerializeField] public TestSO Test;
    public TestingData testingData;

    

    [SerializeField] public int testLength = 10;
    public string test_id;
    
    private float timeRemaining;
    public bool testRunning = false;
    public bool activeTest = false;
    private int windowSize = 5; //for calcullating rolling average
    
    private bool submitting = false;
    
    public bool procGen = false;
    public bool spellProg = false;
    public bool firstGen;
    public bool toggled = false;
    public bool receivingData = false;
    private Queue<float> deathTimestamps = new Queue<float>();
    private Queue<float> waveTimestamps = new Queue<float>();
    public int testNumber;

    public float rollingWindowSeconds;
    public void Awake()
    {
        
         GameObject[] gos = GameObject.FindGameObjectsWithTag("TestManager");

        if(gos.Length > 1){
            foreach(GameObject go in gos){
                if(!go.GetComponent<TestingManager>().activeTest){
                    Destroy(go);
                    break;
                }
            }
        }

        
        SceneManager.sceneLoaded += OnSceneLoaded;
        DontDestroyOnLoad(this.gameObject);   
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if(GameManager.Instance.testingManager == null ){
            GameManager.Instance.testingManager = this;
            GameManager.Instance.multiSpell = spellProg;
            GameManager.Instance.procederalWaves = procGen;
        }
    }

    

    public void Start()
    {
        rollingWindowSeconds = testLength * 60f;

        if(GameManager.Instance.testingManager != null){
            testRunning = true;

            firstGen = true;
            StartCoroutine(GetTestConditions());
        }
        
       
    }

    public void AddWave(WaveData wave){
        if(!Test.data.waveData.Any(x=>x.wave_id == wave.wave_id)){
            Test.data.waveData.Add(wave);
            RegisterWave();
        }
        
    }
    public void AddSpell(SpellData spell){
        if(!Test.data.spellData.Any(x=>x.spell_id == spell.spell_id)){
            Test.data.spellData.Add(spell);
        }

        
    }
    public void AddPotion(PotionData potion){
        if(!Test.data.potionData.Any(x=>x.potion_id == potion.potion_id)){
            Test.data.potionData.Add(potion);
        }

        
    }
    public void AddEvent(EventData evt){
        if(!Test.data.eventData.Any(x=>x.event_id == evt.event_id)){
            Test.data.eventData.Add(evt);
        }
        
    }

    public void StopTest(){
        submitToDB = false;
        testRunning = false;
        
        waveTimestamps.Clear();
        deathTimestamps.Clear();
        
        AddWave(GameManager.Instance.hudController.currentWave);
        
        StopAllCoroutines();
        // Destroy(gameObject);

    }
    

    public void StartTest()
    {   Debug.Log("Starting Test");
        

        activeTest = true;
        Test.data.NewTester(procGen,spellProg);
        
        testingData = Test.data.testData;
        test_id = testingData.test_id;

        timeRemaining = testLength * 60; // Convert minutes to seconds
        StartCoroutine(UpdateTimer());
    }

    private void SetConditions()
    {
         GameManager.Instance.procederalWaves = procGen;
         GameManager.Instance.multiSpell = spellProg;
    }

    private IEnumerator UpdateTimer()
    {
        while (testRunning && timeRemaining > 0)
        {
                if(timeRemaining % 60 == 0){
                    Debug.Log($"{timeRemaining/60} minutes left");
                }

                timeRemaining -= 1;

               
                CleanupOldEvents(waveTimestamps);
                CleanupOldEvents(deathTimestamps);
                testingData.deathsPerMinute =  (float)deathTimestamps.Count / rollingWindowSeconds * 60f;
                testingData.wavesPerMinute =  (float)waveTimestamps.Count / rollingWindowSeconds * 60f;
                
                yield return new WaitForSeconds(1);

                testingData.testDuration+=1;
        }

        waveTimestamps.Clear();
        deathTimestamps.Clear();
        

        AddWave(GameManager.Instance.hudController.currentWave);
        testRunning = false;
        Debug.Log("Test Finished!");
    }

    public void AddWavePotions(Dictionary<string,PotionData> potions){
        foreach(PotionData potion in potions.Values){
            AddPotion(potion);
        }
    }
    public void AddWaveSpells(Dictionary<string,SpellData> spells){
        foreach(SpellData spell in spells.Values){
            AddSpell(spell);
        }
    }

    public void Update()
    {
        bool canSubmit = !testRunning && activeTest && !GameManager.Instance.testSubmitted && !submitting;

        if(canSubmit && submitToDB){
            SaveTestData();
            StopTest();
            StartCoroutine(UploadTestData());
            
        }
        
    }

    public void AddDeath()
    {
        testingData.totalDeaths++;
        RegisterDeath();
    }
    public void RegisterWave()
    {
        float now = Time.time;
        waveTimestamps.Enqueue(now);
    }
    public void RegisterDeath()
    {
        float now = Time.time;
        deathTimestamps.Enqueue(now);
    }
    

    
    private void CleanupOldEvents(Queue<float> eventTimestamps )
    {
        float now = Time.time;
        while (eventTimestamps.Count > 0 && now - eventTimestamps.Peek() > rollingWindowSeconds)
        {
            eventTimestamps.Dequeue();
        }
    }

    public void RandomizeConditions(){
        System.Random rand = new();
        
        procGen = rand.Next(0, 2) == 1;
        spellProg = rand.Next(0, 2) == 1;

    }

   
    // public void SaveData()
    // {
    //     if (Application.platform == RuntimePlatform.WebGLPlayer)
    //     {
    //         string jsonData = testingData.Stringify();
    //         SendDataToD1(jsonData);
    //     }
    // }
  
    public void SaveTestData()
    {
        string jsonData = Test.data.Stringify();
        string path = Path.Combine(Application.persistentDataPath, "testdata.json");

        File.WriteAllText(path, jsonData);
        Debug.Log("Saved to: " + path);
    }
     public IEnumerator UploadTestData()
    {
        submitting = true;
        GameManager.Instance.hudController.testComplete.SetActive(true);
        yield return new WaitForSeconds(1f);
        string jsonData = Test.data.Stringify();
        using (UnityWebRequest www = UnityWebRequest.Post("https://cloudflare-api.tarynthompson349.workers.dev/submit", jsonData, "application/json"))
        {
            yield return www.SendWebRequest();
            
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(www.error);
            }
            else
            {
                string json = www.downloadHandler.text;

                PostResult data = JsonUtility.FromJson<PostResult>(json);
                Debug.Log("Received: " + json);
                testNumber = data.testNumber;
                GameManager.Instance.testSubmitted = true;
                Debug.Log("Test data upload complete!");
                
                yield return new WaitForSeconds(1f);

                if(www.result == UnityWebRequest.Result.Success){
                    GameManager.Instance.hudController.testComplete.SetActive(false);
                    GameManager.Instance.deathHandler.ShowGameOver();
                }
            }


            submitting = false;
            
        }
       
         
    }
     public IEnumerator GetTestConditions()
    {
        receivingData = true;
        string url = "https://cloudflare-api.tarynthompson349.workers.dev/next-condition";

    
        UnityWebRequest request = UnityWebRequest.Get(url);

        yield return request.SendWebRequest();
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error: " + request.error);
        }
        else
        {
            string json = request.downloadHandler.text;
            Debug.Log("Received: " + json);


            ConditionRes data = JsonUtility.FromJson<ConditionRes>(json);

            procGen = data.result.proceduralGeneration != 0;
            spellProg = data.result.spellProgression != 0;
            GameManager.Instance.multiSpell = spellProg;
            GameManager.Instance.procederalWaves = procGen;
            StartTest();
        }
        receivingData = false;
    }
}

[Serializable]
public class ConditionRes{
    public bool success;
    public TestCondition result;
}
[Serializable]
public class TestCondition{
    public int id;
    public int proceduralGeneration;
    public int spellProgression;
    public int used;
}

[Serializable]
public class PostResult{
    public string message;
    public int testNumber;
}