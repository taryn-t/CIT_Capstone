using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeAgent : MonoBehaviour
{
    public Action onTimeTick;

    void Start()
    {
        Init();
    }
    public virtual void Init(){
        if(GameManager.Instance.dayTimeController !=null){
            GameManager.Instance.dayTimeController.GetComponent<DayTimeController>().Subscribe(this);

       }
    }

    public virtual void Invoke(){
        
        onTimeTick?.Invoke();
    }


    private void OnDestroy(){
       
       if(GameManager.Instance !=null){
            GameManager.Instance.dayTimeController.GetComponent<DayTimeController>().Unsubscribe(this);

       }
          
        
        
    }

    
}
