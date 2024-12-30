using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceNode : ToolHit
{
    private GameObject obj;
    [SerializeField]  int dropCount = 5;
    [SerializeField] float spread = 1f;
    [SerializeField] GameObject pickUpDrop;
    [SerializeField] public ResourceNodeType resourceNodeType;
    Vector2 startingPos;
    public bool instantiated = false;
    private Quaternion targetAngle = new Quaternion(0f,0f,90f,0f); 

    public void Awake(){
        startingPos.x = transform.position.x;
        startingPos.y = transform.position.y;
        obj = this.gameObject;
       
    }
    public override void Hit(){

        if(dropCount >0){
            dropCount-=1;
            Vector3 position = transform.position;
            position.x += spread * UnityEngine.Random.value - spread/2;
            position.y += spread * UnityEngine.Random.value - spread/2;
            GameObject go =Instantiate(pickUpDrop);
            go.transform.position = position;
        }else{
            if(resourceNodeType == ResourceNodeType.Tree){
                float startTime = Time.time;
                TreeFall(startTime);
            }else{
                Destroy(gameObject);
            }
            
        }

    }
    private void TreeFall(float startTime){
         
        float t = Time.time - startTime;
        transform.rotation = Quaternion.Slerp(transform.rotation, targetAngle, t);  
        Destroy(gameObject);
      }

    public void SetInstatiated(bool val){
        instantiated= val;
    }
    public override bool CanBeHit(List<ResourceNodeType> canBeHit)
    {
        return canBeHit.Contains(resourceNodeType);
    }
    public override  ResourceNodeType GetResourceNodeType(){
        return resourceNodeType;
    }
}