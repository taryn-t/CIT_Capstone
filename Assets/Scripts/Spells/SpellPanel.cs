
using UnityEngine;

public class SpellPanel : MonoBehaviour
{
    private SpellContainer spells;
    [SerializeField] SpellButton button;
    [SerializeField] GameObject btnGo;

    private void Start(){
        spells = GameManager.Instance.KnownSpells;
        SetIndex();
        if(!GameManager.Instance.multiSpell){
            Show();
        }
        
     
    }

    private void SetIndex()
    {
        
            button.SetIndex(0);
        
       
    }

    private void Show()
    {
        
        
        btnGo.SetActive(true);
        
        button.Set(spells.slots[0]);
            
        
    }
}
