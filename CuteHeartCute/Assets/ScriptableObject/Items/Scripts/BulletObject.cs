using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Bullet Object", menuName = "Inventory/Items/Bullet")]
public class BulletObject : ItemObjects
{
    public int bulletValue; //불렛개수

    private void Awake() 
    {
        type = ItemType.Bullet;
    }
}
