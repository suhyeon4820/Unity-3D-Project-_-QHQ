using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    private Rigidbody myRigid;
    [SerializeField] private float moveSpeed = 0.05f;

    // Start is called before the first frame update
    void Start()
    {
        myRigid = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 inputDir = new Vector3(h, 0f, v).normalized;

        myRigid.velocity = inputDir * moveSpeed;

        transform.LookAt(transform.position + inputDir);    // 자연스럽게 회전


    }
}
