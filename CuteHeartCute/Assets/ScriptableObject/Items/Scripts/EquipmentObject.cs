using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Equipment Object", menuName = "Inventory/Items/Equipment")]
public class EquipmentObject : ItemObjects
{
    public float atcBonus; //장비 보너스값
    public float defenceBonus; //방어력
    private void Awake()
    {
        type = ItemType.Equipment;
    }

}
