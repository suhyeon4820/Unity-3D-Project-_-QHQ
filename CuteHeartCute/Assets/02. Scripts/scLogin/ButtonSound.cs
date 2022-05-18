using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonSound : MonoBehaviour
{
     
    public AudioSource audio;

    public AudioClip buttonEffectClip;  // 버튼클릭사운드
    public AudioClip successClip; // 확인 소리

    public void BtnSound()
    {

        audio.clip = buttonEffectClip;
        audio.Play();
    }

    public void SuccessSound()
    {
        audio.clip = successClip;
        audio.Play();
    }
}
