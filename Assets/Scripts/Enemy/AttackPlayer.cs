using System.Collections;

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
        
        if(enemyBody.gameObject.GetComponent<Enemy>().playerInRayAttack && GameManager.Instance.GetPlayer().visible){
            if (!attack)
            {
                direction = (playerBody.position - enemyBody.position).normalized;
                if(!animator.GetBool("attack")){
                    animator.SetBool("attack",true);
                }
                Enemy enemy = enemyBody.gameObject.GetComponent<Enemy>();
                if(enemy.Mana-spell.manaCost >= 0){
                    CastSpell();
                    return NodeStatus.Success;
                }
                else{
                    return NodeStatus.Failure;
                }
                
            }
        }
        
         return NodeStatus.Running;
    }

  
    private void CastSpell(){
        
        if(!slime){
            GameManager.Instance.CastSpellEnemy(spellPrefab,spell,enemyBody,direction,collider,playerBody);
            Enemy enemy = enemyBody.gameObject.GetComponent<Enemy>();
            enemy.Mana -= spell.manaCost;
        }
        else{
            
            enemyBody.AddForce( enemyBody.gameObject.GetComponent<Enemy>().speed*10f * (Vector3)direction, ForceMode2D.Impulse);
        }
        attack=true;
        
        
        enemyBody.gameObject.GetComponent<MonoBehaviour>().StartCoroutine(AttackCooldown());

    }
    IEnumerator AttackCooldown(){
        float timeout = UnityEngine.Random.Range(0.5f,1.5f);
        animator.SetBool("attack",false);
        yield return new WaitForSeconds(timeout);
        
        attack=false;
    }


}