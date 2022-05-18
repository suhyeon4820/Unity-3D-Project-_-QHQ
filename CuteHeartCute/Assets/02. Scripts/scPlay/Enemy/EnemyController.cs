using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public List<Transform> wayPoints = new List<Transform>();
    private NavMeshAgent myTraceAgent;

    //자신과 타겟 Transform 참조 변수  
    [HideInInspector] public Transform myTr;
    [HideInInspector] public Transform traceTarget;

    [Header("Target")]
    public static bool isDie = false;

    //플레이어를 찾기 위한 배열 
    private GameObject[] players;
    private Transform playerTarget;

    //추적 대상인 터렛
    private GameObject[] turretAll;
    private Transform turretTarget;

    //추적 대상 거리체크 변수 
    float distancePlayer;
    float distanceTurret;

    [SerializeField] private float attackRange;
    // item
    [SerializeField] private GameObject[] items;

    [Header("Animation")]
    private Animator myAnimator;
    private bool isHit;
    private float isHitTime;

    [Header("Photon")]
    Rigidbody myRbody;  //참조할 컴포넌트를 할당할 레퍼런스 (미리 할당하는게 좋음)
    PhotonView pv = null;   //PhotonView 컴포넌트를 할당할 레퍼런스 

    [Header("Sound")]
    private AudioSource source = null;  //AudioSource 컴포넌트 저장할 레퍼런스 
    [SerializeField] private AudioClip enemyDeadClip;


    //위치 정보를 송수신할 때 사용할 변수 선언 및 초기값 설정 
    Vector3 currPos = Vector3.zero;
    Quaternion currRot = Quaternion.identity;

    private void Awake()
    {
        // 레퍼런스 연결
        myAnimator = GetComponent<Animator>();

        // 포톤 추가
        // 만약 Start 함수에서 초기화 했다면 Start 함수를 Awake 함수로 변경 ( 기존  Start 함수에서 처리 할 경우 
        myTr = GetComponent<Transform>(); //전에 OnPhotonSerializeView 콜백 함수가 먼저 호출될 경우 Null Reference 오류 발생)

        //레퍼런스 할당 
        myTraceAgent = GetComponent<NavMeshAgent>();
        myTr = GetComponent<Transform>();   //자기 자신의 Transform 연결
        turretAll = GameObject.FindGameObjectsWithTag("Turret");  //Hierarchy의 모든 Base를 찾음

        // item
        items[0] = (GameObject)Resources.Load("Item/BulletItem", typeof(GameObject));
        items[1] = (GameObject)Resources.Load("Item/DefaultItem", typeof(GameObject));
        items[2] = (GameObject)Resources.Load("Item/EquipmentItem", typeof(GameObject));
        items[3] = (GameObject)Resources.Load("Item/HpFoodItem", typeof(GameObject));
        items[4] = (GameObject)Resources.Load("Item/GoldItem", typeof(GameObject));

        // sound
        source = GetComponent<AudioSource>();   //AudioSource 컴포넌트를 해당 변수에 할당
        //케논 발사 사운드 파일을 Resources 폴더에서 불러와 레퍼런스에 할당 ()
        enemyDeadClip = Resources.Load<AudioClip>("Sound/enemy/enemyDead");  // 
        

        //로밍 위치 얻기
        //roamingCheckPoints = GameObject.Find("RoamingPoint").GetComponentsInChildren<Transform>();

        // photon *****************************************
        myRbody = GetComponent<Rigidbody>();     //컴포넌트를 할당 
        pv = GetComponent<PhotonView>();    //PhotonView 컴포넌트 할당


        //PhotonView Observed Components 속성에 PlayerCtrl(현재) 스크립트 Component를 연결
        pv.ObservedComponents[0] = this;

        //데이타 전송 타입을 설정
        pv.synchronization = ViewSynchronization.UnreliableOnChange;

        //자신의 네트워크 객체가 아닐때...(마스터 클라이언트가 아닐때)
        if (!PhotonNetwork.isMasterClient)
        {
            //원격 네트워크 유저의 아바타는 물리력을 안받게 처리하고
            //또한, 물리엔진으로 이동 처리하지 않고(Rigidbody로 이동 처리시...)
            //실시간 위치값을 전송받아 처리 한다 그러므로 Rigidbody 컴포넌트의
            //isKinematic 옵션을 체크해주자. 한마디로 물리엔진의 영향에서 벗어나게 하여
            //불필요한 물리연산을 하지 않게 해주자...

            //원격 네트워크 플레이어의 아바타는 물리력을 이용하지 않음 (마스터 클라이언트가 아닐때)
            //(원래 게임이 이렇다는거다...우리건 안해도 체크 돼있음...)
            myRbody.isKinematic = true;
            //네비게이션도 중지
            //myTraceAgent.isStopped = true; 이걸로 하면 off Mesh Link 에서 에러 발생 그냥 비활성 하자
            //myTraceAgent.enabled = false; 
        }

        // 원격 플래이어의 위치 및 회전 값을 처리할 변수의 초기값 설정 
        // 잘 생각해보자 이런처리 안하면 순간이동 현상을 목격
        currPos = myTr.position;
        currRot = myTr.rotation;
    }

    //IEnumerator Start()
    //{

    //    // 마스터 클라이언트만 수행
    //    if (PhotonNetwork.isMasterClient)
    //    {
    //        //일단 첫 Base의 Transform만 연결
    //        traceTarget = turretAll[0].transform;
    //        myTraceAgent.SetDestination(traceTarget.position);  // 추적 시작

    //        // 정해진 시간 간격으로 Enemy의 Ai 변화 상태를 셋팅하는 코루틴
    //        //StartCoroutine(ModeSet());

    //        // Enemy의 상태 변화에 따라 일정 행동을 수행하는 코루틴
    //        //StartCoroutine(ModeAction());

    //        // 1. 일정 간격으로 가장 가까운 turret과 플레이어를 찾기
    //        StartCoroutine(this.TargtSetting());

    //        // 로밍 루트 설정
    //        //RoamingCheckStart();
    //    }
    //    else
    //    {
    //        // 마스터 클라이언트가 아닐때 네트워크 객체를 일정 간격으로 애니메이션을 동기화 하는 코루틴
    //        //StartCoroutine(this.NetAnim());
    //    }

    //    yield return null;
    //}

    // Update is called once per frame
    //void Update()
    //{
    //    // 일정 시간 지나면 공격 변수 체크 
    //    if (isHit)
    //    {
    //        if (Time.time > isHitTime)
    //        {
    //            isHit = false;
    //        }
    //    }

    //}


    // 1. 일정 간격으로 가장 가까운 turret과 플레이어를 찾기 - 플레이어가 turret보다 우선순위가 높게 셋팅 추가
    //IEnumerator TargtSetting()
    //{
    //    while (!isDie)
    //    {
    //        yield return new WaitForSeconds(0.1f);

    //        // 자신과 가장 가까운 플레이어 찾음
    //        players = GameObject.FindGameObjectsWithTag("Player");

    //        //플레이어가 있을경우 
    //        if (players.Length != 0)
    //        {
    //            playerTarget = players[0].transform;
    //            distancePlayer = (playerTarget.position - myTr.position).sqrMagnitude;
    //            foreach (GameObject _players in players)
    //            {
    //                if ((_players.transform.position - myTr.position).sqrMagnitude < distancePlayer)
    //                {
    //                    playerTarget = _players.transform;
    //                    distancePlayer = (playerTarget.position - myTr.position).sqrMagnitude;
    //                }
    //            }
    //        }

    //        // 자신과 가장 가까운 터렛 찾음
    //        turretAll = GameObject.FindGameObjectsWithTag("Turret");
    //        turretTarget = turretAll[0].transform;
    //        distanceTurret = (turretTarget.position - myTr.position).sqrMagnitude;
    //        foreach (GameObject _baseAll in turretAll)
    //        {
    //            if ((_baseAll.transform.position - myTr.position).sqrMagnitude < distanceTurret)
    //            {
    //                turretTarget = _baseAll.transform;
    //                distanceTurret = (turretTarget.position - myTr.position).sqrMagnitude;
    //            }
    //        }

    //        //만약 플레이어가 없으면 타겟 목표 설정  
    //        if (players.Length == 0)
    //        {
    //            traceTarget = turretTarget;
    //        }
    //        else
    //        {
    //            traceTarget = playerTarget;
    //            // 플레이어가 베이스보다 우선순위가 높게 셋팅 (게임마다 틀리다 즉 자기 맘)
    //            //if (distancePlayer <= distanceTurret)
    //            //{
    //            //    traceTarget = playerTarget;
    //            //}
    //            //else
    //            //{
    //            //    traceTarget = turretTarget;
    //            //}
    //        }

    //    }
    //}


    // enemy 죽음 처리 ***************************************************
    public void EnemyDie()
    {
        // 포톤 추가
        if (pv.isMine)
        {
            StartCoroutine(this.Die());
        }
    }

    // Enemy의 사망 처리
    IEnumerator Die()
    {
        //isDie = true;   // Enemy를 죽이자
        myAnimator.SetTrigger("dead");  // 애니메이션 처리
        source.PlayOneShot(enemyDeadClip, enemyDeadClip.length);
        // item 생성
        int num = Random.Range(1, 100);
        Vector3 position = transform.position;
        pv.RPC("SpawnItem", PhotonTargets.All, num, position);

        //Enemy의 태그를 Untagged로 변경하여 더이상 플레이어랑 포탑이 찾지 못함
        this.gameObject.tag = "Untagged";
     
        //Enemy에 추가된 모든 Collider를 비활성화(모든 충돌체는 Collider를 상속했음 따라서 다음과 같이 추출 가능)
        foreach (Collider coll in gameObject.GetComponentsInChildren<Collider>())
        {
            coll.enabled = false;
        }

        yield return new WaitForSeconds(1.5f);  //1.5 초후 오브젝트 삭제

        //PhotonNetwork.Destroy(gameObject);
        if (pv.isMine)
            PhotonNetwork.Destroy(this.gameObject);  // 자신과 네트워크상의 모든 아바타를 삭제
        else
            pv.RPC("EnemyDead", PhotonTargets.AllBuffered);
    }
    [PunRPC]
    void EnemyDead()
    {
        PhotonNetwork.Destroy(this.gameObject);
    }
    [PunRPC]
    void SpawnItem(int num, Vector3 position)
    {
        if (num % 8 == 0 || num % 8 == 1 || num % 8 == 2 || num % 8 == 3 || num % 8 == 4 )
            Instantiate(items[num% 8], position, Quaternion.identity);
        else
            return;
    }

    public void EnemyHomeDie()
    {
        // 포톤 추가
        if (pv.isMine)
        {
            StartCoroutine(this.GameOver());
        }
    }
    IEnumerator GameOver()
    {
        isDie = true;   // Enemy를 죽이자
        source.PlayOneShot(enemyDeadClip, enemyDeadClip.length);

        //Enemy의 태그를 Untagged로 변경하여 더이상 플레이어랑 포탑이 찾지 못함
        this.gameObject.tag = "Untagged";

        //Enemy에 추가된 모든 Collider를 비활성화(모든 충돌체는 Collider를 상속했음 따라서 다음과 같이 추출 가능)
        foreach (Collider coll in gameObject.GetComponentsInChildren<Collider>())
        {
            coll.enabled = false;
        }
        yield return new WaitForSeconds(0.1f);  // 0.1초후 오브젝트 삭제

        //PhotonNetwork.Destroy(gameObject);

        if (pv.isMine)
            PhotonNetwork.Destroy(this.gameObject);  // 자신과 네트워크상의 모든 아바타를 삭제
        else
            pv.RPC("EnemyDead", PhotonTargets.AllBuffered);
    }


    //일정 확률로 Enemy 타격 ************************************************************************************
    //public void HitEnemy()
    //{
    //    //isHitTime = Time.time + 0.1f;   // 시간 딜레이
    //    //isHit = true;
    //    //myAnimator.SetTrigger("hit");
    //}

    // photon ***********************************************************************
    /*
     * PhotonView 컴포넌트의 Observe 속성이 스크립트 컴포넌트로 지정되면 PhotonView
     * 컴포넌트는 데이터를 송수신할 때, 해당 스크립트의 OnPhotonSerializeView 콜백 함수를 호출한다. 
     */
    // 데이터를 전송주기에 맞춰 송/수신하는 역할을 함.
    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //로컬 플레이어의 위치 정보를 송신
        if (stream.isWriting)
        {
            //박싱
            stream.SendNext(myTr.position);
            stream.SendNext(myTr.rotation);
            //stream.SendNext(net_Aim);
        }
        //원격 플레이어의 위치 정보를 수신
        else
        {
            //언박싱
            currPos = (Vector3)stream.ReceiveNext();
            currRot = (Quaternion)stream.ReceiveNext();
            //net_Aim = (int)stream.ReceiveNext();
        }

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
