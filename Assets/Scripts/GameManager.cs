

using System.Linq;
using Cinemachine;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [SerializeField] public Games games;
    public GameData gameData;
    public CineMachineScript virtualCamera;
    public WalkerGenerator mapGenerator;
    public PlayerController player;

    public DayTimeController dayTimeController;
    public AutoSave autoSave;
    public bool mapGenerated = false;


    private void Awake()
    {
    
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
    

        Instance = this;
        
        
        DontDestroyOnLoad(gameObject);
    }

    
    public ItemContainer inventoryContainer;
    public Menu menu;


    public void SetAutoSave(GameObject go){
        Instance.autoSave = go.GetComponent<AutoSave>();
    }
    public AutoSave GetAutoSave(){
        return Instance.autoSave ;
    }
    public void SetDayTime(GameObject go){
        Instance.dayTimeController = go.GetComponent<DayTimeController>();
    }
    public DayTimeController GetDayTime(){
        return Instance.dayTimeController ;
    }
    public void SetMenu(GameObject go){
        Instance.menu = go.GetComponent<Menu>();
    }
    public Menu GetMenu(){
        return Instance.menu ;
    }
    public void SetPlayer(GameObject go){
        Instance.player = go.GetComponent<PlayerController>();
    }
    public PlayerController GetPlayer(){
        return Instance.player ;
    }
    public Games GetGames(){
        return Instance.games ;
    }
     public void SetMapGen(GameObject go){
        Instance.mapGenerator = go.GetComponent<WalkerGenerator>();
    }
    public WalkerGenerator GetMapGenerator(){
        return Instance.mapGenerator ;
    }

    public void SetGameData(GameData game){
        Instance.gameData = game;
    }
    public GameData GetGameData(){
        return Instance.gameData;
    }

    public void SetVirtualCamera(GameObject go){
        Instance.virtualCamera = go.GetComponent<CineMachineScript>();
    }
    public CineMachineScript GetVirtualCamera(){
        return Instance.virtualCamera;
    }


    

}