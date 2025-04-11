using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SpellButton : MonoBehaviour
{
    [SerializeField] Image icon;
    [SerializeField] public Spell spell;
    public bool multiSpell;
    [SerializeField] public GameObject[] spellButtons;
    [SerializeField] public List<Spell> spells = new(new Spell[3]);
    [SerializeField] public List<Spell> allSpells = new(new Spell[3]);
    private GameObject activeButton;


    int myIndex;

    void Start(){
       foreach(Transform transform in transform){
         icon = transform.gameObject.GetComponent<Image>();
         break;
       }

       if (GameManager.Instance.multiSpell && !multiSpell){

            gameObject.SetActive(false);
       }
       else if(!GameManager.Instance.multiSpell && multiSpell){
            gameObject.SetActive(false);
       }

       if(GameManager.Instance.multiSpell && multiSpell){
            SetUpSpells();
       }
        GameManager.Instance.SelectedSpell = this;
        GameManager.Instance.hudController.UpdateSpells();
       
    }
    

    void SetUpSpells(){
        
        
        for(int i = 0; i<spells.Count; i++){

            if(spells[i] != null){
                spellButtons[i].transform.GetChild(0).GetComponent<Image>().color = new Color(1,1,1,1);
                spellButtons[i].transform.GetChild(0).GetComponent<Image>().sprite = spells[i].Icon;
            }
            else{
                 spellButtons[i].transform.GetChild(0).GetComponent<Image>().color = new Color(1,1,1,0);
                spellButtons[i].transform.GetChild(0).GetComponent<Image>().sprite = null;
            }

            if(i==0){
                spell = spells[i];
                activeButton = spellButtons[i];
            }
            
        }
        SelectSpell();
        
    }

    public void AddSpell(Spell newSpell){
        for(int i = 0; i<spells.Count; i++){
            if(spells[i] == null){
                spells[i] = newSpell;
                break;
            }
        }
        
    SetUpSpells();
        

    }

    public bool CheckToAdd(Spell newSpell){
        return !spells.Contains(newSpell);
    }

    void ShiftSpellsRight(){
         if (spells.Count > 1)
        {
            Spell firstSpell = spells[0];
            spells.RemoveAt(0);
            spells.Add(firstSpell);
        }

        SetUpSpells();
    }
    void ShiftSpellsLeft(){
        if (spells.Count > 1)
        {
            Spell lastSpell = spells[^1]; 
            spells.RemoveAt(spells.Count - 1); 
            spells.Insert(0, lastSpell); 
        }
        SetUpSpells();
    }

    void Update(){
        if(Input.inputString == (myIndex+1).ToString()){
            SelectSpell();
        }

        if(Input.GetKeyDown(KeyCode.Alpha1)){
            ShiftSpellsLeft();
        }
        
        if(Input.GetKeyDown(KeyCode.Alpha2)){
            ShiftSpellsRight();
        }
    }

    public void SetIndex(int index){
        myIndex =index;
    }

    public void Set(SpellSlot slot){

        if(!transform.GetChild(0).gameObject.activeSelf){
            transform.GetChild(0).gameObject.SetActive(true);
        }
        
        
        transform.GetChild(0).gameObject.GetComponent<Image>().sprite = slot.spell.Icon;
        
        
        spell = slot.spell;
        SelectSpell();
    }

    public void Clean(){
        transform.GetChild(0).GetComponent<Image>().sprite = null;
        transform.GetChild(0).gameObject.SetActive(false);
        spell = null;
    }

    public void SelectSpell(){
        
        
        GameManager.Instance.SetSpell(gameObject);
            
       
    }



}