
using System.Collections.Generic;
using TMPro;
using UnityEngine;

using Image = UnityEngine.UI.Image;
public class PotionButton : MonoBehaviour
{
    [SerializeField] Image icon;
    [SerializeField] public Potion potion;
    [SerializeField] TMP_Text countLabel;
    [SerializeField] Color activeColor;
    [SerializeField] Color inactiveColor;

    public int count = 0;


    void Start(){
        
        GameManager.Instance.potionButtonGO = gameObject;
        countLabel.text = count.ToString();
        
        transform.GetChild(0).GetComponent<Image>().sprite = potion.icon;
        transform.GetChild(0).GetComponent<Image>().color = inactiveColor;

    }


    void Update(){
        // if(potion!=null ){
        //     if( Input.GetKeyDown(KeyCode.Q) ){
        //         if(potion.Name == "Healing"){
        //             UsePotion();
        //         }
        //         else if(!GameManager.Instance.GetPlayer().potionActive){
        //              UsePotion();
        //         }
                
        //     }
            
        // }
    }

    public void Set(Potion slot){
        
        count++;

        if(transform.GetChild(0).GetComponent<Image>().color == inactiveColor){
            transform.GetChild(0).GetComponent<Image>().color = activeColor;
        }

        countLabel.text = count.ToString();
        
    }

    public void Clean(){
        count--;
        if(count == 0){
            transform.GetChild(0).GetComponent<Image>().color = inactiveColor;
        }

        countLabel.text = count.ToString();

        
    }

    public void UsePotion(){
        if(count > 0){
            potion.potionEffect.OnApply(potion);
            
        }
        

    }



}