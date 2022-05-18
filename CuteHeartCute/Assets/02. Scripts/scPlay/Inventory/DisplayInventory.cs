using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class DisplayInventory : MonoBehaviour
{
    public GameObject inventoryPrefab;
    public InventoryObject inventory;

    public int X_START; //슬롯채워지는 시작
    public int Y_START; 

    public int X_SPACE_BETWEEN_ITEM; //아이템 간격
    public int NUMBER_OF_COLUMN; //열수
    public int Y_SPACE_BETWEEN_ITEM; //아이템 간격

    Dictionary<InventorySlot,GameObject> itemDisplayed = new Dictionary<InventorySlot, GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        CreateDisPlay();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateDisPlay();
    }

    public void UpdateDisPlay()
    {
     for(int i = 0; i <inventory.Container.Items.Count; i++)
        {
            InventorySlot slot = inventory.Container.Items[i];
            if (itemDisplayed.ContainsKey(slot))
            // 만약 itemsDisplayd의 포함하고있는 키가 (inventory.Container[i])의 키라면
            {
                itemDisplayed[slot].GetComponentInChildren<TextMeshProUGUI>().text = slot.amount.ToString("n0");
                //itemDisplayed[인벤토리 컨테이너[i]값].의 자식컴퍼넌트<텍스트>를 인벤토리 컨테이너[i].수량을 표시해줌
            }
           else
            {
                var obj = Instantiate(inventoryPrefab, Vector3.zero, Quaternion.identity, transform);
                obj.transform.GetChild(0).GetComponentInChildren<Image>().sprite = inventory.database.GetItem[slot.item.Id].uiDisplay;
                //오브젝은 = 생성됨(인벤토리 컨테이너[i].아이템 프리팹과, 벡터 000, 쿼터년, 트랜스폼);
                obj.GetComponent<RectTransform>().localPosition = GetPosition(i);
                // 슬롯의 위치 정하기
                obj.GetComponentInChildren<TextMeshProUGUI>().text = slot.amount.ToString("n0");
                //수량표시
                itemDisplayed.Add(slot, obj);
                //디스플레이에 키 밸류값을 더해준다 (키, 값)
            }
        }
    }

    public void CreateDisPlay()
    {
  for(int i = 0; i <inventory.Container.Items.Count; i++)
        {
            InventorySlot slot = inventory.Container.Items[i];
            if (itemDisplayed.ContainsKey(slot))
            // 만약 itemsDisplayd의 포함하고있는 키가 (inventory.Container[i])의 키라면
            {
                itemDisplayed[slot].GetComponentInChildren<TextMeshProUGUI>().text = slot.amount.ToString("n0");
                //itemDisplayed[인벤토리 컨테이너[i]값].의 자식컴퍼넌트<텍스트>를 인벤토리 컨테이너[i].수량을 표시해줌
            }
           else
            {
                var obj = Instantiate(inventoryPrefab, Vector3.zero, Quaternion.identity, transform);
                obj.transform.GetChild(0).GetComponentInChildren<Image>().sprite = inventory.database.GetItem[slot.item.Id].uiDisplay;
                //오브젝은 = 생성됨(인벤토리 컨테이너[i].아이템 프리팹과, 벡터 000, 쿼터년, 트랜스폼);
                obj.GetComponent<RectTransform>().localPosition = GetPosition(i);
                // 슬롯의 위치 정하기
                obj.GetComponentInChildren<TextMeshProUGUI>().text = slot.amount.ToString("n0");
                //수량표시
                itemDisplayed.Add(slot, obj);
                //디스플레이에 키 밸류값을 더해준다 (키, 값)
            }
        }
    }

    public Vector3 GetPosition(int i)
    {
        return new Vector3( X_START + (X_SPACE_BETWEEN_ITEM * ( i % NUMBER_OF_COLUMN)), 
                            Y_START + (-Y_SPACE_BETWEEN_ITEM * ( i / NUMBER_OF_COLUMN)) );
    }
}
