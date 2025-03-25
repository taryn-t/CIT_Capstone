using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using UnityEngine.U2D;
using Vector2 = UnityEngine.Vector2;
public enum Grid{
        FLOOR,
        WALL,
        STRUCTURE,
        WATER,
        EMPTY
    }

public class WalkerGenerator : MonoBehaviour
{

    public List<Walker> Walkers;
    
    [SerializeField] public GameObject GridGameObject;
    [SerializeField] public Tilemap tilemap;
    [SerializeField] public Tilemap decorTileMap;
    [SerializeField] public Tilemap resourceTileMap;
    [SerializeField] public Tilemap leafyTilemap;
    [SerializeField] public Tilemap hillTileMap;
    [SerializeField] public Tilemap hillFloorTileMap;
    [SerializeField] public TileBase Floor;
    [SerializeField] public TileBase DecorFoliage;
    [SerializeField] public TileBase Water;
    [SerializeField] public TileBase Hill;
    [SerializeField] public TileBase HillFloor;
    [SerializeField] public TileBase Leafy;
    [SerializeField] public TileBase StructureFloor;
    [SerializeField] PolygonCollider2D CameraConfiner;
    [SerializeField] public Sprite InnerHillSprite;
    public List<Vector3Int> availablePositions = new List<Vector3Int>();
    public bool regenerating = false;


    public int MapWidth = 32;
    public int MapHeight = 32;
    public int MaximumWalkers = 10;
    private int TileCount = default;
    private int TreeCount = default;
    private int RockCount = default;
    private int HillCount = default;
    public float ResourceCount = default;
    public float HillPercent = 0.2f;
    public float ResourcePercent = 0.2f;
    
    public float RockPercent = 0.2f;
     public float TreePercent = 0.8f;
    public float DecorPercentage = 0.4f;
    public float WaitTime = 0.05f;
    public float ChanceToChange = 0.5f;
    public int FillRadius = 4;
    public int ResourceRadius = 4;
    public float LeafyChance = 0.997f;
    public float FloorNoise = 0.3f;
    public int maxEnemyStructures = 4;
    public int maxMushrooms = 10;
    public int maxFairyJars = 3;

    private int xOrg;
    private int yOrg;
    private int Seed;
    private Map map;
    private Chunk currentChunk;
    [SerializeField] Games games;
    [SerializeField] ResourceTiles resourceTiles;
    [SerializeField] GameObject playerPrefab;
    [SerializeField] GameObject mapPrefab;
    [SerializeField] GameObject fairyJar;
    [SerializeField] GameObject healingMushroom;
    private List<GameObject> healingMushrooms = new List<GameObject>();
    private GameData game;
    private string key;

    public int FloorScale = 10;
    public int HillScale = 5;
    public int DecorScale = 10000;
    public int LeafyScale = 1000;
    private int enemyStructuresGenerated = default;
    public int StructureDistance = 25;
   
    public Grid[,] gridHandler;
    
    void Start(){
        // Seed = (int)UnityEngine.Random.value;
        // key = Seed.ToString();
        // map = maps.maps.FirstOrDefault(m=>m.key == key);

        // if(map!=null)
        // {
        //     xOrg =games.data.Last().map.MapWidth/ 2;
        //     yOrg =games.data.Last().map.MapHeight / 2;
        //     Vector3Int TileCenter = new Vector3Int(xOrg, yOrg, 0);

        //     games.data.Last().map.LoadMap(tilemap, decorTileMap, resourceTileMap, hillTilemap, TileCenter,Wall);
        // }

        // else
        // {
        //      InitializeGrid();
        // }
        
        GameManager.Instance.SetMapGen(this.gameObject);
        GameManager.Instance.baseTilemap = tilemap;
    }


