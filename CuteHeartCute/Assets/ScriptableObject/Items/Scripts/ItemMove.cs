using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemMove : MonoBehaviour
{
    public float rotSpeed = 200f;
    public SpriteRenderer sp;
    private void Update()
    {
       sp.transform.Rotate(new Vector3(0,rotSpeed * Time.deltaTime, 0));
    }
}
