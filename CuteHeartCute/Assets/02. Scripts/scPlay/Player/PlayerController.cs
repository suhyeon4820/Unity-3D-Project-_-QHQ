using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
// 플레이어 이동 + 무기 교환 + 공격 애니메이션 + hpBar
public class PlayerController : MonoBehaviour
{
    public InventoryObject inventory; //플레이어 인벤토리 선언

    public int gold = 0; // 골드 획득 변수
    private bool isDead = false;
    // player move & jump ----------------------
    public float currentMoveSpeed;
    public float walkAnimationSpeed;
    public float jumpForce;
    public float runAnimationSpeed;
    [SerializeField] private float moveSpeed;//
    [SerializeField] private float rotationSpeed;//
    private Vector3 velocity;//

    // For Player Component
    public Animator animator;
    private bool isDance = true;
    private PhotonView pv;

    // For Other GameObjects
    private GameObject cameraObj;
    public GameObject playerMesh;

    // health bar ----------------------
    [SerializeField] private Image healthBar;       // hp 이미지
    private float maxHealth = 10000;                  // 최대 hp
    private float currentHealth;                    // 현재 hp
    [SerializeField] private Text currentHPText;    // 현재 hp값

    // weapon change ----------------------
    private WeaponController weaponController;
    public static int currentWeaponIndex;   // 현재 무기 인덱스 - static
    public static string weaponName;
    public Transform weaponPivot_R;         // 오른손 위치(도끼, 총) 
    public Transform weaponPivot_L;         // 왼손 위치(활)
    private int previousWeaponIndex;        // 이전 무기 인덱스(삭제시 필요)
    [SerializeField] private GameObject[] weapons;   // 무기 프리팹
    [SerializeField] private GameObject[] weaponEffects; // 무기 이펙트
    // 공격
    //[SerializeField] private GameObject bullet; // 총알 프리팹
    [SerializeField] private GameObject arrow;  // 화살 프리팹
    [SerializeField] private Transform arrowPoint;  // 화살 위치
    [SerializeField] private Transform effectPoint;
    [SerializeField] private Text playerName;

    public Transform aimPosition;
    GameObject currentTarget;
    public float distance = 3f;
    bool isAiming;

    [Header("Sound")]
    private AudioSource source = null;  //AudioSource 컴포넌트 저장할 레퍼런스 
    [SerializeField] private AudioClip arrowReadyCllip;
    [SerializeField] private AudioClip swordReadyCllip;
    [SerializeField] private AudioClip rifleReadyCllip;
    [SerializeField] private AudioClip arrowCllip;
    [SerializeField] private AudioClip swordCllip;
    [SerializeField] private AudioClip rifleCllip;


    //자신의 Transform 참조 변수  
    private Transform myTr;
    private Rigidbody myRigid;
    //위치 정보를 송수신할 때 사용할 변수 선언 및 초기값 설정 
    Vector3 currPos = Vector3.zero;
    Quaternion currRot = Quaternion.identity;

    //public bool arrowAttack = true;
    //public bool riffleAttack = true;
    //public bool swordAttack = true;

