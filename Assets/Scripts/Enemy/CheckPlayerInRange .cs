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
        Debug.Log("Checking if in range");
        float distance = Vector2.Distance(enemyTransform.position, playerTransform.position);
        return distance <= detectionRange ? NodeStatus.Success : NodeStatus.Failure;
    }
}