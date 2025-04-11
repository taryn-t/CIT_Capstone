


using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Character:MonoBehaviour
{
    [SerializeField] GameObject burnParticles;
    [SerializeField] GameObject poisonParticles;
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
    public float speed = 2f;
    public bool slow = false;



    private Rigidbody2D body;
    public Rigidbody2D Body{
        get{return body;}
        set{body=value;}
    }

    public int level = 1;
    public float healthLevelBoost = 0f;
    public float manaLevelBoost = 0f;
    public bool frozen = false;

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
        int randDamage = (int) Random.Range(damage, damage+damageShift);

        randDamage = (int) Mathf.Clamp(randDamage,1f,100f);
        Health -= randDamage;
        GameManager.Instance.onScreenMessageSystem.PostMessage(transform.position, randDamage.ToString(), direction);
        GetComponent<Rigidbody2D>().AddForce( knockback * direction);

        if(TryGetComponent<Enemy>(out var enemy))
        {
            GameManager.Instance.playerDamageDone += randDamage;
            GameManager.Instance.hudController.currentWave.damageDone+= randDamage;

            if(effect != SpellEffect.None){
            GameManager.Instance.hudController.waveSpells[effect.ToString()].damageDone+= randDamage;
    
            }

            
        }else{
            GameManager.Instance.playerDamageTaken += randDamage;
            
            GameManager.Instance.hudController.currentWave.damageReceived+= randDamage;

            if(effect != SpellEffect.None){
                GameManager.Instance.hudController.waveSpells[effect.ToString()].damageReceived+= randDamage;

            }


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
        
        int burnDamage =3;
        float damageShift = burnDamage*takeDamagePercent;
        int randDamage = (int) Random.Range(burnDamage, burnDamage+damageShift);

        randDamage = (int) Mathf.Clamp(randDamage,1f,5f);
        burnParticles.SetActive(true);
        for(int i = 0; i<duration; i++){

            TakeDamage(randDamage,0,lastMotionVector);
            yield return new WaitForSeconds(1f);

        }
        burnParticles.SetActive(false);
    }

    IEnumerator Slow(){
        float duration = 5;
        float slowMultiplier = 0.01f;
        float prevSpeedMS = moveSpeed;
        float prevSpeed = speed;

        moveSpeed*=slowMultiplier;
        speed*=slowMultiplier;
        poisonParticles.SetActive(true);

        slow = true;

        yield return new WaitForSeconds(duration);

        slow = false;

        moveSpeed=prevSpeedMS;
        speed=prevSpeed;
        poisonParticles.SetActive(false);
    }
    void OnDestroy()
    {
        StopAllCoroutines();
    }

    public void Freeze(){
        Body.velocity=Vector2.zero;
        frozen = true;
    }

    public void UnFreeze(){
        frozen = false;
    }



}


