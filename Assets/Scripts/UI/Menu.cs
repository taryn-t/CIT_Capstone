using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{

    [SerializeField] public List<GameObject> MenuPanels;
    
    public int currentMenuIndex = 0;
    
    private void Start(){
        GameManager.Instance.SetMenu(this.gameObject);
        Show();

    }

    public void Show()
    {
         gameObject.SetActive(true);
        
        foreach(GameObject go in MenuPanels){
           GameObject inst = Instantiate(go,transform);
           inst.gameObject.SetActive(false);
        }

        transform.GetChild(currentMenuIndex).gameObject.SetActive(true);
         
    }

    public void ChangePanel(int index){
        
        transform.GetChild(index).gameObject.SetActive(true);

        foreach(Transform child in transform.GetChild(index)){
            child.gameObject.SetActive(true);
        }

        transform.GetChild(index).gameObject.GetComponent<MenuPanel>().SetPrevIndex(currentMenuIndex);


        transform.GetChild(currentMenuIndex).gameObject.SetActive(false);
         
    }

    public void GoBack(){
        int prevIndex = MenuPanels[currentMenuIndex].GetComponent<MenuPanel>().prevIndex;

        ChangePanel(prevIndex);
    }

    public void Close(){
       foreach(Transform child in transform){
            child.gameObject.SetActive(false);
            Destroy(child.gameObject);
       }
        
    }
}
