using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    private BehaviorTree behaviorTree;

    public float attackRange = 1f;
    public float patrolRange = 5f;
    public float detectionRange = 5f;
    public float moveSpeed = 2f;

    void Start()
    {
        behaviorTree = new BehaviorTree();
        InitializeBehaviorTree();
    }

    void Update()
    {
        behaviorTree.Tick();
    }

    private void InitializeBehaviorTree()
    {
        Transform playerTransform = GameObject.FindWithTag("Player").transform;
       

        Node checkPlayerInRange = new CheckPlayerInRange(transform, playerTransform, detectionRange);

        Node moveTowardsPlayer = new MoveTowardsPlayer(transform, playerTransform, moveSpeed);

        Node attackPlayer = new AttackPlayer(transform,playerTransform,attackRange);

        Node patrol = new Patrol(transform,moveSpeed,patrolRange);

        Node[] sequenceNodes = { checkPlayerInRange, moveTowardsPlayer, attackPlayer, patrol };
        Node behaviorTreeRoot = new Sequence(sequenceNodes);
        
        behaviorTree.SetRoot(behaviorTreeRoot);
    }
}