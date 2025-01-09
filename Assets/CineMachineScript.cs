using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CineMachineScript : MonoBehaviour
{

    public CinemachineVirtualCamera vc;
    // Start is called before the first frame update
    void Start()
    {
        vc = GetComponent<CinemachineVirtualCamera>();
        GameManager.Instance.SetVirtualCamera(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetFollow(Transform t){
        vc.Follow = t;
    }
}
