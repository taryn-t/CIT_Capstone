using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatusUI : MonoBehaviour
{
    TMP_Text statusLabel;
    Image image;

    void Start()
    {
        
        GameManager.Instance.statusUI = this;
        image = GetComponent<Image>();
        statusLabel = gameObject.transform.GetChild(0).gameObject.GetComponent<TMP_Text>();

        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetStatus(Color color, string label){
        gameObject.SetActive(true);
        
        image.color = color;
        statusLabel.text = label;

    }
    public void CleanStatus(){

        gameObject.SetActive(false);

    }

}


