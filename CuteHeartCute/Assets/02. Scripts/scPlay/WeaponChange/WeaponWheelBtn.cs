using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// weaponwheel 버튼 애니메이션과 버튼 클릭시 가운데 text 변경
public class WeaponWheelBtn : MonoBehaviour
{
    private Animator anim;
    [SerializeField] private string itemName;   // 입력받은 무기 이름
    [SerializeField] private Text itemText;   // UI Text
    private bool selected = false;

    private void Awake()
    { 
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        // 버튼 클릭시 UI에 itemName 출력
        if(selected)
        {
            itemText.text = itemName;
        }
        else
        {
            return;
        }
    }

    // 지금 버튼 클릭시
    public void Selected()
    {
        selected = true;
    }
    // 지금버튼 선택되지 않을 경우(다른 버튼 선택된 경우)
    public void Deselected()
    {
        selected = false;
    }
    // 버튼에 마우스 오버랩 시작 - 커지는 애니메이션
    public void HoverEnter()
    {
        anim.SetBool("Hover", true);
        itemText.text = itemName;
    }
    // 버튼에 마우스 오버랩 끝 - 작아지는 애니메이션
    public void HoverExit()
    {
        anim.SetBool("Hover", false);
        itemText.text = "";
    }
}
