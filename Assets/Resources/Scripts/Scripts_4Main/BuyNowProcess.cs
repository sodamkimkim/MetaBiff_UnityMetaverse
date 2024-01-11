using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Biff.BackgroundInfo;

public class BuyNowProcess : MonoBehaviour
{
    [SerializeField]
    private InventoryManager inventoryManager = null;
    [SerializeField]
    private MoneyManager moneyManager = null;
    [SerializeField]
    private GameObject buyNowGo = null;
    [SerializeField]
    private GameObject itemDetails = null;
    ItemDetail[] itemDetailArrs = null;

    public void OpenBuyNowAsk()
    {
        UpdateStatusMsg("");
        buyNowGo.SetActive(true);
    }
    public void NotNow()
    {
        buyNowGo.SetActive(false);
    }
    public void OnClickBuyYes()
    {
        itemDetailArrs = itemDetails.GetComponentsInChildren<ItemDetail>();
        foreach (ItemDetail detail in itemDetailArrs)
        {
            if (detail.gameObject.activeSelf == true)
            {
                // item detail�� price���� ��������
                GameObject priceGo = detail.gameObject.transform.Find("Money").gameObject.transform.Find("Price").gameObject;
                string pricetxt = priceGo.GetComponent<TextMeshProUGUI>().text;

                // price ���ڿ� -> int
                pricetxt = pricetxt.Replace(",", "");
                // Debug.Log("price : " + int.Parse(pricetxt));
                int itemPrice = int.Parse(pricetxt);
                int playerMoney = PlayerInfoManager.GetMoney();
                if (playerMoney >= itemPrice)
                { // ������ >= �����̸� ���� ���� ����!
                    inventoryManager.SearchEmptySlot();
                   
                    if (inventoryManager.CheckInventoryFull() != true)
                    { // # inventory is not full
                        moneyManager.Pay(itemPrice);
                        inventoryManager.PushIntoInventoryAfterPurchasing(detail.gameObject.name.Replace("(Clone)", ""));
                        Debug.Log(detail.gameObject.name + " ���� �Ϸ�! price: " + itemPrice);

                        UpdateStatusMsg("Payment is success, Price : "+itemPrice.ToString());

                        // # ui���ֱ�
                        buyNowGo.SetActive(false);
                    }
                    else 
                    { // # inventory is full
                        Debug.Log("Market: ����� inventory�� full�̿��� �ŷ� �� �� �����ϴ�.");
                        UpdateStatusMsg("Your Inventory is full!!");
                    }
 
                }
                else
                { // # ������
                    Debug.Log("���� �����մϴ�.");
                    UpdateStatusMsg("You are short of balance.");
                }

            }
        }
    }

    private void UpdateStatusMsg(string _msg)
    {
        TextMeshProUGUI tmpStatusMsg = buyNowGo.transform.Find("TMP_Status").gameObject.GetComponent<TextMeshProUGUI>();
        tmpStatusMsg.text = _msg;
        tmpStatusMsg.color = new Color(0f, 0f, 255f);
    }
} // end of class
