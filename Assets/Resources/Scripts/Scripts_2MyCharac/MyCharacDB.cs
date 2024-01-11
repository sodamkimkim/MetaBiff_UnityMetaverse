using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using TMPro;
using UnityEngine.SceneManagement; 
using Biff.BackgroundInfo;

public class MyCharacDB : MonoBehaviour
{
    [SerializeField]
    private ChoosingMyCharacterManager choosingMyCharacterManager = null;
    [SerializeField]
    private GameObject mycharactersUI = null;
    private List<MyCharacDB.MyCharacterInfoDTO> myCharacList = new List<MyCharacDB.MyCharacterInfoDTO>();
    private void Awake()
    {
        PlayerInfoManager.myCharacsNowWearingDic_.Clear();
    }
    public struct MyCharacterInfoDTO
    {
        public string nickName;
        public string userId;
        public string model;

        public override string ToString()
        {
            return "nickName : " + nickName + ", userId : " + userId + ", model : " + model;
        }
    } // end of struct MyCharacterInfo
    public struct NowWearingDTO
    {
        public string nickname;
        public string clothes;
        public string hands;
        public string head;
        public string bag;
        public string pet;

        public NowWearingDTO(string nickname, string clothes, string hands, string head, string bag, string pet)
        {
            this.nickname = nickname;
            this.clothes = clothes;
            this.hands = hands;
            this.head = head;
            this.bag = bag;
            this.pet = pet;
        }

