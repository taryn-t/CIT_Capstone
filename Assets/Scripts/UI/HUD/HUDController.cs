

using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] public GameObject nextWaveGo;
    [SerializeField] public TMP_Text nextWaveText;
     [SerializeField] public TMP_Text nextWaveSpellLabel;
     [SerializeField] public GameObject nextWaveSpellGO;
    [SerializeField] public GameObject testComplete;
    public WaveData currentWave;
    bool endGame = false;

    private GameObject tmpGO = null;
    private bool popupShowing = false;
    public int wave = default;
    public int waveTime = 0;
    public bool counting = false;
    private WalkerGenerator MapGen;
    public bool nextWave = false;
    public int fairyJarsGathered = 0;
    private Color transparentColor = new Color(0,0,0,0);
    public Dictionary<string,PotionData> wavePotions = new Dictionary<string, PotionData>();
    public Dictionary<string,SpellData> waveSpells = new();
    private string test_id;
    private string seed;
    [SerializeField] public Spell[] spells; 
    
    public void Start(){
        nextWaveGo.SetActive(false);
        seed = GameManager.Instance.mapGenerator.Seed.ToString();
        GameManager.Instance.hudController = this;
        test_id = GameManager.Instance.testingManager.test_id;
        currentWave = new WaveData(test_id,wave,seed);
        currentWave.totalEnemies =  GameManager.Instance.waveMaxEnemies;
        waveLabel.text = wave.ToString();
        
        timeLabel.text = currentWave.totalEnemies.ToString();
        fairyJarsLabel.text = fairyJarsGathered.ToString();
        GameManager.Instance.mapPanel.GetComponent<MapPanel>().open = false;
        GameManager.Instance.mapPanel.SetActive(false);
        GameManager.Instance.instructionsUI.SetActive(false);
        GameManager.Instance.testingManager.AddWave(currentWave);

        CreatePotions();
         StartCoroutine(ShowNextWave());
    }
    
    public void CreatePotions(){
        PotionData potion = new PotionData(currentWave, "Healing");
        wavePotions.Add("Healing", potion);

        potion = new PotionData(currentWave, "Invisibility");
        wavePotions.Add("Invisibility", potion);

        potion = new PotionData(currentWave, "Speed");
        wavePotions.Add("Speed", potion);

         foreach(var p in wavePotions.Values)
        {
            GameManager.Instance.testingManager.AddPotion(p);
        }
    }
    public void UpdateSpells(){

        foreach(Spell spell in GameManager.Instance.SelectedSpell.allSpells){
            if(spell != null && !waveSpells.ContainsKey(spell.spellEffect.ToString())){
                Debug.Log(spell.spellEffect.ToString());
                SpellData spellData = new SpellData(currentWave, spell.spellEffect.ToString());
                waveSpells.Add(spell.spellEffect.ToString(),spellData);
            }
        }

         foreach(var spell in waveSpells.Values)
        {
            if(GameManager.Instance.testingManager != null){
                 GameManager.Instance.testingManager.AddSpell(spell);
            }
           
        }
    }
    void AddNewSpell(){
        int spellIndex = Mathf.Clamp(wave-1, 0, spells.Length-1);
        Spell spell = spells[spellIndex];
        if(GameManager.Instance.hudController.multiButton.CheckToAdd(spell)){
                    
            GameManager.Instance.hudController.multiButton.AddSpell(spell);
            GameManager.Instance.hudController.UpdateSpells();
            GameManager.Instance.hudController.waveSpells[spell.spellEffect.ToString()].pickedUp++;
        
        }
    }

    public IEnumerator ShowNextWave(){
        nextWaveGo.SetActive(true);

        if(wave <= 3 && wave > 1 && GameManager.Instance.multiSpell){
            AddNewSpell();
            nextWaveSpellGO.SetActive(true);
            
        }else{
            nextWaveSpellGO.SetActive(false);
        }
        
        
        
        nextWaveText.text = $"Wave {wave}";

        yield return new WaitForSeconds(3f);

        nextWaveGo.SetActive(false);

        if(GameManager.Instance.testingManager.firstGen){
            GameManager.Instance.instructionsUI.SetActive(true);
            GameManager.Instance.instructionsUI.GetComponent<Instructions>().open = true;
            GameManager.Instance.testingManager.firstGen = false;
        }
    
        foreach(EnemySpawner es in GameManager.Instance.activeSpawners){
            es.StartSpawn(wave);
        }

    }
    

    public void NextWave(){
        currentWave.completed = true;
        GameManager.Instance.wavesSurvived++;
        GameManager.Instance.testingManager.testingData.wavesCompleted++;
        wave++;
        

        GameManager.Instance.testingManager.AddWave(currentWave);

        foreach(var potion in wavePotions.Values)
        {
            GameManager.Instance.testingManager.AddPotion(potion);
        }

         foreach(var spell in waveSpells.Values)
        {
            GameManager.Instance.testingManager.AddSpell(spell);
        }
        
                
        fairyJarsGathered -= fairyJarsGathered;
        fairyJarsLabel.text = fairyJarsGathered.ToString();
        GameManager.Instance.GetPlayer().LevelUp();

        if(GameManager.Instance.procederalWaves && wave != 1){

            RegenerateMap();
            seed = GameManager.Instance.mapGenerator.Seed.ToString();

        }else{
            GameManager.Instance.mapGenerator.AddHealingMushrooms();
            GameManager.Instance.mapGenerator.AddFairyJars();
            nextWave = false;
            StartCoroutine(ShowNextWave());
        }
        
        if(GameManager.Instance.isDev){
            GameManager.Instance.waveMaxEnemies = 4 * 1;
        }else{
            GameManager.Instance.waveMaxEnemies = 4 * wave*2;
        }
        
         
         
        
        
       

        
        GameManager.Instance.heartsContainer.SetInterval();
        GameManager.Instance.manaContainer.SetInterval();

        

        GameManager.Instance.testingManager.AddWavePotions(wavePotions);      
        wavePotions.Clear();
        CreatePotions();

        GameManager.Instance.testingManager.AddWaveSpells(waveSpells);
        waveSpells.Clear();
        UpdateSpells();

        currentWave = new WaveData(test_id,wave,seed);
        GameManager.Instance.testingManager.AddWave(currentWave);
        GameManager.Instance.waveSpellDropped = false;
    }

    IEnumerator WaitJars(){
        yield return new WaitForSeconds(1f);
        fairyJarsGathered = 0;
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

        if(fairyJarsGathered == 3){
            EventData newEvent = new EventData(currentWave, "ALL_JARS_FOUND", "");
            GameManager.Instance.testingManager.AddEvent(newEvent);
        }

        StartCoroutine(popupCoroutine);
        
    }

    public void ShowTestCompleted(){
        IEnumerator popupCoroutine = ShowPopupMessage("Test Completed");
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

            
            GameManager.Instance.mapPanel.transform.GetChild(0).gameObject.GetComponent<Image>().color = transparentColor;
            
            
            if(!counting){
                StartCoroutine(TrackTime());
            }

            bool noMoreEnemies = GameManager.Instance.waveMaxEnemies != 0 && GameManager.Instance.totalEnemies == 0;
            bool allFairyJarsFound = fairyJarsGathered ==3;

            if(noMoreEnemies && allFairyJarsFound && !currentWave.completed ){
                
                if(wave < 3){
                    nextWave = true;
                    NextWave();
                    StartCoroutine(WaitJars());
                }
                else if(wave >=3 && !endGame){
                    currentWave.completed = true;
                    endGame = true;

                    if(GameManager.Instance.testingManager.submitToDB){
                        GameManager.Instance.testingManager.StopTest();
                        StartCoroutine(GameManager.Instance.testingManager.UploadTestData());
                    }

                    
                }  
            }
            
           
        }

        waveLabel.text = wave.ToString();

        if(Input.GetKeyDown(KeyCode.Tab)){

            if(GameManager.Instance.mapPanel.GetComponent<MapPanel>().open){
                GameManager.Instance.mapPanel.GetComponent<MapPanel>().open = false;
                GameManager.Instance.mapPanel.SetActive(false);

            }
            else if(!GameManager.Instance.mapPanel.GetComponent<MapPanel>().open){

                
                GameManager.Instance.mapPanel.GetComponent<MapPanel>().open = true;
                GameManager.Instance.mapPanel.SetActive(true);

            }
               
                
        }

        if(Input.GetKeyDown(KeyCode.I)){
            if(GameManager.Instance.instructionsUI.GetComponent<Instructions>().open){
                GameManager.Instance.instructionsUI.GetComponent<Instructions>().open = false;
                GameManager.Instance.instructionsUI.SetActive(false);
                
            }
            else{
                EventData newEvent = new EventData(currentWave, "INSTRUCTIONS_VIEWED", "");
                GameManager.Instance.testingManager.AddEvent(newEvent);
                
                GameManager.Instance.instructionsUI.SetActive(true);
                GameManager.Instance.instructionsUI.GetComponent<Instructions>().open = true;
            }
            
        }

        if(Input.GetKeyDown(KeyCode.Escape)){
            if(GameManager.Instance.pauseMenu.activeSelf){
                GameManager.Instance.pauseMenu.SetActive(false);

            }
            else{
                GameManager.Instance.pauseMenu.SetActive(true);

            }
        }

        if(GameManager.Instance.GetPlayer().Health ==0){
            StopAllCoroutines();
            
        }
       
    }


    public IEnumerator TrackTime(){
        counting = true;
        
       while(!nextWave){


            GameManager.Instance.totalTime++;
            yield return new WaitForSeconds(1f);
            if(GameManager.Instance.testingManager.testRunning){
                currentWave.timeComplete++;
            }
            
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