    public void LoadMap(string saveGame){
        GameManager.Instance.changeCursor.Loading();
        GameManager.Instance.menu.ChangePanel(4);
        key = saveGame;
        game = games.data.FirstOrDefault(m=>m.name == saveGame);
        
        
         if(game!=null)
        {
            map = game.map;
            currentChunk = map.Chunks.Last();
           
            
            xOrg = currentChunk.ChunkWidth/ 2;
            yOrg = currentChunk.ChunkHeight / 2;

            Vector3Int TileCenter = new Vector3Int(xOrg, yOrg, 0);

            game.map.LoadMap(tilemap, decorTileMap, resourceTileMap, leafyTilemap, hillTileMap, TileCenter,Water);

            foreach(EnemyStructure structure in currentChunk.enemyStructures){
                
                Instantiate(GameManager.Instance.EnemyStructures[structure.index], structure.pos, UnityEngine.Quaternion.identity, GridGameObject.transform );
                enemyStructuresGenerated++;
            }
            Instantiate(GameManager.Instance.HUDPrefab);
            GameManager.Instance.mapGenerated = true;
            
            
        }
        

        GameManager.Instance.SetGameData(game);
        
        GameObject player = Instantiate(playerPrefab, (UnityEngine.Vector3)game.playerData.lastPosition , UnityEngine.Quaternion.identity);

        GameManager.Instance.GetVirtualCamera().SetFollow(player.transform);

        GameManager.Instance.mapGenerated =true;
        GameManager.Instance.menu.Close();
        GameManager.Instance.dayTimeController.time = game.gameTime;
        GameManager.Instance.dayTimeController.days = game.daysPlayed;
        GameManager.Instance.changeCursor.Default();

    }



   public void StartGeneration(string seed, string gameName)
   {
       
        GameManager.Instance.changeCursor.Default();
        key = gameName;
        
        if(seed== ""){
              Seed = (int)UnityEngine.Random.value;
        }else{

            char[] chars = seed.ToCharArray();
            int seedInt = 0;
            foreach(char character in chars){
                seedInt += character -'0';
            }

            Seed = seedInt;
        }

        
        InitializeGrid();
        
   }

    public void RegenerateMap(string seed, string gameName)
   {

        GameManager.Instance.regenerating = true;
         GameManager.Instance.mapPanel.GetComponent<MapPanel>().generatingLabel.SetActive(true);
         
         
         GameManager.Instance.mapPanel.GetComponent<MapPanel>().mapContainer.GetComponent<RectTransform>().localPosition = new UnityEngine.Vector3(150,0,0);
        GameObject[] structures = GameObject.FindGameObjectsWithTag("Structure");
        

        foreach(GameObject structure in structures){
            Destroy(structure);
            enemyStructuresGenerated--;
        }
        ClearMushrooms();
        System.Random rnd = new System.Random();
        int randInt =rnd.Next();
        key = gameName + randInt.ToString();
        

        char[] chars = seed.ToCharArray();
        int seedInt = 0;
        foreach(char character in chars){
            seedInt += character -'0';
        }

        Seed = seedInt + randInt;

        hillTileMap.ClearAllTiles();
        decorTileMap.ClearAllTiles();
        leafyTilemap.ClearAllTiles();
        resourceTileMap.ClearAllTiles();
        hillFloorTileMap.ClearAllTiles();
        
        InitializeGrid();

   }

    void OnEnable()
    {
        gridHandler = new Grid[MapWidth, MapHeight];
        for (int x = 0; x <gridHandler.GetLength(0); x++)
        {
            for (int y = 0; y <gridHandler.GetLength(1); y++)
            {
                gridHandler[x, y] = Grid.EMPTY;
            }
        }
    }


