using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using Photon.Realtime;

using UnityEngine.SceneManagement;


/* 포톤 클라우드의 주요 흐름
 * 1. 서버접속
 * 2. 로비접속
 * 3. 방으로 접근
 * 4. 방으로 접근시도후 실패시 방만듬
 * 5. 방입장
 * 6. "방장"의 명령에 따라 Object들을 Sync
*/

public class NetworkManager : MonoBehaviour
{
    [Header("Sound")]
    public int stage = 0;
    private SoundManager _sMgr;

    static bool isFirstTime = true;
    [Header("Menus")]
    [SerializeField] private GameObject[] menus;

    [Header("Login")]
    [SerializeField] private InputField nickNameInput;

    //[Header("CharacterSelect")]
 
    [Header("Lobby")]
    [SerializeField] private Text welcomeText;
    [SerializeField] private Text currentCountText;
    [SerializeField] private GameObject createRoomPnl;
    [SerializeField] private InputField searchRoomName;
    public GameObject scrollContents;   // RoomItem이 차일드로 생성될 Parent 객체의 레퍼런스 
    public GameObject roomItem;         // 룸 목록만큼 생성될 RoomItem 프리팹 연결 레퍼런스

    public GameObject memberlogin;
    public bool ismembercheck;

    [Header("CreatePnl")]
    public InputField roonNameInput;
    [SerializeField] private InputField roomPasswordInput;
    [SerializeField] private Toggle singlePlay;
    private bool isSinglePlay = false;

    [SerializeField] private Toggle multiPlay;

    [Header("Room")]
    [SerializeField] private Text roomNameText;          // 방 이름
    [SerializeField] private Text playerCountText;       // 방 인원수
    [SerializeField] private Text masterPlayerNameText;  // 플로필 이름
    [SerializeField] private RawImage masterPlayerImage; // 프로필 사진
    [SerializeField] private RenderTexture profileImage; // 입력할 이미지
    [SerializeField] private Transform[] playerPos;
    private GameObject[] Players;
    [SerializeField] private Text startBtnText;          // 다음으로 넘어가는 버튼

    //[Header("Room")]

    // photon 
    PhotonView pv;
    string version = "ver 0.1.0";    // photon version
    public PhotonLogLevel LogLevel = PhotonLogLevel.Full;   //예상하는 대로 동작하는 것에 대하여 확신이 서면 로그 레벨을 Informational 으로 변경 하자.

    private void Awake()
    {
        _sMgr = GameObject.Find("SoundManager").GetComponent<SoundManager>();

        //PhotonView 컴포넌트를 레퍼런스에 할당
        pv = GetComponent<PhotonView>();

        // 마스터 클라이언트와 일반 클라이언트들이 레벨을 동기화할지 결정한다.
        // 이때 방의 모든 클라이언트가 마스터 클라이언트와 동일한 레벨을 자동으로 로드함. 즉 true시 레벨 동기화.
        PhotonNetwork.automaticallySyncScene = true;
        
        if (isFirstTime)
        {
            Screen.SetResolution(960, 540, false);
            //Screen.SetResolution(1920, 1080, false);
        }
        else
        {
            // 바로 로비로 접속
            menus[0].SetActive(false);  // login창 비활성화
            menus[4].SetActive(false);  // login 오브젝트 비활성화

            menus[2].SetActive(true);   // lobby창 활성화
            Debug.Log("비활성화3");     // 로비에 접속 - Room에 참가하려면 로비에 접속된 상태여야 함  
        }
    }

    private void Start()
    {
        _sMgr.PlayBackground(stage);
    }

    void Update()
    {
        // 현재 접속한 인원 수 출력
        currentCountText.text = "Players : " + PhotonNetwork.countOfPlayers;
        ismembercheck = memberlogin.GetComponent<csMemberLogin>().logincheck;
    }

    // ***********************************************************************************
    // Login Window - SignIn 버튼 관련은 ButtonManager.cs에 있음
    // ***********************************************************************************

    // Photon 서버 연결 - login 버튼 클릭시 실행 + 기존창 비활성화 + 다음창 활성화

