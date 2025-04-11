using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Instructions : MonoBehaviour
{
    public bool open = false;
    void Start()
    {
        GameManager.Instance.instructionsUI = gameObject;
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(open){
            GameManager.Instance.gameplayStarted = false;
        }
        else if(GameManager.Instance.gameplayStarted){
            GameManager.Instance.gameplayStarted = true;
        }
    }
    void OnEnable()
    {
        if(GameManager.Instance != null)
            GameManager.Instance.gameplayStarted = false;
    }
    void OnDisable()
    {
        if(GameManager.Instance != null)
            GameManager.Instance.gameplayStarted = true;
    }

    public void ClosePanel(){
        open=false;
        gameObject.SetActive(false);
    }
    
}
