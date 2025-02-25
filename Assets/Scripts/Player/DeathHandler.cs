



using UnityEngine;

public class DeathHandler : MonoBehaviour
{

    [SerializeField] GameObject gameOverUI;
    private bool gameOver = false;
    void Update()
    {
        if(GameManager.Instance.GetPlayer().Health == 0 && !gameOver){
            ShowGameOver();
        }
    }

    void ShowGameOver(){
        gameOver = true;
        Instantiate(gameOverUI);
        Time.timeScale = 0;
    }
}