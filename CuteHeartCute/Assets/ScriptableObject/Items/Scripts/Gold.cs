
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Gold Object", menuName = "Inventory/Items/Gold")]
public class Gold : ItemObjects
{
    public int goldValue;
    private void Awake()
    {
        type = ItemType.Gold;
    }
}
