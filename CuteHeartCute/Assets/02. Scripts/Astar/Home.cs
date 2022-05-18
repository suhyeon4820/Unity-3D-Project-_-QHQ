using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Home : MonoBehaviour
{
    [SerializeField] private float searchRadious = 10f;
    [SerializeField] private GameObject damageEffectWindow;
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Enemy")
        {
            StartCoroutine(ShowDamageEffect());
        }
    }

    IEnumerator ShowDamageEffect()
    {
        damageEffectWindow.SetActive(true);
        yield return new WaitForSeconds(1f);
        damageEffectWindow.SetActive(false);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, searchRadious);
    }
}
