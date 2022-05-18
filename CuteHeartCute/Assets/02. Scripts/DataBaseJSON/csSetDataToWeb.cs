using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using UnityEngine.Networking;

public class csSetDataToWeb : MonoBehaviour
{
    public int hairKindIndex = 1;
    public int outfitIndex = 2;
    public int hairColorIndex = 3;
    public int skinColorIndex = 2;
    public int eyeColorIndex = 1;

    string phpURL = "http://bananacco.dothome.co.kr/setcustom.php";   //PHP 파일의 위치를 저장
    string jsonURL = "http://bananacco.dothome.co.kr/setcustom.json";
    //private string boardSaveURL = "http://rosrom.happycarservice.com/controller/board_save.json.php";
    private string SecretCode = "rosrom";

    void Start()
    {
        StartCoroutine("SaveCustom");
    }

    public IEnumerator SaveCustom()
    {
        WWWForm form = new WWWForm();
        //form.AddField("SecretCode", SecretCode);
        form.AddField("id", PhotonNetwork.playerName);

        form.AddField("hairkind", hairKindIndex);
        form.AddField("outfit", outfitIndex);
        form.AddField("hairKind", hairColorIndex);
        form.AddField("skinColor", skinColorIndex);
        form.AddField("eyeColor", eyeColorIndex);

        using (var www = UnityWebRequest.Post(phpURL, form))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("upload complete");
                StartCoroutine("SetCustom");
            }
        }
    }

    public IEnumerator GetResult()
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
                strJsonData = string.Join("", strJsonData.Split('[', ']'));
                var jSon = JSON.Parse(strJsonData);

               // var result = jSon["result"].ToString();
                Debug.Log(jSon["result"].ToString());
                Debug.Log(jSon["id"].ToString());
                Debug.Log(jSon["hairKind"].ToString());
                Debug.Log(jSon["outfit"].ToString());
                Debug.Log(jSon["hairColor"].ToString());
                Debug.Log(jSon["skinColor"].ToString());
                Debug.Log(jSon["eyeColor"].ToString());
            }
        }
    }
}
