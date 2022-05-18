using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public Slider timerSlider;
    public Text timerText;
    public float gameTime = 3f;

    private bool stopTimer;

    void Start()
    {
        stopTimer = false;
        timerSlider.maxValue = gameTime;
        timerSlider.value = gameTime;
    }
    void Update()
    {
        float time = gameTime - Time.time;

        int minutes = Mathf.FloorToInt(time / 60);
        //int seconds = Mathf.FloorToInt(time - minutes * 60f);
        int seconds = Mathf.FloorToInt(time) + 1;
        string textTime = string.Format("{0}", seconds);
        //string textTime = string.Format("{0:0}:{1:00}", minutes, seconds);
        if (time<=0)
        {
            stopTimer = true;
        }
        if(stopTimer == false)
        {
            timerText.text = textTime;
            timerSlider.value = time;
        }
    }
}
