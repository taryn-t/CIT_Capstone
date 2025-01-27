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
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using Vector2 = UnityEngine.Vector2;
public enum Grid{
        FLOOR,
        WALL,
        STRUCTURE,
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
    [SerializeField] public TileBase Floor;
    [SerializeField] public TileBase DecorFoliage;
    [SerializeField] public TileBase Wall;
    [SerializeField] public TileBase Hill;

    [SerializeField] public TileBase Leafy;
    [SerializeField] PolygonCollider2D CameraConfiner;


    public int MapWidth = 32;
    public int MapHeight = 32;
    public int MaximumWalkers = 10;
    private int TileCount = default;
    private int TreeCount = default;
    private int RockCount = default;
    private int HillCount = default;
    public float ResourceCount = default;
    private float HillPercent = 0.2f;
    public float ResourcePercent = 0.2f;
    
    public float RockPercent = 0.2f;
     public float TreePercent = 0.8f;
    public float FillPercentage = 0.4f;
    public float DecorPercentage = 0.4f;
    public float WaitTime = 0.05f;
    public float ChanceToChange = 0.5f;
    public int FillRadius = 4;
    public int ResourceRadius = 4;
    public float LeafyChance = 0.997f;
    public float FloorNoise = 0.3f;
    public int maxEnemyStructures = 3;

    private int xOrg;
    private int yOrg;
    private int Seed;
    private Map map;
    private Chunk currentChunk;
    [SerializeField] Games games;
    [SerializeField] ResourceTiles resourceTiles;
    [SerializeField] GameObject playerPrefab;
    private GameData game;
    private string key;

    public int FloorScale = 10;
    private int enemyStructuresGenerated = default;
    public int StructureDistance = 25;

    
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

