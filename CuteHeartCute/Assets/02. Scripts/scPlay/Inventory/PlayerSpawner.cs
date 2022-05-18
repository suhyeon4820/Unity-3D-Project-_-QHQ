using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerSpawner : MonoBehaviour
{
    //RPC 호출을 위한 PhotonView 연결 레퍼런스
    PhotonView pv;
    //플레어의 생성 위치 저장 레퍼런스
    public Transform[] playerPos;
    int playerNum = 0;
    private void Awake()
    {
        pv = GetComponent<PhotonView>();
        
    }
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(this.CreatePlayer());
    }


    // 포톤 추가
    // 플레이어를 생성하는 함수
    IEnumerator CreatePlayer()
    {
        PhotonNetwork.Instantiate("Player2", new Vector3(0, 2.3f,2.3f), playerPos[playerNum].rotation, 0);
        playerNum++;

        Debug.Log("character spawn");

        //foreach (PhotonPlayer p in PhotonNetwork.playerList)
        //{
        //    PhotonNetwork.Instantiate("Player2", playerPos[playerNum].position, playerPos[playerNum].rotation, 0);
        //    playerNum++;
        //}

        //PhotonNetwork.Instantiate("Player2", playerPos[PhotonNetwork.room.PlayerCount-1].position, playerPos[PhotonNetwork.room.PlayerCount-1].rotation, 0);

        yield return null;
    }

}
