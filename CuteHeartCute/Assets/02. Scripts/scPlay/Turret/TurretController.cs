using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class TurretController : MonoBehaviour
{
    [Header("Turret")]
    // 인스펙터에 노출은 막고 외부 노출은 원하는 경우 사용
    [HideInInspector] public bool turretIsDie;  //죽었는지 상태변수
 
    //Enemy를 찾기 위한 배열 
    private GameObject[] Enemys;
    private Transform EnemyTarget;

    // 터렛 변수 
    private Transform myTr; //자신의 Transform 참조 변수
    private bool shotStart;  // 터렛 발사 변수
    private float bulletSpeed;  //총알 발사 주기
    private float enemyLookTime;    // 적을 봐라보는 회전 속도
    private Quaternion enemyLookRotation;   //적을 봐라보는 회전각
    [SerializeField] private float attackRange;   // 공격 범위
    [SerializeField] private float raycastRange;  // 탐지 범위
    [SerializeField] private Transform targetTr;  // 회전의 중심축
    [SerializeField] private GameObject bullet;   //총탄 프리팹을 위한 레퍼런스
    [SerializeField] private Transform firePos;   //총탄의 발사 시작 좌표 연결 변수 
    [SerializeField] private GameObject muzzleFlash;  //MuzzleFlash GameObject를 연결 할 레퍼런스 
    //적과의 거리를 위한 변수
    public float dist1;
    public float dist2;

    // sound
    private AudioSource source = null;  //AudioSource 컴포넌트 저장할 레퍼런스 
    //[SerializeField] private AudioClip fireSfx;   //총탄의 발사 사운드 

    // RayCast *******************************
    Ray ray;    //Ray 정보 저장 구조체 
    RaycastHit hitInfo; // Ray에 맞은 오브젝트 정보를 저장 할 구조체 (충돌 감지)
    bool enemyFindCheck; //Ray 센서를 위한 변수
    public LineRenderer leftRayLine;     //레이저 발사를 위한 컴포넌트
    public LineRenderer rightRayLine;    //레이저 발사를 위한 컴포넌트
    public Transform leftRayDot;    //레이저 도트 타겟을 위한 변수
    public Transform rightRayDot;    //레이저 도트 타겟을 위한 변수

    // Photon *******************************
    public PhotonView pv = null;    //PhotonView 컴포넌트를 할당할 레퍼런스 
    Quaternion currRot = Quaternion.identity;   //위치 정보를 송수신할 때 사용할 변수 선언 및 초기값 설정 
    public int playerId = -1;   //플레이어의 Id를 저장하는 변수
    public int killCount = 0;   //몬스터의 파괴 스코어를 저장하는 변수 
    public PlayerCtrl localPlayer;  //로컬  플레이어 연결 레퍼런스

   // 로비체크용 변수
   // public bool loby;
 

    void Awake()
    {
        // Bullet prefab을 타입이 Gameobject인지 확인 후 Resources 폴더에서 불러와 변수에 할당 
        bullet = (GameObject)Resources.Load("Turret/TurretBullet", typeof(GameObject));
        source = GetComponent<AudioSource>();   //AudioSource 컴포넌트를 해당 변수에 할당
        //케논 발사 사운드 파일을 Resources 폴더에서 불러와 레퍼런스에 할당 ()
        //fireSfx = Resources.Load<AudioClip>("Turret/bazooka");  // 

        localPlayer = transform.root.GetComponent<PlayerCtrl>();    // 로컬 플레이어 연결
        myTr = GetComponent<Transform>();   //자기 자신의 Transform 연결
        

        //muzzleFlash.SetActive(false);   //처음에 MuzzleFlash 를 비활성화  **

        // photon
        pv = GetComponent<PhotonView>();    // PhotonView 컴포넌트 할당 
        pv.ObservedComponents[0] = this;    // PhotonView Observed Components 속성에 BaseCtrl(현재) 스크립트 Component를 연결
        pv.synchronization = ViewSynchronization.UnreliableOnChange;    // 데이타 전송 타입을 설정
        playerId = pv.ownerId;  //PhotonView의 ownerId를 playerId에 저장 - 유저 ownerId 부여(숫자 1부터~)

        
        currRot = myTr.rotation;    // 원격 플래이어의 회전 값을 처리할 변수의 초기값 설정 

        transform.parent.parent = null;  // 플레이어와 분리

        // StageManager와 연결
        //if (pv.isMine)
        //{
        //    GameObject.FindWithTag("Mgr").GetComponent<StageManager>().turretStart = this;
        //}
    }

    // StageManager에서 호출
    public void StartTurret()
    {
        if (pv.isMine)
        {
            // 터렛 1 - 일정 간격으로 가장 가까운 Enemy 찾기 
            StartCoroutine(this.TargtSetting());

            // 터렛 2 - 가장 가까운 적을 찾아 발사
            StartCoroutine(this.ShotSetting());
        }
    }

    // Update is called once per frame
    void Update()
    {
        // RayCast(green) 적 탐색 + 발사 상태 체크 ***************************************************
        ray.origin = firePos.position;                                          // ray가 발사될 지점 저장
        ray.direction = firePos.TransformDirection(Vector3.forward);            // ray가 발사되는 방향 - firePos local space(앞 방향)를 world space로 변환 
        //ray.direction = firePos.forward;
        //레이캐스트 쏘는 위치, 방향, 결과값, 최대인식거리
        Debug.DrawRay(ray.origin, ray.direction, Color.green, raycastRange);   // Scene 뷰에만 시각적으로 표현함

        //위에서 미리 생성한 ray를 인자로 전달, out(메서드 안에서 메서드 밖으로 데이타를 전달 할때 사용)hit, ray 거리
        if (Physics.Raycast(ray, out hitInfo, raycastRange))
        {
            Vector3 posValue = firePos.InverseTransformPoint(hitInfo.point);  // hitInfo.point 는 월드좌표이다 따라서 로컬 좌표로 변환
            leftRayLine.SetPosition(0, posValue);   // 타겟 거리체크 레이저 생성
            rightRayLine.SetPosition(0, posValue);   // 타겟 거리체크 레이저 생성
            leftRayDot.localPosition = posValue;    // 타겟에 레이저 Dot 생성 
            rightRayDot.localPosition = posValue;    // 타겟에 레이저 Dot 생성 

            // 포톤 추가
            if (pv.isMine && shotStart && hitInfo.collider.tag == "Enemy")
            {
                enemyFindCheck = true;   //발사를 위한 변수 true
            }
        }
        else
        {
            leftRayLine.SetPosition(0, new Vector3(0, 0, raycastRange));   //기본 거리체크 레이저 생성
            rightRayLine.SetPosition(0, new Vector3(0, 0, raycastRange));
            leftRayDot.localPosition = Vector3.zero;    //타겟에 레이저 Dot 초기화 
            rightRayDot.localPosition = Vector3.zero;
        }

        // 포톤 추가
        if (pv.isMine)
        {
            if (!shotStart)
            {
                myTr.RotateAround(targetTr.position, Vector3.up, Time.deltaTime * 55.0f);
                enemyFindCheck = false;  //발사를 위한 변수 false
            }
            else
            {
                // 적을 봐라봄  
                if (shotStart)
                {
                    if (Time.time > enemyLookTime)
                    {
                        if(EnemyTarget != null)
                        {
                            enemyLookRotation = Quaternion.LookRotation(EnemyTarget.position - myTr.position * 2); // - 해줘야 바라봄  
                            myTr.rotation = Quaternion.Lerp(myTr.rotation, enemyLookRotation, Time.deltaTime * 2.0f);
                            enemyLookTime = Time.time + 0.01f;
                        }
                    }
                }
            }

            //만약 발사가 true 이면....
            if (shotStart && enemyFindCheck)
            {
                if (Time.time > bulletSpeed)
                {
                    // 일정 주기로 발사 - (포톤 추가)자신의 플레이어일 경우는 로컬함수를 호출하여 총을 발포
                    ShotStart();

                    // 원격 네트워크 플레이어의 자신의 아바타 플레이어에는 RPC로 원격으로 FireStart 함수를 호출 
                    pv.RPC("ShotStart", PhotonTargets.Others, null);

                    //(포톤 추가)모든 네트웍 유저에게 RPC 데이타를 전송하여 RPC 함수를 호출, 로컬 플레이어는 로컬 Fire 함수를 바로 호출 
                    //pv.RPC("ShotStart", PhotonTargets.All, null);

                    bulletSpeed = Time.time + 0.3f;
                }
            }
        }
        //원격 플레이어일 때 수행
        else
        {
            //원격 베이스의 아바타를 수신받은 각도만큼 부드럽게 회전시키자
            myTr.rotation = Quaternion.Slerp(myTr.rotation, currRot, Time.deltaTime * 3.0f);
        }

    }

    // 터렛 1 - 일정 간격으로 가장 가까운 Enemy 찾기 
    IEnumerator TargtSetting()
    {
        while (!turretIsDie)
        {
            yield return new WaitForSeconds(0.2f); // 0.2초 후에 실행

            //  에네미를 찾음
            Enemys = GameObject.FindGameObjectsWithTag("Enemy");
            
            if (Enemys.Length > 0)
            {
                // 자신과 가장 가까운 에네미 위치
                Transform EnemyTargets = Enemys[0].transform;

                // 사각형 충돌?
                float dist = (EnemyTargets.position - myTr.position).sqrMagnitude;
                foreach (GameObject _Enemy in Enemys)
                {
                    if ((_Enemy.transform.position - myTr.position).sqrMagnitude < dist)
                    {
                        EnemyTargets = _Enemy.transform;
                        dist = (EnemyTargets.position - myTr.position).sqrMagnitude;
                    }
                }
                EnemyTarget = EnemyTargets;
            }
                
        }
    }

    // 터렛 2 - 가장 가까운 적을 찾아 발사
    IEnumerator ShotSetting()
    {
        while (!turretIsDie)
        {
            yield return new WaitForSeconds(0.2f);  // 0.2초 후에 실행

            if (EnemyTarget != null)
            {
                dist1 = (EnemyTarget.position - myTr.position).sqrMagnitude;
                dist2 = Vector3.Distance(myTr.position, EnemyTarget.position);
            }
            else
            {
                shotStart = false;
            }

            // 체크후 발사 지정... 코루틴으로 처리가 더 효율
            if (dist2 < attackRange)
            {
                shotStart = true;
            }
            else
            {
                shotStart = false;
            }

            //float distanceBteweenEnemy = Vector3.Distance(myTr.position, EnemyTarget.position);

            //// 터렛 범위 내에 있으면 shot true
            //if (distanceBteweenEnemy < attackRange)
            //{
            //    shotStart = true;
            //}
            //else
            //{
            //    shotStart = false;
            //}
        }
    }

    // 터렛 발사
    [PunRPC]
    private void ShotStart()
    {
        if(Enemys!=null)
            StartCoroutine(this.FireStart());   //잠시 기다리는 로직처리를 위해 코루틴 함수로 호출
    }

    // 총탄 발사 코루틴 함수
    IEnumerator FireStart()
    {
        //Bullet 프리팹을 동적 생성
        TurretBullet obj = Instantiate(bullet, firePos.position, firePos.rotation).GetComponent<TurretBullet>();
        
        // 동적 생성한 총알에 유저 ownerId 부여(숫자 1부터~)
        //obj.playerId = pv.ownerId;

        //source.PlayOneShot(fireSfx, fireSfx.length + 0.2f);   //총탄 사운드 발생 


        // 머즐 플레시 보여주기
        muzzleFlash.SetActive(true);    //활성화 시킴
        //yield return new WaitForSeconds(Random.Range(0.05f, 0.2f)); //랜덤 시간 동안 Delay한 다음 MeshRenderer를 비활성화
        yield return new WaitForSeconds(5f);
        muzzleFlash.SetActive(false);   //비활성
    }



    //인스펙터에 스크립트 우 클릭시 컨텍스트 메뉴에서 함수호출 가능
    [ContextMenu("FireStart")]
    void Fire()
    {
        shotStart = true;
    }

    // 터렛 범위 보여주는 기즈모 
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, raycastRange);
    }

    // 포톤 추가/////////////////////////////////////////////////

    //네트워크 플레이어의 스코어 증가 및 HUD 설정 함수
    public void PlusKillCount()
    {
        ++killCount;    //Enemy 파괴 스코어 증가
        localPlayer.txtKillCount.text = killCount.ToString();   //HUD Text UI 항목에 스코어 표시

        /* 포톤 클라우드에서 제공하는 플레이어의 점수 관련 메서드
         * PhotonPlayer.AddScore ( int score )      점수를 누적
         * PhotonPlayer.SetScore( int totScore )    해당 점수로 셋팅
         * PhotonPlayer.GetScore()                  현재 점수를 조회
         */

        //스코어를 증가시킨 베이스가 자신인 경우에만 저장
        if (pv.isMine)
        {
            /* PhotonNetwork.player는 로컬 플레이어 즉 자신을 의미한다.
               즉 다음 로직은 자기 자신의 스코어에 1점을 증가시킨다. 이 정보는 동일 룸에
               입장해있는 다른 네트워크 플레이어와 실시간으로 공유된다.*/
            PhotonNetwork.player.AddScore(1);
        }
    }

    /*
     * PhotonView 컴포넌트의 Observe 속성이 스크립트 컴포넌트로 지정되면 PhotonView
     * 컴포넌트는 데이터를 송수신할 때, 해당 스크립트의 OnPhotonSerializeView 콜백 함수를 호출한다. 
     */
    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //로컬 플레이어의 위치 정보를 송신
        if (stream.isWriting)
        {
            //박싱
            stream.SendNext(myTr.rotation);
        }
        //원격 플레이어의 위치 정보를 수신
        else
        {
            //언박싱
            currRot = (Quaternion)stream.ReceiveNext();
        }

    }
    
}
