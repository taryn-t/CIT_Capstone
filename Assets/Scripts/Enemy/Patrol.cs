using System.Collections;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Patrol : Node
{
    private Transform enemyTransform;
   
    private float speed;
    public UnityEngine.Vector2 walkPoint;
    bool walkPointSet;
    public float walkPointRange;
    Rigidbody2D Body;
    CapsuleCollider2D collider ;
    public float avoidanceForceMultiplier = 0.5f;
    public float raySpacing = 2f;
    public float maxSpeed = 10f;
    public LayerMask obstacleLayerMask;
    private CancellationTokenSource cancellationTokenSource;

    public Patrol(Transform enemy, float moveSpeed, float range, Animator animator, Rigidbody2D body, CapsuleCollider2D collider, LayerMask layer, CancellationTokenSource cancellationTokenSource )
    {
        enemyTransform = enemy;
       
        speed = moveSpeed;
        walkPointRange = range;
        this.animator = animator;
        Body = body;
        this.collider = collider;
        obstacleLayerMask = layer;
        this.cancellationTokenSource = cancellationTokenSource;
        
    }

    public override IEnumerator Execute(MonoBehaviour mono)
    {
   
        if (!walkPointSet){ SearchWalkPoint(); }

        if (walkPointSet){
            
           
            SmoothMovement(walkPoint);

            // enemyTransform.position += speed * Time.deltaTime * (Vector3)direction;
        }
         UnityEngine.Vector3 distanceToWalkPoint = Body.position - walkPoint;

          if (distanceToWalkPoint.magnitude < 1f){
                walkPointSet = false;   
                yield return NodeStatus.Success;
          }
            
        yield return NodeStatus.Running; // This action runs continuously
    }

    

     async void SmoothMovement( UnityEngine.Vector2 end)
    {   
       
           await Task.Delay(3000);
    
        Vector3Int direction = Vector3Int.RoundToInt((end-Body.position).normalized);
        UnityEngine.Vector3 dV3 = direction;
     

        // If you really need a precision down to epsilon
        //while (!Mathf.Approximately(Vector2.Distance(Body.position, end), 0f))
        // otherwise for most use cases in physics the default precision of 
        // 0.00001f should actually be enough

        
        
        // Calculate avoidance force
       
         RaycastHit2D[] hits = new RaycastHit2D[1];

            UnityEngine.Vector2 rayStart = enemyTransform.position + ((int) Body.velocity.magnitude) * ((int)Time.deltaTime) * dV3;
            collider.Raycast(dV3, hits, raySpacing, obstacleLayerMask);
            UnityEngine.Vector2 avoidanceForce = UnityEngine.Vector2.zero;
            UnityEngine.Vector2 playerDirectionV =  new UnityEngine.Vector2(dV3.x,dV3.y);
            Debug.DrawRay(rayStart, dV3 * raySpacing, Color.red);

        while(Body.position != end)
        {
           
            foreach (RaycastHit2D hit in hits)
            {
                if (hit.collider != null && !hit.collider.gameObject.CompareTag("Player"))
                {
                    float distanceToObstacle = UnityEngine.Vector2.Distance(enemyTransform.position, hit.collider.transform.position);
                    float distanceToRay =  UnityEngine.Vector2.Distance(rayStart, hit.point);
                    avoidanceForce +=  UnityEngine.Vector2.Lerp(playerDirectionV, hit.normal, distanceToRay / distanceToObstacle) * avoidanceForceMultiplier;
                }
            }

            Body.AddForce(avoidanceForce);

            Body.AddForce(dV3 * speed, ForceMode2D.Impulse);
           
            await Task.Delay(1200, cancellationTokenSource.Token);
        }

        Body.position = end;
        
  
        
    }

     private async void SearchWalkPoint()
    {

            
            System.Random rnd = new();
            // Calculate random point in range
            float randomY = rnd.Next((int)-walkPointRange, (int)walkPointRange);
            float randomX = rnd.Next((int)-walkPointRange,(int) walkPointRange);

            int maxX = GameManager.Instance.baseTilemap.cellBounds.xMax;
            int maxY = GameManager.Instance.baseTilemap.cellBounds.yMax;
            int minX = GameManager.Instance.baseTilemap.cellBounds.xMin;
            int minY = GameManager.Instance.baseTilemap.cellBounds.yMin;

            UnityEngine.Vector3Int maxCell = new UnityEngine.Vector3Int(maxX,maxY);
            UnityEngine.Vector3Int minCell = new UnityEngine.Vector3Int(minX,minY);
            
            UnityEngine.Vector3 maxWorld = GameManager.Instance.baseTilemap.CellToWorld(maxCell);
            UnityEngine.Vector3 minWorld = GameManager.Instance.baseTilemap.CellToWorld(minCell);

            int xClamp = (int)Mathf.Clamp(Body.position.x + randomX, minWorld.x+10, maxWorld.x-10);
            int yClamp =(int)Mathf.Clamp(Body.position.y + randomY, minWorld.y+10,maxWorld.y-10);

            

            walkPoint = new UnityEngine.Vector2(xClamp, yClamp);
            
            
            walkPointSet = IsGround();
            await Task.Yield();
      
        
    }

    private bool IsGround(){
       Chunk chunk = GameManager.Instance.gameData.map.GetChunk(Vector3Int.FloorToInt(enemyTransform.position));

       if(chunk!=null){
            return Task.Run(()=>chunk.IsPosGround(Body.position)).GetAwaiter().GetResult();
       }

       return false;
    }

    public void Move( UnityEngine.Vector3 end){
        UnityEngine.Vector3 playerV = ( enemyTransform.position-end).normalized;
        UnityEngine.Vector3Int playerDirection = UnityEngine.Vector3Int.RoundToInt(playerV);
        // Cast rays to detect obstacles
        RaycastHit2D[] hits = new RaycastHit2D[3];
        UnityEngine.Vector2 rayStart = enemyTransform.position + playerDirection * ((int) Body.velocity.magnitude) * ((int)Time.deltaTime);
        for (int i = 0; i < 3; i++)
        {
            UnityEngine.Vector2 rayDirection =  UnityEngine.Quaternion.AngleAxis((i - 1) * 30f, UnityEngine.Vector3.forward) * playerDirection;
            hits[i]= Physics2D.Raycast(rayStart, rayDirection, raySpacing, obstacleLayerMask);

         Debug.DrawRay(rayStart, rayDirection * raySpacing, Color.red);
        }

        // Calculate avoidance force
         UnityEngine.Vector2 avoidanceForce = UnityEngine.Vector2.zero;
         UnityEngine.Vector2 playerDirectionV =  new UnityEngine.Vector2(playerDirection.x,playerDirection.y);
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider != null)
            {
                float distanceToObstacle = UnityEngine.Vector2.Distance(enemyTransform.position, hit.collider.transform.position);
                float distanceToRay =  UnityEngine.Vector2.Distance(rayStart, hit.point);
                avoidanceForce +=  UnityEngine.Vector2.Lerp(playerDirectionV, hit.normal, distanceToRay / distanceToObstacle) * avoidanceForceMultiplier;
            }
        }

        // Apply avoidance force to velocity
        Body.AddForce(avoidanceForce);
        
        // // Normalize velocity to max speed
        // if (Body.velocity.magnitude > maxSpeed)
        // {
        //     Body.velocity = Body.velocity.normalized * maxSpeed;
        // }

        UnityEngine.Vector2 finalPos = end; 
        
         while(Body.position !=finalPos)
        {

            Body.AddForce(playerDirectionV * speed, ForceMode2D.Impulse);
           
            
        }
       
       
    }
}