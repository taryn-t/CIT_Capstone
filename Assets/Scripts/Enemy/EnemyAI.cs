using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using UnityEditor.Rendering;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public BehaviorTree behaviorTree;

    public float attackRange =0.5f;
    public float patrolRange = 5f;
    public float detectionRange = 2f;
    private float moveSpeed = 20f;
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
    public CapsuleCollider2D col;

    public bool damaged = false;
   
    public CancellationTokenSource cancellationTokenSource;
    public void InitializeBehaviorTree()
    {
        Rigidbody2D body = GetComponent<Rigidbody2D>();
        Transform playerTransform = GameManager.Instance.player.transform;
        Rigidbody2D playerbody = GameManager.Instance.player.GetComponent<Rigidbody2D>();

        Node checkPlayerInRange = new CheckPlayerInRange(transform, playerTransform, detectionRange);

        Node moveTowardsPlayer = new MoveTowardsPlayer(transform, playerTransform, moveSpeed, Body);

        Node attackPlayer = new AttackPlayer(body,playerbody,attackRange, animator, spellAttack, spellPrefab, lastMotionVector, col, damaged);

        Node patrol = new Patrol(transform,moveSpeed,patrolRange, Body, col, obstacleLayerMask);

        Node[] sequenceNodes = {  patrol, checkPlayerInRange, moveTowardsPlayer, attackPlayer };
        
        Node behaviorTreeRoot = new Sequence(sequenceNodes);
        
        behaviorTree.SetRoot(behaviorTreeRoot);
    }

    public void  Movement(){
       
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
       
       

        
       
    }
     private void OnDestroy()
    {
        StopAllCoroutines();
        cancellationTokenSource?.Cancel();
    }

    

  
}