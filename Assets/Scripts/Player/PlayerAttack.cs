using System.Collections;
using System.Linq;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    
    private PlayerMovement playerMovement;
    
    public Spell SelectedSpell;
    [SerializeField] GameObject spell;
    OffsetRotation offsetRotation;

    CapsuleCollider2D collider;
    Rigidbody2D body;
    bool attack = false;
    
  
    
    
    void Start()
    {
        
        playerMovement = GetComponent<PlayerMovement>();
        offsetRotation = new OffsetRotation();
        collider = GetComponent<CapsuleCollider2D>();

        body = GetComponent<Rigidbody2D>();

        // if(GameManager.Instance.SelectedSpell == null){
        //     GameManager.Instance.SelectedSpell.spell = GameManager.Instance.gameData.playerData.KnownSpells.slots[0].spell;
        //     SelectedSpell = GameManager.Instance.SelectedSpell.spell;
        // }
        
    }

    // Update is called once per frame
    void Update()
    {
        if(GameManager.Instance.GetSpell() != null){
             if(GameManager.Instance.GetSpell() != null && !playerMovement.frozen){
            if(Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.Mouse1) && !attack){
                Vector3 targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                StartCoroutine(CastSpell( targetPosition));
                
            }
        }
        }
       
    }

    private int GetSpellDamage(){
        SpellLevel spellLevel = GameManager.Instance.GetPlayer().spellLevels.First(p => p.spell == SelectedSpell);

        return GameManager.Instance.isDev ? 25 : (int)(spellLevel.damageBoost * SelectedSpell.damage) + SelectedSpell.damage;
    }

    public IEnumerator CastSpell(Vector3 targetPosition){
        

        
        attack=true;
        if(!playerMovement.animator.GetBool("attack")){
                playerMovement.animator.SetBool("attack",true);
            }
        
        SelectedSpell = GameManager.Instance.SelectedSpell.spell;
        
        if(GameManager.Instance.GetPlayer().Mana >= SelectedSpell.manaCost || GameManager.Instance.isDev){
            CastedSpell castedSpell = spell.GetComponent<CastedSpell>();

            castedSpell.effect =  SelectedSpell.spellEffect;
            castedSpell.damage =  GetSpellDamage();
            castedSpell.knockback =  SelectedSpell.knockback;
            castedSpell.caster =  gameObject.tag;
            castedSpell.spell = SelectedSpell;
            castedSpell.targetPosition = targetPosition;
            castedSpell.range = SelectedSpell.range;
            castedSpell.casterAnimator = playerMovement.animator;

            var direction = ((Vector2)targetPosition - playerMovement.Body.position).normalized;
            GetRotation(direction);

            castedSpell.rotation =  offsetRotation.rotation;
            Vector3 pos = new(body.position.x,body.position.y,0);

            Instantiate(spell, pos + offsetRotation.offset, offsetRotation.rotation,GameManager.Instance.SpellsGO.transform);
            
            GameManager.Instance.GetPlayer().Mana -= SelectedSpell.manaCost;
        }
        
        

        yield return new WaitForSeconds(0.5f);
        
        playerMovement.animator.SetBool("attack",false);
        attack = false;
        yield return null;

    }

    public void GetRotation(Vector2 pos){

        
        string direction = "";

        if (Mathf.Abs(pos.x) > Mathf.Abs(pos.y))
        {
            if (pos.x > 0)
                direction= "right";
            else
                direction=  "left";
        }
        else
        {
            if (pos.y > 0)
                direction=  "up";
        
            else
                direction= "down";
        }
        Debug.Log(direction);

        switch(direction) 
        {
        case "left":
            offsetRotation.rotation = Quaternion.Euler(180, 0, 180 );
            offsetRotation.offset = new Vector3(-collider.bounds.size.x*1.5f,0,0);
            break;
        case "right":
             offsetRotation.rotation =  Quaternion.Euler(0, 0, 0 );
             offsetRotation.offset = new Vector3(collider.bounds.size.x*1.5f,0,0);
             break;
        case "down":
             offsetRotation.rotation =  Quaternion.Euler(0, 0, -90 );
             offsetRotation.offset = new Vector3(collider.bounds.size.x-(collider.bounds.size.x/2),-collider.bounds.size.y,0);
             break;
        case "up":
             offsetRotation.rotation =  Quaternion.Euler(0, 0, 90 );
             offsetRotation.offset = new Vector3(collider.bounds.size.x-(collider.bounds.size.x/2),collider.bounds.size.y,0);
             break;
        default:
            
            offsetRotation.offset = new Vector3(0,0,0);
            break;
        }


    }

    void OnDestroy()
    {
        StopAllCoroutines();
    }
}


public class OffsetRotation{
    public Quaternion rotation;
    public Vector3 offset;
}
