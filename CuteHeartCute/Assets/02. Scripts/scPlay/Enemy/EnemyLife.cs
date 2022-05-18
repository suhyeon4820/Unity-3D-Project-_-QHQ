using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class EnemyLife : MonoBehaviour
{
    [Header("Enemy")]
    private Transform myTr; //자신의 트렌스폼
    public EnemyController enemy; // EnemyCtrl 연결 레퍼런스
    [HideInInspector] public int attackDamage;

    [Header("HealthBar")]
    [SerializeField] private Image healthBar;
    private float currentHealth;
    private float maxHealth = 15f;


    // damage
    [SerializeField] private GameObject damageText;
    [SerializeField] private GameObject weaponDamageText;
    [SerializeField] private Transform damageTr;
    [SerializeField] private Text currentHPText;

    //포톤 추가
    // PhotonView 컴포넌트를 할당할 레퍼런스 
    public PhotonView pv = null;

    void Awake()
    {
        myTr = GetComponent<Transform>();   // 레퍼런스 할당
        pv = PhotonView.Get(this);          // PhotonView 컴포넌트 할당 (다른방식 연결)

        currentHealth = maxHealth;          // hp 초기화

        attackDamage = Random.Range(5, 8);  // enemy attack damage;
    }

    private void Update()
    {
        // 현재 체력 출력
        if (currentHealth <= 0)
            currentHPText.text = "";
        else
            currentHPText.text = currentHealth.ToString();
    }


    void OnCollisionEnter(Collision coll)
    {
        // 터렛이 발사하는 총알과 충돌할 경우
        if (coll.gameObject.tag == "Bullet")
        {
            // bullet의 대미지를 저장
            int pow = coll.gameObject.GetComponent<TurretBullet>().power;
            //(포톤 추가)모든 네트웍 유저의 몬스터에 RPC 데이타를 전송하며 RPC 함수를 호출, 로컬 플레이어는 로컬 Deamage 함수를 바로 호출 
            pv.RPC("TurretDeamage", PhotonTargets.All, pow);
            //몬스터 타격 루틴을 위한 호출
            //enemy.HitEnemy();
        }

        // 플레이어가 발사하는 화살과 충돌할 경우
        if (coll.gameObject.tag == "ARROW")
        {
            // Arrow의 대미지를 저장
            int pow = coll.gameObject.GetComponent<Arrow>().power;
            //(포톤 추가)모든 네트웍 유저의 몬스터에 RPC 데이타를 전송하며 RPC 함수를 호출, 로컬 플레이어는 로컬 Deamage 함수를 바로 호출 
            pv.RPC("PlayerDeamage", PhotonTargets.All, pow);
            //몬스터 타격 루틴을 위한 호출
            //enemy.HitEnemy();
        }

        // 마지막 home에 닿으면 사라짐 - 나중에 게임 오버됨
        if (coll.gameObject.tag == "Home")
        {
            pv.RPC("CollideWithHome", PhotonTargets.All);
            
        }

    }

    // 터렛에 공격 당했을 경우
    [PunRPC]
    void TurretDeamage(int dam)
    {
        currentHealth -= dam;    //맞은 총알의 파워를 가져와 Enemy의 life를 감소
        healthBar.fillAmount = currentHealth/maxHealth;

        if(currentHealth>0)
        {
            // damage 보여주기
            var damage = Instantiate(damageText, damageTr.position, damageTr.rotation);
            damage.GetComponent<TextMesh>().text = "-" + dam.ToString();
        }
        // 체력이 0이면 죽음
        if (currentHealth <= 0)
        {
            enemy.EnemyDie();
        }
    }

    // 플레이어에 공격 당했을 경우
    [PunRPC]
    void PlayerDeamage(int dam)
    {
        currentHealth -= 20;    //맞은 총알의 파워를 가져와 Enemy의 life를 감소
        healthBar.fillAmount = currentHealth / maxHealth;

        if (currentHealth > 0)
        {
            // damage 보여주기
            var damage = Instantiate(weaponDamageText, damageTr.position, damageTr.rotation);
            damage.GetComponent<TextMesh>().text = "-" + dam.ToString();
        }
        // 체력이 0이면 죽음
        if (currentHealth <= 0)
        {
            enemy.EnemyDie();
        }
    }
    // 마지막 홈에 접촉했을 경우
    [PunRPC]
    void CollideWithHome()
    {
        enemy.EnemyHomeDie();
    }

    // Photon ***********************************************************************************
    //자신을 파괴시킨 네트워크 베이스를 검색해서 스코어를 증가시켜주는 함수
    IEnumerator SaveKillCount(int firePlayerId)
    {
        //Base 태그로 지정된 모든 네트워크 베이스를 가져와 배열에 저장
        GameObject[] bases = GameObject.FindGameObjectsWithTag("Turret");

        // 전체 네트워크 베이스를 검색하여 총알의 주인을 찿아줌...
        foreach (GameObject _base in bases)
        {
            var baseCtrl = _base.GetComponent<TurretController>();
            //네트워크베이스의 playerId가 총알의 playerId와 동일한지 판단
            if (baseCtrl != null && baseCtrl.playerId == firePlayerId)
            {
                //동일한 베이스일 경우 스코어를 증가시켜줌
                baseCtrl.PlusKillCount();
                break;

            }
        }
        yield return null;
    }
}
