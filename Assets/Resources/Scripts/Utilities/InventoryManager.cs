using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using Biff.BackgroundInfo;
using Unity.VisualScripting;

public class InventoryManager : MonoBehaviour
{
    [SerializeField]
    private GameObject inventoryUI_ = null;
    [SerializeField]
    private InventoryDB inventoryDB = null;
    private DropSlot[] dropSlotArr = null;

    private bool inventoryIsFull = false;
    private int firstEmptySlotIdx = 0;
    private GameObject[] itemsArr = null;
    int emptySlotCnt = 0;

    private void Awake()
    {
        dropSlotArr = inventoryUI_.GetComponentsInChildren<DropSlot>();
        itemsArr = Resources.LoadAll<GameObject>("Items");
        inventoryUI_.SetActive(false);
    }
    /// <summary>
    /// �Ѱ� ���� ����� �Ǵ� GameObject ��ȯ
    /// </summary>
    /// <returns></returns>
    public GameObject GetUIGo()
    {
        return inventoryUI_;
    }
    public void GetInventoryInfo(string _nick)
    {
        inventoryDB.GetInventoryInfo(PlayerInfoManager.GetNickname());
    }
    public void OpenInvenUI()
    {
        inventoryUI_.SetActive(true);
    }
    public void CloseInvenUI()
    {
        inventoryUI_.SetActive(false);
    }
    public void UpdateInventoryInfo()
    {
        inventoryDB.UpdateInventoryInfo();
    }
    /// <summary>
    /// nowwearing info <-> inventory�� �������� ������ �ƴ϶� �θ� �Ű��ִ� ��.
    /// ��, �� �Լ��� instantiate����!
    /// </summary>
    /// <param name="_inputItem"></param>
    public void SetItemWearingObjParentIntoInventory()
    {
        CheckInventoryFull();
        SearchEmptySlot();
        if (CheckInventoryFull() != true)
        {
            RectTransform parentRtr = dropSlotArr[firstEmptySlotIdx].GetComponent<RectTransform>();
            ItemFunction.wearingObj.transform.SetParent(parentRtr);
        }
        else
        { // # inventory == full�̶�� �������� inventory�� ���ִ´�.
            return;
        }
    }
    public void SetItemParentIntoInventory()
    {
        SearchEmptySlot();
        if (CheckInventoryFull() != true)
        {
            RectTransform parentRtr = dropSlotArr[firstEmptySlotIdx].GetComponent<RectTransform>();
            NowWearingManager.PositionChangingItem.transform.SetParent(parentRtr);
            NowWearingManager.PositionChangingItem.transform.localPosition = Vector3.zero;
            // NowWearingManager.PositionChangingItem.GetComponent<DragItem>().enabled = true;
        }
    }
    /// <summary>
    /// ���Ͽ��� ������ ���� �� �κ��丮�� �������ִ� �Լ�
    /// </summary>
    /// <param name="_inputItem"></param>
    public void PushIntoInventoryAfterPurchasing(string _inputItem)
    {   // # inventory fullüũ�ؼ� full�̸� push���ϰ� ����(������ �ŷ� ���ϵ���)
        // # full �ƴϸ� ������ ������ �����ؼ� first empty slot �� �־��� 
        CheckInventoryFull();
        SearchEmptySlot();
        Debug.Log(" firstEmptySlotIdx : " + firstEmptySlotIdx);
        if (CheckInventoryFull() != true)
        {
            for (int i = 0; i < (int)ItemInfo.EItemName.Len; i++)
            {
                if (_inputItem.Equals(((ItemInfo.EItemName)i).ToString()))
                {
                    Instantiate(itemsArr[i], dropSlotArr[firstEmptySlotIdx].gameObject.transform);
                }
            }
        }
        else
        { // # inventory == full�̶��
            return;
        }
        CheckInventoryFull();
        inventoryDB.UpdateInventoryInfo();
    }
    public void SearchEmptySlot()
    { // # �� ù emtpy slot ã���ִ� �Լ�
        for (int i = 0; i < dropSlotArr.Length; i++)
        {
            if (dropSlotArr[i].gameObject.GetComponentInChildren<DragItem>() == null)
            {
                //     Debug.Log("slot" + (i + 1) + " is empty");
                firstEmptySlotIdx = i; // * idx ���� 0 ~ 19 (db�� ����Ǵ� �̸��̶� �ٸ� ����)
                inventoryIsFull = false;
                break;
            }
        }
    }
    public bool CheckInventoryFull()
    { // # inventory�� full ���� üũ���ִ� �Լ�
        emptySlotCnt = 0;
        for (int i = 0; i < dropSlotArr.Length; i++)
        {
            if (dropSlotArr[i].gameObject.GetComponentInChildren<DragItem>() == null)
            {
                ++emptySlotCnt;
            }
        }
        if (emptySlotCnt == 0)
        {
            inventoryIsFull = true;
            Debug.Log("inventory is full");
        }
        else
        {
            inventoryIsFull = false;
        }
        return inventoryIsFull;
    }
    /// <summary>
    ///  ù empty slot �� draggingobj�� �θ�� ������ �ִ� �Լ�
    /// </summary>
    public void SetDraggingItemParent()
    { // #
        SearchEmptySlot();
        RectTransform parentRtr = dropSlotArr[firstEmptySlotIdx].GetComponent<RectTransform>();
        DragItem.SetDraggingObjPosition(parentRtr.position);
        DragItem.draggingObj.transform.SetParent(parentRtr);
    }

} // end of class
