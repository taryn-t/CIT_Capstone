using System.Collections.Generic;
using UnityEngine;

public abstract class Spawner : TimeAgent
{
    
    [SerializeField] protected GameObject[] spawnPrefabs;
    [SerializeField] protected Vector3 spawnPosition;
    
    
    [SerializeField] protected List<GameObject> spawnedObjects;
    private int amountSpawned = default;
    [SerializeField] protected int maxAmount;
    [SerializeField] private float cullDistance = 16f;
    private void Start(){    
        spawnedObjects = new List<GameObject>();
         onTimeTick += Spawn;
         Init();
    }
    protected virtual void Spawn()
    {
        if(GameManager.Instance.player !=null){
            Transform playerTransform = GameManager.Instance.player.transform;
            float distance = Vector2.Distance(transform.position, playerTransform.position);
            
            if(distance<cullDistance)
            {
                if(amountSpawned <= maxAmount){
                    int randomIndex = UnityEngine.Random.Range(0, spawnPrefabs.Length);

                    GameObject go = Instantiate(spawnPrefabs[randomIndex],transform.position, Quaternion.identity); 
                    spawnedObjects.Add(go);
                    amountSpawned++;
                }
            } 
        }
        

        
         
        // spawnedObject.transform.position = spawnPosition;
    }
}