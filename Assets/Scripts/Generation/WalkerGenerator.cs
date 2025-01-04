using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
    [SerializeField] public Tilemap hillTilemap;
    [SerializeField] public TileBase Floor;
    [SerializeField] public TileBase DecorFoliage;
    [SerializeField] public TileBase Wall;
    [SerializeField] public TileBase Hill;

    [SerializeField] public TileBase Leafy;


    public int MapWidth = 32;
    public int MapHeight = 32;
    public int MaximumWalkers = 10;
    public int TileCount = default;
    public int DecorCount = default;
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
    public float ResourceNoise = 0.3f;

    private int xOrg;
    private int yOrg;
    public int Seed;
    public Map map;
    [SerializeField] Maps maps;
    [SerializeField] ResourceTiles resourceTiles;
    string key;

    
    void Start(){
        Seed = (int)UnityEngine.Random.value;
        key = SceneManager.GetActiveScene().name;
        map = maps.maps.FirstOrDefault(m=>m.key == key);

        if(map!=null)
        {
            xOrg =map.MapWidth/ 2;
            yOrg =map.MapHeight / 2;
            Vector3Int TileCenter = new Vector3Int(xOrg, yOrg, 0);

            map.LoadMap(tilemap, decorTileMap, resourceTileMap, hillTilemap, TileCenter,Wall);
        }

        else
        {
             InitializeGrid();
        }

         

    }

    private void InitializeSavedMap()
    {
        throw new NotImplementedException();
    }

    void InitializeGrid()
    {

        Grid[,] grid = new Grid[MapWidth, MapHeight];

        map = new Map(grid,MapWidth,MapHeight,key);
   
        for (int x = 0; x <map.gridHandler.GetLength(0); x++)
        {
            for (int y = 0; y <map.gridHandler.GetLength(1); y++)
            {
              map.gridHandler[x, y] = Grid.EMPTY;
            }
        }

        Walkers = new List<Walker>();
        xOrg =map.gridHandler.GetLength(0) / 2;
        yOrg =map.gridHandler.GetLength(1) / 2;

        Vector3Int TileCenter = new Vector3Int(xOrg, yOrg, 0);

        Walker curWalker = new Walker(new Vector2(TileCenter.x, TileCenter.y), GetDirection(), ChanceToChange);
        map.gridHandler[TileCenter.x, TileCenter.y] = Grid.FLOOR;
        tilemap.BoxFill(TileCenter, Wall, 0, 0 , MapWidth-1, MapHeight-1);
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

        for (int searchX = x - distance; searchX <= x + distance; searchX++)
            {
                for (int searchY = y - distance; searchY <= y + distance; searchY++)
                {
                    int xClamp = Mathf.Clamp(searchX, 0,map.gridHandler.GetLength(0)-1);
                    int yClamp =Mathf.Clamp(searchY, 0,map.gridHandler.GetLength(1)-1);

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

    public void FillSurroundingCells(int x, int y, int distance, string type = "Floor")
    {
        if(type=="Floor"){
            for (int searchX = x - distance; searchX <= x + distance; searchX++)
            {
                for (int searchY = y - distance; searchY <= y + distance; searchY++)
                {
                    int xClamp = Mathf.Clamp(searchX, 0,map.gridHandler.GetLength(0)-1);
                    int yClamp =Mathf.Clamp(searchY, 0,map.gridHandler.GetLength(1)-1);
                    
                    float noise = CalcNoise(xClamp,yClamp);
                    if(map.gridHandler[xClamp, yClamp] != Grid.FLOOR )
                    {
                        Vector3Int pos = new Vector3Int(xClamp,yClamp);
                    
                        SetFloorTile(pos.x,pos.y,noise);
                        
                    }
                }
            }
        }
          else if(type == "Hill"){
            for (int searchX = x - distance; searchX <= x + distance; searchX++)
            {
                for (int searchY = y - distance; searchY <= y + distance; searchY++)
                {
                    int xClamp = Mathf.Clamp(searchX, 0,map.gridHandler.GetLength(0)-1);
                    int yClamp =Mathf.Clamp(searchY, 0,map.gridHandler.GetLength(1)-1);

                    if(map.gridHandler[xClamp, yClamp] == Grid.FLOOR )
                    {
                        Vector3Int pos = new Vector3Int(xClamp,yClamp);
                    
                        hillTilemap.SetTile(pos, Hill);
            
                        HillCount++;

                        MapTile maptile =  new MapTile(pos,Hill);
                        map.hillTiles.Add(maptile);
                    }
                }
            }
        }
        
    
    }
     private  void DrawHorizontalLine( int startX, int endX, int y )
    {
        
        for (int x = startX; x <= endX; x++)
        {
            float noise = CalcNoise(x,y);
            
            SetFloorTile(x,y,noise);
        }
    }

       public  void DrawFilledCircle( int xCenter, int yCenter, float radius)
    {
        int x = (int)radius;
        int y = 0;
        int radiusError = 1 - x;

        

        while (x >= y)
        {
            DrawHorizontalLine( xCenter - x, xCenter + x, yCenter + y);
            DrawHorizontalLine( xCenter - x, xCenter + x, yCenter - y);
            DrawHorizontalLine( xCenter - y, xCenter + y, yCenter + x);
            DrawHorizontalLine( xCenter - y, xCenter + y, yCenter - x);

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
        int xClamp = Mathf.Clamp(x, 0,map.gridHandler.GetLength(0)-1);
        int yClamp =Mathf.Clamp(y, 0,map.gridHandler.GetLength(1)-1);

        // if(map.gridHandler[xClamp, yClamp] == Grid.EMPTY && map.gridHandler[xClamp, yClamp] != Grid.FLOOR ){

            MapTile maptile;
            Vector3Int curPos = new Vector3Int(xClamp,yClamp,0);

            
                
                tilemap.SetTile(curPos, Floor);

                if(map.gridHandler[xClamp, yClamp] != Grid.FLOOR){
                    TileCount++;
                }            
                
                
                map.gridHandler[curPos.x, curPos.y] = Grid.FLOOR;

                

                maptile =  new MapTile(curPos,Floor);
                map.basetiles.Add(maptile);
            
            

             if(noise > LeafyChance){
                decorTileMap.SetTile(curPos, Leafy);
        
                DecorCount++;

                maptile =  new MapTile(curPos,Leafy);
                map.decorTiles.Add(maptile);
            }
        // }
        
    }
     
     
  

    IEnumerator CreateFloors()
    {
        while ((float)TileCount / (float)map.gridHandler.Length < FillPercentage)
        {
            Vector3Int curPos = new Vector3Int(0,0,0);
            float noise = 0.0f;
            bool hasCreatedFloor = false;
            foreach (Walker curWalker in Walkers)
            {
                 curPos = new Vector3Int((int)curWalker.Position.x, (int)curWalker.Position.y, 0);
                
                 noise = CalcNoise(curPos.x,curPos.y);
            

                if(map.gridHandler[curPos.x, curPos.y] != Grid.FLOOR  )
                {
                    
                    if(noise > FloorNoise){
                        SetFloorTile(curPos.x,curPos.y,noise);
                        DrawFilledCircle(curPos.x,curPos.y, FillRadius);
                        //    FillSurroundingCells(curPos.x,curPos.y, FillRadius);
                     
                    }
                     hasCreatedFloor = true;

                }
               
                // AddHills(curPos, noise);
               
            }
           
            AddDecor(curPos, noise);
            AddResources(curPos,noise);
            //Walker Methods
            ChanceToRemove();
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
            
            maps.maps.Add(map);
            map = null;
        }

    public void AddDecor( Vector3Int curPos, float noise){

        // if((float)DecorCount / (float)map.gridHandler.Length < DecorPercentage){

            if( noise > 0.8f){
                decorTileMap.SetTile(curPos, DecorFoliage);
        
                DecorCount++;

                MapTile maptile =  new MapTile(curPos,DecorFoliage);
                map.decorTiles.Add(maptile);
    
            }
         
        // }
       
    }

    public void AddResources(Vector3Int curPos, float noise){

        if( noise < ResourceNoise){

            foreach (ResourceTile resource in resourceTiles.data){

                    if(resource.resourceType == ResourceNodeType.Rock){

                        if((float)RockCount / TileCount >= RockPercent){
                            break;
                        }

                    }

                    if(resource.resourceType == ResourceNodeType.Tree){

                        if((float)TreeCount / TileCount >= TreePercent){
                            break;
                        }

                    }

                    string type = resource.resourceType.ToString();

                    if(CheckSurroundingCells(curPos.x,curPos.y, type, ResourceRadius)){

                        resourceTileMap.SetTile(curPos,resource.tile);

                        MapTile maptile =  new MapTile(curPos,resource.tile);
                        map.resourceTiles.Add(maptile);

                        if(resource.resourceType == ResourceNodeType.Tree){
                            TreeCount++;
                        }
                        if(resource.resourceType == ResourceNodeType.Rock){
                            RockCount++;
                        }
                        
                         ResourceCount = ResourceCount+ TreeCount+RockCount;
                    }

                   

                }
            }
        
        
        

    }

    public float CalcNoise(int x, int y ,int offset =1000, int scale = 100)
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
            FoundWalker.Position.x = Mathf.Clamp(FoundWalker.Position.x, 1,map.gridHandler.GetLength(0) - 2);
            FoundWalker.Position.y = Mathf.Clamp(FoundWalker.Position.y, 1,map.gridHandler.GetLength(1) - 2);
            Walkers[i] = FoundWalker;
        }
    }

    // void AddHills(Vector3Int curPos, float noise)
    // {
    //     if((float)HillCount / (float)map.gridHandler.Length < HillPercent){
    //         if( noise > 0.7f  )
    //         {

    //             hillTilemap.SetTile(curPos,Hill);

    //             MapTile maptile =  new MapTile(curPos,Hill);
    //             map.hillTiles.Add(maptile);
    //             FillSurroundingCells(curPos.x,curPos.y, 2, "Hill");    
    //             HillCount++;
    //         }
    //     }

        
                
    // }
    

  
    IEnumerator CreateWalls()
    {
        for (int x = 0; x <map.gridHandler.GetLength(0) - 1; x++)
        {
            for (int y = 0; y <map.gridHandler.GetLength(1) - 1; y++)
            {
                int xClamp =Mathf.Clamp(x, 1,map.gridHandler.GetLength(0) - 1);
                int yClamp = Mathf.Clamp(y, 1,map.gridHandler.GetLength(1) - 1);

                
                if(map.gridHandler[xClamp, yClamp] == Grid.FLOOR)
                {
                    
                    bool hasCreatedWall = false;

                    if(map.gridHandler[xClamp + 1, yClamp] == Grid.EMPTY)
                    {
                        tilemap.SetTile(new Vector3Int(xClamp + 1, yClamp, 0), Wall);
                       map.gridHandler[xClamp + 1, yClamp] = Grid.WALL;
                        hasCreatedWall = true;
                    }
                    if(map.gridHandler[xClamp - 1, yClamp] == Grid.EMPTY)
                    {
                        tilemap.SetTile(new Vector3Int(xClamp - 1, yClamp, 0), Wall);
                       map.gridHandler[xClamp - 1, yClamp] = Grid.WALL;
                        hasCreatedWall = true;
                    }
                    if(map.gridHandler[xClamp, yClamp + 1] == Grid.EMPTY)
                    {
                        tilemap.SetTile(new Vector3Int(xClamp, yClamp + 1, 0), Wall);
                       map.gridHandler[xClamp, yClamp + 1] = Grid.WALL;
                        hasCreatedWall = true;
                    }
                    if(map.gridHandler[xClamp, yClamp - 1] == Grid.EMPTY)
                    {
                        tilemap.SetTile(new Vector3Int(xClamp, yClamp - 1, 0), Wall);
                       map.gridHandler[xClamp, yClamp - 1] = Grid.WALL;
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
    public List<MapTile> hillTiles;

    public Map(Grid[,] gridHandler, int MapWidth, int MapHeight, string key){
        this.gridHandler = gridHandler;
        this.MapHeight = MapHeight;
        this.MapWidth = MapWidth;
        this.key = key;
        basetiles = new List<MapTile>();
        decorTiles = new List<MapTile>();
        hillTiles = new List<MapTile>();
        resourceTiles = new List<MapTile>();
    }
      public void LoadMap(Tilemap baseTilemap, Tilemap decorTilemap, Tilemap resourceTilemap, Tilemap hillTilemap, Vector3Int TileCenter, TileBase Water ){
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
         foreach(MapTile mapTile in hillTiles){
            hillTilemap.SetTile(mapTile.pos, mapTile.tile);
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