    void InitializeGrid()
    {
        Debug.Log("Initializing grid");
        gridHandler = new Grid[MapWidth, MapHeight];

        
        Vector3Int c = new Vector3Int(MapWidth/2, MapHeight/2,0);

       

        game = new GameData(key, map, DateTime.Now, c, GameManager.Instance.KnownSpells);

        games.data.Add(game);
        

        for (int x = 0; x <gridHandler.GetLength(0); x++)
        {
            for (int y = 0; y <gridHandler.GetLength(1); y++)
            {
                gridHandler[x, y] = Grid.EMPTY;
            }
        }

        Walkers = new List<Walker>();
        xOrg =gridHandler.GetLength(0) / 2;
        yOrg =gridHandler.GetLength(1) / 2;

        Vector3Int TileCenter = new Vector3Int(xOrg, yOrg, 0);

        Walker curWalker = new Walker(new UnityEngine.Vector2(TileCenter.x, TileCenter.y), GetDirection(), ChanceToChange);
        gridHandler[TileCenter.x, TileCenter.y] = Grid.FLOOR;

        tilemap.origin = Vector3Int.zero;
        tilemap.size = new Vector3Int(64, 64, 0);
        tilemap.ResizeBounds();
        tilemap.BoxFill(TileCenter, Floor, 0, 0 , MapWidth, MapHeight);
        tilemap.SetTile(TileCenter, Floor);
        Walkers.Add(curWalker);

        TileCount++;

        StartCoroutine(CreateFloors());
    }
    UnityEngine.Vector2 GetDirection()
    {
        System.Random rnd = new System.Random();
        int choice = Mathf.FloorToInt(rnd.Next() * 3.99f);

        switch (choice)
        {
            case 0:
                return UnityEngine.Vector2.down;
            case 1:
                return UnityEngine.Vector2.left;
            case 2:
                return UnityEngine.Vector2.up;
            case 3:
                return UnityEngine.Vector2.right;
            default:
                return UnityEngine.Vector2.zero;
        }
    }

    public bool CheckSurroundingCells(int x, int y, string resourceType, int distance = 2){

        Vector3Int curPos = new Vector3Int(x,y);

         if(resourceTileMap.HasTile(curPos)){
            return false;
         }
         int xClamp = Mathf.Clamp(x, 0,gridHandler.GetLength(0)-1);
         int yClamp =Mathf.Clamp(y, 0,gridHandler.GetLength(1)-1);
         if(gridHandler[xClamp, yClamp] != Grid.FLOOR ){
            return false;
         }

    

        for (int searchX = x - distance; searchX <= x + distance; searchX++)
            {
                for (int searchY = y - distance; searchY <= y + distance; searchY++)
                {
                     xClamp = Mathf.Clamp(searchX, 0,gridHandler.GetLength(0)-1);
                     yClamp =Mathf.Clamp(searchY, 0,gridHandler.GetLength(1)-1);

                    Vector3Int pos = new Vector3Int(xClamp,yClamp);

                    
                    if(resourceTileMap.HasTile(pos) ){
                        GameObject resourceObj = resourceTileMap.GetInstantiatedObject(pos);

                        return false;                  
                      
                        
                    }
                }
            }
            return true;
    }

  public bool CheckCellsForStructure(int x, int y, int distance =25 ){
       
         int xClamp = Mathf.Clamp(x, 0,gridHandler.GetLength(0)-1);
         int yClamp =Mathf.Clamp(y, 0,gridHandler.GetLength(1)-1);

         if(gridHandler[xClamp, yClamp] != Grid.FLOOR  ){
            return false;
         }

    

        for (int searchX = x - distance; searchX <= x + distance; searchX++)
            {
                for (int searchY = y - distance; searchY <= y + distance; searchY++)
                {
                     xClamp = Mathf.Clamp(searchX, 0,gridHandler.GetLength(0)-1);
                     yClamp =Mathf.Clamp(searchY, 0,gridHandler.GetLength(1)-1);

                    
                    if(gridHandler[xClamp, yClamp] == Grid.STRUCTURE ){
                        return false;                     
                        
                    }
                }
            }
            return true;
    }

