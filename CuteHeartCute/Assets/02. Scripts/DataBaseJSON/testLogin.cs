using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

public class testLogin : MonoBehaviour
{
	// post 방식(get 방식 X)
	private void Start()
	{
		//StartCoroutine(Login("test", "test1"));
		StartCoroutine(RegisterUser("test1", "test2"));
	}

	IEnumerator Login(string username, string password)
    {
		string serverPath = "http://bananacco.dothome.co.kr/LoadMySQL.php"; //PHP 파일의 위치를 저장
		WWWForm form = new WWWForm();
		form.AddField("id", username);
		form.AddField("pass", password);

		using(UnityWebRequest www = UnityWebRequest.Post(serverPath, form))
        {
			yield return www.SendWebRequest();
			if(www.isNetworkError || www.isHttpError)
            {
				Debug.Log(www.error);
            }
			else
            {
				Debug.Log(www.downloadHandler.text);
            }
        }
	}
	IEnumerator RegisterUser(string username, string password)
	{
		string serverPath = "http://bananacco.dothome.co.kr/join.php"; //PHP 파일의 위치를 저장
		WWWForm form = new WWWForm();
		form.AddField("id", username);
		form.AddField("pass", password);

		using (UnityWebRequest www = UnityWebRequest.Post(serverPath, form))
		{
			yield return www.SendWebRequest();
			if (www.isNetworkError || www.isHttpError)
			{
				Debug.Log(www.error);
			}
			else
			{
				//Debug.Log(www.downloadHandler.text);
			}
		}
	}

	private IEnumerator GetMySQLData()
	{
		string serverPath = "http://bananacco.dothome.co.kr/LoadMySQL.php"; //PHP 파일의 위치를 저장

		WWWForm form = new WWWForm(); //Post 방식으로 넘겨줄 데이터(AddField로 넘겨줄 수 있음)

		using (UnityWebRequest webRequest = UnityWebRequest.Post(serverPath, form)) //웹 서버에 요청
		{
			yield return webRequest.SendWebRequest(); //요청이 끝날 때까지 대기

			Debug.Log(webRequest.downloadHandler.text); //서버로부터 받은 데이터를 string 형태로 출력
		}
	}

	//public string inputUserName;
	//public string inputPassword;

	//string LoginURL = "http://bananacco.dothome.co.kr/logintest.php";

	//// Update is called once per frame
	//void Update()
	//{
	//	if (Input.GetKeyDown(KeyCode.Space))
	//		StartCoroutine(LoginToDB(inputUserName, inputPassword));
	//}

	//IEnumerator LoginToDB(string username, string password)
	//{
	//	WWWForm form = new WWWForm();
	//	form.AddField("usernamePost", username);
	//	form.AddField("passwordPost", password);

	//	WWW www = new WWW(LoginURL, form);

	//	yield return www;
	//	Debug.Log(www.text);

	//}
}
