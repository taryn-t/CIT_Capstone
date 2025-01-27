using UnityEngine;

public class AttackPlayer : Node
{
    private Transform enemyTransform;
    private Transform playerTransform;
    private float attackRange;

    private Spell spell;
    OffsetRotation offsetRotation;

    CapsuleCollider2D collider;
    GameObject spellPrefab;
    Vector2 lastMotionVector;

    public AttackPlayer(Transform enemy, Transform player, float range, Animator animator, Spell spell, GameObject spellPrefab, Vector2 lastMotionVector, CapsuleCollider2D collider)
    {
        enemyTransform = enemy;
        playerTransform = player;
        attackRange = range;
        this.animator = animator;
        this.spell = spell;
        this.spellPrefab = spellPrefab;
        this.lastMotionVector = lastMotionVector;
        this.collider = collider;
        offsetRotation = new OffsetRotation();

    }

    public override NodeStatus Execute()
    {
        
        float distance = Vector2.Distance(enemyTransform.position, playerTransform.position);
        if (distance <= attackRange)
        {
            Debug.Log("Attacking player");
            // Implement attack logic here
            GameManager.Instance.CastSpellEnemy(spellPrefab,spell,offsetRotation,enemyTransform,lastMotionVector,collider);
            
            return NodeStatus.Success;
        }

        return NodeStatus.Failure;
    }


    public void GetRotation(Vector2 pos){

        
        string direction = "";

        if(pos == Vector2.left){
            direction= "left";
        }
        else if(pos == Vector2.right){
            direction= "right";
        }
        else if(pos == Vector2.down){
            direction= "down";
        }
        else if(pos == Vector2.up){
            direction= "up";
        }

        switch(direction) 
        {
        case "left":
            offsetRotation.rotation = Quaternion.Euler(180, 0, 180 );
            offsetRotation.offset = new Vector3(-collider.bounds.size.x*4,0,0);
            break;
        case "right":
             offsetRotation.rotation =  Quaternion.Euler(0, 0, 0 );
             offsetRotation.offset = new Vector3(collider.bounds.size.x,0,0);
             break;
        case "down":
             offsetRotation.rotation =  Quaternion.Euler(0, 0, -90 );
             offsetRotation.offset = new Vector3(collider.bounds.size.x*2,-collider.bounds.size.y,0);
             break;
        case "up":
             offsetRotation.rotation =  Quaternion.Euler(0, 0, 90 );
             offsetRotation.offset = new Vector3(collider.bounds.size.x,collider.bounds.size.y*1.5f,0);
             break;
        default:
            offsetRotation.rotation =  Quaternion.Euler(0, 0, 0 );
            break;
        }


    }
}