        public override string ToString()
        {
            return "nickname : " + nickname + ", clothes : " + clothes
                            + ", hands : " + hands + ", head : " + head + ", bag : " + bag
                            + ", pet : " + pet;
        }
    } // end of struct NowWearing
    // # MyCharacters �ҷ����� �Լ�
    public void GetMyCharacters(string _userId)
    {
        StartCoroutine(GetMyCharactersCoroutine(_userId));
        // StartCoroutine(GetMyCharactersCoroutine(testUserId)); // test��
    }
    private IEnumerator GetMyCharactersCoroutine(string _userId)
    {
        WWWForm form = new WWWForm();
        form.AddField("id", _userId);

        using (UnityWebRequest www = UnityWebRequest.Post("http://127.0.0.1:80/biffprj/MyCharacs/searchmycharacters.php", form))
        {
            yield return www.SendWebRequest();
            if (DBUtility.CheckError(www))
            {
                Debug.Log(www.error);
            }
            else if (www.result.ToString().Equals("Success"))
            {
                // Debug.Log("DB Connection Success");
                string data = www.downloadHandler.text;

                // Debug.Log(data);
                if (data.Equals("No UserInformation"))
                {
                    // ��ġ�ϴ� User Information ���� ��� Process
                    choosingMyCharacterManager.NoCharacterProcess();
                }
                else
                {
                    // ����Ʈ�� �����صΰ� 
                    List<MyCharacDB.MyCharacterInfoDTO> myCharacs = JsonConvert.DeserializeObject<List<MyCharacDB.MyCharacterInfoDTO>>(data);
                    myCharacList.Clear();
                    myCharacList = myCharacs;
                    MyCharacUI[] myCharacArr = mycharactersUI.GetComponentsInChildren<MyCharacUI>();

                    // # ĳ���� 3�� �̻��̸� ���̻� ������ ���ϰ� ��ư ������� ��.
                    if (myCharacs.Count >= 3)
                    {
                        choosingMyCharacterManager.MoreCreateCharacter(myCharacs.Count, false);
                    }
                    else if (myCharacs.Count < 3)
                    {
                        choosingMyCharacterManager.MoreCreateCharacter(myCharacs.Count, true);
                    }

                    for (int i = 0; i < myCharacs.Count; i++)
                    {
                        Debug.Log(myCharacs[i].ToString());
                        // ���� ������ ȭ�鿡 �ѷ���

                        // # �г���
                        TextMeshProUGUI tmpNick = myCharacArr[i].gameObject.transform.Find("TMP_Nick").gameObject.GetComponent<TextMeshProUGUI>();
                        tmpNick.text = myCharacs[i].nickName;
                        // # ��
                        TextMeshProUGUI tmpModel = myCharacArr[i].gameObject.transform.Find("TMP_Model").gameObject.GetComponent<TextMeshProUGUI>();
                        tmpModel.text = myCharacs[i].model;

                        choosingMyCharacterManager.SettingBtnPicks();
                        GetNowWearing(myCharacs[i].nickName);
                    }
                    // Debug.Log(data);
                }
            }
            www.Dispose();
        }
        yield return null;
    }
    // # Character�� NowWearing���� �ҷ����� �Լ�
    public void GetNowWearing(string _nickName)
    {
        StartCoroutine(GetNowWearingCoroutine( _nickName));
    }
    private IEnumerator GetNowWearingCoroutine(string _nickName)
    {
        WWWForm form = new WWWForm();
        form.AddField("nickName", _nickName);

        using (UnityWebRequest www = UnityWebRequest.Post("http://127.0.0.1:80/biffprj/MyCharacs/getnowwearing.php", form))
        {
            yield return www.SendWebRequest();
            if (DBUtility.CheckError(www))
            {
                Debug.Log(www.error);
            }
            else if (www.result.ToString().Equals("Success"))
            {
                Debug.Log("DB Connection Success");
                string data = www.downloadHandler.text;

                // Debug.Log(data);
                if (data.Equals("No UserInformation"))
                { // # ��ġ�ϴ� nowWearing ���� ���� ��� Process

                    Debug.Log("�ش� �г����� nowWearing������ �����Ǿ� ���� �ʽ��ϴ�.");
                }
                else
                { // # wearing ���� �ҷ�����
                    List<MyCharacDB.NowWearingDTO> nowWearings = JsonConvert.DeserializeObject<List<MyCharacDB.NowWearingDTO>>(data);
                    MyCharacDB.NowWearingDTO nowWDto = new MyCharacDB.NowWearingDTO(
                        nowWearings[0].nickname, nowWearings[0].clothes, nowWearings[0].hands, nowWearings[0].head, nowWearings[0].bag, nowWearings[0].pet
                        );
                    
                    Debug.Log("@@@ " + data);
                    //nowWearingDic.Add(_nickName, nowWDto);
                    PlayerInfoManager.myCharacsNowWearingDic_.Add(_nickName, nowWDto);
                    //    MyCharacUI[] myCharacArr = mycharactersUI.GetComponentsInChildren<MyCharacUI>();
                    Debug.Log("!!!! nowWearingsDic"+ PlayerInfoManager.myCharacsNowWearingDic_[_nickName].ToString());
                    choosingMyCharacterManager.SetActivePicBtn(true);
                    
                }
            }
            www.Dispose();
        }
        yield return null;
    }
    /// <summary>
    ///  �Ű����� �޾Ƽ� nowWearing���� static class�� �����ϰ� mainScene���� ��� �� �ְ� ���ִ� �Լ�
    /// </summary>
    /// <param name="_nick">���õ� ĳ������ �г���</param>
    public void UpdateMyCharacNowWearingInfo(string _nick)
    {
        PlayerInfoManager.SetNW_Clothes(PlayerInfoManager.myCharacsNowWearingDic_[_nick].clothes);
        PlayerInfoManager.SetNW_Hands(PlayerInfoManager.myCharacsNowWearingDic_[_nick].hands);
        PlayerInfoManager.SetNW_Head(PlayerInfoManager.myCharacsNowWearingDic_[_nick].head);
        PlayerInfoManager.SetNW_Bag(PlayerInfoManager.myCharacsNowWearingDic_[_nick].bag);
        PlayerInfoManager.SetNW_Pet(PlayerInfoManager.myCharacsNowWearingDic_[_nick].pet);
    }
    public void DeleteMyCharacter(string _deleteCharacNick)
    {
        StartCoroutine(DeleteMyCharacCoroutine(_deleteCharacNick));
    }
    private IEnumerator DeleteMyCharacCoroutine(string _deleteCharacNick)
    {
        WWWForm form = new WWWForm();
        form.AddField("nickname", _deleteCharacNick);

        using (UnityWebRequest www = UnityWebRequest.Post("http://127.0.0.1:80/biffprj/MyCharacs/deletemycharac.php", form))
        {
            yield return www.SendWebRequest();
            if (DBUtility.CheckError(www))
            {
                Debug.Log(www.error);
            }
            else if (www.result.ToString().Equals("Success"))
            {
                Debug.Log("DB Connection Success");
                string data = www.downloadHandler.text;
                Debug.Log(data);
                // character ��Ÿ���ִ� ���� �����ֱ�
                RemoveMyCharacterInfo(_deleteCharacNick);

            }
            www.Dispose();
        }
        yield return null;
    }
    public string GetPickedModel(int _btnIdx)
    {
        return myCharacList[_btnIdx].model;
    }
    private void RemoveMyCharacterInfo(string _nickName)
    {
        SceneManager.LoadScene("2_MyCharacters");
    }
} // end of class
