using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// tap키 누르면 weaponwheel창 활성화/비활성화 시켜줌
public class WeaponController : MonoBehaviour
{
    [SerializeField] private GameObject weaponWheal;
    [SerializeField] private GameObject inventory;

    private bool weaponWhealSelected = false;
    private bool inventorySelected = false;

    public GameObject[] weapons;    // weapon 종류

    private void Update()
    {
        // tab 키를 누르면 무기 창이 활성화
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            weaponWhealSelected = !weaponWhealSelected;
        }
        if (weaponWhealSelected)
        {
            weaponWheal.SetActive(true);
        }
        else
        {
            weaponWheal.SetActive(false);
        }

        // i키를 누르면 인벤토리 창이 활성화
        if (Input.GetKeyDown(KeyCode.I))
        {
            inventorySelected = !inventorySelected;
        }
        if (inventorySelected)
        {
            inventory.SetActive(true);
        }
        else
        {
            inventory.SetActive(false);
        }
    }

    public void inventoryCloseBtn()
    {
        inventorySelected = false;
    }
}
