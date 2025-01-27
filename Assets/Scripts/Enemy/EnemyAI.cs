using System.Threading.Tasks;
using UnityEditor.Rendering;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public BehaviorTree behaviorTree;

    public float attackRange = 1f;
    public float patrolRange = 1f;
    public float detectionRange = 5f;
    public float moveSpeed = 1f;
    public Animator animator;

    [SerializeField] public Spell spellAttack;
    [SerializeField]public  GameObject spellPrefab;
    [SerializeField] public LayerMask obstacleLayerMask;
    public Vector2 lastMotionVector;
      private Rigidbody2D body;
    public Rigidbody2D Body{
        get{return body;}
        set{body=value;}
    }
    public Vector2 smoothDeltaPosition = Vector2.zero;
    public bool moving;
    public CapsuleCollider2D collider;

    
   

    public void InitializeBehaviorTree()
    {
        Transform playerTransform = GameManager.Instance.player.transform;
       

        Node checkPlayerInRange = new CheckPlayerInRange(transform, playerTransform, detectionRange);

        Node moveTowardsPlayer = new MoveTowardsPlayer(transform, playerTransform, moveSpeed, animator, Body, detectionRange, collider, obstacleLayerMask);

        Node attackPlayer = new AttackPlayer(transform,playerTransform,attackRange, animator, spellAttack, spellPrefab, lastMotionVector, collider);

        Node patrol = new Patrol(transform,moveSpeed,patrolRange, animator, Body, collider, obstacleLayerMask);

        Node[] sequenceNodes = { patrol, moveTowardsPlayer, attackPlayer };
        
        Node behaviorTreeRoot = new Sequence(sequenceNodes);
        
        behaviorTree.SetRoot(behaviorTreeRoot);
    }

    public async void Movement(){
        
        float horizontal = Body.velocity.x;
        float vertical = Body.velocity.y;
        

        UnityEngine.Vector2 motionVector = new UnityEngine.Vector2(horizontal,vertical);
        float smooth = Mathf.Min(1.0f, Time.deltaTime / 0.15f);
        smoothDeltaPosition = Vector2.Lerp(smoothDeltaPosition, motionVector, smooth);

        animator.SetFloat("horizontal",horizontal);
        animator.SetFloat("vertical",vertical);
        moving = horizontal != 0 || vertical != 0;
        animator.SetBool("moving",moving);


        if(moving){
            lastMotionVector = smoothDeltaPosition.normalized;
            animator.SetFloat("lastHorizontal",lastMotionVector.x);
            animator.SetFloat("lastVertical",lastMotionVector.y);
        }
        body.velocity = smoothDeltaPosition / Time.deltaTime *0.01f;
        await Task.Yield();
    }

  
}