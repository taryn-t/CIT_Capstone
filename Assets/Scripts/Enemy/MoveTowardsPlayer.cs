using UnityEngine;

public class MoveTowardsPlayer : Node
{
    private Transform enemyTransform;
    private Transform playerTransform;
    private float speed;
    Rigidbody2D body;

    public MoveTowardsPlayer(Transform enemy, Transform player, float moveSpeed, Rigidbody2D body)
    {
        enemyTransform = enemy;
        playerTransform = player;
        speed = moveSpeed;
        this.body = body;
    }

    public override NodeStatus Execute()
    {
        if(GameManager.Instance.GetPlayer().visible){
            
           Vector2 direction = (playerTransform.position - enemyTransform.position).normalized;
            body.AddForce((Vector3)direction * speed * Time.deltaTime);

            return NodeStatus.Running; // This action runs continuously 
        }

        return NodeStatus.Failure;
    }
}