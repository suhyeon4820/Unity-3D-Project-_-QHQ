﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomData : MonoBehaviour
{
    // 외부 접근을 위해 public으로 선언했지만 Inspector에 노출하고 싶지 않을 때 사용
    [HideInInspector] public string roomName = "";    // 방 이름
    [HideInInspector] public int connectPlayer = 0;  // 현재 접속 유저수
    [HideInInspector] public int maxPlayers = 0;      // 룸의 최대 접속자수


    public Text textRoomName;       //룸 이름 표시할 Text UI 항목 연결 레퍼런스
    public Text textConnectInfo;    //룸 최대 접속자 수와 현재 접속자 수를 표시할 Text UI 항목 연결 레퍼런스

    
    //룸 정보를 전달한 후 Text UI 항목에 룸 정보를 표시하는 함수 
    public void DisplayRoomData()
    {
        textRoomName.text = roomName;
        textConnectInfo.text = "(" + connectPlayer.ToString() + "/" + maxPlayers.ToString() + ")";
    }
}
