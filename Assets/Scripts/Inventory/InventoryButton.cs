using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryButton : MonoBehaviour
{
    [SerializeField] Image icon;
    [SerializeField] TMP_Text text;

    int myIndex;

    public void SetIndex(int index){
        myIndex =index;
    }

    public void Set(ItemSlot slot){
        icon.gameObject.SetActive(true);
        icon.sprite = slot.item.Icon;

        if(slot.item.Stackable == true){
            text.gameObject.SetActive(true);
            text.text = slot.count.ToString();
        
        }
        else{
            text.gameObject.SetActive(false);
        }
    }

    public void Clean(){
        icon.sprite = null;
        icon.gameObject.SetActive(false);
        text.gameObject.SetActive(false);
    }

}