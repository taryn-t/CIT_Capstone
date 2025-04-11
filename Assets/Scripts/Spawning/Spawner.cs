using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Spawner : MonoBehaviour
{
    
    [SerializeField] protected GameObject[] spawnPrefabs;
    [SerializeField] protected Vector3 spawnPosition;
    
    [SerializeField] protected List<GameObject> spawnedObjects;
    public int amountSpawned = default;
    [SerializeField] protected int maxAmount;
   
   

    private void Start(){    
        spawnedObjects = new List<GameObject>();
        
    }
 

    protected virtual IEnumerator Spawn(Transform parent){
        int randomIndex = UnityEngine.Random.Range(0, spawnPrefabs.Length);

            GameObject go = Instantiate(spawnPrefabs[randomIndex],transform.position, Quaternion.identity,parent); 
            spawnedObjects.Add(go);
            yield return new WaitForSeconds(0.5f);
    }

    protected  void DestroySpawns(){
        foreach(GameObject go in spawnedObjects){
            Destroy(go);
        }
    }
}