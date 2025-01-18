using UnityEngine;
using UnityEngine.Tilemaps;

public class Patrol : Node
{
    private Transform enemyTransform;
   
    private float speed;
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    public Patrol(Transform enemy, float moveSpeed, float range)
    {
        enemyTransform = enemy;
       
        speed = moveSpeed;
        walkPointRange = range;
    }

    public override NodeStatus Execute()
    {
        if (!walkPointSet){ SearchWalkPoint(); }

        if (walkPointSet){
            Vector2 direction = (walkPoint - enemyTransform.position).normalized;

            enemyTransform.position += speed * Time.deltaTime * (Vector3)direction;
        }
         Vector3 distanceToWalkPoint = enemyTransform.position - walkPoint;

          if (distanceToWalkPoint.magnitude < 1f){
                walkPointSet = false;   
                return NodeStatus.Success;
          }
            
        return NodeStatus.Running; // This action runs continuously
    }

     private void SearchWalkPoint()
    {
        // Calculate random point in range
        float randomY = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(enemyTransform.position.x + randomX, enemyTransform.position.y + randomY, 0);

        
        walkPointSet = IsGround();
    }

    private bool IsGround(){
       Chunk chunk = GameManager.Instance.gameData.map.GetChunk(Vector3Int.FloorToInt(enemyTransform.position));

       if(chunk!=null){
            return chunk.IsPosGround(enemyTransform.position);
       }

       return false;
    }
}