using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NewGamePanel : MenuPanel
{

    private static int index = 1;
    private WalkerGenerator MapGen;

    [SerializeField] Button GenerateBtn;
    [SerializeField] TMP_InputField  GameName;
    [SerializeField] TMP_InputField  Seed;
    [SerializeField] GameObject LoadingUI;

    string seedVal = "";
    string nameVal = "";

    public void GenerateMap()
    {
        

        MapGen = GameManager.Instance.GetMapGenerator();
        
        MapGen.StartGeneration(Seed.text,GameName.text, gameObject);
        
        
    }

    public void SetSeedVal(string seed){
        seedVal = seed;
    }
     public void SetNameVal(string name){
        nameVal = name;
    }


}
