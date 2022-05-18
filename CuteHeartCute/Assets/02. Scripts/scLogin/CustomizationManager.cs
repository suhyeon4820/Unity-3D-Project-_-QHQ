using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

using SimpleJSON;
using UnityEngine.Networking;

public class CustomData
{
    public int hairKindData;
    public int outfitData;
    public int hairColorData;
    public int skinColorData;
    public int eyeColorData;

    

    public CustomData(int hairKindData, int outfitData, int hairColorData, int skinColorData, int eyeColorData)
    {
        this.hairKindData = hairKindData;
        this.outfitData = outfitData;
        this.hairColorData = hairColorData;
        this.skinColorData = skinColorData;
        this.eyeColorData = eyeColorData;
    }
}
public class CustomizationManager : MonoBehaviour
{
    public CustomData customData;

    PhotonView pv;

    string phpURL = "http://bananacco.dothome.co.kr/setcustom.php";   //PHP 파일의 위치를 저장
    string jsonURL = "http://bananacco.dothome.co.kr/setcustom.json";
    string get_phpURL = "http://bananacco.dothome.co.kr/getcustom.php";   //PHP 파일의 위치를 저장
    string get_jsonURL = "http://bananacco.dothome.co.kr/getcustom.json";

    public static bool iscustomEnd = false;
    // 커스텀할 종류와 색상 열거형으로 작성 - switch문에 사용 
    enum AppearanceDetails
    {
        HAIR_KIND,
        HAIR_COLOR,
        EYE_COLOR,
        OUTFIT_KIND,
        SKIN_COLOR,
    }

    // 변경할 대상
    [SerializeField] private SkinnedMeshRenderer hairRenderer;              // 머리
    [SerializeField] private SkinnedMeshRenderer outfitTopRenderer;         // 옷 - 상의
    [SerializeField] private SkinnedMeshRenderer outfitBottomRenderer;      // 옷 - 하의
    [SerializeField] private SkinnedMeshRenderer outfitFootwearRenderer;    // 옷 - 신발
    [SerializeField] private SkinnedMeshRenderer leftEyeRenderer;           // 눈 - 왼쪽 
    [SerializeField] private SkinnedMeshRenderer rightEyeRenderer;          // 눈 - 오른쪽 
    [SerializeField] private SkinnedMeshRenderer headRenderer;              // 피부 - 얼굴
    [SerializeField] private SkinnedMeshRenderer bodyRenderer;              // 피부 - 몸

    // 커스터마이징할 모양, 색상 종류
    [SerializeField] private Mesh[] hairKinds;                          // 머리 - 종류  
    [SerializeField] private Material[] hairColors;                         // 머리 - 색상
    [SerializeField] private Mesh[] outfitTopKinds;                     // 상의 - 종류
    [SerializeField] private Material[] outfitTopColors;                    // 상의 - 색상
    [SerializeField] private Mesh[] outfitBottomKinds;                  // 하의 - 종류
    [SerializeField] private Material[] outfitBottomColors;                 // 하의 - 색상  
    [SerializeField] private Mesh[] outfitFootwearKinds;                // 신발 - 종류
    [SerializeField] private Material[] outfitFootwearColors;               // 신발 - 색상
    [SerializeField] private Material[] eyeColors;                          // 눈 - 색상
    [SerializeField] private Material[] headColors;                         // 피부 - 얼굴 색상
    [SerializeField] private Material[] bodyColors;                         // 피부 - 몸체 색상

    // ui button index
    public static int hairKindIndex = 0;
    public static int outfitIndex = 0;
    public static int hairColorIndex = 1;
    public static int skinColorIndex = 1;
    public static int eyeColorIndex = 0;

    private void Awake()
    {
        pv = GetComponent<PhotonView>();
    }
    private void Start()
    {
        //if (pv.isMine)
        //    LoadSaveDresses();

        if(pv.isMine)
            StartCoroutine(GetCustom());
    }
 
    public IEnumerator GetCustom()
    {
        WWWForm form = new WWWForm();       //Post 방식으로 넘겨줄 데이터(AddField로 넘겨줄 수 있음)
        string name = PhotonNetwork.playerName.ToString();
        form.AddField("id", name.ToString());            // 받아온 id값 서버에 전달

        using (var www = UnityWebRequest.Post(get_phpURL, form))    //웹 서버에 요청
        {
            yield return www.SendWebRequest();   //요청이 끝날 때까지 대기

            if (www.isNetworkError || www.isHttpError)
                Debug.Log(www.error);

            else
            {
                Debug.Log("upload complete");
                StartCoroutine("DisplayResult");
            }
        }
    }

