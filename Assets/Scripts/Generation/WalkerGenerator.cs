using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
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
    [SerializeField] public TileBase Floor;
    [SerializeField] public TileBase DecorFoliage;
    [SerializeField] public TileBase Wall;
    public int MapWidth = 32;
    public int MapHeight = 32;
    public int MaximumWalkers = 10;
    public int TileCount = default;
    public int DecorCount = default;
    public float FillPercentage = 0.4f;
    public float DecorPercentage = 0.4f;
    public float WaitTime = 0.05f;
    public float ChanceToChange = 0.5f;
    public int FillRadius = 4;
    public int EdgeBuffer = 4;
    private int xOrg;
    private int yOrg;
    public Map map;
    [SerializeField] Maps maps;
    string key;

    
    void Start(){
        key = SceneManager.GetActiveScene().name;
        map = maps.maps.FirstOrDefault(m=>m.key == key);

        if(map!=null)
        {
            xOrg =map.MapWidth/ 2;
            yOrg =map.MapHeight / 2;
            Vector3Int TileCenter = new Vector3Int(xOrg, yOrg, 0);

            map.LoadMap(tilemap, decorTileMap, TileCenter,Wall);
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

    public void FillSurroundingCells(int x, int y, int distance, float noise)
    {

        for (int searchX = x - distance; searchX <= x + distance; searchX++)
        {
            for (int searchY = y - distance; searchY <= y + distance; searchY++)
            {
                int xClamp = Mathf.Clamp(searchX, 0,map.gridHandler.GetLength(0)-1);
                int yClamp =Mathf.Clamp(searchY, 0,map.gridHandler.GetLength(1)-1);

                if(map.gridHandler[xClamp, yClamp] != Grid.FLOOR )
                {
                    Vector3Int pos = new Vector3Int(xClamp,yClamp);
                 
                    tilemap.SetTile(pos, Floor);

                    TileCount++;

                    map.gridHandler[pos.x, pos.y] = Grid.FLOOR;

                     MapTile maptile =  new MapTile(pos,Floor);
                    map.basetiles.Add(maptile);
                }
            }
        }
    
    }

    IEnumerator CreateFloors()
    {
        while ((float)TileCount / (float)map.gridHandler.Length < FillPercentage)
        {
            bool hasCreatedFloor = false;
            foreach (Walker curWalker in Walkers)
            {
                Vector3Int curPos = new Vector3Int((int)curWalker.Position.x, (int)curWalker.Position.y, 0);
                float noise = CalcNoise(curPos.x,curPos.y);

                if(map.gridHandler[curPos.x, curPos.y] != Grid.FLOOR  )
                {
                    
                    if(noise > 0.5f){
                        
                        tilemap.SetTile(curPos, Floor);
                    
                        TileCount++;
                        
                        map.gridHandler[curPos.x, curPos.y] = Grid.FLOOR;

                        

                         MapTile maptile =  new MapTile(curPos,Floor);
                        map.basetiles.Add(maptile);
                        
                        
                    }

                    hasCreatedFloor = true;
                }

                FillSurroundingCells(curPos.x,curPos.y, FillRadius, noise);
            }

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
            
        }

    public float CalcNoise(int x, int y)
    {
       
        float xCoord = xOrg + x / MapWidth ;
        float yCoord = yOrg + y / MapHeight;
        float sample = Mathf.PerlinNoise(xCoord, yCoord);
        return sample;
       
       
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

    public Map(Grid[,] gridHandler, int MapWidth, int MapHeight, string key){
        this.gridHandler = gridHandler;
        this.MapHeight = MapHeight;
        this.MapWidth = MapWidth;
        this.key = key;
        basetiles = new List<MapTile>();
        decorTiles = new List<MapTile>();
    }
      public void LoadMap(Tilemap baseTilemap, Tilemap decorTilemap, Vector3Int TileCenter, TileBase Water ){
        baseTilemap.BoxFill(TileCenter, Water, 0, 0 , MapWidth, MapHeight);

        foreach(MapTile mapTile in basetiles){
            baseTilemap.SetTile(mapTile.pos, mapTile.tile);
        }
        foreach(MapTile mapTile in decorTiles){
            decorTilemap.SetTile(mapTile.pos, mapTile.tile);
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