 public void FillSurroundingCells(int x, int y, int distance)
    {        
        
       
            for (int searchX = x - distance; searchX <= x + distance; searchX++)
            {
                for (int searchY = y - distance; searchY <= y + distance; searchY++)
                {
                    int xClamp = Mathf.Clamp(searchX, 0, gridHandler.GetLength(0)-1);
                    int yClamp =Mathf.Clamp(searchY, 0, gridHandler.GetLength(1)-1);
                    
                    float noise = CalcNoise(xClamp,yClamp, FloorScale);

                        
                        Vector3Int pos = new Vector3Int(xClamp,yClamp);
                        if(noise < FloorNoise){

                            if(gridHandler[pos.x, pos.y] == Grid.EMPTY && gridHandler[pos.x, pos.y] != Grid.FLOOR ){
                                gridHandler[pos.x, pos.y] = Grid.FLOOR;
                                TileCount++;
                            }
                            

                            try{
                                AddDecor(pos);
                                AddResources(pos);
                                AddLeafy(pos);
                                AddHills(pos); 
                            }
                            catch (System.Exception ex)
                            {
                                Debug.LogError($"Coroutine Error: {ex.Message}");
                            }
                            
                            
                             
                // // StartCoroutine(DrawFilledCircle(curPos.x,curPos.y, FillRadius));
                 
                        }
                        else{
                             tilemap.SetTile(pos, Water);
                
                            gridHandler[pos.x, pos.y] = Grid.WATER;

                
                        }
                        
                }
            }
      
            
        
         
        
    
    }
      void SetSurroundingCellStructure(int x,int xMax, int y,int yMax, Tilemap structureTilemap)
    {   
            
        
            for (int searchX = x ; searchX <= xMax ; searchX++)
            {
                for (int searchY = y ; searchY <= yMax ; searchY++)
                {
                    int xClamp = Mathf.Clamp(searchX, 0, gridHandler.GetLength(0)-1);
                    int yClamp =Mathf.Clamp(searchY, 0, gridHandler.GetLength(1)-1);
                     Vector3Int pos = new Vector3Int(searchX,searchY);
                    
                    
                    UnityEngine.Vector3 posV3 = structureTilemap.CellToWorld(pos);
                    
                    pos = UnityEngine.Vector3Int.FloorToInt(posV3);
                   
                    
                    hillTileMap.SetTile(pos,null);
                    hillFloorTileMap.SetTile(pos,null);
                    resourceTileMap.SetTile(pos,null);
                    if(availablePositions.Contains(pos)){
                        availablePositions.Remove(pos);
                    }
                    gridHandler[xClamp, yClamp] = Grid.STRUCTURE;

                    if(tilemap.GetTile(pos) == Water){
                        tilemap.SetTile(pos, StructureFloor);
                    }

                    
                }
            }
        
         
       
        
    
    }

  
    // public IEnumerator SetFloorTile(int x, int y, float noise, Chunk chunk){
    //     int xClamp = Mathf.Clamp(x, 0,chunk.gridHandler.GetLength(0)-1);
    //     int yClamp =Mathf.Clamp(y, 0,chunk.gridHandler.GetLength(1)-1);

    //     if(chunk.gridHandler[xClamp, yClamp] == Grid.EMPTY && chunk.gridHandler[xClamp, yClamp] != Grid.FLOOR ){

    //        Vector3Int curPos = new Vector3Int(xClamp,yClamp,0);
            
    //        chunk.gridHandler[curPos.x, curPos.y] = Grid.FLOOR;

    //        TileCount++;
            

            
    //     }

    //     yield return null;
        
    // }

    
    public IEnumerator CreateFloors( )
    {
       
         for (int x = 0; x <gridHandler.GetLength(0) - 1; x+=FillRadius)
        {
            for (int y = 0; y <gridHandler.GetLength(1) - 1; y+=FillRadius)
            {
                Vector3Int curPos = new Vector3Int(x,y,0);
                
                   
                      
                    Debug.Log("creating floors");
                    
                    FillSurroundingCells(curPos.x,curPos.y, FillRadius);
                    
                    yield return new WaitForSeconds(WaitTime);
                    Debug.Log("floor created");
                    //   yield return new WaitForSeconds(WaitTime);
                
            }
        }
        
        tilemap.ResizeBounds();

        
        yield return StartCoroutine(DecorateHills());
        yield return StartCoroutine(CreateStructures());

        InitGame();
        if(GameManager.Instance.regenerating){
            SaveMap();
        }else{
             GameManager.Instance.mapPanel.GetComponent<MapPanel>().startButton.SetActive(true);
        }
        
    }

