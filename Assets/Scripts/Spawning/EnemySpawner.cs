using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
public class EnemySpawner : Spawner
{
    [SerializeField] private Color spawnColor;
    

    protected int currentWave = 0;
    

    private void Start(){    
    
        

    }

    private void OnDestroy()
    {
        
        cancellationTokenSource?.Cancel();
    }

   
   private void Update(){
       
        if(GameManager.Instance.totalEnemies <= maxAmount &&  GameManager.Instance.hudController != null ){
            Spawn();
        }  

   }
    
    //Overriding base class method
    protected override async void Spawn(){

        cancellationTokenSource = new CancellationTokenSource();

        
        if(currentWave < GameManager.Instance.hudController.wave){
            currentWave =  GameManager.Instance.hudController.wave;
       
            maxAmount =  GameManager.Instance.hudController.wave == 1 ? 3 :  (int)(GameManager.Instance.hudController.wave*3f);
        }

        try{
                   
                base.Spawn();
                
                amountSpawned++;
                GameManager.Instance.totalEnemies++;
                await Task.Delay(500,cancellationTokenSource.Token);
   
        }
        catch{
            return;
        }
        finally{
            cancellationTokenSource?.Dispose();
            cancellationTokenSource = null;
        }
         
    }
}