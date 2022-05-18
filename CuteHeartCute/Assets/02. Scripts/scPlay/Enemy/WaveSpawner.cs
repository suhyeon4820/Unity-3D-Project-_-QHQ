using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class WaveSpawner : MonoBehaviour
{
    
    public Transform spawnPoint;    // 생성되는 곳

    public float timeBetweenWaves = 10f; // 웨이브 간격
    private float countdown = 5f;            // 첫 시작 카운트다운

    public Text waveCountdownText;

    private int monsterNumb = 1;
    private int waveIndex = 0; // 웨이브 수
    private int lastWave = 3;   // 마지막 웨이브 수
    private int waveNumb = 3;   // 웨이브 카운트
    public static bool isSuccess = false;  // 게임 성공 변수

    // 스테이지 Enemy들을 위한 레퍼런스
    private GameObject[] Enemys;

    // 웨이브 정보 텍스트
    public GameObject waveInform;

    // sound
    private AudioSource source = null;  //AudioSource 컴포넌트 저장할 레퍼런스 
    [SerializeField] private AudioClip enemyShowClip;   //총탄의 발사 사운드 

    PhotonView pv = null;

    private void Awake()
    {
        source = GetComponent<AudioSource>();   //AudioSource 컴포넌트를 해당 변수에 할당
        //케논 발사 사운드 파일을 Resources 폴더에서 불러와 레퍼런스에 할당 ()
        enemyShowClip = Resources.Load<AudioClip>("Sound/enemy/enemyShow");  // 
    }

    void Update()
    {
        if(StageManager.gamePlay == true)
        {
            // 다음 웨이브 카운트다운 - 마지막 웨이브까지만 생성됨
            if (countdown <= 0f && waveIndex < lastWave)
            {
                // spawn info 보여주기
                ShowSpawnInform();
                // 에네미 생성
                StartCoroutine(SpawnWave());
                countdown = timeBetweenWaves;
            }
            countdown -= Time.deltaTime;

            if (countdown >= 0)
            {
                // 값을 화면에 표시해줌
                waveCountdownText.text = Mathf.Round(countdown).ToString();
            }

            // 스테이지 총 몬스터 객수 제한을 위하여 찾자~
            Enemys = GameObject.FindGameObjectsWithTag("Enemy");
            // 마지막 웨이브에 에네미가 모두 없으면 게임 성공
            if (waveNumb == 0 && Enemys.Length == 0 && EnemyController.isDie == false)
            {
                isSuccess = true;
            }
        }
        else
        {
            Enemys = GameObject.FindGameObjectsWithTag("Enemy");
            for (int i = 0; i < Enemys.Length; i++)
            {
                Enemys[i].gameObject.tag = "Untagged";
            }
        }
    }
   
    IEnumerator SpawnWave()
    {
        // waveIndex로 웨이브마다 몬스터 수 증가
        for (int i = 0; i < monsterNumb; i++)
        {
            SpawnEnemy();
            // 0.5초 단위로 생성
            yield return new WaitForSeconds(1f);
        }
        monsterNumb++;
        waveIndex++;
        waveNumb--;
    }

    void ShowSpawnInform()
    {
        
        //var damage = Instantiate(waveInform);
        //damage.GetComponent<TextMesh>().text = "-" + dam.ToString();
    }
    void SpawnEnemy()
    {
        // 에네미 생성
        if(PhotonNetwork.isMasterClient)
            PhotonNetwork.InstantiateSceneObject("Enemy", spawnPoint.position, spawnPoint.rotation, 0, null);
        
        // sound
        source.PlayOneShot(enemyShowClip, enemyShowClip.length + 0.2f);
    }

}
