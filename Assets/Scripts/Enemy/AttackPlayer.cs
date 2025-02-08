using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class AttackPlayer : Node
{
    private Rigidbody2D enemyBody;
    private Rigidbody2D playerBody;
    private float attackRange;

    private Spell spell;
    OffsetRotation offsetRotation;

    CapsuleCollider2D collider;
    GameObject spellPrefab;
    Vector2 lastMotionVector;
    bool damaged;
    bool attack;

   
    
    public AttackPlayer(Rigidbody2D enemy, Rigidbody2D player, float range, Animator animator, Spell spell, GameObject spellPrefab, Vector2 lastMotionVector, CapsuleCollider2D collider, bool damaged)
    {
        enemyBody = enemy;
        playerBody = player;
        attackRange = range;
        this.animator = animator;
        this.spell = spell;
        this.spellPrefab = spellPrefab;
        this.lastMotionVector = lastMotionVector;
        this.collider = collider;
        offsetRotation = new OffsetRotation();
        this.damaged = damaged;

       

    }

    public override NodeStatus Execute( )
    {
  
        
        float distance = Vector2.Distance(enemyBody.position, playerBody.position);
        if (distance <= attackRange && !attack)
        {

            if(!animator.GetBool("attack")){
                animator.SetBool("attack",true);
            }
            
            // Implement attack logic here
            GameManager.Instance.CastSpellEnemy(spellPrefab,spell,offsetRotation,enemyBody,lastMotionVector,collider,playerBody);
            attack=true;
            
            AttackCooldown();
            
            
            return NodeStatus.Running;
        }

         return NodeStatus.Failure;
    }

    async void AttackCooldown(){
        animator.SetBool("attack",false);
        await Task.Delay(1000);
        
        attack=false;
        await Task.Yield();
    }

}