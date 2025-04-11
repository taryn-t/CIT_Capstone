using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using UnityEngine.UIElements;
public class PickUpScroll : MonoBehaviour
{
    // Start is called before the first frame update

    Transform player;
    [SerializeField] float speed = 0.2f;
    [SerializeField] float pickUpDistance = 0.01f;
    [SerializeField] float ttl = 90f;
    [SerializeField] public SpellSlot spell;

    public string key;
    
    void Start()
    {
        
        player = GameManager.Instance.player.transform;

       key = spell.spell.spellEffect.ToString();
    }




    // Update is called once per frame
    void Update()
    {
        ttl -= Time.deltaTime;
        if(ttl<=0){Destroy(gameObject);}
       StartCoroutine(PickUp());
    }

     IEnumerator PickUp()
    {

        float distance = Vector3.Distance(transform.position, player.position);

        if(distance > pickUpDistance){
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);

        transform.position = Vector3.MoveTowards( 
            transform.position,
            player.position,
            speed*Time.deltaTime
        );

        if(distance < 0.3f){

            if(GameManager.Instance.hudController.multiButton.multiSpell){
                GameManager.Instance.soundEffectController.PlayPositiveSound();
                if(GameManager.Instance.hudController.multiButton.CheckToAdd(spell.spell)){
                    
                    GameManager.Instance.hudController.multiButton.AddSpell(spell.spell);
                    GameManager.Instance.hudController.UpdateSpells();
                    GameManager.Instance.hudController.waveSpells[key].pickedUp++;
                    StartCoroutine(GameManager.Instance.hudController.ShowPopupMessage($"Learned spell {spell.spell.name}"));
                
                }
                else
                {
                    SpellLevel spellLevel = GameManager.Instance.GetPlayer().spellLevels.First(p => p.spell == spell.spell);
                    spellLevel.IncreaseLevel();
                    StartCoroutine(GameManager.Instance.hudController.ShowPopupMessage($"{spellLevel.spell.name} level {spellLevel.level}"));
                }
                
                Destroy(gameObject);
            }
            
            if(!GameManager.Instance.hudController.multiButton.multiSpell){
                GameManager.Instance.soundEffectController.PlayPositiveSound();
               if(GameManager.Instance.SelectedSpell.spell != spell.spell){
                    GameManager.Instance.SelectedSpell.Set(spell); 
               }
               else{
                    SpellLevel spellLevel = GameManager.Instance.GetPlayer().spellLevels.First(p => p.spell == spell.spell);
                    spellLevel.IncreaseLevel();
                    StartCoroutine(GameManager.Instance.hudController.ShowPopupMessage($"{spellLevel.spell.name} level {spellLevel.level}"));
               }
             GameManager.Instance.hudController.waveSpells[key].pickedUp++;

               Destroy(gameObject);
            }
            
       
           
        }
       
        
    }


}
