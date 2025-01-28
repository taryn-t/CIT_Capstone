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
    CancellationTokenSource cancellationTokenSource;
    
    public AttackPlayer(Rigidbody2D enemy, Rigidbody2D player, float range, Animator animator, Spell spell, GameObject spellPrefab, Vector2 lastMotionVector, CapsuleCollider2D collider, bool damaged, CancellationTokenSource cancellationTokenSource)
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
        this.cancellationTokenSource = cancellationTokenSource;

       

    }

    public override IEnumerator Execute(MonoBehaviour mono)
    {
  

        float distance = Vector2.Distance(enemyBody.position, playerBody.position);
        if (distance <= attackRange && !damaged && !attack)
        {
            
            // Implement attack logic here
            GameManager.Instance.CastSpellEnemy(spellPrefab,spell,offsetRotation,enemyBody,lastMotionVector,collider);
            attack=true;
            AttackWait();
            yield return NodeStatus.Success;
        }

        yield return NodeStatus.Failure;
    }
    public async void AttackWait(){
     
        await Task.Delay(1500);
        attack = true;
    
        
    }
      
 

    public async void GetRotation(Vector2 pos){

         
                await Task.Delay(100);
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

            await Task.Yield();
       
    }
}