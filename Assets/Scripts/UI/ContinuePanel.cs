using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ContinuePanel : MenuPanel
{
    private List<GameData> games;
    [SerializeField] GameObject gamesContainer;
    [SerializeField] public Button gameBtn;
    private static int index = 2;
    

    void Start(){
         games = GameManager.Instance.GetGames().data;
         ShowSaves();
       
    }

  
    public void ShowSaves(){
        
        games.Sort((p, q) => DateTime.Parse(p.lastSaved).CompareTo(DateTime.Parse(q.lastSaved)));


        foreach(GameData game in games){

            Button btn = Instantiate(gameBtn,gamesContainer.transform);

            btn.gameObject.GetComponent<GameButton>().ChangeButton(game.name, game.lastSaved);
            

        }

    }

    public void LoadGame(){

    }

 
}


