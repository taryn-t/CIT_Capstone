using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapCamera : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.minimapCamera = this;
    }

    // Update is called once per frame
     private void Update()
        {
            
        }

}
