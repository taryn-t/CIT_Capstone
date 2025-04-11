
using System.Collections;
using UnityEngine;

public class Patrol : Node
{
    private float stuckTimer = 0f;
    private Vector2 lastPosition;
    public float stuckCheckInterval = 1f;
    public float stuckThreshold = 0.1f;
    public float maxStuckTime = 1f;
    private bool isStuck = false;
    private Transform enemyTransform;
   
    private float speed;
    bool walkPointSet;
    public float walkPointRange;
    Rigidbody2D Body;
    CapsuleCollider2D collider ;
    public float raySpacing = 1f;
    public float maxSpeed = 10f;
    public LayerMask obstacleLayerMask;
    public bool moving;
    Vector2 walkPoint;
    public float stopDistance = 0.2f;
    public float avoidanceForce = 0.05f;
    private Enemy enemy;
    public Patrol(Transform enemyTransform, float moveSpeed, float range, Rigidbody2D body, CapsuleCollider2D collider, LayerMask layer )
    {
        this.enemyTransform = enemyTransform;
       
        speed = moveSpeed;
        walkPointRange = range;
        Body = body;
        this.collider = collider;
        obstacleLayerMask = layer;
        ContactFilter2D contactFilter = new ContactFilter2D();
        contactFilter.layerMask = obstacleLayerMask;
        enemy = enemyTransform.gameObject.GetComponent<Enemy>();
    }

    public override NodeStatus Execute( )
    {
        stuckTimer += Time.fixedDeltaTime;

        // if(enemyTransform.gameObject.GetComponent<EnemyAI>().playerInSightRange){
            

        //     return NodeStatus.Failure;
        
        // }
        

        if (stuckTimer >= stuckCheckInterval)
        {
            float movedDistance = Vector2.Distance(Body.position, lastPosition);

            if (movedDistance < stuckThreshold)
            {
                isStuck = true;
                Debug.Log("Stuck detected!");
            }
            else
            {
                isStuck = false;
            }

            lastPosition = Body.position;
            stuckTimer = 0f;
        }

        if(!moving || isStuck){
             walkPoint = FindNewAccessibleTarget();
             enemyTransform.gameObject.GetComponent<EnemyAI>().targetPosition = walkPoint;
        }

        
        Vector2 currentPosition = Body.position;
        Vector2 toTarget = walkPoint - currentPosition;
        
        if (toTarget.magnitude <= stopDistance)
        {
            Body.velocity = Vector2.zero;
            Debug.Log("Target Reached");
            moving = false;
            return NodeStatus.Success;
        }

        SmoothMovement(toTarget,currentPosition);

             
        return NodeStatus.Running;
            
    }

    public void SmoothMovement(Vector2 toTarget, Vector2 currentPosition){
        

        Vector2 seek = toTarget.normalized * speed ;

        if(enemy.slow){
          seek *= 0.5f;   
        }

        Vector2 avoid = Vector2.zero;
        
        Collider2D[] hits = Physics2D.OverlapCircleAll(currentPosition, 0.5f, obstacleLayerMask);

         foreach (Collider2D hit in hits)
        {
            Vector2 away = currentPosition - (Vector2)hit.ClosestPoint(currentPosition);
            if (away != Vector2.zero)
                avoid += away.normalized / away.magnitude; // Stronger repulsion if closer
        }

        avoid *= avoidanceForce;

        // Combine and apply
        Vector2 totalForce = seek + avoid;
        Body.AddForce(totalForce,ForceMode2D.Force);
    }

    Vector2 FindNewAccessibleTarget()
    {
        for (int i = 0; i < 10; i++) // Try a few times
        {
            Vector2 randomOffset = Random.insideUnitCircle.normalized * Random.Range(-walkPointRange, walkPointRange);
            Vector2 candidate = Body.position + randomOffset;

            // Check if it's in open space
            if (!Physics2D.OverlapCircle(candidate, 0.5f, obstacleLayerMask))
            {
                return candidate;
            }
        }

        // If nothing found, fallback
        return Body.position; // Or some other safe fallback
    }

     private  Vector2 SearchWalkPoint()
    {


        System.Random rnd = new();
        // Calculate random point in range
        float randomY = rnd.Next((int)-walkPointRange, (int)walkPointRange);
        float randomX = rnd.Next((int)-walkPointRange,(int) walkPointRange);

        
        int xClamp = (int)Mathf.Clamp(enemyTransform.position.x + randomX, 0,  64);
        int yClamp =(int)Mathf.Clamp(enemyTransform.position.y + randomY, 0, 64);
        

        Vector2 walkPoint = new Vector2(xClamp, yClamp);
        

        Vector3Int vInt = Vector3Int.CeilToInt(walkPoint);

        if(GameManager.Instance.baseTilemap.GetTile(vInt) == GameManager.Instance.mapGenerator.Floor){
            return walkPoint;
        } 
        
        return SearchWalkPoint();
 
    }



   
       
       
    
}