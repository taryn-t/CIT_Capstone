using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] TMP_Text timeSurvived;
    [SerializeField] TMP_Text wavesSurvived;
    [SerializeField] TMP_Text enemiesDefeated;
    [SerializeField] TMP_Text damageTaken;
    [SerializeField] TMP_Text damageGiven;
    // Start is called before the first frame update
    void Start()
    {
        int minutes = (int)GameManager.Instance.totalTime/60;
        int hours = minutes /60;
        int seconds = (int)GameManager.Instance.totalTime%60;

        TimeSpan timeSpan = TimeSpan.FromSeconds((int)GameManager.Instance.totalTime);
        string formattedTime = string.Format("{0:D2}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);
        timeSurvived.text = formattedTime;
        wavesSurvived.text = GameManager.Instance.wavesSurvived > 0 ?(GameManager.Instance.wavesSurvived-1).ToString() : "0" ;
        enemiesDefeated.text = GameManager.Instance.enemiesDefeated.ToString();
        damageTaken.text = GameManager.Instance.playerDamageTaken.ToString();
        damageGiven.text = GameManager.Instance.playerDamageDone.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayAgain(){
        Time.timeScale = 1;
        GameManager.Instance.DestroyManager();
        
        SceneManager.LoadScene( SceneManager.GetActiveScene().name );
    }

    public void QuitGame(){
        Application.Quit();
    }
    void OnDestroy()
    {
        StopAllCoroutines();
    }
}
