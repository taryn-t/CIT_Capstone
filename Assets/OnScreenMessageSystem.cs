using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OnScreenMessage{
    public GameObject go;
    public float ttl = 5f;
    public OnScreenMessage(GameObject go){
        this.go = go;
    }

}
public class OnScreenMessageSystem : MonoBehaviour
{ 
    List<OnScreenMessage> onScreenMessages;
    List<OnScreenMessage> openList;
    [SerializeField] GameObject textPrefab;


    public void Start(){
        onScreenMessages = new List<OnScreenMessage>();
         openList = new List<OnScreenMessage>();
        GameManager.Instance.onScreenMessageSystem = this;

    }
    public void Update(){

        for(int i = onScreenMessages.Count -1; i >= 0; i--){
            onScreenMessages[i].ttl -= Time.deltaTime;
            if(onScreenMessages[i].ttl <0){
                onScreenMessages[i].go.SetActive(false);

                openList.Add(onScreenMessages[i]);
                onScreenMessages.RemoveAt(i);
                
            }
        }

     
    }

    public void PostMessage(Vector3 worldPosition, string message, Vector3 direction){

        worldPosition.z =+1;
        if(openList.Count >0){
            OnScreenMessage osm = openList[0];
            osm.go.SetActive(true);
            osm.ttl = 1f;
            osm.go.GetComponent<TMP_Text>().text= message.ToString();
            float randomIndex = UnityEngine.Random.Range(1, 2);
            osm.go.transform.position =  worldPosition + (Vector3.up * randomIndex);
            StartCoroutine(FloatMessage(osm.go, direction));
            openList.RemoveAt(0);
            onScreenMessages.Add(osm);
        }
        else{
            CreateNewOnScreenMessage(worldPosition,message,direction);
        }
        

    }
    public IEnumerator FloatMessage(GameObject osmGO, Vector3 direction){
        float tick = 0;
        float maxTick = 1f;
        float colorAlpha = 1f;
        while(tick < maxTick){
            osmGO.GetComponent<TMP_Text>().color = new Color(1,0,0,colorAlpha);
            
            osmGO.transform.position += (Vector3.up*0.1f)+(direction * 0.01f);
            
            yield return new WaitForSeconds(0.1f);
            colorAlpha -= 0.1f;
            tick += 0.1f;
        }
    }

      public void CreateNewOnScreenMessage(Vector3 worldPosition, string message, Vector3 direction){


        
        GameObject textGO = Instantiate(textPrefab,transform.GetChild(0).transform);
        float randomIndex = UnityEngine.Random.Range(1, 2);
        textGO.transform.position = worldPosition + (Vector3.up * randomIndex);
        
         textGO.GetComponent<TMP_Text >().text = message.ToString();
         
         OnScreenMessage onScreenMessage = new OnScreenMessage(textGO);
         StartCoroutine(FloatMessage(onScreenMessage.go,direction));
         onScreenMessage.ttl = 1f;
         onScreenMessages.Add(onScreenMessage);

    }
   
}
