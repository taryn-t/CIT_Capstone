

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cinemachine;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [SerializeField] public Games games;
    [SerializeField] public GameObject[] EnemyStructures;
    [SerializeField] public SpellContainer KnownSpells;
    [SerializeField] public PotionContainer availablePotions;
    [SerializeField] public GameObject HUDPrefab;
    public HUDController hudController;
    public GameData gameData;
    public CineMachineScript virtualCamera;
    public WalkerGenerator mapGenerator;
    public GameObject player;
    public PlayerMovement playerMovement;

    public DayTimeController dayTimeController;
    public AutoSave autoSave;
    public SpellButton SelectedSpell;
    public PotionButton potionButton;
    public GameObject potionButtonGO;
    public Tilemap baseTilemap;
    public HeartsContainer heartsContainer;

    public CursorController changeCursor;
    public bool mapGenerated = false;
     public bool saving = false;
     public int totalEnemies = default;
     public WorldBounds worldBounds;
    

    private void Awake()
    {
    
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
    

        Instance = this;
        
        SelectedSpell = new SpellButton();
        
        DontDestroyOnLoad(gameObject);
    }

    
    public ItemContainer inventoryContainer;
    public Menu menu;

    public void SetSpell(GameObject go){
        Instance.SelectedSpell = go.GetComponent<SpellButton>();
    }
    public Spell GetSpell(){
        return SelectedSpell.spell;
    }
    public void SetPotion(PotionButton potion){
        Instance.potionButton = potion;
    }
    public PotionButton GetPotion(){
        return potionButton;
    }
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
        Instance.player = go;
    }
    public PlayerController GetPlayer(){
        return Instance.player.GetComponent<PlayerController>();
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

    public void CastSpellEnemy(GameObject spellPrefab, Spell spell, OffsetRotation offsetRotation, Rigidbody2D enemyBody, Vector2 lastMotionVector,CapsuleCollider2D collider, Rigidbody2D target){
        
        
       
        
        spellPrefab.GetComponent<CastedSpell>().effect =  spell.spellEffect;
        spellPrefab.GetComponent<CastedSpell>().damage =  spell.damage;
        spellPrefab.GetComponent<CastedSpell>().knockback =  spell.knockback;
         spellPrefab.GetComponent<CastedSpell>().caster =  "Enemy";
         spellPrefab.GetComponent<CastedSpell>().target = target;

        GetRotation(lastMotionVector, offsetRotation, collider);
        spellPrefab.GetComponent<CastedSpell>().rotation =  offsetRotation.rotation;

        Vector3 pos = new(enemyBody.position.x,enemyBody.position.y,0);
        
        Instantiate(spellPrefab, pos + offsetRotation.offset, offsetRotation.rotation);
        
        
    }
    
      public void GetRotation(Vector2 pos, OffsetRotation offsetRotation,CapsuleCollider2D collider){

        
        string direction = "";

        if(pos == Vector2.left){
            direction= "left";
        }
        else if(pos == Vector2.right){
            direction= "right";
        }
        else if(pos == Vector2.down){
            direction= "down";
        }
        else if(pos == Vector2.up){
            direction= "up";
        }

         switch(direction) 
        {
        case "left":
            offsetRotation.rotation = Quaternion.Euler(180, 0, 180 );
            offsetRotation.offset = new Vector3(-collider.bounds.size.x*1.5f,0,0);
            break;
        case "right":
             offsetRotation.rotation =  Quaternion.Euler(0, 0, 0 );
             offsetRotation.offset = new Vector3(collider.bounds.size.x*1.5f,0,0);
             break;
        case "down":
             offsetRotation.rotation =  Quaternion.Euler(0, 0, -90 );
             offsetRotation.offset = new Vector3(collider.bounds.size.x-(collider.bounds.size.x/2),-collider.bounds.size.y,0);
             break;
        case "up":
             offsetRotation.rotation =  Quaternion.Euler(0, 0, 90 );
             offsetRotation.offset = new Vector3(collider.bounds.size.x-(collider.bounds.size.x/2),collider.bounds.size.y*1.5f,0);
             break;
        default:
            offsetRotation.rotation =  Quaternion.Euler(0, 0, 0 );
            break;
        }


    }

}