     public IEnumerator DecorateHills( )
    {
        Debug.Log("Creating Hills");
        for (int x = 0; x <gridHandler.GetLength(0) - 1; x++)
        {
            for (int y = 0; y <gridHandler.GetLength(1) - 1; y++)
            {
                Vector3Int curPos = new Vector3Int(x,y,0);

                if(hillTileMap.HasTile(curPos)){
                 

                   Sprite tileSprite = hillTileMap.GetSprite(curPos);

                  if(tileSprite != null){
                     if(tileSprite.name == InnerHillSprite.name){
                        hillFloorTileMap.SetTile(curPos,HillFloor);
                      
                    }
                  }
                   
                  

                }
            }
        }
       yield return null;
    }

    public IEnumerator CreateStructures( )
    {
        Vector3Int c = new Vector3Int(MapWidth/2, MapHeight/2,0);
        Debug.Log("Creating Structures");

        foreach (Vector3Int position in tilemap.cellBounds.allPositionsWithin)
        {
            Debug.Log("Generating structures");
            if (Floor == tilemap.GetTile(position))
            {
                float xPow = Mathf.Pow(c.x-position.x,2f);
                float yPow = Mathf.Pow(c.y-position.y,2f);
                float distanceFromCenter = Mathf.Sqrt(xPow+yPow);

                if(distanceFromCenter > StructureDistance){

                    if(position.x+StructureDistance < MapWidth && position.x-StructureDistance >0 && position.y+StructureDistance<MapHeight && position.y-StructureDistance>0 ){
                        
                        bool hillExists = hillTileMap.HasTile(position) || hillFloorTileMap.HasTile(position);
                        bool waterExists = tilemap.GetTile(position) == Water;
                        if(availablePositions.Count>0)
                        {
                            bool tooClose = false;
                            foreach(Vector3Int pos in availablePositions)
                            {
                                xPow = Mathf.Pow(pos.x-position.x,2f);
                                yPow = Mathf.Pow(pos.y-position.y,2f);
                                distanceFromCenter = Mathf.Sqrt(xPow+yPow);
                                tooClose = distanceFromCenter < StructureDistance && tooClose == false;
                            }
                            if(!tooClose)
                            {
                                

                                if(!hillExists && !waterExists){
                                    availablePositions.Add(position); 
                                }
                            }
                        }
                        else
                        {
                           
                                if(!hillExists && !waterExists){
                                    availablePositions.Add(position); 
                                }
                            
                        }

                    }  
                }                 
            }               
        }

        while (enemyStructuresGenerated  < maxEnemyStructures){

           
            int randomIndex = UnityEngine.Random.Range(0, availablePositions.Count);
            var pos = availablePositions[randomIndex];
            bool hasCreatedStructure = AddEnemyStructure(pos);
            if(hasCreatedStructure){
                availablePositions.Remove(pos);
            }
            yield return new WaitForSeconds(0.5f);
        }

        yield return new WaitForSeconds(0.5f);

    }

    public void AddFairyJars(){
        for(int i = 0; i<maxFairyJars; i++){
            int randomIndex = UnityEngine.Random.Range(0, availablePositions.Count);

            Instantiate(fairyJar,availablePositions[randomIndex], UnityEngine.Quaternion.identity, GameManager.Instance.MapContainer.transform);
            availablePositions.RemoveAt(randomIndex);
        }
    }
    public void ClearMushrooms(){
        foreach(GameObject mushroom in healingMushrooms){
            Destroy(mushroom);
        }
    }
    public void InitGame(){
        if(!GameManager.Instance.regenerating){
            map = null;
            GameManager.Instance.waveMaxEnemies = enemyStructuresGenerated * 4;
            GameManager.Instance.SetGameData(game);

            int randomIndex = UnityEngine.Random.Range(0, availablePositions.Count);

            GameObject player = Instantiate(playerPrefab,availablePositions[randomIndex], UnityEngine.Quaternion.identity, GameManager.Instance.MapContainer.transform);
            availablePositions.RemoveAt(randomIndex);

            AddFairyJars();

            AddHealingMushrooms();
            
            GameManager.Instance.GetVirtualCamera().SetFollow(player.transform);
        }
        else{
            int randomIndex = UnityEngine.Random.Range(0, availablePositions.Count);
            AddFairyJars();
            AddHealingMushrooms();
            Vector3Int playerPos = Vector3Int.FloorToInt(GameManager.Instance.player.transform.position);

            bool inHill = hillTileMap.HasTile(playerPos) || hillFloorTileMap.HasTile(playerPos);
            bool inWater = tilemap.GetTile(playerPos) == Water;

            if(inHill || inWater){
                GameManager.Instance.player.transform.position = availablePositions[randomIndex];
            }

            GameManager.Instance.regenerating = false;
        }
    }
     
