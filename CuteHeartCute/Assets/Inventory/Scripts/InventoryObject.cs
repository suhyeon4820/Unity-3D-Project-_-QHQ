using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Runtime.Serialization;
using UnityEditor;

[CreateAssetMenu(fileName = "New Inventory" , menuName = "Inventory/Create inventory")]
public class InventoryObject : ScriptableObject
{
    public string savePath;
    public ItemDataBaseObject database;
    public Inventory Container;

    public void AddItem(Item _item, int _amount)
    {
        for (int i = 0; i <Container.Items.Count; i++)
        {
            if (Container.Items[i].item.Id == _item.Id)
            {
                Container.Items[i].AddAmount(_amount);
                return;
            }
        }
        
        Container.Items.Add(new InventorySlot(_item.Id, _item, _amount));
    }

    [ContextMenu("Save")]
    public void Save()
    {
        IFormatter formatter = new BinaryFormatter();
        Stream stream = new FileStream(string.Concat(Application.persistentDataPath,savePath), FileMode.Create, FileAccess.Write);
        formatter.Serialize(stream,Container);
        stream.Close();
    }

    [ContextMenu("Load")]
    public void Load()
    {
        if(File.Exists(string.Concat(Application.persistentDataPath, savePath)))
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(string.Concat(Application.persistentDataPath, savePath), FileMode.Open, FileAccess.Read);
            Container = (Inventory)formatter.Deserialize(stream);
            stream.Close();
        }
    }

    [ContextMenu("Clear")]
    public void Clear()
    {
        Container = new Inventory();
    }
}

[System.Serializable]
public class Inventory
{
    public List<InventorySlot> Items = new List<InventorySlot>();
}

[System.Serializable]
public class InventorySlot
{
    public int ID; //아이디
    public int amount; //아이템수량
    public Item item; //아이템 종류

    public InventorySlot(int _id, Item _item, int _amount)
    {
        ID = _id;
        item = _item;
        amount = _amount;
    }

    public void AddAmount(int value)
    {
        amount += value;
    }//똑같은 아이템 먹었을 때 수량 더해주는 함수
}
