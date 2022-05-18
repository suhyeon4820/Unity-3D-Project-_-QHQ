using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class StageManager : MonoBehaviour
{
    // 포톤 추가////////////////////////////////////////////////
    //public Text txtConnect;  //접속된 플레이어 수를 표시할 Text UI 항목 연결 레퍼런스 (Text 컴포넌트 연결 레퍼런스)
    //public Text txtLogMsg;  //접속 로그를 표시할 Text UI 항목 연결 레퍼런스 선언
    [Header("Photon & Etc")]
    
    PhotonView pv;          //RPC 호출을 위한 PhotonView 연결 레퍼런스
    [SerializeField] private GameObject gameOverwindow;
    private bool windowClosed = false;
    public static bool gamePlay = false;
    [SerializeField] private GameObject endEffect;

    [Header("Player")]
    [SerializeField] public Transform[] playerPos;  //플레어의 생성 위치 저장 레퍼런스
    // camera follow
    private GameObject playerObj;
    private GameObject cameraObj;
    public Vector3 offset;
    private int playerNum;


    [Header("Enemy")]
    //public Transform[] EnemySpawnPoints;   // enemy 스폰 장소
    //private GameObject[] Enemys;    // 스테이지 Enemy들을 위한 레퍼런스
    [SerializeField] private GameObject Enemy;  //Enemy 프리팹을 위한 레퍼런스

    [Header("Sound")]
    public int stage = 1;
    private SoundManager _sMgr;
    private AudioSource source = null;  //AudioSource 컴포넌트 저장할 레퍼런스 
    [SerializeField] private AudioClip playerShowClip;   // 시작 사운드 
    [SerializeField] private AudioClip windClip1;
    [SerializeField] private AudioClip windClip2;
    [SerializeField] private AudioClip faileClip;
    //[Header("Turret")]
    //public TurretController turretStart;

    void Awake()
    {
    
        // sound
        _sMgr = GameObject.Find("SoundManager").GetComponent<SoundManager>();
        source = GetComponent<AudioSource>();   //AudioSource 컴포넌트를 해당 변수에 할당
        playerShowClip = Resources.Load<AudioClip>("Sound/effect/start");  

        // Photon
        pv = GetComponent<PhotonView>();    //PhotonView 컴포넌트를 레퍼런스에 할당

        //포톤 클라우드로부터 네트워크 메시지 수신을 다시 연결
        PhotonNetwork.isMessageQueueRunning = true;

        EnemyController.isDie = false;
    }

    
    // Use this for initialization
    IEnumerator Start()
    {
        _sMgr.PlayBackground(stage);

        //룸에 있는 네트워크 객체 간의 통신이 완료될 때까지 잠시 대기
        yield return new WaitForSeconds(1.0f);

        yield return new WaitForSeconds(2.0f); // 5초 대기
        
        StartCoroutine(this.CreatePlayer());
        source.PlayOneShot(playerShowClip, playerShowClip.length + 0.2f); // sound

        gamePlay = true;

    }


    private void Update()
    {
        // 에네미가 home에 닿으면 게임 종료
        if (EnemyController.isDie == true && !windowClosed)
        {
            //EnemyController.isDie = false;
            windowClosed = true;
            gamePlay = false;
            GameObject.Find("SoundManager").GetComponent<AudioSource>().Stop();
            source.PlayOneShot(faileClip, faileClip.length); // sound
            StartCoroutine(ExitRoom());
        }
        // 게임 성공하면
        if(WaveSpawner.isSuccess )
        {
            EnemyController.isDie = false;
            windowClosed = true;
            endEffect.SetActive(true);
            GameObject.Find("SoundManager").GetComponent<AudioSource>().Stop();
            //source.PlayOneShot(windClip1, windClip1.length); // sound
            source.PlayOneShot(windClip2, windClip2.length); // sound
            StartCoroutine(SuccessGame());
        }
    }

    void CameraRotation()
    {
        cameraObj = Camera.main.gameObject;
    }

    private void FixedUpdate()
    {
        if (playerObj != null && cameraObj != null)
        {
            // 게임 실행 중에 카메라 플레이어 따라다님
            if (!WaveSpawner.isSuccess)
            {
                cameraObj.transform.position = playerObj.transform.position - offset;
            }
            else
            {
                // 게임 성공하면 플레이어 주변으로 카메라 회전
                cameraObj.transform.LookAt(playerObj.transform);
                cameraObj.transform.Translate(Vector3.right * Time.deltaTime * 3);
            }
        }
    }

    IEnumerator ExitRoom()
    {
        // gameover창 활성화
        gameOverwindow.SetActive(true);
        yield return new WaitForSeconds(3f);    // 3초 뒤에 넘어감
        
        PhotonNetwork.LeaveRoom();              // 방 나가기
        //SceneManager.LoadScene("scLogin");      // 로비 창으로 이동
        PhotonNetwork.LoadLevel("scLogin");
    }

    IEnumerator SuccessGame()
    { 
        yield return new WaitForSeconds(4f);    // 3초 뒤에 넘어감
        WaveSpawner.isSuccess = false;
        PhotonNetwork.LeaveRoom();              // 방 나가기
        
        //SceneManager.LoadScene("scLogin");      // 로비 창으로 이동
        PhotonNetwork.LoadLevel("scLogin");
    }
    //// leaveroom 나오면 호출
    //void OnLeftRoom()
    //{
    //    PhotonNetwork.LoadLevel("scLogin");
    //}

    // 포톤 추가
    // 플레이어를 생성하는 함수
    IEnumerator CreatePlayer()
    {

        //현재 입장한 룸 정보를 받아옴(레퍼런스 연결)
        Room currRoom = PhotonNetwork.room;
        //GameObject player = PhotonNetwork.Instantiate("Player2", playerPos[0].position, playerPos[0].rotation, 0);
        //foreach (PhotonPlayer p in PhotonNetwork.playerList)
        //int num = currRoom.PlayerCount;

        GameObject player = PhotonNetwork.Instantiate("Player2", playerPos[playerNum].position, playerPos[playerNum].rotation, 0);
        playerNum++;

        // camera
        playerObj = player;
        cameraObj = Camera.main.gameObject;
        cameraObj.transform.position = playerObj.transform.position;

        yield return null;
    }

    //포톤 추가
    //룸 접속자 정보를 조회하는 함수
    //void GetConnectPlayerCount()
    //{
    //    //현재 입장한 룸 정보를 받아옴(레퍼런스 연결)
    //    Room currRoom = PhotonNetwork.room;

    //    //현재 룸의 접속자 수와 최대 접속 가능한 수를 문자열로 구성한 다음 Text UI 항목에 출력
    //    //txtConnect.text = currRoom.PlayerCount.ToString()
    //    //                    + "/"
    //    //                    + currRoom.MaxPlayers.ToString();
    //}

    //포톤 추가
    //네트워크 플레이어가 룸으로 접속했을 때 호출되는 콜백 함수
    //PhotonPlayer 클래스 타입의 파라미터가 전달(서버에서...)
    //PhotonPlayer 파라미터는 해당 네트워크 플레이어의 정보를 담고 있다.
    //void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
    //{

    //    // 플레이어 ID (접속 순번), 이름, 커스텀 속성
    //    Debug.Log(newPlayer.ToStringFull());
    //    // 룸에 현재 접속자 정보를 display
    //    //GetConnectPlayerCount();

    //}

    // 포톤 추가
    //네트워크 플레이어가 룸을 나가거나 접속이 끊어졌을 경우 호출되는 콜백 함수
    //void OnPhotonPlayerDisconnected(PhotonPlayer outPlayer)
    //{
    //    // 룸에 현재 접속자 정보를 display
    //    //GetConnectPlayerCount();

    //}

    // 포톤 추가
    //[PunRPC]
    //void LogMsg(string msg)
    //{
    //    //로그 메시지 Text UI에 텍스트를 누적시켜 표시
    //    //txtLogMsg.text = txtLogMsg.text + msg;
    //}

    // 포톤 추가
    // 룸 나가기 버튼 클릭 이벤트에 연결될 함수
    //public void OnClickExitRoom()
    //{

    //    // 로그 메시지에 출력할 문자열 생성
    //    string msg = "\n\t<color=#ff0000>["
    //                + PhotonNetwork.player.NickName
    //                + "] Disconnected</color>";

    //    //RPC 함수 호출
    //    pv.RPC("LogMsg", PhotonTargets.AllBuffered, msg);

    //    //현재 룸을 빠져나가며 생성한 모든 네트워크 객체를 삭제
    //    PhotonNetwork.LeaveRoom();

    //    //(!) 서버에 통보한 후 룸에서 나가려는 클라이언트가 생성한 모든 네트워크 객체및 RPC를 제거하는 과정 진행(포톤 서버에서 진행)
    //}

    // 포톤 추가
    //룸에서 접속 종료됐을 때 호출되는 콜백 함수 ( (!) 과정 후 포톤이 호출 )
    //void OnLeftRoom()
    //{
    //    // 로비로 이동
    //    SceneManager.LoadScene("scNetLobby");
    //}

    /////////////////////////////////////////////////////////////////////////////
}
