using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SimpleJSON;
using UnityEngine.Networking;

public class csMemberLogin : MonoBehaviour
{  
    public bool logincheck = false;
    public InputField inputId;
    public InputField inputPass;
    public Text txtLog;

    public string id;
    public string pass;
    string phpURL = "http://bananacco.dothome.co.kr/login.php";   //PHP 파일의 위치를 저장
    string jsonURL = "http://bananacco.dothome.co.kr/login.json";
    //string phpURL = "http://dabiny.dothome.co.kr/login.php";
    //string jsonURL = "http://dabiny.dothome.co.kr/login.json";

    private void Update()
    {
        pass = inputPass.text;
        id = inputId.text;
    }
    // login button과 연결
    public void StartCoroutineLoginMember()
    {
        StartCoroutine("LoginMember");
    }

    public IEnumerator LoginMember()
    {
        WWWForm form = new WWWForm();       //Post 방식으로 넘겨줄 데이터(AddField로 넘겨줄 수 있음)
        form.AddField("id", id);            // 받아온 id값 서버에 전달
        form.AddField("pass", pass);        // 받아온 id값 서버에 전달

        using (var www = UnityWebRequest.Post(phpURL, form))    //웹 서버에 요청
        {
            yield return www.SendWebRequest();   //요청이 끝날 때까지 대기

            if (www.isNetworkError || www.isHttpError)
                Debug.Log(www.error);
            
            else
            {
                Debug.Log("upload complete");
                //Debug.Log(www.downloadHandler.text);    //서버로부터 받은 데이터를 string 형태로 출력
                StartCoroutine("DisplayResult");
            }
        }
    }

    public IEnumerator DisplayResult()
    {
        using (var www = UnityWebRequest.Get(jsonURL))
        {
            yield return www.SendWebRequest();
            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                var strJsonData = www.downloadHandler.text;
                strJsonData = string.Join("", strJsonData.Split('[',']'));
                var json = JSON.Parse(strJsonData);

                var result = json["result"].ToString();

                if (result.CompareTo("\"IDFAIL\"") == 0)
                {
                    Debug.Log(result);
                    txtLog.text = "아이디 오류 !\n";
                }
                else if (result.CompareTo("\"PASSFAIL\"") == 0)
                {
                    Debug.Log(result);
                    txtLog.text = "비밀번호 오류!!\n";
                }
                else
                {
                    logincheck = true;
                    Debug.Log("SUCCESS");
                    txtLog.text = "로그인 성공!!\n";
                    
                }
            }
        }
    }
}