     public void SaveMap( )
    {
        
        if(!GameManager.Instance.regenerating){
            

            // Instantiate(mapPrefab);
            Instantiate(GameManager.Instance.HUDPrefab);
            GameManager.Instance.menu.Close();
            GameManager.Instance.mapGenerated =true;
            GameManager.Instance.changeCursor.Default();
            tilemap.CompressBounds();
                
            WorldBounds worldBounds = new WorldBounds(tilemap);
            GameManager.Instance.worldBounds = worldBounds; 
            
            
        }

        GameManager.Instance.mapPanel.GetComponent<MapPanel>().generatingLabel.SetActive(false);
        GameManager.Instance.mapPanel.GetComponent<MapPanel>().mapContainer.GetComponent<RectTransform>().localPosition = new UnityEngine.Vector3(0,0,0);
    }

    public void AddHealingMushrooms(){
        
        for(int i = 0; i<maxMushrooms; i++){
            int randomIndex = UnityEngine.Random.Range(0, availablePositions.Count);
            GameObject mushroom = Instantiate(healingMushroom,availablePositions[randomIndex],UnityEngine.Quaternion.identity, GameManager.Instance.MapContainer.transform);
            availablePositions.RemoveAt(randomIndex);
            healingMushrooms.Add(mushroom);
        }
    }
    

    public void AddDecor( Vector3Int curPos){
        // if((float)DecorCount / (float)games.data.Last().map.gridHandler.Length < DecorPercentage){
            int xClamp = Mathf.Clamp(curPos.x, 0,gridHandler.GetLength(0)-1);
            int yClamp =Mathf.Clamp(curPos.y, 0,gridHandler.GetLength(1)-1);

            if(gridHandler[xClamp, yClamp] == Grid.FLOOR ){
                 float noise = CalcNoise(curPos.x,curPos.y, DecorScale);
                Vector3Int v = new Vector3Int(xClamp,yClamp,0);

                if( noise < DecorPercentage ){
                    decorTileMap.SetTile(v, DecorFoliage);

                    

                }
            }
               
         
       
       
    }

    
    public void AddHills( Vector3Int curPos){
        // if((float)DecorCount / (float)games.data.Last().map.gridHandler.Length < DecorPercentage){
              int xClamp = Mathf.Clamp(curPos.x, 0,gridHandler.GetLength(0)-1);
            int yClamp =Mathf.Clamp(curPos.y, 0,gridHandler.GetLength(1)-1);

            if(gridHandler[xClamp, yClamp] == Grid.FLOOR ){

                float noise = CalcNoise(curPos.x,curPos.y, HillScale);

                Vector3Int v = new Vector3Int(xClamp,yClamp,0);

                if( noise < HillPercent ){

                    hillTileMap.RefreshTile(v);
                    hillTileMap.SetTile(v, Hill);


                }
            }
               
         
       
    }
       public void AddLeafy( Vector3Int curPos){
        // if((float)DecorCount / (float)games.data.Last().map.gridHandler.Length < DecorPercentage){
            int xClamp = Mathf.Clamp(curPos.x, 0,gridHandler.GetLength(0)-1);
            int yClamp =Mathf.Clamp(curPos.y, 0,gridHandler.GetLength(1)-1);

            if(gridHandler[xClamp, yClamp] == Grid.FLOOR ){
                

              

                 float noise = CalcNoise(curPos.x,curPos.y, LeafyScale);

                Vector3Int v = new Vector3Int(xClamp,yClamp,0);

    
                 if(noise < LeafyChance){
                    leafyTilemap.SetTile(v, Leafy);

             
                }
            }
               
         
       
    }



