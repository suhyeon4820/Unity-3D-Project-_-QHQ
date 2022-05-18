using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretBullet : MonoBehaviour
{
    [SerializeField] private float speed = 1;    // 발사 속도
    [SerializeField] private float range = 2;       // 발사 후 소멸되는 거리

    public int power;   
    [SerializeField] private GameObject ExplotPtcl; // 폭발 이펙트
    private float dist; // 이동 거리

    private void Awake()
    {
        ExplotPtcl = (GameObject)Resources.Load("Turret/Fire", typeof(GameObject));
        Destroy(this.gameObject, 3f);   // 3초 뒤 자동 삭제
        power = Random.Range(5, 15);    // 총알 대미지 적용
    }

    void Update()
    {
        // 총알 앞 방향으로 이동
        transform.Translate(Vector3.forward.normalized * Time.deltaTime * speed);
        // 총알 이동 거리 업데이트
        dist += Time.deltaTime * speed;

        // 이동 거리가 소멸 거리를 벗어나면
        if (dist >= range)
        {
            //Instantiate(ExplotPtcl, transform.position, transform.rotation);
            Destroy(this.gameObject);   // 자동 사라짐
        }
    }
   
    // 충돌시 파티클 생성 후 삭제
    void OnCollisionEnter(Collision coll)
    {
        if(coll.gameObject.tag == "Enemy")
        {
            Instantiate(ExplotPtcl, transform.position, transform.rotation);    // 폭발 이펙트 생성
            Destroy(this.gameObject);   // 제거
        }

        if (coll.gameObject.tag == "Player")
        {
            Destroy(this.gameObject);   // 제거
        }
    }
}
