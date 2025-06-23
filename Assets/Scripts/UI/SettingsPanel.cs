using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsPanel : MonoBehaviour
{

    private static int index = 3;
    
    [SerializeField] Toggle devMode;
    [SerializeField] Toggle proceduralWaves;
    [SerializeField] Toggle spellProgression;
    [SerializeField] Toggle submitToDB;

    public bool toggling = false;


    public void Start()
    {
         if(GameManager.Instance.testingManager != null ){
            devMode.isOn = GameManager.Instance.isDev;
            proceduralWaves.isOn = GameManager.Instance.testingManager.procGen;
            spellProgression.isOn = GameManager.Instance.testingManager.spellProg;
            submitToDB.isOn = GameManager.Instance.testingManager.submitToDB; 
        }
    }

    public void GoBack(){
        GameManager.Instance.GetMenu().ChangePanel(0);
    }
    
    public void ChangeDevMode(){
        GameManager.Instance.isDev = devMode.isOn;
    }

    public void ChangeProcGen(){
        GameManager.Instance.testingManager.procGen = proceduralWaves.isOn;
        GameManager.Instance.procederalWaves = proceduralWaves.isOn;
    }

    public void ChangeSpellProg(){
       GameManager.Instance.testingManager.spellProg = spellProgression.isOn;
       GameManager.Instance.multiSpell = spellProgression.isOn;
    }

    public void ChangeSubmit(){
        GameManager.Instance.testingManager.submitToDB = submitToDB.isOn;
    }
    


}
