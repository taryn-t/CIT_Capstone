using UnityEngine;

public class MoveTowardsPlayer : Node
{
    private Transform enemyTransform;
    private Transform playerTransform;
    private float speed;
    Rigidbody2D body;
    LayerMask playerLayerMask;
  public float stopDistance = 0.2f;
    public float avoidanceForce = 0.05f;
    LayerMask obsticleLayerMask;
    private Enemy enemy;
    public MoveTowardsPlayer(Transform enemyTransform, Transform player, float moveSpeed, Rigidbody2D body, LayerMask playerLayerMask, LayerMask obsticleLayerMask)
    {
        this.enemyTransform = enemyTransform;
        playerTransform = player;
        speed = moveSpeed;
        this.body = body;
        this.playerLayerMask = playerLayerMask;
        this.obsticleLayerMask = obsticleLayerMask;
        enemy = enemyTransform.gameObject.GetComponent<Enemy>();
    }

    public override NodeStatus Execute()
    {
        if(GameManager.Instance.GetPlayer().visible && enemy.gameObject.GetComponent<EnemyAI>().playerInSightRange){
            
            

            Vector2 target  = playerTransform.position;

            Vector2 currentPosition = body.position;
            Vector2 toTarget = (target - (Vector2)body.position).normalized ;
            
            // if (toTarget.magnitude <= stopDistance || enemyTransform.gameObject.GetComponent<EnemyAI>().playerInRayAttack)
            // {
            //     body.velocity = Vector2.zero;
            //     return NodeStatus.Success;
            // }

             SmoothMovement(toTarget,currentPosition);
                
            return NodeStatus.Success;
        }

        return NodeStatus.Running;
    }


    public void SmoothMovement(Vector2 toTarget, Vector2 currentPosition){
        Vector2 seek = toTarget.normalized * speed ;

         if(enemy.slow){
          seek *= 0.5f;   
        }

        Vector2 avoid = Vector2.zero;
        
        Collider2D[] hits = Physics2D.OverlapCircleAll(currentPosition, 0.5f, obsticleLayerMask);

        foreach (Collider2D hit in hits)
        {
            Vector2 away = currentPosition - (Vector2)hit.ClosestPoint(currentPosition);
            if (away != Vector2.zero)
                avoid += away.normalized / away.magnitude; // Stronger repulsion if closer
        }

        avoid *= avoidanceForce;

        // Combine and apply
        Vector2 totalForce = seek + avoid;
        body.AddForce(totalForce);
    }
}