    public IEnumerator DisplayResult()
    {
        using (var www = UnityWebRequest.Get(get_jsonURL))
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
                var json = JSON.Parse(strJsonData);

                hairKindIndex = json["hairKind"].AsInt;
                outfitIndex = json["outfit"].AsInt;
                hairColorIndex = json["hairColor"].AsInt;
                skinColorIndex = json["skinColor"].AsInt;
                eyeColorIndex = json["eyeColor"].AsInt;
                InitializeCharacter();
            }
        }
    }
    // 머리 종류 변경 함수  
    public void HairModelUp()
    {
        // 전체 크기보다 작으면 index +1
        if (hairKindIndex < hairKinds.Length - 1)
            hairKindIndex++;
        // 인덱스가 전체 크기 벗어나면 0으로 초기화
        else
            hairKindIndex = 0;

        // 함수 호출
        ApplyModification(AppearanceDetails.HAIR_KIND, hairKindIndex);
    }

    public void HairModelDown()
    {
        // 인덱스 감소
        if (hairKindIndex > 0)
            hairKindIndex--;
        // 인덱스 0보다 작으면 마지막 인덱스로 초기화
        else
            hairKindIndex = hairKinds.Length - 1;

        ApplyModification(AppearanceDetails.HAIR_KIND, hairKindIndex);
    }

    // 옷 종류 변경 함수 
    public void OutfitKindUp()
    {
        if (outfitIndex < outfitTopKinds.Length - 1)
            outfitIndex++;
        else
            outfitIndex = 0;

        // 함수 호출
        ApplyModification(AppearanceDetails.OUTFIT_KIND, outfitIndex);

    }
    public void OutfitKindDown()
    {
        if (outfitIndex > 0)
            outfitIndex--;
        else
            outfitIndex = outfitTopKinds.Length - 1;

        ApplyModification(AppearanceDetails.OUTFIT_KIND, outfitIndex);
    }

    // 머리 색상 변경 함수 - 인덱스 받아옴
    public void HairColorChange(int num)
    {
        hairColorIndex = num;
        ApplyModification(AppearanceDetails.HAIR_COLOR, num);
    }
    // 피부 색상 변경 함수 - 인덱스 받아옴
    public void SkinColorChange(int num)
    {
        
        skinColorIndex = num;
        ApplyModification(AppearanceDetails.SKIN_COLOR, num);
    }
    // 눈 색상 변경 함수 - 인덱스 받아옴
    public void EyeColorChange(int num)
    {
        eyeColorIndex = num;
        ApplyModification(AppearanceDetails.EYE_COLOR, num);
    }

    //  스타일 적용 함수 ***********************************************
    void ApplyModification(AppearanceDetails detail, int id)
    {
        switch (detail)
        {
            // 머리 종류
            case AppearanceDetails.HAIR_KIND:
                hairRenderer.sharedMesh = hairKinds[id];
                break;
            // 피부 색상
            case AppearanceDetails.SKIN_COLOR:
                headRenderer.sharedMaterial = headColors[id];
                bodyRenderer.sharedMaterial = bodyColors[id];
                break;
            // 머리 색상
            case AppearanceDetails.HAIR_COLOR:
                hairRenderer.sharedMaterial = hairColors[id];
                break;
            // 눈 색상
            case AppearanceDetails.EYE_COLOR:
                leftEyeRenderer.sharedMaterial = eyeColors[id];
                rightEyeRenderer.sharedMaterial = eyeColors[id];
                break;
            // 아웃핏 종류 + 색상
            case AppearanceDetails.OUTFIT_KIND:
                // 상의
                outfitTopRenderer.sharedMesh = outfitTopKinds[id];      // 종류
                outfitTopRenderer.sharedMaterial = outfitTopColors[id]; // 색상
                // 하의
                outfitBottomRenderer.sharedMesh = outfitBottomKinds[id];      // 종류
                outfitBottomRenderer.sharedMaterial = outfitBottomColors[id]; // 색상
                // 신발
                outfitFootwearRenderer.sharedMesh = outfitFootwearKinds[id];      // 종류
                outfitFootwearRenderer.sharedMaterial = outfitFootwearColors[id]; // 색상
                break;
        }
    }

    //// 저장하기

    void SaveDress(int _hairKind, int _outfit, int _hairColor, int _skinColor, int _eyeColor)
    {
        PlayerPrefs.SetInt("hairKindIndex", _hairKind);
        PlayerPrefs.SetInt("outfitIndex", _outfit);
        PlayerPrefs.SetInt("hairColorIndex", _hairColor);
        PlayerPrefs.SetInt("skinColorIndex", _skinColor);
        PlayerPrefs.SetInt("eyeColorIndex", _eyeColor);
    }

    [PunRPC]
    // 로드하기
    public void LoadSaveDresses()
    {
        hairKindIndex = PlayerPrefs.GetInt("hairKindIndex");
        outfitIndex = PlayerPrefs.GetInt("outfitIndex");
        hairColorIndex = PlayerPrefs.GetInt("hairColorIndex");
        skinColorIndex = PlayerPrefs.GetInt("skinColorIndex");
        eyeColorIndex = PlayerPrefs.GetInt("eyeColorIndex");

        InitializeCharacter();
    }

   
    public void InitializeCharacter()
    {
        // 머리 종류
        hairRenderer.sharedMesh = hairKinds[hairKindIndex];
        // 피부 색상
        headRenderer.sharedMaterial = headColors[skinColorIndex];
        bodyRenderer.sharedMaterial = bodyColors[skinColorIndex];
        // 머리 색상
        hairRenderer.sharedMaterial = hairColors[hairColorIndex];
        // 눈 색상   
        leftEyeRenderer.sharedMaterial = eyeColors[eyeColorIndex];
        rightEyeRenderer.sharedMaterial = eyeColors[eyeColorIndex];
        // 아웃핏 상의
        outfitTopRenderer.sharedMesh = outfitTopKinds[outfitIndex];      // 종류
        outfitTopRenderer.sharedMaterial = outfitTopColors[outfitIndex]; // 색상
        // 아웃핏 하의                                                       
        outfitBottomRenderer.sharedMesh = outfitBottomKinds[outfitIndex];      // 종류
        outfitBottomRenderer.sharedMaterial = outfitBottomColors[outfitIndex]; // 색상
        // 아웃핏 신발                                                       
        outfitFootwearRenderer.sharedMesh = outfitFootwearKinds[outfitIndex];      // 종류
        outfitFootwearRenderer.sharedMaterial = outfitFootwearColors[outfitIndex]; // 색상

    }


    public void NextBtnClick()
    {
        //customData = new CustomData(hairKindIndex, outfitIndex, hairKindIndex, skinColorIndex, eyeColorIndex);

        //// json 저장하기
        //string jsonData = JsonUtility.ToJson(customData, true);
        //string path = Path.Combine(Application.dataPath + "/12. DB", PhotonNetwork.player.NickName+".json");
        //File.WriteAllText(path, jsonData);    // JSON 형식의 string을 파일에 텍스트로 저장

        //GameObject.Find("MyCharacter").GetComponent<CustomizationManager>().

        // 
        //SaveDress(hairKindIndex, outfitIndex, hairColorIndex, skinColorIndex, eyeColorIndex);

        if (this.gameObject.activeInHierarchy)
        StartCoroutine("SaveCustom");
    }

    public IEnumerator SaveCustom()
    {
        WWWForm form = new WWWForm();
        string name = PhotonNetwork.playerName.ToString();
        form.AddField("id", name.ToString());
        form.AddField("hairkind", hairKindIndex.ToString());
        form.AddField("outfit", outfitIndex.ToString());
        form.AddField("hairColor", hairColorIndex.ToString());
        form.AddField("skinColor", skinColorIndex.ToString());
        form.AddField("eyeColor", eyeColorIndex.ToString());

   
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
                iscustomEnd = true;
                //StartCoroutine("GetCustom");
            }
        }
    }

    

    //public IEnumerator SetCustom()
    //{
    //    using (var www = UnityWebRequest.Get(jsonURL))
    //    {
    //        yield return www.SendWebRequest();
    //        if (www.isNetworkError || www.isHttpError)
    //        {
    //            Debug.Log(www.error);
    //        }
    //        else
    //        {
    //            var strJsonData = www.downloadHandler.text;
    //            strJsonData = string.Join("", strJsonData.Split('[', ']'));
    //            var jSon = JSON.Parse(strJsonData);

    //            // var result = jSon["result"].ToString();
    //            Debug.Log(jSon["result"].ToString());
    //            Debug.Log(jSon["id"].ToString());
    //            Debug.Log(jSon["hairKind"].ToString());
    //            Debug.Log(jSon["outfit"].ToString());
    //            Debug.Log(jSon["hairColor"].ToString());
    //            Debug.Log(jSon["skinColor"].ToString());
    //            Debug.Log(jSon["eyeColor"].ToString());
    //        }
    //    }
    //}


}