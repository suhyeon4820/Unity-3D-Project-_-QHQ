using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DontDestroy : MonoBehaviour
{
    private void Awake() {
        DontDestroyOnLoad(this.gameObject); // 다음 신으로 넘어가도  destroy 안됨
        
        Application.LoadLevel("scLogin");
    }
}
