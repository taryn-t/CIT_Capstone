

using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class HUDController : MonoBehaviour{

    [SerializeField] TMP_Text waveLabel;
    [SerializeField] TMP_Text timeLabel;
    public int seconds = 60;
    public int wave = default;
    public int waveTime = 60;
    public bool counting = false;
    public void Start(){
        GameManager.Instance.hudController = this;

        waveLabel.text = wave.ToString();
        timeLabel.text = seconds.ToString();
    }

    public void NextWave(){
        wave++;
        waveLabel.text = (wave-1).ToString();
    }

     public void UpdateTime(int time){

        // seconds=time/wave;

    
        // timeLabel.text = seconds.ToString();
    }
    public void ResetTime(){
        seconds = 0;
    }
    void Update()
    {
        if(!counting){
             StartCoroutine(Countdown());
        }
       
    }


    public IEnumerator Countdown(){
        counting = true;
       while(seconds > 0){
            seconds--;
            timeLabel.text = seconds.ToString();
            yield return new WaitForSeconds(1f);
       }
       seconds = 60;
       counting = false;
        
    }



}