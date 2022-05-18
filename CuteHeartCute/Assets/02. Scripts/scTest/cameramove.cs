using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameramove : MonoBehaviour
{
    private GameObject cameraObj;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        cameraObj = Camera.main.gameObject;
        cameraObj.transform.LookAt(transform);
        cameraObj.transform.Translate(Vector3.right * Time.deltaTime);

    }
}
