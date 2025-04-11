using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
public class EnemySpawner : Spawner
{
    [SerializeField] private Color spawnColor;
    
    public bool spawning;
    protected int currentWave = 0;
    

    private void Start(){    
      
        
        GameManager.Instance.activeSpawners.Add(this);

        
    }
   
   private void LateUpdate(){
       
        // if(!GameManager.Instance.procederalWaves && GameManager.Instance.hudController != null){

             
        //     bool noMoreEnemies = GameManager.Instance.waveMaxEnemies != 0 && GameManager.Instance.totalEnemies == 0;
        //     Debug.Log($"{noMoreEnemies}  no more enemies");
        //      bool allFairyJarsFound = GameManager.Instance.hudController.fairyJarsGathered ==3;
        //      Debug.Log($"{allFairyJarsFound}  no more jars");



        //     if(allFairyJarsFound && noMoreEnemies){
        //         Debug.Log("Spawning next wave");    
        //         currentWave = GameManager.Instance.hudController.wave;
                
        //         maxAmount =(int)(Math.Exp(0.35f*currentWave)+2 - 1  );
                
        //         StartCoroutine(Spawn(GameManager.Instance.SpellsGO.transform));
        //     }

        // }
        
   }

   public void StartSpawn(int wave){
        amountSpawned=0;
        currentWave = wave;
                
        maxAmount = wave*2;
        
        StartCoroutine(Spawn(GameManager.Instance.EnemiesGO.transform));
    
    }

    private int SetSpawnAmount(){

        
        return (int)GameManager.Instance.hudController.wave+1;
    }

    protected override IEnumerator Spawn(Transform parent){
        spawning= true;
        while(amountSpawned < maxAmount){
            if(GameManager.Instance.player != null){
                yield return StartCoroutine(base.Spawn(parent));
                amountSpawned++;
                GameManager.Instance.totalEnemies++;
                    
            }
            yield return new WaitForSeconds(1.5f) ;           
        } 
        spawning = false;  
    }
}