    public void LoginButtonClick()
    {
        StartCoroutine("LoginButton");
    }
   IEnumerator LoginButton()
    {
        yield return new WaitForSeconds(0.3f);

        if (ismembercheck == true)
        {
            Debug.Log("창전환");

            // 창 전환
            menus[0].SetActive(false);  // login창 비활성화
            menus[4].SetActive(false);  // login 오브젝트 비활성화
            menus[6].SetActive(true);   // 로딩창 활성화

            // 2. Photon 서버 연결
            // 룸 -> 로비로 씬 전환시 반복해서 다시 포톤 클라우드 서버로 접속하는 로직이 다시 실행되는것을 막음
            if (!PhotonNetwork.connected)
            {
                // 버전 정보를 전달하며 포톤 클라우드에 접속 
                PhotonNetwork.ConnectUsingSettings(version);

                //예상하는 대로 동작하는 것에 대하여 확신이 서면 로그 레벨을 Informational 으로 변경 하자.
                //PhotonNetwork.logLevel = LogLevel;

                //현재 클라이언트 유저의 이름을 포톤에 설정 - PhotonView 컴포넌트의 요소 Owner의 값이 된다.
                PhotonNetwork.playerName = nickNameInput.text;
                Debug.Log("Server Connedted");
            }
        }  
    }
    // 마스터 서버 접속시 강제 실행
    void OnConnectedToMaster()
    {
        if (isFirstTime)
        {
            // 창 전환
            menus[6].SetActive(false);  // 로딩창 비활성화
            menus[1].SetActive(true);   // characterSelect창 활성화
            menus[5].SetActive(true);   // characterobject 비활성화
        }
        else
        {
            PhotonNetwork.JoinLobby();
            //OnJoinedLobby();
        }
    }

    // ***********************************************************************************
    // CharacterSelect Window
    // ***********************************************************************************

    // Photon 로비 연결 + 캐릭터 Resources 폴더에 저장 + 창 전환
    public void CharacterSelectButtonClick()
    {
        if(CustomizationManager.iscustomEnd == true)
        {
            // 창 전환
            menus[1].SetActive(false);  // characterSelect창 비활성화
            menus[5].SetActive(false);  // character object 비활성화
            menus[2].SetActive(true);   // lobby창 활성화

            // 로비에 접속 - Room에 참가하려면 로비에 접속된 상태여야 함  
            PhotonNetwork.JoinLobby();
        }
    }

    // OnJoinedLobby : Photon 서버의 로비에 입장 했을 때 호출 
    void OnJoinedLobby()
    {
        isFirstTime = false;
        Debug.Log("Joined the lobby");
        
        welcomeText.text = PhotonNetwork.playerName + " Welcome ♥"; // welcomeText 값 전달

    }

    // ***********************************************************************************
    // Lobby Window - MakeRoom 버튼 관련은 ButtonManager.cs에 있음
    // ***********************************************************************************
    public void MakeRoomCencelClick()
    {
        createRoomPnl.SetActive(false);
    }

    public void MakeRoomBtnClick()
    {
        createRoomPnl.SetActive(true);
    }

    public void CreateRoomBtnClick()
    {
        createRoomPnl.SetActive(false); // createRoomPnl

        // 1. 방 이름 설정
        string _roonName = roonNameInput.text;
        // 입력된 값이 없으면
        if (string.IsNullOrEmpty(roonNameInput.text))
        {
            // 방이름 랜덤으로 생성
            _roonName = "ROOM_" + Random.Range(0, 999).ToString("000");
        }
       
        // 2. 방옵션 설정
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.IsOpen = true;      // 입장 가능 여부
        roomOptions.IsVisible = true;   // 로비에 보이는지 여부
        
        if(isSinglePlay == true)
        {
           roomOptions.MaxPlayers = 1;
        }
        else
        {
            roomOptions.MaxPlayers = 4;
        }
        // 방장 버튼 Text - Start
        startBtnText.text = "Start";

        // 3. 방 생성
        PhotonNetwork.CreateRoom(_roonName, roomOptions, TypedLobby.Default);  
    }

    public void RandomRoomBtnClick()
    {
        // 참가자 버튼 Text - Ready
        startBtnText.text = "Ready";
        // 방이 있으면 랜덤 방에 참가
        PhotonNetwork.JoinRandomRoom();
    }

    public void SearchRoomBtnClick()
    {
        // 방 번호로 찾기 - 안에 내용 변경해주기
        OnClickRoomItem(searchRoomName.text);
    }

    // Room 생성시 Room 리스트 업데이트
    void OnReceivedRoomListUpdate()
    {
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("ROOM_ITEM"))
        {
            Destroy(obj);   // room 리스트 업데이트 될 때마다 모든 룸 삭제
        }
        int rowCount = 0;
        scrollContents.GetComponent<RectTransform>().sizeDelta = Vector2.zero;
        
