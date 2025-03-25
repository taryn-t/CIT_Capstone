using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro.Examples;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class MapPanel : MonoBehaviour
{

    public bool open = true;

    [SerializeField] GameObject mapImage;
    [SerializeField] GameObject markerPrefab;
    [SerializeField] GameObject mapMask;
    [SerializeField] public GameObject mapContainer;
    [SerializeField] public GameObject generatingLabel;
    [SerializeField] public GameObject startButton;
    public float generatingXPos = -345f;
    public float runtimeXPos = 0f;
    private List<MapMarker> mapMarkers = new List<MapMarker>();
    private Vector3 mapScale = new Vector3(1,1,1);
    private float minScale = 1;

    private float maxScale = 10;
    private float curScale =1;

    private Vector2 mapPos = new Vector2(32,32);
    private Vector2 basePos;
    private bool zoom = false;
    private Vector2 markerScale;
    
    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.mapPanel = gameObject;
       startButton.SetActive(false);
       generatingLabel.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
     
    }


    // public void OnScroll(PointerEventData eventData){
       

    //     var scale = mapImage.transform.localScale.x;
    //     //mousePosition contains position of mouse inside scaled area in percentages
    //     var mousePosition = (Vector2) (Input.mousePosition - mapImage.transform.position) - (Vector2)mapImage.GetComponent<RectTransform>().rect.position * scale;
    //     mousePosition.x /= mapImage.GetComponent<RectTransform>().rect.width * scale;
    //     mousePosition.y /= mapImage.GetComponent<RectTransform>().rect.height * scale;

    //     var contentSize = mapImage.GetComponent<RectTransform>().rect;
    //     var shiftX = -eventData.scrollDelta.y* contentSize.width * (mousePosition.x - 0.5f);
    //     var shiftY = -eventData.scrollDelta.y* contentSize.height * (mousePosition.y - 0.5f);
    //     var currPos = mapImage.GetComponent<RectTransform>().localPosition;

    //     mapImage.GetComponent<RectTransform>().localPosition = new Vector3(currPos.x + shiftX, currPos.y + shiftY, currPos.z);
    //     mapScale.x += eventData.scrollDelta.y ;
    //     mapScale.y += eventData.scrollDelta.y ;
    //     mapScale.x = Mathf.Clamp(mapScale.x,minScale, maxScale);
    //     mapScale.y = Mathf.Clamp(mapScale.y,minScale, maxScale);

    //     mapImage.transform.localScale = mapScale;
    //     curScale = mapScale.x;

    //     if(curScale == minScale){
    //         mapImage.transform.position = basePos;
    //     }

    // }

    public void SetMarker(GameObject markedObject, Sprite markerSprite){
        
        GameObject curMarker;
        Character markedChar = markedObject.GetComponent<Character>();
        if(!mapMarkers.Any(mapMaker => mapMaker.index == markedChar.mapIndex)){
            curMarker = Instantiate(markerPrefab,mapMask.transform);
            curMarker.GetComponent<Image>().sprite = markerSprite;

            int index = mapMarkers.Count;
            MapMarker newMarker = new MapMarker(curMarker,markedChar.gameObject);
            
            markedChar.mapIndex = newMarker.index;

            mapMarkers.Add(newMarker);
        }
        else{
            MapMarker curMapMarker = mapMarkers.First(mapMaker => mapMaker.index == markedChar.mapIndex);
            curMarker = curMapMarker.markerGO;
            
        }
        
        Camera Cam = GameManager.Instance.minimapCamera.gameObject.GetComponent<Camera>();
        Vector3 ViewportPosition=Cam.WorldToViewportPoint(markedChar.transform.position);
        var contentSize = mapImage.GetComponent<RectTransform>().rect;
       
        Vector3 localPosition = mapImage.GetComponent<RectTransform>().InverseTransformDirection(ViewportPosition);
        var shiftX = contentSize.width * (localPosition.x - 0.5f) ;
        var shiftY = contentSize.height * (localPosition.y - 0.5f) ;

        var pos = new Vector2(localPosition.x+shiftX, localPosition.y+shiftY);

        
        
        curMarker.GetComponent<RectTransform>().localPosition = pos;


    }
    public IEnumerator RemoveMarker(GameObject markedObject)
    {
        string index = markedObject.GetComponent<Enemy>().mapIndex;
        try{
           GameObject markerGO = mapMarkers.First(mapMarker => mapMarker.index == index).markerGO;
        
            mapMarkers.RemoveAll(mapMarker => mapMarker.index == index);
            
            Destroy(markerGO);  
        }
        catch{

        }
        
        
        yield return null;
    }

    public void ResetCamera(){
        mapImage.transform.localScale = new Vector3(1,1,1);
        mapImage.transform.position = basePos;
        curScale =1;
    }
    
    
    public void StartGame(){
        GameManager.Instance.mapGenerator.SaveMap();
    }


}


public class MapMarker{
    public string index;
    public GameObject markedObject;
    public GameObject markerGO;
 

    public MapMarker( GameObject marker, GameObject marked){
        index = Guid.NewGuid().ToString();
        markerGO =marker;
        markedObject = marked;
    }
}