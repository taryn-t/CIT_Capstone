using UnityEngine;

public class CheckPlayerInRange : Node
{
    private Transform enemyTransform;
    private Transform playerTransform;
    private float detectionRange;

    public CheckPlayerInRange(Transform enemy, Transform player, float range)
    {
        enemyTransform = enemy;
        playerTransform = player;
        detectionRange = range;
    }

    public override NodeStatus Execute()
    {
        if (GameManager.Instance.GetPlayer().visible)
        {
            float distance = Vector2.Distance(enemyTransform.position, playerTransform.position);
            enemyTransform.gameObject.GetComponent<EnemyAI>().playerInSightRange = distance <= detectionRange;
            return distance <= detectionRange ? NodeStatus.Success : NodeStatus.Running;
        }
        else
        {
            enemyTransform.gameObject.GetComponent<EnemyAI>().playerInSightRange = false;
            return NodeStatus.Running;
        }

    }
}