        foreach (RoomInfo _room in PhotonNetwork.GetRoomList())
        {
            Debug.Log(_room.Name);
            GameObject room = (GameObject)Instantiate(roomItem);
            room.transform.SetParent(scrollContents.transform, false);

            RoomData roomData = room.GetComponent<RoomData>();
            roomData.roomName = _room.Name;
            roomData.connectPlayer = _room.playerCount;
            roomData.maxPlayers = _room.maxPlayers;

            roomData.DisplayRoomData();

            // roomButton 클릭시 OnClickRoomItem 함수 호출
            roomData.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(delegate
            {
                OnClickRoomItem(roomData.roomName);
                Debug.Log("Room Click " + roomData.roomName);
            });

            scrollContents.GetComponent<GridLayoutGroup>().constraintCount = ++rowCount;
            scrollContents.GetComponent<RectTransform>().sizeDelta += new Vector2(0, 120);
        }
    }
    void OnClickRoomItem(string roonName)
    {
        // 참가자 버튼 Text - Ready
        startBtnText.text = "Ready";
        PhotonNetwork.JoinRoom(roonName);
    }

    // 싱글 플레이 토글
    public void Single(bool _bool)
    {
        // 싱글, 멀티 둘 다 체크되어 있으면
        if (multiPlay.isOn && singlePlay.isOn)
        {
            // 멀티 토글 비활성화
            multiPlay.isOn = false;
        }
        isSinglePlay = true;
    }
    // 멀티 플레이 토글
     public void Multi(bool _bool)
    {
        // 싱글, 멀티 둘 다 체크되어 있으면
        if (singlePlay.isOn && multiPlay.isOn)
        {
            // 싱글 토글 비활성화
            singlePlay.isOn = false;
        }
        isSinglePlay = false;
    }

    // 같은 이름의 방 생성시 방만들기 실패
    public void OnCreateRoomFailed(short returnCode, string message)
    {
        MakeRoomBtnClick();
    }
    // 랜덤 방이 없을 때 실행
    public void OnJoinRandomFailed(short returnCode, string message)
    {
        //MakeRoomBtnClick();
    }

    public void OnJoinRoomFailed(short returnCode, string message)
    {
        //MakeRoomBtnClick();
    }

    // ***********************************************************************************
    // Room Window
    // ***********************************************************************************

    // OnJoinedRoom - 방에 성공적으로 입장했을 경우 실행되는 메소드
    public void OnJoinedRoom()
    {
        // 창전환
        menus[2].SetActive(false);  // lobby창 비활성화
        menus[3].SetActive(true);   // room창 활성화

        // 방이름, 정원수(현재 정원 수 / 최대 정원 수) 출력
        roomNameText.text = PhotonNetwork.room.Name;
        int playerNum = PhotonNetwork.room.PlayerCount;
        playerCountText.text = "Players : " + playerNum.ToString() + "/" + PhotonNetwork.room.MaxPlayers.ToString();

        // 방장 프로필 정해주기
        if (PhotonNetwork.isMasterClient)
        {
            // 프로필용 캐릭터 스폰
            PhotonNetwork.Instantiate("Player2", playerPos[0].position, playerPos[0].rotation, 0);
            masterPlayerNameText.text = PhotonNetwork.playerName;   // 이름
            masterPlayerImage.texture = profileImage;               // 이미지
        }
        else 
        {
            PhotonNetwork.Instantiate("Player2", playerPos[playerNum - 1].position, playerPos[playerNum - 1].rotation, 0);
        }
    }


    public void GameStartBtnClick()
    {
        // 방장이면 다음씬으로 가고
        if (PhotonNetwork.isMasterClient)
        {
            // 다음 씬으로 전환하는 코루틴 실행 (UI 버전에서 사용)
            StartCoroutine(this.LoadStage());
        }
        else
        {
            // 버튼 ready를 check로 변경
            startBtnText.text = "Check";
        }
    }

    //다음 씬으로 이동하는 코루틴 함수 (UI 버전에서 사용)
    IEnumerator LoadStage()
    {
        //씬을 전환하는 동안 포톤 클라우드 서버로부터 네트워크 메시지 수신 중단
        //(Instantiate, RPC 메시지 그리고 모든 네트워크 이벤트를 안받음 )
        //차후 전환된 scene의 초기화 설정 작업이 완료후 이 속성을 true로 변경
        PhotonNetwork.isMessageQueueRunning = false;

        // 백그라운드로 씬 로딩
        //AsyncOperation ao = SceneManager.LoadSceneAsync("scPlay");
        PhotonNetwork.LoadLevel("scPlay");

       
        // 씬 로딩이 완료 될때까기 대기...
        yield return null;
        //yield return ao;
    }
}