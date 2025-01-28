using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    
    private PlayerMovement playerMovement;
    
    public Spell SelectedSpell;
    [SerializeField] GameObject spell;
    OffsetRotation offsetRotation;

    CapsuleCollider2D collider;
    Rigidbody2D body;
    
    
    
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
            if(Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.Mouse1)){
               
                CastSpell();
                
            }
        }
    }

    public void CastSpell(){
        SelectedSpell = GameManager.Instance.SelectedSpell.spell;
        
        


        
        spell.GetComponent<CastedSpell>().effect =  SelectedSpell.spellEffect;
        spell.GetComponent<CastedSpell>().damage =  SelectedSpell.damage;
        spell.GetComponent<CastedSpell>().knockback =  SelectedSpell.knockback;
         spell.GetComponent<CastedSpell>().caster =  gameObject.tag;
         spell.GetComponent<CastedSpell>().spell = SelectedSpell;
        GetRotation(playerMovement.lastMotionVector);
        spell.GetComponent<CastedSpell>().rotation =  offsetRotation.rotation;
        Vector3 pos = new(body.position.x,body.position.y,0);
        
        Instantiate(spell, pos + offsetRotation.offset, offsetRotation.rotation);

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
            offsetRotation.offset = new Vector3(-collider.bounds.size.x*2,0,0);
            break;
        case "right":
             offsetRotation.rotation =  Quaternion.Euler(0, 0, 0 );
             offsetRotation.offset = new Vector3(collider.bounds.size.x*2,0,0);
             break;
        case "down":
             offsetRotation.rotation =  Quaternion.Euler(0, 0, -90 );
             offsetRotation.offset = new Vector3(collider.bounds.size.x,-collider.bounds.size.y,0);
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


public class OffsetRotation{
    public Quaternion rotation;
    public Vector3 offset;
}