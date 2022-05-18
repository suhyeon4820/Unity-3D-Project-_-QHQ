using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEditor;

public class GroundItem : MonoBehaviour, ISerializationCallbackReceiver
{
    public ItemObjects item;

    public void OnAfterDeserialize()
    {

    }

    public void OnBeforeSerialize()
    {
        GetComponentInChildren<SpriteRenderer>().sprite = item.uiDisplay;
        //EditorUtility.SetDirty(GetComponentInChildren<SpriteRenderer>());
    } //아이템 스프라이트로 납작하게 보이게 하는 방식 (큐브x)

    //void OnCollisionEnter(Collision collision)
    //{
    //    if(collision.gameObject.tag == "Player")
    //    {
    //        Destroy(this.gameObject);
    //    }
    //}
}
