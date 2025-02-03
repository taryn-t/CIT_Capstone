





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

        GameManager.Instance.gameData.playerData.lastPosition = Vector3Int.FloorToInt(GameManager.Instance.player.transform.position);
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

             
             try
            {
                
                int indx = GameManager.Instance.games.data.FindIndex(m=>m.lastSaved == GameManager.Instance.gameData.lastSaved);

                GameManager.Instance.games.data.RemoveAt(indx);
                GameManager.Instance.gameData.lastSaved = DateTime.Now.ToString("g");
                GameManager.Instance.gameData.gameTime = GameManager.Instance.GetDayTime().time;
                GameManager.Instance.gameData.daysPlayed =  GameManager.Instance.GetDayTime().days;
                GameManager.Instance.games.data.Insert(indx, GameManager.Instance.gameData);
            
                await Task.Delay(1000, cancellationTokenSource.Token);
                 GameManager.Instance.saving = false;
            }
            catch
            {
               
                return;
            }
            finally
            {
               
                cancellationTokenSource.Dispose();
                cancellationTokenSource = null;
            }

    }

  
   
}