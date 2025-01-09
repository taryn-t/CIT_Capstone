




using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameButton : MonoBehaviour
{

    [SerializeField] RectTransform Name;
    [SerializeField] RectTransform Date;

    void Start(){
        Button btn = GetComponent<Button>();
        btn.onClick.AddListener(PointerClick);
    }
    public void ChangeButton(string name, string date){

        Name.GetComponent<TMP_Text>().text = name;
        Date.GetComponent<TMP_Text>().text = date;

    }

    void PointerClick(){
        GameManager.Instance.mapGenerator.LoadMap(Name.GetComponent<TMP_Text>().text);

    }



}