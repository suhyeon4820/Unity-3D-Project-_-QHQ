using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectGizmo : MonoBehaviour
{
    //기즈모 색상 
    public Color Mycolor = Color.red;
    //기즈모 반지름 
    public float explosionRadius = 7.0f;

    // 선택시 폭발 반경(기즈모)을 보여준다.  
    void OnDrawGizmosSelected()
    {
        Vector3 p = transform.position;         // 위치 가져옴
        Gizmos.color = Mycolor;                 // 기즈모 색상
        Gizmos.DrawSphere(p, explosionRadius);  // 구 그려주기
    }
}
