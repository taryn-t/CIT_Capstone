using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlayerController : Character
{
 

    public bool visible = true;

    public bool invincible = false;
    
    [SerializeField] public List<SpellLevel> spellLevels = new List<SpellLevel>();

    void Awake()
    {
        GameManager.Instance.player = this.gameObject;
    }
    void Start()
    {
        mapPanel = GameManager.Instance.mapPanel.GetComponent<MapPanel>();
        damageGlow= GetComponentInChildren<Light2D>();
        damageGlow.intensity = 0;
        maxHealth = Health;
        maxMana = Mana;

        if(GameManager.Instance.isDev){
            invincible = true;
        }
        
         animator = GetComponentInChildren<Animator>();
    }

    
    void Update(){
        if(Health==0){
            // DestroyPlayer();
        }


       
    }
    
    void FixedUpdate()
    {
        if(mapPanel.open){
            mapPanel.SetMarker(gameObject, mapMarker);
        }

        
        if( !manaRegenerating && Mana < maxMana){
            StartCoroutine(ManaRegen());
        }
    }

    
    void DestroyPlayer(){
        Destroy(this);
    }
    public override void TakeDamage(float damage, float knockback, Vector2 direction, SpellEffect effect){
        if(!invincible){
            base.TakeDamage(damage,knockback,direction, effect);
        }else{
            float damageShift = damage*takeDamagePercent;
            int randDamage = (int) UnityEngine.Random.Range(damage-damageShift, damage+damageShift);
            GameManager.Instance.onScreenMessageSystem.PostMessage(transform.position, randDamage.ToString(), direction);
            
        }
        
    }

    public void Burn(){
        Health -= 2;
    }
   public void Poison(){
        Health -= 2;
    }



 



    void FixStuck(){
        
    }




}

[Serializable]
public class SpellLevel
{
    public Spell spell;
    public float damageBoost = 0f;
    public int level = 1;

    public void IncreaseLevel(){
        damageBoost += 0.05f;
        level++;
    }

}
