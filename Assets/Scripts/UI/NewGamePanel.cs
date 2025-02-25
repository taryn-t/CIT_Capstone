using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NewGamePanel : MenuPanel
{

    public static int index = 1;
    private WalkerGenerator MapGen;

    [SerializeField] Button GenerateBtn;
    [SerializeField] TMP_InputField  GameName;
    [SerializeField] TMP_InputField  Seed;
    [SerializeField] GameObject LoadingUI;

    string seedVal = "";
    string nameVal = "";
    public void Start()
    {
        GameManager.Instance.GetMenu().currentMenuIndex = index;
    }

    public void GenerateMap()
    {
        
        GameManager.Instance.genSeed = Seed.text;
        GameManager.Instance.gameName = GameName.text;
        MapGen = GameManager.Instance.GetMapGenerator();
        
        MapGen.StartGeneration(Seed.text,GameName.text);
        
        
    }

    public void SetSeedVal(string seed){
        seedVal = seed;
    }
     public void SetNameVal(string name){
        nameVal = name;
    }


}
