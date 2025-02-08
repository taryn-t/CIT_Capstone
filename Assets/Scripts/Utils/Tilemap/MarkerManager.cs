using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MarkerManager : MonoBehaviour
{
    [SerializeField] Tilemap targetTilemap;

    [SerializeField] TileBase tile;

    public Vector3Int markedCellPosition;
    public Vector3Int oldCellPosition;
    public Vector3 mousePosition;
    bool show;

    private void Update(){
        targetTilemap.SetTile(oldCellPosition,null);
        targetTilemap.SetTile(markedCellPosition, tile);
        oldCellPosition = markedCellPosition;
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
      
    }

    internal void Show(bool selectable){
        show = selectable;
        targetTilemap.gameObject.SetActive(show);
    }

}