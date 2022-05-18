using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;


public class csGetDataFromWeb : MonoBehaviour {


    private string boardListURL = "http://rosrom.happycarservice.com/controller/board.json.php"; //정보가 저장된 페이지주소
    private const string SecretCode = "rosrom"; //접속을 제한하기위한 시크릿코드.

    void Start()
    {
        StartCoroutine("GetBoardList");
    }

    public IEnumerator GetBoardList()
    {
        WWWForm form = new WWWForm();
        //           파라메타명(키) , 값
        form.AddField("SecretCode", SecretCode); //post방식 전달.

        var www = new WWW(boardListURL, form);
        yield return www; //웹의 다운로드 완료시까지 대기.
        Debug.Log("URL 다운로드 완료");
        /*
         * 1. string.IsNullOrEmpty
         * - 지정된 문자열이 null이거나 Empty문자열인지 여부를 나타냄
         * 
         * 2. string.IsNullOrWhiteSpace
         * - 지정된 문자열이 null이거나 비어있거나 공백문자로만 구성되어 있는지 여부를 나타냄
         */

        //오류검증.
        if (string.IsNullOrEmpty(www.error))
        {
            Debug.Log(www.text); //웹페이지를 그대로 출력
            DisplayBoardList(www.text); //파싱후 출력.
        }
        else
        {
            Debug.Log("Error : " + www.error);
        }
    }

    void DisplayBoardList(string strJsonData)
    {
        var jSon = JSON.Parse(strJsonData);

        for (int i = 0; i < jSon.Count; i++)
        {
            int idx = jSon[i]["idx"].AsInt;
            string name = jSon[i]["name"].ToString();
            string email = jSon[i]["email"].ToString();
            string subject = jSon[i]["subject"].ToString();
            string contents = jSon[i]["contents"].ToString();
            string registdate = jSon[i]["registdate"].ToString();

            Debug.Log("일련번호 : " + idx);
            Debug.Log("제목 : " + subject);
            Debug.Log("내용 : " + contents);
            Debug.Log("가입일 : " + registdate);
            Debug.Log("--------------------------");

        }
    }
}
