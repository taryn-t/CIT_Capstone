using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class MoveTowardsPlayer : Node
{
    private Transform enemyTransform;
    private Transform playerTransform;
    private float speed;
    Rigidbody2D Body;
     private float detectionRange;
     private NodeStatus status;
     CapsuleCollider2D collider;
     public float avoidanceForceMultiplier = 0.5f;
    public float raySpacing = 2f;
    public float maxSpeed = 10f;
    public LayerMask obstacleLayerMask;
    private CancellationTokenSource cancellationTokenSource;

    public MoveTowardsPlayer(Transform enemy, Transform player, float moveSpeed, Animator animator, Rigidbody2D body, float range, CapsuleCollider2D collider,  LayerMask layer,CancellationTokenSource cancellationTokenSource)
    {
        enemyTransform = enemy;
        playerTransform = player;
        speed = moveSpeed;
        this.animator = animator;
        Body = body;
        detectionRange = range;
        this.collider = collider;
        obstacleLayerMask = layer;
        this.cancellationTokenSource = cancellationTokenSource;
    }

    public override IEnumerator Execute(MonoBehaviour mono)
    {
        cancellationTokenSource = new CancellationTokenSource();
      
        if(IsInRange())
        {
          
          // Vector2 direction = (playerTransform.position - enemyTransform.position).normalized;
          

           SmoothMovement(playerTransform.position,cancellationTokenSource);

          if(status == NodeStatus.Success){
            yield return status;
          }
        }
        else
        {
            yield return NodeStatus.Failure;
        }
        
        // enemyTransform.position += (Vector3)direction * speed * Time.deltaTime;
        yield return NodeStatus.Running; // This action runs continuously
    }

    //maybe make it not async
     async void SmoothMovement( UnityEngine.Vector2 end, CancellationTokenSource cancellation)
    {   
        

        try
        {
           await Task.Delay(3000, cancellation.Token);
    
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
            
                await Task.Delay(1200, cancellation.Token);
            }

            Body.position = end;
        
        }
        catch
        {
            
            return;
        }
        finally
        {
            cancellation.Dispose();
            cancellation = null;
        }
        
    }

    public bool IsInRange(){
        float distance = Vector2.Distance(Body.position, Body.position);
        return distance <= detectionRange ? true : false;
    }

    // public void Move(){
    //      Vector3 playerV = (playerTransform.position - enemyTransform.position).normalized;
    //     Vector3Int playerDirection = Vector3Int.RoundToInt(playerV);
    //     // Cast rays to detect obstacles
    //     RaycastHit2D[] hits = new RaycastHit2D[3];
    //     Vector2 rayStart = enemyTransform.position + playerDirection * ((int) Body.velocity.magnitude) * ((int)Time.deltaTime);
    //     for (int i = 0; i < 3; i++)
    //     {
    //         Vector2 rayDirection = Quaternion.AngleAxis((i - 1) * 30f, Vector3.forward) * playerDirection;
    //         hits[i]= Physics2D.Raycast(rayStart, rayDirection, raySpacing, obstacleLayerMask);

    //      Debug.DrawRay(rayStart, rayDirection * raySpacing, Color.red);
    //     }

    //     // Calculate avoidance force
    //     Vector2 avoidanceForce = Vector2.zero;
    //     Vector2 playerDirectionV =  new Vector2(playerDirection.x,playerDirection.y);
    //     foreach (RaycastHit2D hit in hits)
    //     {
    //         if (hit.collider != null)
    //         {
    //             float distanceToObstacle = Vector2.Distance(enemyTransform.position, hit.collider.transform.position);
    //             float distanceToRay = Vector2.Distance(rayStart, hit.point);
    //             avoidanceForce += Vector2.Lerp(playerDirectionV, hit.normal, distanceToRay / distanceToObstacle) * avoidanceForceMultiplier;
    //         }
    //     }

    //     // Apply avoidance force to velocity
    //     Body.AddForce(avoidanceForce);
        
    //     UnityEngine.Vector2 finalPos = playerTransform.position; 
        
    //      while(Body.position !=finalPos)
    //     {

    //         Body.AddForce(playerDirectionV * speed, ForceMode2D.Impulse);
           
            
    //     }
    // }

    
    
}