    public void AddResources(Vector3Int curPos){
        bool canCreateTree = (float)TreeCount / TileCount < TreePercent;
        bool canCreateRock = (float)RockCount / TileCount < RockPercent;
          float noise = CalcNoise(curPos.x,curPos.y, 10000);
         if( noise < ResourcePercent){

            foreach (ResourceTile resource in resourceTiles.data){

                if(!canCreateRock && resource.resourceType == ResourceNodeType.Rock){
                        continue;
                }
                if(!canCreateTree && resource.resourceType == ResourceNodeType.Tree){
                        continue;
                }
                    string type = resource.resourceType.ToString();

                    if(CheckSurroundingCells(curPos.x,curPos.y, type, ResourceRadius)){
                        
                        resourceTileMap.SetTile(curPos,resource.tile);

                        if(resource.resourceType == ResourceNodeType.Tree){
                            TreeCount++;
                        }
                        if(resource.resourceType == ResourceNodeType.Rock){
                            RockCount++;
                        }
                        
                    }
  
                }

            }
 

    }

    public float CalcNoise(int x, int y, int scale, int offset = 1000)
    {

        System.Random prng = new System.Random(Seed);
        float seedOffsetX = prng.Next(-100000, 100000);
        float seedOffsetY = prng.Next(-100000, 100000);
       
        float xCoord =  (x * scale / (float) MapWidth) + 0.01f;
        float yCoord =  (y * scale / (float) MapHeight) + 0.01f;

        float noise = Mathf.PerlinNoise(xCoord + offset + seedOffsetX, yCoord + offset + seedOffsetY);

        return noise;
       
       
    } 


    
    public bool AddEnemyStructure(Vector3Int pos){
        int rand = UnityEngine.Random.Range(0, GameManager.Instance.EnemyStructures.Length);

        GameObject structure = GameManager.Instance.EnemyStructures[rand];
        Tilemap structureTilemap = structure.GetComponent<Tilemap>();
        BoundsInt chunkBounds = tilemap.cellBounds;
        

        
        if(!CheckCellsForStructure(pos.x,pos.y, StructureDistance)){
            return false;
        }
        

        GameObject structureGO = Instantiate(structure, pos, UnityEngine.Quaternion.identity, GridGameObject.transform );
        structureTilemap = structureGO.GetComponent<Tilemap>();
        structureTilemap.ResizeBounds();
        structureTilemap.CompressBounds();
        GridLayout gridLayout = GridGameObject.GetComponent<GridLayout>();
        SetSurroundingCellStructure(structureTilemap.cellBounds.xMin, structureTilemap.cellBounds.xMax, structureTilemap.cellBounds.yMin, structureTilemap.cellBounds.yMax,structureTilemap);
        gridHandler[pos.x,pos.y] = Grid.STRUCTURE;


        enemyStructuresGenerated++;

        return true;
    }

   

}

[Serializable]
public class Map 
{
    public string key;
    
    public List<Chunk> Chunks;


    public Map(string key){
        
      this.key = key;
       
       Chunks = new List<Chunk>();
    }
    
    public void LoadMap(Tilemap baseTilemap, Tilemap decorTilemap, Tilemap resourceTilemap, Tilemap leafyTilemap,Tilemap hillTilemap, Vector3Int TileCenter, TileBase Water){
        foreach(Chunk chunk in Chunks){
            chunk.LoadChunk(baseTilemap,decorTilemap,resourceTilemap,leafyTilemap,hillTilemap,TileCenter,Water);
        }
    }

    public Chunk GetChunk(Vector3Int pos){

        foreach(Chunk chunk in Chunks){
                if(chunk.IsPositionInChunk(pos)){
                    return chunk;
                }
             }

        return null;
        
    }

  

 
}

[Serializable]
public class Chunk{