    private void Awake()
    {
        // reference 연결
        myTr = GetComponent<Transform>();
        animator = GetComponent<Animator>();    // animator
        myRigid = GetComponent<Rigidbody>();    // rigidbody
        pv = GetComponent<PhotonView>();
        cameraObj = Camera.main.gameObject;

        // sound
        source = GetComponent<AudioSource>();   //AudioSource 컴포넌트를 해당 변수에 할당
        arrowReadyCllip = Resources.Load<AudioClip>("Sound/Weapon/bow_ready");
        swordReadyCllip = Resources.Load<AudioClip>("Sound/Weapon/sword_ready");
        rifleReadyCllip = Resources.Load<AudioClip>("Sound/Weapon/rifle_ready");
        arrowCllip = Resources.Load<AudioClip>("Sound/Weapon/arrow");
        swordCllip = Resources.Load<AudioClip>("Sound/Weapon/sword");
        rifleCllip = Resources.Load<AudioClip>("Sound/Weapon/rifle");
        

        // 무기
        //bullet = (GameObject)Resources.Load("Weapon/Bullet", typeof(GameObject));
        arrow = (GameObject)Resources.Load("Weapon/Arrow", typeof(GameObject));
        weaponEffects[0] = (GameObject)Resources.Load("Weapon/SwordEffect", typeof(GameObject));
        weaponEffects[1] = (GameObject)Resources.Load("Weapon/RifleEffect", typeof(GameObject));
        weaponEffects[2] = (GameObject)Resources.Load("Weapon/ArrowEffect", typeof(GameObject));

        // weapon 지정해주기
        weapons[0] = weaponPivot_R.GetChild(1).gameObject;
        weapons[1] = weaponPivot_R.GetChild(0).gameObject;
        weapons[2] = weaponPivot_L.GetChild(0).gameObject;

        if (GameObject.Find("GameManager")!=null)
        {
            // weaponController = GameObject.Find("GameManager").transform.Find("WeaponController").GetComponent<WeaponController>();
            previousWeaponIndex = 3;
            currentWeaponIndex = 3;
            //weapons[currentWeaponIndex].SetActive(true);
        }

        // health bar 초기화
        currentHealth = maxHealth;          // hp 초기화

        //PhotonView Observed Components 속성에 PlayerCtrl(현재) 스크립트 Component를 연결
        pv.ObservedComponents[0] = this; // 중요함
        /*
         * PhotonView 컴포넌트의 Observe option 속성
         * 옵션                           설명
         * off                            실시간 데이터 송수신을 하지 않음.
         * ReliableDeltaCompressed        데이터를 정확히 송수신한다(TCP/IP 프로토콜)
         * Unreliable                     데이터의 정합성을 보장할 수 없지만 속도가 빠르다(UDP 프로토콜)
         * UnreliableOnChange             Unreliable 옵션과 같지만 변경사항이 발생했을 경우에만 전송한다
         */
        
        //데이타 전송 타입을 설정
        pv.synchronization = ViewSynchronization.UnreliableOnChange;
        
        // 원격 플래이어의 위치 및 회전 값을 처리할 변수의 초기값 설정 
        // 잘 생각해보자 이런처리 안하면 순간이동 현상을 목격
        currPos = myTr.position;
        currRot = myTr.rotation;

        playerName.text = PhotonNetwork.playerName.ToString();
    }

    private void Update()
    {
        currentHPText.text = currentHealth.ToString();

        // 게임종료 후 애니메이션
        if (WaveSpawner.isSuccess == true && isDance)
        {
            pv.RPC("HappyAnimation", PhotonTargets.All);
            isDance = false;
        }
        CheckTarget();
    }

    // FixedUpdate is called once per frame
    void FixedUpdate()
    {

        // 무기 교환
        if (pv.isMine && GameObject.Find("GameManager") != null)
            pv.RPC("weaponTest", PhotonTargets.All, currentWeaponIndex);

        // 이동
        if (Input.GetAxis("Vertical") != 0.0f || Input.GetAxis("Horizontal") != 0.0f)
        {
            MovePlayer(Input.GetAxis("Vertical"), Input.GetAxis("Horizontal"));
        }

        // 공격
        if(Input.GetKeyDown(KeyCode.F))
        {
            Attack();

            if (isAiming)
            {
                AutoAiming();
            }
        }

        
    }
    private void AutoAiming()
    {
        aimPosition.transform.LookAt(currentTarget.transform);
    }

