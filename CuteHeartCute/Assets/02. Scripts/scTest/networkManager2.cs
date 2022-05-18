using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;

public class networkManager2 : MonoBehaviour
{
    // camera follow
    private GameObject playerObj;
    private GameObject cameraObj;
    public Vector3 offset;
    //플레어의 생성 위치 저장 레퍼런스
    public Transform[] playerPos;

    string version = "ver 0.1.0";    // photon version
    void Awake()
    {
        Screen.SetResolution(960, 540, false);
        PhotonNetwork.ConnectUsingSettings(version);
    }

    private void FixedUpdate()
    {
        // for camera movement
        if (playerObj != null && cameraObj != null)
        {
            if(!WaveSpawner.isSuccess)
            {
                cameraObj.transform.position = playerObj.transform.position - offset;
            }
            else
            {
                cameraObj.transform.LookAt(playerObj.transform);
                cameraObj.transform.Translate(Vector3.right * Time.deltaTime*3);
            }
        }
    }

    void OnConnectedToMaster()
    {
        PhotonNetwork.JoinOrCreateRoom("Room", new RoomOptions { MaxPlayers = 6 }, null);
    }

    public void OnJoinedRoom()
    {
        
        Debug.Log("join the room");

        GameObject o = PhotonNetwork.Instantiate("Player2", playerPos[0].position, playerPos[0].rotation, 0);
        //PhotonNetwork.Instantiate("Player2", playerPos[0].position, playerPos[0].rotation, 0);

        // camera
        playerObj = o;
        cameraObj = Camera.main.gameObject;
        cameraObj.transform.position = playerObj.transform.position;
    }
}