    public int ChunkWidth;
    public int ChunkHeight;
    public Vector3Int ChunkCenter;
    public List<MapTile> basetiles;
    public List<MapTile> decorTiles;
    public List<MapTile> resourceTiles;
    public List<MapTile> leafyTiles;
    public List<MapTile> hillTiles;
    // public Grid[,] gridHandler;

    public Vector3Int[] PolygonPoints;
    public List<EnemyStructure> enemyStructures;

    public Chunk(Grid[,] gridHandler,  Vector3Int chunkCenter, int ChunkHeight, int ChunkWidth ){
        this.ChunkCenter = chunkCenter;
        this.ChunkHeight = ChunkHeight;
        this.ChunkWidth = ChunkWidth;

        basetiles = new List<MapTile>();
        hillTiles = new List<MapTile>();
        decorTiles = new List<MapTile>();
        leafyTiles = new List<MapTile>();
        resourceTiles = new List<MapTile>();
        enemyStructures = new List<EnemyStructure>();

        

        Vector3Int p1 = new Vector3Int(ChunkCenter.x + (ChunkWidth/2), ChunkCenter.y + (ChunkHeight/2) , 0);
        Vector3Int p2 = new Vector3Int(ChunkCenter.x - (ChunkWidth/2), ChunkCenter.y + (ChunkHeight/2) , 0);
        Vector3Int p3 = new Vector3Int(ChunkCenter.x + (ChunkWidth/2), ChunkCenter.y - (ChunkHeight/2) , 0);
        Vector3Int p4 = new Vector3Int(ChunkCenter.x - (ChunkWidth/2), ChunkCenter.y - (ChunkHeight/2) , 0);
        Vector3Int[] points = {p1,p2,p3,p4};
        PolygonPoints = points;
    }


      public void LoadChunk(Tilemap baseTilemap, Tilemap decorTilemap, Tilemap resourceTilemap, Tilemap leafyTilemap, Tilemap hillTilemap,Vector3Int TileCenter, TileBase Water ){
        
        baseTilemap.BoxFill(TileCenter, Water, 0, 0 , ChunkWidth, ChunkHeight);

        
        foreach(MapTile mapTile in basetiles){
            baseTilemap.SetTile(mapTile.pos, mapTile.tile);
        }
    

    
        foreach(MapTile mapTile in decorTiles){
            decorTilemap.SetTile(mapTile.pos, mapTile.tile);
        }

    
    
        foreach(MapTile mapTile in resourceTiles){
            resourceTilemap.SetTile(mapTile.pos, mapTile.tile);
        }

    
    
        foreach(MapTile mapTile in leafyTiles){
            leafyTilemap.SetTile(mapTile.pos, mapTile.tile);
        }
       
        
         foreach(MapTile mapTile in hillTiles){
            hillTilemap.SetTile(mapTile.pos, mapTile.tile);
        }
       
        
         
      }


       public bool IsPositionInChunk( Vector3Int pos)
    {
        bool result = false;
        int j = PolygonPoints.Length - 1;
        for (int i = 0; i < PolygonPoints.Length; i++)
        {
            if (PolygonPoints[i].y < pos.y && PolygonPoints[j].y >= pos.y || 
                PolygonPoints[j].y < pos.y && PolygonPoints[i].y >= pos.y)
            {
                if (PolygonPoints[i].x + (pos.y - PolygonPoints[i].y) /
                   (PolygonPoints[j].y - PolygonPoints[i].y) *
                   (PolygonPoints[j].x - PolygonPoints[i].x) < pos.x)
                {
                    result = !result;
                }
            }
            j = i;
        }
        return result;
    }


  
}


[Serializable]
public class MapTile{
    public Vector3Int pos;
    public TileBase tile;

    public MapTile(Vector3Int pos,TileBase tile){
        this.pos = pos;
        this.tile = tile;
    }
}

[Serializable]
public class EnemyStructure{
    public Vector3Int pos;
    public BoundsInt bounds;
    public int index;

    public EnemyStructure(Vector3Int pos,int index, BoundsInt bounds){
        this.pos = pos;
        this.index = index;
        this.bounds = bounds;
    }
}