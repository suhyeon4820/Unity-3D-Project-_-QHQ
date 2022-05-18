using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paths : MonoBehaviour
{
    public static Transform[] points;
    public bool isStart = false;

    public void FindPoint()
    {
        // 배열 생성
        points = new Transform[transform.childCount];
        // 배열에 자식 오브젝트(최단 경로) 넣어주기
        for (int i = 0; i < points.Length; i++)
        {
            points[i] = transform.GetChild(i);
        }
        
        isStart = true;
    }

    //public Transform GetNextWaypoint(Transform currentWaypoint)
    //{
    //    // 처음에 null -> 맨 첫번째 경로 설정해주기
    //    if (currentWaypoint == null)
    //    {
    //        return transform.GetChild(0);
    //    }
    //    if (currentWaypoint.GetSiblingIndex() < transform.childCount - 1)
    //    {
    //        return transform.GetChild(currentWaypoint.GetSiblingIndex() + 1);
    //    }
    //    else
    //    {
    //        return transform.GetChild(transform.childCount - 1);
    //    }
    //}
}
