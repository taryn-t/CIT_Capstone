using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
public enum Grid{
        FLOOR,
        WALL,
        EMPTY
    }

public class WalkerGenerator : MonoBehaviour
{

    public List<Walker> Walkers;

    [SerializeField] public Tilemap tilemap;
    [SerializeField] public Tilemap decorTileMap;
    [SerializeField] public Tilemap resourceTileMap;
    [SerializeField] public Tilemap leafyTilemap;
    [SerializeField] public TileBase Floor;
    [SerializeField] public TileBase DecorFoliage;
    [SerializeField] public TileBase Wall;
    [SerializeField] public TileBase Hill;

    [SerializeField] public TileBase Leafy;


    public int MapWidth = 32;
    public int MapHeight = 32;
    public int MaximumWalkers = 10;
    public int TileCount = default;
    public int TreeCount = default;
    public int RockCount = default;
    public int HillCount = default;
    public float ResourceCount = default;
    public float HillPercent = 0.2f;
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

    private int xOrg;
    private int yOrg;
    public int Seed;
    public Map map;
    [SerializeField] Games games;
    [SerializeField] ResourceTiles resourceTiles;
    [SerializeField] GameObject playerPrefab;
    private GameData game;
    private string key;

    public int FloorScale = 10;

    
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
    }


    public void LoadMap(string saveGame){

   
        
        GameManager.Instance.menu.ChangePanel(4);
        key = saveGame;
        game = games.data.FirstOrDefault(m=>m.name == saveGame);
        
    
         if(map!=null)
        {
            xOrg =game.map.MapWidth/ 2;
            yOrg =game.map.MapHeight / 2;
            Vector3Int TileCenter = new Vector3Int(xOrg, yOrg, 0);

            game.map.LoadMap(tilemap, decorTileMap, resourceTileMap, leafyTilemap, TileCenter,Wall);
        }

        GameManager.Instance.SetGameData(game);
        
        GameObject player = Instantiate(playerPrefab, game.playerData.lastPosition, Quaternion.identity);

        GameManager.Instance.GetVirtualCamera().SetFollow(player.transform);

        GameManager.Instance.mapGenerated =true;
        GameManager.Instance.menu.Close();
        GameManager.Instance.dayTimeController.time = game.gameTime;

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

        map = new Map(grid,MapWidth,MapHeight,key);
        Vector3 c = new Vector3Int(MapWidth/2, MapHeight/2,0);
        game = new GameData(key, map, DateTime.Now, c);

        games.data.Add(game);
        map = null;
        for (int x = 0; x <games.data.Last().map.gridHandler.GetLength(0); x++)
        {
            for (int y = 0; y <games.data.Last().map.gridHandler.GetLength(1); y++)
            {
              games.data.Last().map.gridHandler[x, y] = Grid.EMPTY;
            }
        }

        Walkers = new List<Walker>();
        xOrg =games.data.Last().map.gridHandler.GetLength(0) / 2;
        yOrg =games.data.Last().map.gridHandler.GetLength(1) / 2;

        Vector3Int TileCenter = new Vector3Int(xOrg, yOrg, 0);

        Walker curWalker = new Walker(new Vector2(TileCenter.x, TileCenter.y), GetDirection(), ChanceToChange);
        games.data.Last().map.gridHandler[TileCenter.x, TileCenter.y] = Grid.FLOOR;
        tilemap.BoxFill(TileCenter, Wall, 0, 0 , MapWidth, MapHeight);
        tilemap.SetTile(TileCenter, Floor);
        Walkers.Add(curWalker);

        TileCount++;

        StartCoroutine(CreateFloors());
    }
    Vector2 GetDirection()
    {
        int choice = Mathf.FloorToInt(UnityEngine.Random.value * 3.99f);

        switch (choice)
        {
            case 0:
                return Vector2.down;
            case 1:
                return Vector2.left;
            case 2:
                return Vector2.up;
            case 3:
                return Vector2.right;
            default:
                return Vector2.zero;
        }
    }

    public bool CheckSurroundingCells(int x, int y, string resourceType, int distance = 2){

        Vector3Int curPos = new Vector3Int(x,y);

         if(resourceTileMap.HasTile(curPos)){
            return false;
         }
         int xClamp = Mathf.Clamp(x, 0,games.data.Last().map.gridHandler.GetLength(0)-1);
         int yClamp =Mathf.Clamp(y, 0,games.data.Last().map.gridHandler.GetLength(1)-1);
         if(games.data.Last().map.gridHandler[xClamp, yClamp] != Grid.FLOOR ){
            return false;
         }

    

        for (int searchX = x - distance; searchX <= x + distance; searchX++)
            {
                for (int searchY = y - distance; searchY <= y + distance; searchY++)
                {
                     xClamp = Mathf.Clamp(searchX, 0,games.data.Last().map.gridHandler.GetLength(0)-1);
                     yClamp =Mathf.Clamp(searchY, 0,games.data.Last().map.gridHandler.GetLength(1)-1);

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

 
 async void FillSurroundingCells(int x, int y, int distance)
    {
        
            for (int searchX = x - distance; searchX <= x + distance; searchX++)
            {
                for (int searchY = y - distance; searchY <= y + distance; searchY++)
                {
                    int xClamp = Mathf.Clamp(searchX, 0,games.data.Last().map.gridHandler.GetLength(0)-1);
                    int yClamp =Mathf.Clamp(searchY, 0,games.data.Last().map.gridHandler.GetLength(1)-1);
                    
                    float noise = CalcNoise(xClamp,yClamp, FloorScale);
                    if(games.data.Last().map.gridHandler[xClamp, yClamp] != Grid.FLOOR )
                    {
                        if(noise < FloorNoise){
                            Vector3Int pos = new Vector3Int(xClamp,yClamp);
                    
                            SetFloorTile(pos.x,pos.y,noise);
                        
                            await AddDecor(pos);
                            await AddResources(pos);
                            await AddLeafy(pos);
                             
                // // StartCoroutine(DrawFilledCircle(curPos.x,curPos.y, FillRadius));
                 
                        }
                        
                    }
                }
            }
        
         
        
        
    
    }
     async Task DrawHorizontalLine( int startX, int endX, int y )
    {
        Vector3Int pos;
        for (int x = startX; x <= endX; x++)
        {
            float noise = CalcNoise(x,y,FloorScale);
            pos = new Vector3Int(x,y,0);
            if(noise < FloorNoise){
                SetFloorTile(x,y,noise);

                await AddDecor(pos);
                await AddResources(pos);
                await AddLeafy(pos);
            }
            
            
            
        }
    }

  async  void DrawFilledCircle( int xCenter, int yCenter, float radius)
    {
        int x = (int)radius;
        int y = 0;
        int radiusError = 1 - x;

        while (x >= y)
        {
            await DrawHorizontalLine( xCenter - x, xCenter + x, yCenter + y);

            await  DrawHorizontalLine( xCenter - x, xCenter + x, yCenter - y);

            await DrawHorizontalLine( xCenter - y, xCenter + y, yCenter + x);

            await DrawHorizontalLine( xCenter - y, xCenter + y, yCenter - x);

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
     
  
 
    public void SetFloorTile(int x, int y, float noise){
        int xClamp = Mathf.Clamp(x, 0,games.data.Last().map.gridHandler.GetLength(0)-1);
        int yClamp =Mathf.Clamp(y, 0,games.data.Last().map.gridHandler.GetLength(1)-1);

        if(games.data.Last().map.gridHandler[xClamp, yClamp] == Grid.EMPTY && games.data.Last().map.gridHandler[xClamp, yClamp] != Grid.FLOOR ){

            MapTile maptile;
            Vector3Int curPos = new Vector3Int(xClamp,yClamp,0);

            
                
                tilemap.SetTile(curPos, Floor);

                   
                
                
                games.data.Last().map.gridHandler[curPos.x, curPos.y] = Grid.FLOOR;

                

                maptile =  new MapTile(curPos,Floor);
                games.data.Last().map.basetiles.Add(maptile);

                TileCount++;
            

            
        }
        
    }
      public IEnumerator CreateFloors()
    {
        while ((float)TileCount / (float)games.data.Last().map.gridHandler.Length < FillPercentage)
        {
            Vector3Int curPos = new Vector3Int(0,0,0);
            float noise = 0.0f;
            bool hasCreatedFloor = false;
            foreach (Walker curWalker in Walkers)
            {
                 curPos = new Vector3Int((int)curWalker.Position.x, (int)curWalker.Position.y, 0);
                
                 noise = CalcNoise(curPos.x,curPos.y, FloorScale);
            

                if(games.data.Last().map.gridHandler[curPos.x, curPos.y] != Grid.FLOOR  )
                {
                    // DrawFilledCircle(curPos.x,curPos.y, FillRadius);
                    FillSurroundingCells(curPos.x,curPos.y, FillRadius);
                     hasCreatedFloor = true;
                     
                }
                
                              
            }
           
            //Walker Methods
            // ChanceToRemove();
            ChanceToRedirect();
            ChanceToCreate();
            UpdatePosition();

            if (hasCreatedFloor)
            {
             
                
                yield return new WaitForSeconds(WaitTime);
            }
        }
        
        SaveMap();

    }
     
     public void SaveMap()
        {
            AddColliders();
            map = null;

            GameManager.Instance.SetGameData(game);
            GameObject player = Instantiate(playerPrefab, new Vector3(MapWidth/2, MapHeight/2,0 ), Quaternion.identity);

            GameManager.Instance.GetVirtualCamera().SetFollow(player.transform);

            GameManager.Instance.menu.Close();
            GameManager.Instance.mapGenerated =true;

           
           
        }

    async Task AddDecor( Vector3Int curPos){
        // if((float)DecorCount / (float)games.data.Last().map.gridHandler.Length < DecorPercentage){
              int xClamp = Mathf.Clamp(curPos.x, 0,games.data.Last().map.gridHandler.GetLength(0)-1);
            int yClamp =Mathf.Clamp(curPos.y, 0,games.data.Last().map.gridHandler.GetLength(1)-1);

            if(games.data.Last().map.gridHandler[xClamp, yClamp] == Grid.FLOOR ){
                 float noise = CalcNoise(curPos.x,curPos.y, 10000);
                Vector3Int v = new Vector3Int(xClamp,yClamp,0);

                if( noise > DecorPercentage ){
                    decorTileMap.SetTile(v, DecorFoliage);

                    MapTile maptile =  new MapTile(v,DecorFoliage);
                    games.data.Last().map.decorTiles.Add(maptile);

                }
            }
               
         
        await Task.Yield();
       
    }
       async Task AddLeafy( Vector3Int curPos){
        // if((float)DecorCount / (float)games.data.Last().map.gridHandler.Length < DecorPercentage){
            int xClamp = Mathf.Clamp(curPos.x, 0,games.data.Last().map.gridHandler.GetLength(0)-1);
            int yClamp =Mathf.Clamp(curPos.y, 0,games.data.Last().map.gridHandler.GetLength(1)-1);

            if(games.data.Last().map.gridHandler[xClamp, yClamp] == Grid.FLOOR ){
                

              

                 float noise = CalcNoise(curPos.x,curPos.y, 1000);

                Vector3Int v = new Vector3Int(xClamp,yClamp,0);

    
                 if(noise < LeafyChance){
                    leafyTilemap.SetTile(v, Leafy);

                    MapTile maptile =  new MapTile(curPos,Leafy);
                    games.data.Last().map.leafyTiles.Add(maptile);
                }
            }
               
         
        await Task.Yield();
       
    }
    

    async Task AddResources(Vector3Int curPos){
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

                        MapTile maptile =  new MapTile(curPos,resource.tile);
                        games.data.Last().map.resourceTiles.Add(maptile);

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
                Vector2 newDirection = GetDirection();
                Vector2 newPosition = Walkers[i].Position;

                Walker newWalker = new Walker(newPosition, newDirection, 0.5f);
                Walkers.Add(newWalker);
            }
        }
    }

    void UpdatePosition()
    {
        for (int i = 0; i < Walkers.Count; i++)
        {
            Walker FoundWalker = Walkers[i];
            FoundWalker.Position += FoundWalker.Direction;
            FoundWalker.Position.x = Mathf.Clamp(FoundWalker.Position.x, 1,games.data.Last().map.gridHandler.GetLength(0) - 2);
            FoundWalker.Position.y = Mathf.Clamp(FoundWalker.Position.y, 1,games.data.Last().map.gridHandler.GetLength(1) - 2);
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
    

  
    IEnumerator CreateWalls()
    {
        for (int x = 0; x <games.data.Last().map.gridHandler.GetLength(0) - 1; x++)
        {
            for (int y = 0; y <games.data.Last().map.gridHandler.GetLength(1) - 1; y++)
            {
                int xClamp =Mathf.Clamp(x, 1,games.data.Last().map.gridHandler.GetLength(0) - 1);
                int yClamp = Mathf.Clamp(y, 1,games.data.Last().map.gridHandler.GetLength(1) - 1);

                
                if(games.data.Last().map.gridHandler[xClamp, yClamp] == Grid.FLOOR)
                {
                    
                    bool hasCreatedWall = false;

                    if(games.data.Last().map.gridHandler[xClamp + 1, yClamp] == Grid.EMPTY)
                    {
                        tilemap.SetTile(new Vector3Int(xClamp + 1, yClamp, 0), Wall);
                       games.data.Last().map.gridHandler[xClamp + 1, yClamp] = Grid.WALL;
                        hasCreatedWall = true;
                    }
                    if(games.data.Last().map.gridHandler[xClamp - 1, yClamp] == Grid.EMPTY)
                    {
                        tilemap.SetTile(new Vector3Int(xClamp - 1, yClamp, 0), Wall);
                       games.data.Last().map.gridHandler[xClamp - 1, yClamp] = Grid.WALL;
                        hasCreatedWall = true;
                    }
                    if(games.data.Last().map.gridHandler[xClamp, yClamp + 1] == Grid.EMPTY)
                    {
                        tilemap.SetTile(new Vector3Int(xClamp, yClamp + 1, 0), Wall);
                       games.data.Last().map.gridHandler[xClamp, yClamp + 1] = Grid.WALL;
                        hasCreatedWall = true;
                    }
                    if(games.data.Last().map.gridHandler[xClamp, yClamp - 1] == Grid.EMPTY)
                    {
                        tilemap.SetTile(new Vector3Int(xClamp, yClamp - 1, 0), Wall);
                       games.data.Last().map.gridHandler[xClamp, yClamp - 1] = Grid.WALL;
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

    async void AddColliders(){
        for (int x = 0; x <games.data.Last().map.gridHandler.GetLength(0); x++)
        {
            for (int y = 0; y <games.data.Last().map.gridHandler.GetLength(1); y++)
            {
                 int xClamp =Mathf.Clamp(x, 1,games.data.Last().map.gridHandler.GetLength(0));
                int yClamp = Mathf.Clamp(y, 1,games.data.Last().map.gridHandler.GetLength(1));

                if(games.data.Last().map.gridHandler[xClamp, yClamp] == Grid.EMPTY){
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
    

}

[Serializable]
public class Map 
{
    public string key;
    public Grid[,] gridHandler;
    public int MapWidth;
    public int MapHeight;
    public List<MapTile> basetiles;
    public List<MapTile> decorTiles;
    public List<MapTile> resourceTiles;
    public List<MapTile> leafyTiles;

    public Map(Grid[,] gridHandler, int MapWidth, int MapHeight, string key){
        this.gridHandler = gridHandler;
        this.MapHeight = MapHeight;
        this.MapWidth = MapWidth;
        this.key = key;
        basetiles = new List<MapTile>();
        decorTiles = new List<MapTile>();
        leafyTiles = new List<MapTile>();
        resourceTiles = new List<MapTile>();
    }
      public void LoadMap(Tilemap baseTilemap, Tilemap decorTilemap, Tilemap resourceTilemap, Tilemap leafyTilemap, Vector3Int TileCenter, TileBase Water ){
        baseTilemap.BoxFill(TileCenter, Water, 0, 0 , MapWidth, MapHeight);

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