

using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class HUDController : MonoBehaviour{

    [SerializeField] TMP_Text waveLabel;
    [SerializeField] TMP_Text timeLabel;
    public int seconds = default;
    public int wave = default;
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

        seconds=time/wave;
        timeLabel.text = seconds.ToString();
    }
    public void ResetTime(){
        seconds = 0;
    }



}