using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TipsController : MonoBehaviour
{

    [SerializeField] private List<string> tips = new List<string>();
    [SerializeField] TMP_Text tipLabel;
    private bool iterating = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(!iterating){
            StartCoroutine(ShowTips());
        }
    }

    IEnumerator ShowTips(){
        iterating = true;
        foreach(string tip in tips){
            tipLabel.text = tip;
            yield return new WaitForSeconds(3f);
        }
        iterating= false;
    }
}
