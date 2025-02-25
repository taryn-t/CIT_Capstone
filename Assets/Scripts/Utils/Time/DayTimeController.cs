using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class DayTimeController : MonoBehaviour
{
    // const float secondsInDay = 86400f; 
    // const float daysInMonth = 28;
    //  string[] daysOfTheWeek = {"Sunday", "Monday", "Tuesday", "Wendnesday", "Thursday", "Friday", "Saturday"};
    public  float time;
    [SerializeField] private float phaseLength = 60f; //20 minutes
    // const float phasesInDay = 72f; //72 segments of 20 minutes in a day  const float phasesInDay = 72f; //72 segments of 20 minutes in a day
    // [SerializeField] float morningTime = 28800f;
    [SerializeField] Color nightLightColor;
    [SerializeField] Color dayLightColor = Color.white;
    [SerializeField] AnimationCurve nightTimeCurve;

    private float timeScale = 60f;
    [SerializeField] GameObject AutoSave;
    public  Light2D globalLight;
  
      public GameObject[] controllers;
    public  int days =1;
    List<TimeAgent> agents;
    TimeAgent saveAgent;
    private float startAtTime = 0f;

    int oldPhase =-1;

    float Hours{
        // get{return (time/3600f)+6;}
        get{return (time/3600f);}
    }
    
    float Minutes{
        get{return time%3600f /60f;}
    }

    void Awake(){
        agents = new List<TimeAgent>();
        
        DontDestroyOnLoad(gameObject);
    }

    void Start(){
        
        GameManager.Instance.SetDayTime(this.gameObject);

        time = startAtTime;
                    
        SceneManager.sceneLoaded += OnSceneLoaded;

    }

    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        globalLight= GameObject.Find("GlobalLight").GetComponent<Light2D>();
        float v = nightTimeCurve.Evaluate(Hours);
        Color c = Color.Lerp(dayLightColor, nightLightColor, v);
        globalLight.color = c;

    }


    string formattedTime;
     void Update(){
      
        if(GameManager.Instance.mapGenerated){
            time+= Time.deltaTime ;
            int hh = (int) Hours;
            int mm = (int)Minutes;
        
            // var (h, amPmDesignator) = hh switch
            //     {
            //         0 => (12, "AM"),
            //         12 => (12, "PM"),
            //         > 12 => (hh - 12, "PM"),
            //         _ => (hh, "AM"),
            //     };

            //     if(mm%15 ==0){
            //         formattedTime = $"{h}:{mm:00} {amPmDesignator}";
            //     }
        
            // timeLabel.text = formattedTime;
            
            float v = nightTimeCurve.Evaluate(Hours);
            Color c = Color.Lerp(dayLightColor, nightLightColor, v);
            
            if(globalLight == null){
                globalLight= GameObject.Find("GlobalLight").GetComponent<Light2D>();
            }
            
            globalLight.color = c;

            // timeLabel.text = formattedTime;
            // dayLabel.text = GetDay();
            // dayCount.text = "Day " + days;
            
            
            // dayLabel.text = "Day " + days;
        
            // if(hh %6 ==0){
               
                // if(GameManager.Instance.mapGenerated &&  GameManager.Instance.gameData != null && GameManager.Instance.player != null){
                //      Instantiate(AutoSave);
                // }
            // }

            // if(hh >= 24){
            //     NextDay();
            // }
            

            // if(oldPhase == -1){
            //     oldPhase = CalculatePhase();
            // }
            
            
            
            
            int phase = CalculatePhase();


            while(oldPhase <phase){
                oldPhase+=1;
                for (int i =0; i<agents.Count;i++){
                        agents[i].Invoke();
                    }

                
                
            }
            GameManager.Instance.hudController.UpdateTime((int)time);
      
        }
        
    }

    // private string GetDay(){
    //    int dayIndex =  (int)(daysInMonth/(days)) % 7;

    
    //    return daysOfTheWeek[dayIndex];
    // }

    private int CalculatePhase()
    {
        return (int)(time / phaseLength) ;
    }

    // private void NextDay(){
    //     time -= secondsInDay;
    //     days += 1;
      
    // }
   
    public void Subscribe(TimeAgent timeAgent){
        agents.Add(timeAgent);
    }
     public void Unsubscribe(TimeAgent timeAgent){
        agents.Remove(timeAgent);
    }

    public void SkipTime(float seconds =0, float minutes = 0, float hours = 0){

        float timeToSkip = seconds;
        timeToSkip +=minutes *60f;
        timeToSkip += hours * 3600f;
        time += timeToSkip;
        

    }

    // public void SkipToMorning()
    // {
    //     float secondsToSkip =0f;
    //     if(time > morningTime){
    //         secondsToSkip += secondsInDay -time +morningTime;
    //     }
    //     else{
    //         secondsToSkip += morningTime -time;
    //     }

    //     SkipTime(secondsToSkip);
    // }

    public void SubscribeSave(TimeAgent timeAgent){
        saveAgent = timeAgent;
    }

}

 