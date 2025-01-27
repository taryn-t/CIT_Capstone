using UnityEngine;
public class EnemySpawner : Spawner
{
    [SerializeField] private Color spawnColor;
    [SerializeField] private int spawnAmount;



    
    //Overriding base class method
    protected override void Spawn(){
       
        for(var i = 0; i < spawnAmount; i++)
            {
                
                //Calling base class method without override
                base.Spawn();
                // var enemyRenderer = spawnedObject.GetComponent<Renderer>();
                // enemyRenderer.material.color = spawnColor;
            }
        
    }
}