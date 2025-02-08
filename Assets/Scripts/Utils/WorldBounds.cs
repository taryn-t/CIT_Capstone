


using UnityEngine;
using UnityEngine.Tilemaps;

public class WorldBounds{


    public int maxX ;
    public int maxY ;
    public int minX ;
    public int minY ;

    public Vector3Int maxCell;
    public Vector3Int minCell ;
    
    public UnityEngine.Vector3 maxWorld ;
   public  UnityEngine.Vector3 minWorld;



    public WorldBounds(Tilemap baseTilemap){

        maxX = baseTilemap.cellBounds.xMax;
        maxY = baseTilemap.cellBounds.yMax;
        minX = baseTilemap.cellBounds.xMin;
        minY = baseTilemap.cellBounds.yMin;

        maxCell = new UnityEngine.Vector3Int(maxX,maxY);
        minCell = new UnityEngine.Vector3Int(minX,minY);
        
        maxWorld =baseTilemap.CellToWorld(maxCell);
        minWorld = baseTilemap.CellToWorld(minCell);
    }
}