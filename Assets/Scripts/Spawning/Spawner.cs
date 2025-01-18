using UnityEngine;

public abstract class Spawner : MonoBehaviour
{
    
    [SerializeField] protected GameObject spawnPrefab;
    [SerializeField] protected Vector3 spawnPosition;
    
    
    [SerializeField] protected GameObject spawnedObject;
    
    protected virtual void Spawn()
    {
        spawnedObject = Instantiate(spawnPrefab,transform);
        // spawnedObject.transform.position = spawnPosition;
    }
}