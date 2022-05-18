using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Item Database", menuName = "Inventory/Items/Database")]
public class ItemDataBaseObject : ScriptableObject, ISerializationCallbackReceiver
{
    public ItemObjects[] Items; // 아이템 배열
    public Dictionary<ItemObjects, int> GetId = new Dictionary<ItemObjects, int>();
    public Dictionary<int, ItemObjects> GetItem = new Dictionary<int, ItemObjects>();
    public void OnAfterDeserialize()
    {
        for (int i = 0; i < Items.Length; i++)
        {
            Items[i].Id = i;
            GetItem.Add(i,Items[i]);
        }
    }

    public void OnBeforeSerialize()
    {
        GetItem = new Dictionary<int, ItemObjects>();
    }

 
}
