using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public int Health
    {
        get { return _health; }
        set { _health = Mathf.Clamp(value, 0, 100); }
    }
    private int _health = 100;
     public int Mana
    {
        get { return _mana; }
        set { _mana = Mathf.Clamp(value, 0, 50); }
    }

    private int _mana = 50;

    public bool visible = true;
    public int maxHealth;

    public int maxMana;

    public bool manaRegenerating = false;
    public int manaRegenAmount = 2;


    void Awake()
    {
        GameManager.Instance.player = this.gameObject;
    }
    void Start()
    {
        maxHealth = Health;
        maxMana = Mana;
        GetComponent<InventoryController>().Init();
        Health = GameManager.Instance.gameData.playerData.Health;
        Mana = GameManager.Instance.gameData.playerData.Mana;

    }

    
    void Update(){
        if(Health==0){
            // DestroyPlayer();
        }

        if(!manaRegenerating && Mana < maxMana){
            StartCoroutine(ManaRegen());
        }
    }

    void DestroyPlayer(){
        Destroy(this);
    }
    public void TakeDamage(int damage, float knockback, Vector2 direction, SpellEffect spellEffect=SpellEffect.None){
        Health -= damage;
        DamageEffect(spellEffect);
        
        GetComponent<Rigidbody2D>().AddForce(direction*knockback);
    }

    public void DamageEffect(SpellEffect spellEffect){
        switch(spellEffect) 
        {
        case SpellEffect.Burn:
            Invoke("Burn",5);
            break;
        case SpellEffect.Poison:
            Invoke("Poison",5);
            break;
        default:
           break;
        }
    }

    public void Burn(){
        Health -= 2;
    }
   public void Poison(){
        Health -= 2;
    }

    public IEnumerator ManaRegen(){
        manaRegenerating = true;

       

        yield return new WaitForSeconds(1f);
         Mana += manaRegenAmount;

        manaRegenerating = false;

    }

   
}