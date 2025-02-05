using System.Collections;
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
    public float avoidanceForceMultiplier = 0.001f;
    public float raySpacing = 1f;
    public float maxSpeed = 10f;
    public LayerMask obstacleLayerMask;
    private CancellationTokenSource cancellationTokenSource;
    UnityEngine.Vector3 lastDirection;
     private NodeStatus status;
     private ContactFilter2D contactFilter;
    private  RaycastHit2D[] hits;
    UnityEngine.Vector2 avoidanceForce;
    UnityEngine.Vector2 rayStart;
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
        ContactFilter2D contactFilter = new ContactFilter2D();
        contactFilter.layerMask = obstacleLayerMask;
        hits = new RaycastHit2D[1];
    }

    public override IEnumerator Execute(MonoBehaviour mono)
    {
        
        cancellationTokenSource = new CancellationTokenSource();
   
        if (!walkPointSet)
        {
              SearchWalkPoint(cancellationTokenSource);
              

         }
        
        if (walkPointSet){
             
            SmoothMovement(walkPoint,cancellationTokenSource);
           
            yield return NodeStatus.Success;
            
            // enemyTransform.position += speed * Time.deltaTime * (Vector3)direction;
        }
        else{
             yield return NodeStatus.Failure;
        }
         
            
        yield return NodeStatus.Running; // This action runs continuously
    }

    

    async void SmoothMovement( UnityEngine.Vector2 end, CancellationTokenSource cancellation)
    {   
        
       try
        {
         
            UnityEngine.Vector3 direction = (end-Body.position).normalized;
     

            rayStart = enemyTransform.position + ((int) Body.velocity.magnitude) * ((int)Time.deltaTime) * direction;
     
            avoidanceForce = UnityEngine.Vector2.zero;

            UnityEngine.Vector2 playerDirectionV =  new UnityEngine.Vector2(direction.x,direction.y);

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
     private async void SearchWalkPoint(CancellationTokenSource cancellation)
    {

        
          try
        {
            

            while(!walkPointSet){
                 System.Random rnd = new();
                // Calculate random point in range
                float randomY = rnd.Next((int)-walkPointRange, (int)walkPointRange);
                float randomX = rnd.Next((int)-walkPointRange,(int) walkPointRange);

                GameManager.Instance.baseTilemap.CompressBounds();

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
                
                Vector3Int direction = Vector3Int.RoundToInt((walkPoint-Body.position).normalized);

                if(direction != lastDirection){

                    Vector3Int vInt = Vector3Int.CeilToInt(walkPoint);
                    walkPointSet = GameManager.Instance.baseTilemap.HasTile(vInt) ;

                }
            
            }
           

           await Task.Delay(500, cancellation.Token);
            
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

    private bool IsGround(){
       Chunk chunk = GameManager.Instance.gameData.map.GetChunk(Vector3Int.FloorToInt(enemyTransform.position));

       if(chunk!=null){
            return Task.Run(()=>chunk.IsPosGround(Body.position)).GetAwaiter().GetResult();
       }

       return false;
    }

   
       
       
    
}