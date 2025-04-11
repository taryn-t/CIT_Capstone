



using UnityEngine;

public class DeathHandler : MonoBehaviour
{

    [SerializeField] GameObject gameOverUI;
    private bool gameOver = false;
    void Update()
    {
        if(GameManager.Instance.player != null){
            if(GameManager.Instance.GetPlayer().Health == 0 && !gameOver){


                if(GameManager.Instance.testingManager != null){
                    GameManager.Instance.testingManager.AddDeath();
                    
                    Destroy(GameManager.Instance.hudController.gameObject);
                }
                

                ShowGameOver();
            }
        }
        
    }

    void ShowGameOver(){
        gameOver = true;
        GameManager.Instance.testingManager.AddWave(GameManager.Instance.hudController.currentWave);
        Instantiate(gameOverUI);
        Time.timeScale = 0;
    }
}