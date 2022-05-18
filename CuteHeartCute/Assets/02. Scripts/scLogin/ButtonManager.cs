using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    // login창 - singin 버튼 클릭 시 활성화되는 창
    public GameObject signInWindow;
    public GameObject soundui;
    // login창에 있는 SignIn 버튼
    public void SignInButtonClick()
    {
        signInWindow.SetActive(true);
    }
    // SignIn 창에 있는 SignIn 버튼
    public void SignInButtonClick2()
    {
        // 서버 연동 내용 추가

        signInWindow.SetActive(false);
    }

    public void CencelButtonClick()
    {
        signInWindow.SetActive(false);
    }

    public void SoundExitButtonClick()
    {
        soundui.SetActive(false);
    }

    public void SoundSettingButtonClick()
    {
        soundui.SetActive(true);
    }
}
