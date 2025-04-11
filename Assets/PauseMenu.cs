using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    

    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.pauseMenu = gameObject;
        gameObject.SetActive(false);
    }

   

    public void QuitGame(){
        
        GameManager.Instance.testingManager.StopTest();
        SceneManager.LoadScene( SceneManager.GetActiveScene().name );
    }

    public void CloseMenu(){
        gameObject.SetActive(false);
    }
}
