using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{   
    public AudioClip[] musicClip;
    
    public float soundVolume = 1.0f;    // 사운드 volume 설정 변수
    public bool isSoundMute = false;    // 사운드 mute 설정 변수

    public Slider sl;   // 슬라이더 컴포넌트 연결 변수
    public Toggle tg;   // 토글 컴포넌트 연결 변수

    public AudioSource audio;   // audio가 가리키는 대상

    public GameObject playSoundBtn;
    public GameObject Sound;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        audio = GetComponent<AudioSource>();
    }
    // Start is called before the first frame update
    void Start()
    {
        sl.value = 1.0f;
        soundVolume = sl.value;
        isSoundMute = tg.isOn;
        
    }

    public void PlayBackground(int stage)
    {
        GetComponent<AudioSource>().clip = musicClip[stage];
        AudioSet();
        GetComponent<AudioSource>().Play();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void SetSound()
    {
        soundVolume = sl.value;
        isSoundMute = tg.isOn;
        AudioSet();
    }

    void AudioSet()
    {
        audio.volume = soundVolume;
        audio.mute = isSoundMute;
    }

    public void SoundUiOpen()
    {
        Sound.SetActive(true);
        playSoundBtn.SetActive(false);
    }
    public void SoundUiClose()
    {
        Sound.SetActive(false);
        playSoundBtn.SetActive(true);
    }


}
