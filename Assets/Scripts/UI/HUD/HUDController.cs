

using System.Collections;
using System.Threading.Tasks;
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
    private WalkerGenerator MapGen;
    public void Start(){
        GameManager.Instance.hudController = this;

        waveLabel.text = wave.ToString();
        timeLabel.text = seconds.ToString();
    }

    public void NextWave(){
        wave++;
        waveLabel.text = (wave-1).ToString();
        GameManager.Instance.wavesSurvived++;
    }

     public void UpdateTime(int time){

        // seconds=time/wave;

    
        // timeLabel.text = seconds.ToString();
    }
    // public void ResetTime(){
    //     seconds = 0;
    // }
    void Update()
    {
        if(!counting && GameManager.Instance.GetPlayer().Health >0 && !GameManager.Instance.regenerating){
            StartCoroutine(Countdown());
            
        }
       
    }


    public IEnumerator Countdown(){
        counting = true;
       while(seconds > 0){
            GameManager.Instance.totalTime++;
            seconds--;
            timeLabel.text = seconds.ToString();
            yield return new WaitForSeconds(1f);
       }

       if(GameManager.Instance.procederalWaves && wave != 1){
            RegenerateMap();
       }
        
        NextWave();
       

       seconds = 60;
       counting = false;
       
        
    }

    public  void RegenerateMap(){
        

        
        
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach(GameObject enemy in enemies){
            Destroy(enemy);
        }
        MapGen = GameManager.Instance.GetMapGenerator();
        
        MapGen.RegenerateMap(GameManager.Instance.genSeed, GameManager.Instance.gameName);

        
        

    }



}