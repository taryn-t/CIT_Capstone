using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float Health
    {
        get { return _health; }
        set { _health = Mathf.Clamp(value, 0, 100); }
    }
    private float _health = 100;
     public float Mana
    {
        get { return _mana; }
        set { _mana = Mathf.Clamp(value, 0, 100); }
    }

    private float _mana = 100;
    
    void Start()
    {
        GetComponent<InventoryController>().Init();
        GameManager.Instance.SetPlayer(this.gameObject);
        Health = GameManager.Instance.gameData.playerData.Health;
        Mana = GameManager.Instance.gameData.playerData.Mana;

    }

    
    void Update(){
        if(Health==0){
            // DestroyPlayer();
        }
    }

    void DestroyPlayer(){
        Destroy(this);
    }
    public void TakeDamage(int damage, float knockback, Vector2 direction, SpellEffect spellEffect){
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

   
}