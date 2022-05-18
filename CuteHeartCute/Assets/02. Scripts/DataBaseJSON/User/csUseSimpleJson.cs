using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;

public class csUseSimpleJson : MonoBehaviour
{
    public TextAsset jsonData = null;
    public string strJsonData = null;
    void Start()
    {
        jsonData = Resources.Load<TextAsset>("user_info");
        strJsonData = jsonData.text;
        var json = JSON.Parse(strJsonData); //배열 형태로 자동 파싱

        string user_name = json["이름"].ToString();
        int level = json["능력치"]["레벨"].AsInt; //인트형으로 형변
        string level1 = json["능력치"]["레벨"].ToString();

        Debug.Log(user_name);
        Debug.Log(level.ToString());
        Debug.Log(level1);

        for (int i = 0; i < json["보유스킬"].Count; i++)
        {
            Debug.Log(json["보유스킬"][i].ToString());
        }

    }

}
