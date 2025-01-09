





using System;
using System.Collections;
using UnityEngine;
using System.Threading.Tasks;
public class AutoSave : TimeAgent
{

    public int secondsToSave = 60;
    private int ticks = default;

    public bool saving = false; 

     void Start(){
        TimeAgent timeAgent = GetComponent<TimeAgent>();
        timeAgent.onTimeTick += SaveGame;
        
        GameManager.Instance.SetAutoSave(this.gameObject);
        
    }

     async void SaveGame(){

        if(GameManager.Instance.mapGenerated &&  GameManager.Instance.gameData != null && GameManager.Instance.player != null){
            GameManager.Instance.gameData.playerData.lastPosition = GameManager.Instance.player.gameObject.transform.position;
        
            int indx = GameManager.Instance.games.data.FindIndex(m=>m.lastSaved == GameManager.Instance.gameData.lastSaved);

            GameManager.Instance.games.data.RemoveAt(indx);
            GameManager.Instance.gameData.lastSaved = DateTime.Now.ToString("g");
            GameManager.Instance.gameData.gameTime = GameManager.Instance.GetDayTime().time;
            GameManager.Instance.games.data.Insert(indx, GameManager.Instance.gameData);
            

           
        }
        

    }

    public override void Init(){
        if(GameManager.Instance.dayTimeController !=null){
            GameManager.Instance.dayTimeController.GetComponent<DayTimeController>().SubscribeSave(this);

        }

    }

  
   
}