using UnityEngine;

public class MoveTowardsPlayer : Node
{
    private Transform enemyTransform;
    private Transform playerTransform;
    private float speed;

    public MoveTowardsPlayer(Transform enemy, Transform player, float moveSpeed)
    {
        enemyTransform = enemy;
        playerTransform = player;
        speed = moveSpeed;
    }

    public override NodeStatus Execute()
    {
        Vector2 direction = (playerTransform.position - enemyTransform.position).normalized;
        enemyTransform.position += (Vector3)direction * speed * Time.deltaTime;
        return NodeStatus.Running; // This action runs continuously
    }
}