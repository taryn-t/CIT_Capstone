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
    bool walkPointSet;
    public float walkPointRange;
    Rigidbody2D Body;
    CapsuleCollider2D collider ;
    public float avoidanceForceMultiplier = 0.001f;
    public float raySpacing = 1f;
    public float maxSpeed = 10f;
    public LayerMask obstacleLayerMask;

    public Patrol(Transform enemy, float moveSpeed, float range, Rigidbody2D body, CapsuleCollider2D collider, LayerMask layer )
    {
        enemyTransform = enemy;
       
        speed = moveSpeed;
        walkPointRange = range;
        Body = body;
        this.collider = collider;
        obstacleLayerMask = layer;
        ContactFilter2D contactFilter = new ContactFilter2D();
        contactFilter.layerMask = obstacleLayerMask;

    }

    public override NodeStatus Execute( )
    {

        ContactPoint2D[] contacts = new ContactPoint2D[10];
        
        collider.GetContacts(contacts);
         Vector2 walkPoint = SearchWalkPoint();

        Vector2 avoidanceForce = Vector2.zero;
        
        foreach(ContactPoint2D contact in contacts){
            avoidanceForce += contact.normal;
        }

        Vector2 direction = (walkPoint - Body.position).normalized;
        Body.AddForce(speed * Time.deltaTime * (Vector3)direction);

        if(avoidanceForce != Vector2.zero){
            Body.velocity = Vector2.zero;
            Body.AddForce(speed/2 * Time.deltaTime * (Vector3)avoidanceForce, ForceMode2D.Impulse);
        }

        
        
        
               
             
        return NodeStatus.Running;
            
    }

    

     private  Vector2 SearchWalkPoint()
    {


                System.Random rnd = new();
                // Calculate random point in range
                float randomY = rnd.Next((int)-walkPointRange, (int)walkPointRange);
                float randomX = rnd.Next((int)-walkPointRange,(int) walkPointRange);

                
                int xClamp = (int)Mathf.Clamp(Body.position.x + randomX, GameManager.Instance.worldBounds.minWorld.x+10,  GameManager.Instance.worldBounds.maxWorld.x-10);
                int yClamp =(int)Mathf.Clamp(Body.position.y + randomY, GameManager.Instance.worldBounds.minWorld.y+10,GameManager.Instance.worldBounds.maxWorld.y-10);
                

                Vector2 walkPoint = new Vector2(xClamp, yClamp);
                
                Vector3Int direction = Vector3Int.RoundToInt((walkPoint-Body.position).normalized);

                

                    Vector3Int vInt = Vector3Int.CeilToInt(walkPoint);

                    if(GameManager.Instance.baseTilemap.HasTile(vInt)){
                        return walkPoint;
                    } 
                    
                    return SearchWalkPoint();
 

           

  
        
    }

    private bool IsGround(){
       Chunk chunk = GameManager.Instance.gameData.map.GetChunk(Vector3Int.FloorToInt(enemyTransform.position));

       if(chunk!=null){
            return Task.Run(()=>chunk.IsPosGround(Body.position)).GetAwaiter().GetResult();
       }

       return false;
    }

   
       
       
    
}