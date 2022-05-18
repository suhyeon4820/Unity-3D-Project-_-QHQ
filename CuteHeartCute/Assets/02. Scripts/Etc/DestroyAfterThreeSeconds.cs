using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterThreeSeconds : MonoBehaviour
{
    private void Awake()
    {
        Destroy(this.gameObject, 4f);
    }
}
