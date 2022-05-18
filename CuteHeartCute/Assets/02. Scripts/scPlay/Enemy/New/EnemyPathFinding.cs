using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPathFinding : MonoBehaviour
{

    [SerializeField] private float moveSpeed = 2f;
    private Transform target;
    private int wavePointIndex = 0;
    private void Start()
    {
        // start point에 생성
        target = Paths.points[0];
    }

    private void Update()
    {
        Vector3 dir = target.position - transform.position;
        transform.Translate(dir.normalized * moveSpeed * Time.deltaTime);

        if(Vector3.Distance(transform.position, target.position)<=1.5f)
        {
            GetNextWayPoint();
        }
    }

    void GetNextWayPoint()
    {
        if(wavePointIndex>=Paths.points.Length-1)
        {
            return;
        }
        wavePointIndex++;
        target = Paths.points[wavePointIndex];
    }
    

}


//void Start()
//{
//    if(waypoints.GetComponent<Paths>().isStart == true)
//    {
//        Debug.Log("dd");
//        // 첫번째 포인트에 위치시킴
//        currentWaypoint = waypoints.GetComponent<Paths>().GetNextWaypoint(currentWaypoint);
//        transform.position = currentWaypoint.position;

//        // 다음 타겟 위치 가져옴
//        currentWaypoint = waypoints.GetComponent<Paths>().GetNextWaypoint(currentWaypoint);
//    }

//}

//void Update()
//{
////    // 이동시키기
////    transform.position = Vector3.MoveTowards(transform.position, currentWaypoint.position, moveSpeed * Time.deltaTime);

////    // 다음 포인트와 이동했으면 실행
////    if (Vector3.Distance(transform.position, currentWaypoint.position) < distanceTreashold)
////    {
////        // 다음 타겟
////        currentWaypoint = waypoints.GetComponent<Paths>().GetNextWaypoint(currentWaypoint);
////    }
//}