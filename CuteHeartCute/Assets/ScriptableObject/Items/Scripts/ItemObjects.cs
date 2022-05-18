using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Food,
    Bullet,
    Equipment,
    Default,
    Gold
}
public abstract class ItemObjects : ScriptableObject
{
    public int Id;
    public Sprite uiDisplay;
    public ItemType type;

    [TextArea(15,20)] public string description;
   

    public Item CreateItem()
    {
        Item newItem = new Item(this);
        return newItem;
    }
    
}

[System.Serializable]
public class Item
{
    public string name;
    public int Id;
    public Item(ItemObjects item)
    {
        name = item.name;
        Id = item.Id;
    }
}