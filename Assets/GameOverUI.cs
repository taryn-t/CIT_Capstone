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
    // Start is called before the first frame update
    void Start()
    {
        timeSurvived.text = GameManager.Instance.totalTime.ToString();
        wavesSurvived.text = (GameManager.Instance.wavesSurvived-1).ToString();
        enemiesDefeated.text = GameManager.Instance.enemiesDefeated.ToString();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayAgain(){
        Destroy(GameObject.Find("GameManager"));
        SceneManager.LoadScene( SceneManager.GetActiveScene().name );
    }

    public void QuitGame(){
        Application.Quit();
    }
}
