using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public abstract class Spawner : MonoBehaviour
{
    
    [SerializeField] protected GameObject[] spawnPrefabs;
    [SerializeField] protected Vector3 spawnPosition;
    
    
    [SerializeField] protected List<GameObject> spawnedObjects;
    public int amountSpawned = default;
    [SerializeField] protected int maxAmount;
   
   
    protected CancellationTokenSource cancellationTokenSource;
    
    

    private void Start(){    
        spawnedObjects = new List<GameObject>();
        
    }
 
     private void OnDestroy()
    {
        cancellationTokenSource?.Cancel();
    }

    protected virtual async void Spawn()
    {   
         cancellationTokenSource = new CancellationTokenSource();


         try{
            int randomIndex = UnityEngine.Random.Range(0, spawnPrefabs.Length);

            GameObject go = Instantiate(spawnPrefabs[randomIndex],transform.position, Quaternion.identity); 
            spawnedObjects.Add(go);
            await Task.Delay(500,cancellationTokenSource.Token);
        }
        catch{
            return;
        }
        finally{
            
            cancellationTokenSource?.Cancel();
            cancellationTokenSource = null;
        }

        
        

        // spawnedObject.transform.position = spawnPosition;
    }

    protected  void DestroySpawns(){
        foreach(GameObject go in spawnedObjects){
            Destroy(go);
        }
    }
}