using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField] private float speed = 1;       // 발사 속도
    [SerializeField] private float range = 2;       // 발사 후 소멸되는 거리

    public int power;
    [SerializeField] private GameObject ExplotPtcl; // 폭발 이펙트
    private float dist; // 이동 거리

    private void Awake()
    {
        //ExplotPtcl = (GameObject)Resources.Load("Turret/ExplosionEffect", typeof(GameObject));
        Destroy(this.gameObject, 10f);   // 3초 뒤 자동 삭제
        power = Random.Range(5, 15);    // 화살 대미지 적용
    }

    void Update()
    {
        // 앞 방향으로 이동
        //transform.Translate(Vector3.forward.normalized * Time.deltaTime * speed);
        transform.Translate(transform.forward * Time.deltaTime * speed, Space.World);
        // 이동 거리 업데이트
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
        if (coll.gameObject.tag == "Enemy")
        {
            //Instantiate(ExplotPtcl, transform.position, transform.rotation);    // 폭발 이펙트 생성
            Destroy(this.gameObject);   // 제거
        }
    }
}