    private void CheckTarget()
    {
        RaycastHit hit;

        Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward), Color.green, distance * 3);
        if (Physics.Raycast(transform.position, transform.forward, out hit, distance))
        {
            if (hit.transform.gameObject.tag == "Enemy")
            {
                if (!isAiming)
                    Debug.Log("target found^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^");

                currentTarget = hit.transform.gameObject;
                isAiming = true;
            }
            else
            {
                currentTarget = null;
                isAiming = false;
            }
        }
    }



    public void MovePlayer(float forward, float right)
    {
        if (pv.isMine)
        {
            // forward parameter mean virtical axis from keyboard , right parameter mean horizontal axis from keyboard
            // Get Vertical axis and horizontal axis and multiple to camera vertical axis and horizontal axis

            Vector3 translation;
            translation = forward * cameraObj.transform.forward;
            translation += right * cameraObj.transform.right;
            translation.y = 0;

            // input값이 입력되면 
            if (translation.magnitude > 0.2f)
            {
                velocity = translation;
            }
            else
            {
                velocity = Vector3.zero;    // 입력 안되면 속도 0
            }
            // 플레이어 이동 : Rigidbody 속도로 player 이동
            myRigid.velocity = new Vector3(velocity.normalized.x * moveSpeed, myRigid.velocity.y, velocity.normalized.z * moveSpeed);

            // 플레이어 회전
            if (velocity.magnitude > 0.2f)
            {
                transform.rotation = Quaternion.Lerp(playerMesh.transform.rotation, Quaternion.LookRotation(velocity), Time.deltaTime * rotationSpeed);
            }
            // Move Animation
            animator.SetFloat("Speed", velocity.magnitude * walkAnimationSpeed);
        }
    }

 
    [PunRPC]
    public void HappyAnimation()
    {
        if (pv.isMine)
        {
            animator.SetTrigger("Happy");
        }
    }


    // 공격  ****************************************************************************************************
    public void Attack()
    {
        if (pv.isMine)
        {
            // 애니메이션
            switch (currentWeaponIndex)
            {
                case 0:
                    animator.SetTrigger("FireSword");
                    source.PlayOneShot(swordCllip, swordCllip.length);
                    break;
                case 1:
                    StartCoroutine(FireRifleCoroutine());
                    break;
                case 2:
                    StartCoroutine(FireArrowCoroutine());
                    break;
            }

        }
    }

    [PunRPC]
    void FireSword()
    {
        
        animator.SetBool("AttackRifle", true);
    }
    IEnumerator FireRifleCoroutine()
    {
        animator.SetBool("AttackRifle", true);
        yield return new WaitForSeconds(0.1f);
        pv.RPC("FireRifle", PhotonTargets.All);
    }
    [PunRPC]
    void FireRifle()
    {
        source.PlayOneShot(rifleCllip, rifleCllip.length);
        animator.SetBool("AttackRifle", false);
    }
    IEnumerator FireArrowCoroutine()
    {
        animator.SetBool("AttackArrow", true);
        yield return new WaitForSeconds(0.3f);
        pv.RPC("FireArrow", PhotonTargets.All);
    }
    [PunRPC]
    void FireArrow()
    {
        // 화살 생성
        source.PlayOneShot(arrowCllip, arrowCllip.length);
        Instantiate(arrow, arrowPoint.position, arrowPoint.rotation);
        animator.SetBool("AttackArrow", false);
    }
    //  **********************************************************************************************************

    // 데미지 ****************************************************************************************************
    // 에네미와 접촉하면 hp 감소
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Enemy")
        {
            // enemy 데미지 저장
            int damage = collision.gameObject.GetComponent<EnemyLife>().attackDamage;
            pv.RPC("EnemyDamage", PhotonTargets.All, damage);
            StartCoroutine(Hit());
        }
        var item = collision.gameObject.GetComponent<GroundItem>();

        if (item)
        {
            if (item.item.type == ItemType.Gold)
            {

            }
            inventory.AddItem(new Item(item.item), 1);
            Destroy(collision.gameObject);
        }
    }
    [PunRPC]
    void EnemyDamage(int dam)
    {
        currentHealth -= dam;    //맞은 총알의 파워를 가져와 player의 life를 감소
        healthBar.fillAmount = currentHealth / maxHealth;
 
        
        //if (currentHealth > 0)
        //{
        //    // damage 보여주기
        //    var damage = Instantiate(damageText, damageTr.position, damageTr.rotation);
        //    damage.GetComponent<TextMesh>().text = "-" + dam.ToString();
        //}
        // 체력이 0이면 죽음
        if (currentHealth <= 0)
        {
            StartCoroutine(Dead());
        }
    }
    IEnumerator Hit()
    {
        if(pv.isMine)
        {
            // 애니메이션
            switch (currentWeaponIndex)
            {
                case 0:
                    animator.SetTrigger("SwordAttacked");
                    break;
                case 1:
                    animator.SetTrigger("RifleAttacked");
                    break;
                case 2:
                    animator.SetTrigger("ArrowAttacked");
                    break;
            }
        }
        // hit 애니메이션
        yield return new WaitForSeconds(0.5f);  //1.5초 후 실행
    }

    IEnumerator Dead() 
    {
        if (pv.isMine)
        {
            isDead = true;
            // 애니메이션
            switch(currentWeaponIndex)
            {
                case 0:
                    animator.SetTrigger("SwordDead");
                    break;
                case 1:
                    animator.SetTrigger("RifleDead");
                    break;
                case 2:
                    animator.SetTrigger("ArrowDead");
                    break;
            }
   
            //this.gameObject.tag = "Untagged";   // enemy가 추적 못하게 tag 변경
            //Enemy에 추가된 모든 Collider를 비활성화(모든 충돌체는 Collider를 상속했음 따라서 다음과 같이 추출 가능)
            //foreach (Collider coll in gameObject.GetComponentsInChildren<Collider>())
            //{
            //    coll.enabled = false;
            //}
        }
        yield return new WaitForSeconds(1.5f);  //1.5 초후 오브젝트 삭제

        PhotonNetwork.Destroy(this.gameObject);  // 자신과 네트워크상의 모든 아바타를 삭제
    }
    //  ************************************************************************************************************


    // 무기 교환 ****************************************************************************************************
    // UI 클릭시 해당하는 인덱스 받아와 static 변수에 념겨줌
    public void ChangeWeapon(int weaponIndex)
    {
        currentWeaponIndex = weaponIndex;
    }

    [PunRPC]
    void weaponTest(int weaponIndex)
    {
        // 처음 무기 선택시
        if (weaponIndex != previousWeaponIndex && previousWeaponIndex==3)
        {
            // 새로운 무기 장착
            weapons[weaponIndex].SetActive(true);
            // 무기 장착 애니메이션 함수 호출
            ChangeWeaponAnim(weaponIndex);
            // 무기 이펙트
            Instantiate(weaponEffects[weaponIndex], effectPoint.position, effectPoint.rotation);
            // 현재 무기 인덱스를 이전 무기 인덱스에 저장
            previousWeaponIndex = weaponIndex;
        } 
        else if (weaponIndex != previousWeaponIndex)// 다른 무기 선택시
        {
            // 기존 무기 삭제
            weapons[previousWeaponIndex].SetActive(false);
            // 새로운 무기 장착
            weapons[weaponIndex].SetActive(true);
            // 무기 장착 애니메이션 함수 호출
            ChangeWeaponAnim(weaponIndex);
            // 무기 이펙트
            Instantiate(weaponEffects[weaponIndex], effectPoint.position, effectPoint.rotation);
            // 현재 무기 인덱스를 이전 무기 인덱스에 저장
            previousWeaponIndex = weaponIndex;
        }
    }

    // 무기에 해당되는 애니메이션 활성화
    private void ChangeWeaponAnim(int weaponIndex)
    {
        switch (weaponIndex)
        {
            case 0: // sword
                source.PlayOneShot(swordReadyCllip, swordReadyCllip.length);
                animator.SetTrigger("Sword");
                animator.SetBool("ArrowReady", false);  // 다른 애니메이션 비활성화
                animator.SetBool("RifleReady", false);  // 다른 애니메이션 비활성화
                animator.SetBool("SwordReady", true);   // 현재 애니메이션 활성화
                break;
            case 1: // rifle
                source.PlayOneShot(rifleReadyCllip, rifleReadyCllip.length);
                animator.SetTrigger("Rifle");
                animator.SetBool("ArrowReady", false);
                animator.SetBool("SwordReady", false);
                animator.SetBool("RifleReady", true);
                break;
            case 2: // arrow
                source.PlayOneShot(arrowReadyCllip, arrowReadyCllip.length);
                animator.SetTrigger("Arrow");
                animator.SetBool("RifleReady", false);
                animator.SetBool("SwordReady", false);
                animator.SetBool("ArrowReady", true);
                break;
        }
    }
    //  ************************************************************************************************************

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, distance);
    }


    // photonview 안에 observedcomponents로 등록된 클래스가 변화할때 호출
    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //로컬 플레이어의 위치 정보를 송신 - we own this player : send the others our data
        if (stream.isWriting)
        {
            //박싱
            stream.SendNext(myTr.position);
            stream.SendNext(myTr.rotation);
        }
        //원격 플레이어의 위치 정보를 수신 - network player, receive data
        else
        {
            //언박싱
            currPos = (Vector3)stream.ReceiveNext();
            currRot = (Quaternion)stream.ReceiveNext();
        }
    }

}
