


using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Character:MonoBehaviour
{
    public float Health
    {
        get { return _health; }
        set { _health = Mathf.Clamp(value, 0, maxHealth); }
    }
    private float _health = 100;
     public float Mana
    {
        get { return _mana; }
        set { _mana = Mathf.Clamp(value, 0, maxMana); }
    }
    private float _mana = 50;
    public float maxHealth = 100;
    public Animator animator;
    public Light2D damageGlow;

    public Sprite mapMarker;
    public string mapIndex;

    public bool dead = false;
    public MapPanel mapPanel;
    public bool manaRegenerating = false;
    public int manaRegenAmount = 5;
    public float maxMana = 50;

    public float takeDamagePercent = 0.15f;
    public Vector2 lastMotionVector;
    public float moveSpeed = 20f;

    private Rigidbody2D body;
    public Rigidbody2D Body{
        get{return body;}
        set{body=value;}
    }

    public int level = 1;
    public float healthLevelBoost = 0f;
    public float manaLevelBoost = 0f;

    public void LevelUp(){
        healthLevelBoost+=0.1f;

        maxHealth += maxHealth*healthLevelBoost;
        Health = maxHealth;


        manaLevelBoost+=0.1f;

        maxMana += maxMana*manaLevelBoost;
        Mana = maxMana;
    }

    public virtual void TakeDamage(float damage, float knockback, Vector2 direction, SpellEffect effect=SpellEffect.None){
        

        float damageShift = damage*takeDamagePercent;
        int randDamage = (int) Random.Range(damage-damageShift, damage+damageShift);
        Health -= randDamage;
        GameManager.Instance.onScreenMessageSystem.PostMessage(transform.position, randDamage.ToString(), direction);
        GetComponent<Rigidbody2D>().AddForce(2f * knockback * direction);

        if(TryGetComponent<Enemy>(out var enemy))
        {
            GameManager.Instance.playerDamageDone += randDamage;
        }else{
            GameManager.Instance.playerDamageTaken += randDamage;
        }

        


        StartCoroutine(DamageAnimation());

        DoSpellEffect(effect);
    }

    public IEnumerator DamageAnimation(){
        
        if(!animator.GetBool("damage")){
                animator.SetBool("damage",true);
                damageGlow.intensity = 1f;
        }
        yield return new WaitForSeconds(1f);
            
                
        animator.SetBool("damage",false); 
        damageGlow.intensity = 0;
        
        
    }

    public IEnumerator ManaRegen(){
        manaRegenerating = true;

       

        yield return new WaitForSeconds(2f);
         Mana += manaRegenAmount;

        manaRegenerating = false;

    }

    public void DoSpellEffect(SpellEffect effect){
        
        switch(effect) 
        {
        case SpellEffect.Explode:
            StartCoroutine(Burn());
            
            break;

        case SpellEffect.Poison:
            StartCoroutine(Slow());
            break;
        case SpellEffect.Gravity:
            
            break;    

        default:
           break;
        }

    }

    IEnumerator Burn(){
        int duration = 5;
        int burnDamage = 1;
        
        for(int i = 0; i<duration; i++){
            TakeDamage(burnDamage,0,lastMotionVector);

            yield return new WaitForSeconds(1f);
        }
    }

    IEnumerator Slow(){
        float duration = 5;
        float slowMultiplier = 0.5f;
        float prevSpeed = moveSpeed;

        moveSpeed*=slowMultiplier;

        yield return new WaitForSeconds(duration);

        moveSpeed=prevSpeed;

    }
    void OnDestroy()
    {
        StopAllCoroutines();
    }



}


