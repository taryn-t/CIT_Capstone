using System.Collections;
using System.Diagnostics;
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
     public float avoidanceForceMultiplier =0.1f;
    public float raySpacing = 1f;
    public float maxSpeed = 10f;
    public LayerMask obstacleLayerMask;
    private CancellationTokenSource cancellationTokenSource;
    private ContactFilter2D contactFilter;
    private  RaycastHit2D[] hits;
    UnityEngine.Vector2 avoidanceForce;
    UnityEngine.Vector2 rayStart;

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
        contactFilter = new ContactFilter2D();
        contactFilter.layerMask = obstacleLayerMask;
    }

    public override IEnumerator Execute(MonoBehaviour mono)
    {
        
        cancellationTokenSource = new CancellationTokenSource();
      
        if(IsInRange())
        {
          
          // Vector2 direction = (playerTransform.position - enemyTransform.position).normalized;
          

           SmoothMovement(playerTransform.position,cancellationTokenSource);

          
            yield return NodeStatus.Success;
          
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
         
            UnityEngine.Vector3 direction = (end-Body.position).normalized;
     

            rayStart = enemyTransform.position + ((int) Body.velocity.magnitude) * ((int)Time.deltaTime) * direction;
     
            avoidanceForce = UnityEngine.Vector2.zero;

            UnityEngine.Vector2 playerDirectionV =  new(direction.x,direction.y);

            while(Body.position != end)
            {
                
                await CalculateAvoidance(direction);

                Body.velocity += avoidanceForce* speed;

                Body.AddForce( direction * speed);
                Body.velocity = Vector2.zero;
            }
            
            Body.position = end;

            await Task.Delay(1500, cancellation.Token);
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
    
    private async Task CalculateAvoidance(Vector3 direction)
    {

         collider.Cast(direction ,contactFilter, hits, raySpacing);

        foreach (RaycastHit2D hit in hits)
        {
            
            if (hit.collider != null)
            {
                
                float distanceToObstacle = UnityEngine.Vector2.Distance(enemyTransform.position, hit.collider.transform.position);
                
                float distanceToRay =  UnityEngine.Vector2.Distance(rayStart, hit.point);
                avoidanceForce +=  UnityEngine.Vector2.Lerp(direction,hit.normal,distanceToObstacle/distanceToRay) * avoidanceForceMultiplier;
            }
        }

        await Task.Yield();
    }


    public bool IsInRange(){
        float distance = Vector2.Distance(Body.position, playerTransform.position);
        return distance <= detectionRange;
    }

   
    
}