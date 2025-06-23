using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class StartPanel : MenuPanel
{

    [SerializeField] Button NewGameBtn;
    // [SerializeField] Button ContinueGameBtn;
    // [SerializeField] Button SettingsBtn;
    // [SerializeField] Button ExitBtn;
    private WalkerGenerator MapGen;
    [SerializeField] GameObject LoadingUI;
    private static int index = 0;
    
    public void Start()
    {
        GameManager.Instance.GetMenu().currentMenuIndex = index;
    }
    public void ExitGame()
    {
        Application.Quit();

    }



    public void NewGame()
    {
        if(GameManager.Instance.procederalWaves){
          GameManager.Instance.GetMenu().ChangePanel(1);  
        }
        else{
            
            MapGen = GameManager.Instance.GetMapGenerator();
            
            MapGen.StartGeneration("","");
            
            Instantiate(LoadingUI);

        }
        
    }
    public void ShowSettings()
    {
        GameManager.Instance.GetMenu().ChangePanel(2);
    }
}