            game.map.LoadMap(tilemap, decorTileMap, resourceTileMap, leafyTilemap, TileCenter,Wall);

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

    }

   public void StartGeneration(string seed, string gameName, GameObject go)
   {
        
        GameManager.Instance.menu.ChangePanel(4);
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

    void InitializeGrid()
    {

        Grid[,] grid = new Grid[MapWidth, MapHeight];

        map = new Map(key);
        
        Vector3Int c = new Vector3Int(MapWidth/2, MapHeight/2,0);

        currentChunk = new Chunk(grid,c, MapWidth, MapHeight );

        map.Chunks.Add(currentChunk);

       

        game = new GameData(key, map, DateTime.Now, c, GameManager.Instance.KnownSpells);

        games.data.Add(game);
        map = null;
        currentChunk = null;

        for (int x = 0; x <games.data.Last().map.Chunks[0].gridHandler.GetLength(0); x++)
        {
            for (int y = 0; y <games.data.Last().map.Chunks[0].gridHandler.GetLength(1); y++)
            {
              games.data.Last().map.Chunks[0].gridHandler[x, y] = Grid.EMPTY;
            }
        }

        Walkers = new List<Walker>();
        xOrg =games.data.Last().map.Chunks[0].gridHandler.GetLength(0) / 2;
        yOrg =games.data.Last().map.Chunks[0].gridHandler.GetLength(1) / 2;

        Vector3Int TileCenter = new Vector3Int(xOrg, yOrg, 0);

        Walker curWalker = new Walker(new UnityEngine.Vector2(TileCenter.x, TileCenter.y), GetDirection(), ChanceToChange);
        games.data.Last().map.Chunks[0].gridHandler[TileCenter.x, TileCenter.y] = Grid.FLOOR;
        tilemap.BoxFill(TileCenter, Wall, 0, 0 , MapWidth, MapHeight);
        tilemap.SetTile(TileCenter, Floor);
        Walkers.Add(curWalker);

        TileCount++;

        StartCoroutine(CreateFloors(games.data.Last().map.Chunks[0]));
    }
    UnityEngine.Vector2 GetDirection()
    {
        int choice = Mathf.FloorToInt(UnityEngine.Random.value * 3.99f);

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

    public bool CheckSurroundingCells(int x, int y, string resourceType,Chunk chunk, int distance = 2){

        Vector3Int curPos = new Vector3Int(x,y);

         if(resourceTileMap.HasTile(curPos)){
            return false;
         }
         int xClamp = Mathf.Clamp(x, 0,chunk.gridHandler.GetLength(0)-1);
         int yClamp =Mathf.Clamp(y, 0,chunk.gridHandler.GetLength(1)-1);
         if(chunk.gridHandler[xClamp, yClamp] != Grid.FLOOR ){
            return false;
         }

    

        for (int searchX = x - distance; searchX <= x + distance; searchX++)
            {
                for (int searchY = y - distance; searchY <= y + distance; searchY++)
                {
                     xClamp = Mathf.Clamp(searchX, 0,chunk.gridHandler.GetLength(0)-1);
                     yClamp =Mathf.Clamp(searchY, 0,chunk.gridHandler.GetLength(1)-1);

                    Vector3Int pos = new Vector3Int(xClamp,yClamp);

                    
                    if(resourceTileMap.HasTile(pos) ){
                        GameObject resourceObj = resourceTileMap.GetInstantiatedObject(pos);

                        if(resourceObj.CompareTag(resourceType)){
                            return false;
                        }                       
                      
                        
                    }
                }
            }
            return true;
    }

  public bool CheckCellsForStructure(int x, int y,Chunk chunk, int distance = 25){
        if(chunk.enemyStructures.Count ==0){
            return true;
        }
       
         int xClamp = Mathf.Clamp(x, 0,chunk.gridHandler.GetLength(0)-1);
         int yClamp =Mathf.Clamp(y, 0,chunk.gridHandler.GetLength(1)-1);

         if(chunk.gridHandler[xClamp, yClamp] != Grid.FLOOR  ){
            return false;
         }

    

        for (int searchX = x - distance; searchX <= x + distance; searchX++)
            {
                for (int searchY = y - distance; searchY <= y + distance; searchY++)
                {
                     xClamp = Mathf.Clamp(searchX, 0,chunk.gridHandler.GetLength(0)-1);
                     yClamp =Mathf.Clamp(searchY, 0,chunk.gridHandler.GetLength(1)-1);

                    
                    if(chunk.gridHandler[xClamp, yClamp] == Grid.STRUCTURE ){
                        return false;                     
                        
                    }
                }
            }
            return true;
    }

 async void FillSurroundingCells(int x, int y, int distance, Chunk chunk)
    {
        
            for (int searchX = x - distance; searchX <= x + distance; searchX++)
            {
                for (int searchY = y - distance; searchY <= y + distance; searchY++)
                {
                    int xClamp = Mathf.Clamp(searchX, 0, chunk.gridHandler.GetLength(0)-1);
                    int yClamp =Mathf.Clamp(searchY, 0, chunk.gridHandler.GetLength(1)-1);
                    
                    float noise = CalcNoise(xClamp,yClamp, FloorScale);
                    if(chunk.gridHandler[xClamp, yClamp] != Grid.FLOOR )
                    {
                        if(noise < FloorNoise){
                            Vector3Int pos = new Vector3Int(xClamp,yClamp);
                    
                            SetFloorTile(pos.x,pos.y,noise, chunk);
                        
                            await AddDecor(pos,chunk);
                            await AddResources(pos,chunk);
                            await AddLeafy(pos,chunk);
                             
                // // StartCoroutine(DrawFilledCircle(curPos.x,curPos.y, FillRadius));
                 
                        }
                        
                    }
                }
            }
        
         
        
        
    
    }
      void SetSurroundingCellStructure(int x,int xMax, int y,int yMax, Chunk chunk)
    {
        
            for (int searchX = x ; searchX <= xMax ; searchX++)
            {
                for (int searchY = y ; searchY <= yMax ; searchY++)
                {
                    int xClamp = Mathf.Clamp(searchX, 0, chunk.gridHandler.GetLength(0)-1);
                    int yClamp =Mathf.Clamp(searchY, 0, chunk.gridHandler.GetLength(1)-1);
                    
                    chunk.gridHandler[xClamp, yClamp] = Grid.STRUCTURE;
                    
                }
            }
        
         
       
        
    
    }
     async Task DrawHorizontalLine( int startX, int endX, int y, Chunk chunk  )
    {
        Vector3Int pos;
        for (int x = startX; x <= endX; x++)
        {
            float noise = CalcNoise(x,y,FloorScale);
            pos = new Vector3Int(x,y,0);
            if(noise < FloorNoise){
                SetFloorTile(x,y,noise,chunk);

                await AddDecor(pos,chunk);
                await AddResources(pos,chunk);
                await AddLeafy(pos,chunk);
            }
        
        }
        
    }

  async  void DrawFilledCircle( int xCenter, int yCenter, float radius, Chunk chunk)
    {
        int x = (int)radius;
        int y = 0;
        int radiusError = 1 - x;

        while (x >= y)
        {
            await DrawHorizontalLine( xCenter - x, xCenter + x, yCenter + y, chunk);

            await  DrawHorizontalLine( xCenter - x, xCenter + x, yCenter - y,chunk);

            await DrawHorizontalLine( xCenter - y, xCenter + y, yCenter + x,chunk);

            await DrawHorizontalLine( xCenter - y, xCenter + y, yCenter - x,chunk);

            y++;

            if (radiusError < 0)
            {
                radiusError += 2 * y + 1;
            }
            else
            {
                x--;
                radiusError += 2 * (y - x + 1);
            }
        }
    
    }
     
  
 
    public void SetFloorTile(int x, int y, float noise, Chunk chunk){
        int xClamp = Mathf.Clamp(x, 0,chunk.gridHandler.GetLength(0)-1);
        int yClamp =Mathf.Clamp(y, 0,chunk.gridHandler.GetLength(1)-1);

        if(chunk.gridHandler[xClamp, yClamp] == Grid.EMPTY && chunk.gridHandler[xClamp, yClamp] != Grid.FLOOR ){

            MapTile maptile;
            Vector3Int curPos = new Vector3Int(xClamp,yClamp,0);

            
                
                tilemap.SetTile(curPos, Floor);
                
                chunk.gridHandler[curPos.x, curPos.y] = Grid.FLOOR;

                

                maptile =  new MapTile(curPos,Floor);
                chunk.basetiles.Add(maptile);

                TileCount++;
            

            
        }
        
    }
      public IEnumerator CreateFloors(Chunk chunk)
    {
       
        for (int x = 0; x <chunk.gridHandler.GetLength(0) - 1; x+=FillRadius)
        {
            for (int y = 0; y <chunk.gridHandler.GetLength(1) - 1; y+=FillRadius)
            {
                Vector3Int curPos = new Vector3Int(x,y,0);

                if(chunk.gridHandler[curPos.x, curPos.y] != Grid.FLOOR  )
                {
                    // DrawFilledCircle(curPos.x,curPos.y, FillRadius);
                    FillSurroundingCells(curPos.x,curPos.y, FillRadius, chunk);

                      yield return new WaitForSeconds(WaitTime);
                }
            }
        }
        // while ((float)TileCount / (float)chunk.gridHandler.Length < FillPercentage)
        // {
        //     Vector3Int curPos = new Vector3Int(0,0,0);
            //  bool hasCreatedFloor = false;
        //     foreach (Walker curWalker in Walkers)
        //     {
        //          curPos = new Vector3Int((int)curWalker.Position.x, (int)curWalker.Position.y, 0);
                
                 

                
                
                           
        //     }
           
        //     //Walker Methods
        //     // ChanceToRemove();
        //     ChanceToRedirect();
        //     ChanceToCreate();
        //     UpdatePosition(chunk);

        //     if (hasCreatedFloor)
        //     {
                
        //         yield return new WaitForSeconds(WaitTime);
        //     }
        // }
        tilemap.ResizeBounds();
         CreateStructures(chunk);

    }

    public async void CreateStructures(Chunk chunk)
    {

        List<Vector3Int> availablePositions = new List<Vector3Int>();

        foreach (Vector3Int position in tilemap.cellBounds.allPositionsWithin)
        {
            if (Floor == tilemap.GetTile(position))
            {
                float xPow = Mathf.Pow(chunk.ChunkCenter.x-position.x,2f);
                float yPow = Mathf.Pow(chunk.ChunkCenter.y-position.y,2f);
                float distanceFromCenter = Mathf.Sqrt(xPow+yPow);

                if(distanceFromCenter > StructureDistance){

                    if(position.x+StructureDistance < MapWidth && position.x-StructureDistance >0 && position.y+StructureDistance<MapHeight && position.y-StructureDistance>0 ){
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
                                availablePositions.Add(position); 
                            }
                        }
                        else
                        {
                            availablePositions.Add(position); 
                        }

                    }  
                }                 
            }               
        }

        while (enemyStructuresGenerated  < maxEnemyStructures){

           
            int randomIndex = UnityEngine.Random.Range(0, availablePositions.Count);
            bool hasCreatedStructure = AddEnemyStructure(chunk,availablePositions[randomIndex]);

             if (hasCreatedStructure)
            {
                await Task.Delay(50);
            }
        }
        


            
        // while (enemyStructuresGenerated  < maxEnemyStructures)
        // {
        //     Vector3Int curPos = new Vector3Int(0,0,0);
        //     float noise = 0.0f;
        //     bool hasCreatedStructure = false;
        //     foreach (Walker curWalker in Walkers)
        //     {
        //          curPos = new Vector3Int((int)curWalker.Position.x, (int)curWalker.Position.y, 0);

        //         noise = CalcNoise(curPos.x,curPos.y, 5);

        //         float rand = UnityEngine.Random.Range(0f, 1f);

        //         if( rand < 0.025 && chunk.gridHandler[curPos.x, curPos.y] == Grid.FLOOR)
        //         {
        //             

        //         }
                
                           
        //     }
           
        //     //Walker Methods
        //     // ChanceToRemove();
        //     ChanceToRedirect();
        //     ChanceToCreate();
        //     UpdatePosition(chunk);

        //     if (hasCreatedStructure)
        //     {
                
        //         await Task.Delay(50);
        //     }
        // }
        
        SaveMap(chunk);

    }
     
     public void SaveMap(Chunk chunk)
        {
            // AddColliders(chunk);
            map = null;

            GameManager.Instance.SetGameData(game);
            GameObject player = Instantiate(playerPrefab, new UnityEngine.Vector3(chunk.ChunkWidth/2, chunk.ChunkHeight/2,0 ), UnityEngine.Quaternion.identity);
            Vector2[] vector2s =   new Vector2[4];
            
            GameManager.Instance.GetVirtualCamera().SetFollow(player.transform);
            Instantiate(GameManager.Instance.HUDPrefab);
            GameManager.Instance.menu.Close();
            GameManager.Instance.mapGenerated =true;

            
           
        }

    async Task AddDecor( Vector3Int curPos, Chunk chunk){
        // if((float)DecorCount / (float)games.data.Last().map.gridHandler.Length < DecorPercentage){
              int xClamp = Mathf.Clamp(curPos.x, 0,chunk.gridHandler.GetLength(0)-1);
            int yClamp =Mathf.Clamp(curPos.y, 0,chunk.gridHandler.GetLength(1)-1);

            if(chunk.gridHandler[xClamp, yClamp] == Grid.FLOOR ){
                 float noise = CalcNoise(curPos.x,curPos.y, 10000);
                Vector3Int v = new Vector3Int(xClamp,yClamp,0);

                if( noise > DecorPercentage ){
                    decorTileMap.SetTile(v, DecorFoliage);

                    MapTile maptile =  new MapTile(v,DecorFoliage);
                    chunk.decorTiles.Add(maptile);

                }
            }
               
         
        await Task.Yield();
       
    }
       async Task AddLeafy( Vector3Int curPos, Chunk chunk){
        // if((float)DecorCount / (float)games.data.Last().map.gridHandler.Length < DecorPercentage){
            int xClamp = Mathf.Clamp(curPos.x, 0,chunk.gridHandler.GetLength(0)-1);
            int yClamp =Mathf.Clamp(curPos.y, 0,chunk.gridHandler.GetLength(1)-1);

            if(chunk.gridHandler[xClamp, yClamp] == Grid.FLOOR ){
                

              

                 float noise = CalcNoise(curPos.x,curPos.y, 1000);

                Vector3Int v = new Vector3Int(xClamp,yClamp,0);

    
                 if(noise < LeafyChance){
                    leafyTilemap.SetTile(v, Leafy);

                    MapTile maptile =  new MapTile(curPos,Leafy);
                    chunk.leafyTiles.Add(maptile);
                }
            }
               
         
        await Task.Yield();
       
    }
    

    async Task AddResources(Vector3Int curPos, Chunk chunk){
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

                    if(CheckSurroundingCells(curPos.x,curPos.y, type, chunk, ResourceRadius)){
                        
                        resourceTileMap.SetTile(curPos,resource.tile);

                        MapTile maptile =  new MapTile(curPos,resource.tile);
                        chunk.resourceTiles.Add(maptile);

                        if(resource.resourceType == ResourceNodeType.Tree){
                            TreeCount++;
                        }
                        if(resource.resourceType == ResourceNodeType.Rock){
                            RockCount++;
                        }
                        
                    }
  
                }

            }

      
         
        await Task.Yield();
       

    }

    public float CalcNoise(int x, int y , int scale,int offset =1000)
    {

        System.Random prng = new System.Random(Seed);
        float seedOffsetX = prng.Next(-100000, 100000);
        float seedOffsetY = prng.Next(-100000, 100000);
       
        float xCoord =  (x *  scale /(float)MapWidth )+0.01f;
        float yCoord =  (y  * scale /(float)MapHeight)+0.01f;

        float noise = Mathf.PerlinNoise(xCoord + offset + seedOffsetX, yCoord + offset + seedOffsetY);

        return noise;
       
       
    }

    void ChanceToRemove()
    {
        int updatedCount = Walkers.Count;
        for (int i = 0; i < updatedCount; i++)
        {
            if (UnityEngine.Random.value < Walkers[i].ChanceToChange && Walkers.Count > 1)
            {
                Walkers.RemoveAt(i);
                break;
            }
        }
    }

    void ChanceToRedirect()
    {
        for (int i = 0; i < Walkers.Count; i++)
        {
            if (UnityEngine.Random.value < Walkers[i].ChanceToChange)
            {
                Walker curWalker = Walkers[i];
                curWalker.Direction = GetDirection();
                Walkers[i] = curWalker;
            }
        }
    }

    void ChanceToCreate()
    {
        int updatedCount = Walkers.Count;
        for (int i = 0; i < updatedCount; i++)
        {
            if (UnityEngine.Random.value < Walkers[i].ChanceToChange && Walkers.Count < MaximumWalkers)
            {
                UnityEngine.Vector2 newDirection = GetDirection();
                UnityEngine.Vector2 newPosition = Walkers[i].Position;

                Walker newWalker = new Walker(newPosition, newDirection, 0.5f);
                Walkers.Add(newWalker);
            }
        }
    }

    void UpdatePosition(Chunk chunk)
    {
        for (int i = 0; i < Walkers.Count; i++)
        {
            Walker FoundWalker = Walkers[i];
            FoundWalker.Position += FoundWalker.Direction;
            FoundWalker.Position.x = Mathf.Clamp(FoundWalker.Position.x, 1,chunk.gridHandler.GetLength(0) - 2);
            FoundWalker.Position.y = Mathf.Clamp(FoundWalker.Position.y, 1,chunk.gridHandler.GetLength(1) - 2);
            Walkers[i] = FoundWalker;
        }
    }

    // void AddHills(Vector3Int curPos, float noise)
    // {
    //     if((float)HillCount / (float)games.data.Last().map.gridHandler.Length < HillPercent){
    //         if( noise > 0.7f  )
    //         {

    //             hillTilemap.SetTile(curPos,Hill);

    //             MapTile maptile =  new MapTile(curPos,Hill);
    //             games.data.Last().map.hillTiles.Add(maptile);
    //             FillSurroundingCells(curPos.x,curPos.y, 2, "Hill");    
    //             HillCount++;
    //         }
    //     }

        
                
    // }
    

  
    IEnumerator CreateWalls(Chunk chunk)
    {
        for (int x = 0; x <chunk.gridHandler.GetLength(0) - 1; x++)
        {
            for (int y = 0; y <chunk.gridHandler.GetLength(1) - 1; y++)
            {
                int xClamp =Mathf.Clamp(x, 1,chunk.gridHandler.GetLength(0) - 1);
                int yClamp = Mathf.Clamp(y, 1,chunk.gridHandler.GetLength(1) - 1);

                
                if(chunk.gridHandler[xClamp, yClamp] == Grid.FLOOR)
                {
                    
                    bool hasCreatedWall = false;

                    if(chunk.gridHandler[xClamp + 1, yClamp] == Grid.EMPTY)
                    {
                        tilemap.SetTile(new Vector3Int(xClamp + 1, yClamp, 0), Wall);
                       chunk.gridHandler[xClamp + 1, yClamp] = Grid.WALL;
                        hasCreatedWall = true;
                    }
                    if(chunk.gridHandler[xClamp - 1, yClamp] == Grid.EMPTY)
                    {
                        tilemap.SetTile(new Vector3Int(xClamp - 1, yClamp, 0), Wall);
                       chunk.gridHandler[xClamp - 1, yClamp] = Grid.WALL;
                        hasCreatedWall = true;
                    }
                    if(chunk.gridHandler[xClamp, yClamp + 1] == Grid.EMPTY)
                    {
                        tilemap.SetTile(new Vector3Int(xClamp, yClamp + 1, 0), Wall);
                       chunk.gridHandler[xClamp, yClamp + 1] = Grid.WALL;
                        hasCreatedWall = true;
                    }
                    if(chunk.gridHandler[xClamp, yClamp - 1] == Grid.EMPTY)
                    {
                        tilemap.SetTile(new Vector3Int(xClamp, yClamp - 1, 0), Wall);
                       chunk.gridHandler[xClamp, yClamp - 1] = Grid.WALL;
                        hasCreatedWall = true;
                    }

                    if (hasCreatedWall)
                    {
                        yield return new WaitForSeconds(WaitTime);
                    }
                }
                
                
            }
        }
    }

    async void AddColliders(Chunk chunk){
        for (int x = 0; x <chunk.gridHandler.GetLength(0); x++)
        {
            for (int y = 0; y <chunk.gridHandler.GetLength(1); y++)
            {
                 int xClamp =Mathf.Clamp(x, 1,chunk.gridHandler.GetLength(0));
                 int yClamp = Mathf.Clamp(y, 1,chunk.gridHandler.GetLength(1));

                if(chunk.gridHandler[xClamp, yClamp] == Grid.EMPTY){
                    Vector3Int pos = new Vector3Int(xClamp,yClamp,0);
                    tilemap.SetColliderType( pos, Tile.ColliderType.Grid); 
                }else{
                    Vector3Int pos = new Vector3Int(xClamp,yClamp,0);
                    tilemap.SetColliderType( pos, Tile.ColliderType.None); 
                }
               
            }
        }
         await Task.Yield();
    }

   
    
    public bool AddEnemyStructure(Chunk chunk,Vector3Int pos){
        int rand = UnityEngine.Random.Range(0, GameManager.Instance.EnemyStructures.Length);

        GameObject structure = GameManager.Instance.EnemyStructures[rand];
        Tilemap structureTilemap = structure.GetComponent<Tilemap>();
        BoundsInt chunkBounds = tilemap.cellBounds;

        for(int x =structureTilemap.cellBounds.xMin; x<structureTilemap.cellBounds.max.x; x++){
            for(int y =structureTilemap.cellBounds.yMin; y<structureTilemap.cellBounds.max.y; y++){
                Vector3Int v = new(x,y,0);
                

                if(!chunkBounds.Contains(v) ){
                    return false;
                }
            }
        }

        
            if(!CheckCellsForStructure(pos.x,pos.y,chunk, 25)){
                return false;
            }
        

        Instantiate(structure, pos, UnityEngine.Quaternion.identity, GridGameObject.transform );

        SetSurroundingCellStructure(structureTilemap.cellBounds.xMin, structureTilemap.cellBounds.xMax, structureTilemap.cellBounds.yMin, structureTilemap.cellBounds.yMax,chunk);
        chunk.gridHandler[pos.x,pos.y] = Grid.STRUCTURE;

        EnemyStructure enemyStructure = new EnemyStructure(pos,rand, structureTilemap.cellBounds);
        chunk.enemyStructures.Add(enemyStructure);

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
    
    public void LoadMap(Tilemap baseTilemap, Tilemap decorTilemap, Tilemap resourceTilemap, Tilemap leafyTilemap, Vector3Int TileCenter, TileBase Water){
        foreach(Chunk chunk in Chunks){
            chunk.LoadChunk(baseTilemap,decorTilemap,resourceTilemap,leafyTilemap,TileCenter,Water);
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
    public Grid[,] gridHandler;

    public Vector3Int[] PolygonPoints;
    public List<EnemyStructure> enemyStructures;

    public Chunk(Grid[,] gridHandler,  Vector3Int chunkCenter, int ChunkHeight, int ChunkWidth ){
        this.gridHandler = gridHandler;
        this.ChunkCenter = chunkCenter;
        this.ChunkHeight = ChunkHeight;
        this.ChunkWidth = ChunkWidth;

        basetiles = new List<MapTile>();
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


      public void LoadChunk(Tilemap baseTilemap, Tilemap decorTilemap, Tilemap resourceTilemap, Tilemap leafyTilemap, Vector3Int TileCenter, TileBase Water ){
        gridHandler = new Grid[ChunkWidth, ChunkHeight];
        
        baseTilemap.BoxFill(TileCenter, Water, 0, 0 , ChunkWidth, ChunkHeight);

        
        foreach(MapTile mapTile in basetiles){
            baseTilemap.SetTile(mapTile.pos, mapTile.tile);
            gridHandler[mapTile.pos.x, mapTile.pos.y] = Grid.FLOOR;
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
       
        
        AddColliders(baseTilemap);
        
         
      }

      async void AddColliders(Tilemap tilemap){
        for (int x = 0; x <gridHandler.GetLength(0); x++)
        {
            for (int y = 0; y <gridHandler.GetLength(1); y++)
            {
                 int xClamp =Mathf.Clamp(x, 1,gridHandler.GetLength(0));
                 int yClamp = Mathf.Clamp(y, 1,gridHandler.GetLength(1));

                if(gridHandler[xClamp, yClamp] == Grid.EMPTY){
                    Vector3Int pos = new Vector3Int(xClamp,yClamp,0);
                    tilemap.SetColliderType( pos, Tile.ColliderType.Grid); 
                }else{
                    Vector3Int pos = new Vector3Int(xClamp,yClamp,0);
                    tilemap.SetColliderType( pos, Tile.ColliderType.None); 
                }
               
            }
        }
         await Task.Yield();
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

    public bool IsPosGround(UnityEngine.Vector3 pos){

        Vector3Int intPos = Vector3Int.FloorToInt(pos);

        if(gridHandler[intPos.x,intPos.y] == Grid.FLOOR){
            return true;
        }



        return false;
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