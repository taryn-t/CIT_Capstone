





using System;
using System.Collections;
using UnityEngine;
using System.Threading;
using System.Threading.Tasks;

public class AutoSave : MonoBehaviour
{

    public int secondsToSave = 60;
    private int ticks = default;

    public bool saving = false; 
    CancellationTokenSource cancellationTokenSource;

     void Start(){
        
        GameManager.Instance.SetAutoSave(this.gameObject);

        GameManager.Instance.gameData.playerData.lastPosition = GameManager.Instance.player.transform.position;
        GameManager.Instance.saving = true;    
        SaveGameTask();
        Destroy(gameObject);
        
        
    }

    private void OnDestroy()
    {
        cancellationTokenSource?.Cancel();
    }

    async void SaveGameTask(){
            
            cancellationTokenSource = new CancellationTokenSource();

             Debug.Log("Async Task Started on: " + gameObject);
             try
            {
                
                int indx = GameManager.Instance.games.data.FindIndex(m=>m.lastSaved == GameManager.Instance.gameData.lastSaved);

                GameManager.Instance.games.data.RemoveAt(indx);
                GameManager.Instance.gameData.lastSaved = DateTime.Now.ToString("g");
                GameManager.Instance.gameData.gameTime = GameManager.Instance.GetDayTime().time;
                GameManager.Instance.games.data.Insert(indx, GameManager.Instance.gameData);
            
                await Task.Delay(1000, cancellationTokenSource.Token);
                 GameManager.Instance.saving = false;
            }
            catch
            {
                Debug.Log("Task was cancelled!");
                return;
            }
            finally
            {
               
                cancellationTokenSource.Dispose();
                cancellationTokenSource = null;
            }

    }

  
   
}