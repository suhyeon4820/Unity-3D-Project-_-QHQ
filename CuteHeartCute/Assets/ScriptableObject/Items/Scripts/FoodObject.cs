using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Food Object", menuName = "Inventory/Items/Food")]
public class FoodObject : ItemObjects
{
    public int healthValue;
    private void Awake()
    {
        type = ItemType.Food;
    }
}
