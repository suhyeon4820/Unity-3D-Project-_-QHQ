// ********************************
//  [class-name]
//
//  [class-description]
//
//  [project-name]
//  2016 - Larissa Redeker
//  Kittens and Elves at Work
//  http://www.keawstudio.com
//
// ********************************

using UnityEngine;
using System.Collections.Generic;
using System;

public class CameraPoints : MonoBehaviour {

	[Serializable]
	public class CamPoint {
		public Transform camAnchor; // camera 위치 정보
		public float FOV = 5f;
	}

	// 위의 클래스를 list로 생성
	[SerializeField]
	public List<CamPoint> cameraPoints = new List<CamPoint>();

	public Camera cam;

	public void SetPoint(int indexPoint){
		// 등록된 위치값 가져옴
		Vector3 finalPos = cameraPoints[indexPoint].camAnchor.position;
		// 메인카메라에 list에 등록된 위치와 fieldfview값 전달
		cam.transform.position = finalPos;
		cam.fieldOfView = cameraPoints[indexPoint].FOV;
	}

}
