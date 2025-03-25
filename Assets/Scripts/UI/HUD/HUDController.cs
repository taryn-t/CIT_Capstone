

using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour{

    [SerializeField] TMP_Text waveLabel;
    [SerializeField] TMP_Text timeLabel;
    [SerializeField] TMP_Text fairyJarsLabel;
    [SerializeField] public GameObject popupMessage;
    [SerializeField] private Transform popupParent;
    [SerializeField] public SpellButton multiButton;
    private GameObject tmpGO = null;
    private bool popupShowing = false;
    public int seconds = 60;
    public int wave = default;
    public int waveTime = 60;
    public bool counting = false;
    private WalkerGenerator MapGen;
    public bool nextWave = false;
    public int fairyJarsGathered = 0;
    
    public void Start(){
        GameManager.Instance.hudController = this;

        waveLabel.text = wave.ToString();
        timeLabel.text = GameManager.Instance.waveMaxEnemies.ToString();
        fairyJarsLabel.text = fairyJarsGathered.ToString();
        GameManager.Instance.mapPanel.GetComponent<MapPanel>().open = false;
        GameManager.Instance.mapPanel.transform.GetChild(0).gameObject.GetComponent<Image>().color = new Color(0,0,0,0);
        GameManager.Instance.mapPanel.SetActive(false);


        
    }

    public void NextWave(){
        wave++;
        if(GameManager.Instance.procederalWaves && wave != 1){
            RegenerateMap();
        }
        GameManager.Instance.waveMaxEnemies = GameManager.Instance.EnemyStructures.Length * GameManager.Instance.hudController.wave * 3;
        
        
        waveLabel.text = wave.ToString();
        GameManager.Instance.wavesSurvived++;
        nextWave = false;
        GameManager.Instance.GetPlayer().LevelUp();
    }

     public void UpdateTime(int time){

        // seconds=time/wave;

    
        // timeLabel.text = seconds.ToString();
    }
    // public void ResetTime(){
    //     seconds = 0;
    // }

    public void UpdateFairyJar(){
        fairyJarsGathered++;
        fairyJarsLabel.text = fairyJarsGathered.ToString();    
        IEnumerator popupCoroutine = ShowPopupMessage("Fairy jar found");

        
        StartCoroutine(popupCoroutine);
        
        
    }

    public IEnumerator ShowPopupMessage(string message){
        popupShowing = true;

        tmpGO = Instantiate(popupMessage,popupParent);

        PopupMessage popup = tmpGO.GetComponent<PopupMessage>();
        popup.SetMessage(message);
        popup.ShowMessage();
        yield return new WaitForSeconds(popup.fadeDuration + popup.showDuration + popup.fadeDuration);

        popupShowing = false;
        yield return null;
    }

    void Update()
    {
        timeLabel.text = GameManager.Instance.totalEnemies.ToString();
        if(!GameManager.Instance.regenerating ){
            if(!counting){
                StartCoroutine(TrackTime());
            }

            bool noMoreEnemies = GameManager.Instance.waveMaxEnemies != 0 && GameManager.Instance.totalEnemies == 0;
            bool allFairyJarsFound = fairyJarsGathered ==3;

            if(noMoreEnemies && allFairyJarsFound ){
                nextWave = true;
                NextWave();
                fairyJarsGathered = 0;
            }
            
        }

        if(Input.GetKeyDown(KeyCode.Tab)){

            if(GameManager.Instance.mapPanel.GetComponent<MapPanel>().open == true){
                GameManager.Instance.mapPanel.GetComponent<MapPanel>().open = false;
                GameManager.Instance.mapPanel.SetActive(false);

            }
            else if(GameManager.Instance.mapPanel.GetComponent<MapPanel>().open == false){
                GameManager.Instance.mapPanel.GetComponent<MapPanel>().open = true;
                GameManager.Instance.mapPanel.SetActive(true);

            }
               
                
        }
       
    }


    public IEnumerator TrackTime(){
        counting = true;
       while(!nextWave){
            GameManager.Instance.totalTime++;
            yield return new WaitForSeconds(1f);
       } 
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