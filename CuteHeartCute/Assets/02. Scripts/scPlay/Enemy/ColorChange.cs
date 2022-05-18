using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorChange : MonoBehaviour
{
    public Color startColor = Color.black;
    public Color endColor = Color.red;

    [Range(0, 10)]
    public float speed = 1;

    public float colorTime = 2f;
    Renderer ren;

    private void Awake()
    {
        ren = GetComponent<Renderer>();
        color();
        //StartCoroutine(EnemyColorChanged());
    }
    private void Update()
    {
        colorTime -= Time.deltaTime;
        color();
    }

    void color()
    {
        
        Mathf.Round(colorTime);

        if(colorTime >=1)
        {
            ren.material.color = Color.red;
        }
        else if(colorTime<1 && colorTime>=0)
        {
            ren.material.color = Color.black;
        }
        else
        {
            ren.material.color = Color.white;
        }

    }
    //IEnumerator EnemyColorChanged()
    //{
    //    // 1초마다 두 가지 색상이 반짝거림
    //    if(colorTime>=0)
    //    {
    //        ren.material.color = Color.Lerp(startColor, endColor, Mathf.PingPong(Time.time * speed, 1));
    //        colorTime--;
    //    }
    //    else
    //    {
    //        ren.material.color = Color.white;
    //    }

    //    yield return new WaitForSeconds(1f);

    //}
}
