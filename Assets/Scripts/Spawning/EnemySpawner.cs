using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
public class EnemySpawner : Spawner
{
    [SerializeField] private Color spawnColor;
    

    protected int currentWave = 0;
    

    private void Start(){    
            if(GameManager.Instance.hudController != null){
            maxAmount =  GameManager.Instance.hudController.wave == 1 ? 2 :  (int)(GameManager.Instance.hudController.wave*1.5f);

        }else{
            maxAmount = 2;
        }
    
        Spawn(GameManager.Instance.SpellsGO.transform);
    }
   
   private void Update(){
       
       

   }
    
    //Overriding base class method
    protected override async void Spawn(Transform parent){

        cancellationTokenSource = new CancellationTokenSource();

        

        try{
                while(amountSpawned < maxAmount){
                    if(GameManager.Instance.player != null){
              
                        base.Spawn(parent);
                    
                        amountSpawned++;
                        GameManager.Instance.totalEnemies++;
                         
                    }
                    await Task.Delay(500,cancellationTokenSource.Token);    
                    
                }   
                
   
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