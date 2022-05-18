using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;


public class ChatManager : MonoBehaviour
{
    public Text[] playerNames;          // profile text ui
    public RawImage[] rawImages;     // profiel rawimage ui
    public RenderTexture[] profileImage;  // render texture
    [SerializeField] private Transform[] playerPos;      //플레어의 생성 위치 저장 레퍼런스
    [SerializeField] private Text playerCountText;

    public Button sendBtn;              // 채팅 보내는 버튼
    public InputField chatInput;        // 채팅창에 입력되는  텍스트
    public Text chatLog;                // 채팅이 입력되는 창
    public ScrollRect scrollRect;       // 채팅이 올라가는 창

    // PhotonView : 네트워크상에 접속한 플레이어 간의 데이터를 송수신하는 통신 모듈
    PhotonView pv;                      // RPC 호출을 위한 PhotonView 연결 레퍼런스
    

    void Awake()
    {
        //PhotonView 컴포넌트를 레퍼런스에 할당
        pv = GetComponent<PhotonView>();
        // 포톤 클라우드로부터 네트워크 메시지 수신을 다시 연결
        PhotonNetwork.isMessageQueueRunning = true;
    }

    // 다른 플레이어가 방에 접속했을 때 
    void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
    {
        int playerNum = PhotonNetwork.room.PlayerCount;
        playerCountText.text = "Players : " + playerNum.ToString() + "/" + PhotonNetwork.room.MaxPlayers.ToString();

        // 프로필용 캐릭터 스폰
        //PhotonNetwork.Instantiate("MyCharacter", playerPos[PhotonNetwork.room.PlayerCount - 1].position, playerPos[PhotonNetwork.room.PlayerCount - 1].rotation, 0);

        // 1. 프로필 업데이트
        foreach (PhotonPlayer p in PhotonNetwork.playerList)
        {
            // 방장 작성
            string name = PhotonNetwork.masterClient.NickName.ToString();
            pv.RPC("playerNameRPC", PhotonTargets.All, name, 0);

            if (p != PhotonNetwork.masterClient)
            {
                name = p.NickName.ToString();
                pv.RPC("playerNameRPC", PhotonTargets.All, name, PhotonNetwork.room.PlayerCount - 1);
            }
        }
        // 2. player 들어오면 채팅창에 알림
        pv.RPC("CharRPC", PhotonTargets.AllBuffered, "<color=yellow>" + newPlayer.NickName + " joined the game</color>");
    }

    // 방에 참가한 player이름 보여주는 RPC
    [PunRPC]
    public void playerNameRPC(string name, int numb)
    {
        //PhotonNetwork.Instantiate("MyCharacter", playerPos[numb].position, playerPos[numb].rotation, 0);
        playerNames[numb].text = name;
        rawImages[numb].texture = profileImage[numb];
    }

    // 채팅 메세지 보내는 버튼
    public void SendMsgBtnClick()
    {
        // 빈 텍스트 입력시 리턴
        if(chatInput.text.Equals(""))
        {
            return;
        }
        string msg = PhotonNetwork.playerName + " : " + chatInput.text; // 메세지 내용
        pv.RPC("CharRPC", PhotonTargets.AllBuffered, msg);              // 메세지 RPC 호출

        chatInput.ActivateInputField(); // 채팅 입력창 활성화
        chatInput.text = "";            // 채팅창 비워주기
    }

    // 메세지 보내는 RPC
    [PunRPC]
    public void CharRPC(string msg)
    {
        chatLog.text += "\n" + msg;                         
        scrollRect.verticalNormalizedPosition = 0.0f;
    }
}
