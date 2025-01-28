using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartPanel : MenuPanel
{

    [SerializeField] Button NewGameBtn;
    [SerializeField] Button ContinueGameBtn;
    [SerializeField] Button SettingsBtn;
    [SerializeField] Button ExitBtn;

    private static int index = 0;
    

    public void ExitGame()
    {
        Application.Quit();

    }


    public void NewGame()
    {
        
        GameManager.Instance.GetMenu().ChangePanel(1);
    }

    public void ContinueGame()
    {
        GameManager.Instance.GetMenu().ChangePanel(2);
    }
    public void ShowSettings()
    {
        GameManager.Instance.GetMenu().ChangePanel(3);
    }
}
