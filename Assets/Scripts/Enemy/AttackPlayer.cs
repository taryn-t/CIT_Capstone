using UnityEngine;

public class AttackPlayer : Node
{
    private Transform enemyTransform;
    private Transform playerTransform;
    private float attackRange;

    public AttackPlayer(Transform enemy, Transform player, float range)
    {
        enemyTransform = enemy;
        playerTransform = player;
        attackRange = range;
    }

    public override NodeStatus Execute()
    {
        float distance = Vector2.Distance(enemyTransform.position, playerTransform.position);
        if (distance <= attackRange)
        {
            // Implement attack logic here
            Debug.Log("Attacking the player!");
            return NodeStatus.Success;
        }
        return NodeStatus.Failure;
    }
}