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
    bool slime;
    LayerMask playerLayerMask;
    bool playerInRay;
    Vector2 direction;
    public AttackPlayer(Rigidbody2D enemy, Rigidbody2D player, float range, Animator animator, Spell spell, GameObject spellPrefab, Vector2 lastMotionVector, CapsuleCollider2D collider, bool damaged, bool slime, bool playerInRay)
    {
        enemyBody = enemy;
        playerBody = player;
        attackRange = range;
        this.animator = animator;
        this.spell = spell;
        this.spellPrefab = spellPrefab;
        this.lastMotionVector = lastMotionVector;
        this.collider = collider;
        this.damaged = damaged;
        this.slime = slime;
        this.playerInRay = playerInRay;
    }

    public override NodeStatus Execute( )
    {
        
        if(enemyBody.gameObject.GetComponent<Enemy>().playerInRayAttack){
            if (!attack)
            {
                direction = (playerBody.position - enemyBody.position).normalized;
                if(!animator.GetBool("attack")){
                    animator.SetBool("attack",true);
                }
                
                CastSpell();
                
                
                return NodeStatus.Running;
            }
        }
        
         return NodeStatus.Failure;
    }

  
    private void CastSpell(){
        
        if(!slime){
            GameManager.Instance.CastSpellEnemy(spellPrefab,spell,enemyBody,direction,collider,playerBody);
            Enemy enemy = enemyBody.gameObject.GetComponent<Enemy>();
            enemy.Mana -= spell.manaCost;
        }
        else{
            
            enemyBody.AddForce( enemyBody.gameObject.GetComponent<Enemy>().speed * (Vector3)direction, ForceMode2D.Impulse);
        }
        attack=true;
        
        
        enemyBody.gameObject.GetComponent<MonoBehaviour>().StartCoroutine(AttackCooldown());

    }
    IEnumerator AttackCooldown(){
        float timeout = UnityEngine.Random.Range(0.5f,1f);
        animator.SetBool("attack",false);
        yield return new WaitForSeconds(timeout);
        
        attack=false;
    }


}