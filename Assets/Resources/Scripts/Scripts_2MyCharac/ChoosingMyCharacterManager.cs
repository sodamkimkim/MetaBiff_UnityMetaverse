using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Biff.BackgroundInfo;

using Photon.Pun;
using Photon.Realtime;

public class ChoosingMyCharacterManager : MonoBehaviourPunCallbacks
{
private string gameVersion_ = "0.0.1";
  private byte maxPlayerPerRoom = 10;
    [SerializeField]
    private MyCharacDB db = null;
    [SerializeField]
    private Button btnCreateNewCharacter = null;
    [SerializeField]
    private Button BtnGo = null;
    [SerializeField]
    private TextMeshProUGUI statusMsgTxt = null;
    // private static string pickedNickName = "";
    [SerializeField]
    private Button[] btnPicArr = null;

    [SerializeField]
    private TextMeshProUGUI[] nickTmpArr = null;
    private bool isJoined = false;

    private void Awake()
    {
        // ������ ȥ�� ���� �ε��ϸ�, ������ ������� �ڵ����� ��ũ ��
       //PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.GameVersion = gameVersion_;
        // ���� ����
        PhotonNetwork.ConnectUsingSettings();
        BtnGo.interactable = true;
        btnCreateNewCharacter.onClick.AddListener(OnClickBtnCreateNewCharac);
        statusMsgTxt.gameObject.SetActive(false);
        btnPicArr[0].onClick.AddListener(() => OnClickPickBtn0(0));
        btnPicArr[1].onClick.AddListener(() => OnClickPickBtn1(1));
        btnPicArr[2].onClick.AddListener(() => OnClickPickBtn2(2));
        BtnGo.onClick.AddListener(OnClickBtnGo);
        SetActivePicBtn(false);
    }
    private void Start()
    {
        db.GetMyCharacters(PlayerInfoManager.GetID());
        //db.GetMyCharacters("theka265"); // test
    }
    public void SetActivePicBtn(bool _isActive)
    {
        for (int i = 0; i < btnPicArr.Length; i++)
        {
            btnPicArr[i].interactable = _isActive;
        }
    }
    public void SettingBtnPicks()
    { // # pick��ư �⺻ ���� �Լ�
        for (int i = 0; i < btnPicArr.Length; i++)
        {
            // # �г��� ���� pick��ư�� ��ä������
            btnPicArr[i].gameObject.GetComponentInChildren<TextMeshProUGUI>().color = new Color(100f, 100f, 100f);
            string nick = nickTmpArr[i].text;

            if (nick != "")
            { // # �г����ִ� pick��ư�� �ʷϻ�����
                btnPicArr[i].gameObject.GetComponentInChildren<TextMeshProUGUI>().color = new Color(0f, 255f, 0f);
                //  MyCharacterInfo.SetNickName(nick);
                // Debug.Log("pickedNickName: "+ pickedNickName);
            }
        }
    }
    private void OnClickPickBtn0(int _btnIdx)
    {
        PickBtnCallback(_btnIdx);
    }
    private void OnClickPickBtn1(int _btnIdx)
    {
        PickBtnCallback(_btnIdx);
    }
    private void OnClickPickBtn2(int _btnIdx)
    {
        PickBtnCallback(_btnIdx);
    }
    private void PickBtnCallback(int _btnIdx)
    {
        foreach (Button btn in btnPicArr)
        {
            btn.gameObject.GetComponentInChildren<TextMeshProUGUI>().color = new Color(100f, 100f, 100f);
        }

        string nick = nickTmpArr[_btnIdx].text;
        if (nick != "")
        {
            // Debug.Log("idx : " + _btnIdx);
            btnPicArr[_btnIdx].gameObject.GetComponentInChildren<TextMeshProUGUI>().color = new Color(0f, 255f, 0f);
            PlayerInfoManager.SetNickname(nick);
            PlayerInfoManager.SetModel(db.GetPickedModel(_btnIdx));
            Debug.Log(" MyCharacterInfo.nickName: " + PlayerInfoManager.GetNickname());

            // MyNowWearing ���� �������ֱ�
            db.UpdateMyCharacNowWearingInfo(nick);
        }
        else
        {
            PlayerInfoManager.SetNickname("");
            PlayerInfoManager.SetModel("");
        }
    }
    private void OnClickBtnCreateNewCharac()
    {
        //GameObject loginManagerGo = transform.Find("LoginManager").gameObject;
        //LoginManager loginMng = loginManagerGo.GetComponent<LoginManager>();
        Debug.Log(PlayerInfoManager.GetID());
        SceneManager.LoadScene("3_NewCharac");
    }
    // # �ش� ID�� ĳ���Ͱ� ���� ��� ȣ��
    public void NoCharacterProcess()
    {
        statusMsgTxt.gameObject.SetActive(true);
    }
    public void MoreCreateCharacter(int _myCharacterCnt, bool _newCharacMore)
    {
        // Debug.Log("���� ĳ���� ��: " + _myCharacterCnt);
        btnCreateNewCharacter.interactable = _newCharacMore;
    }
    private void OnClickBtnGo()
    {
        Debug.LogErrorFormat("Go!!");
        if (PlayerInfoManager.GetNickname() != "" && isJoined)
        {
            // PhotonNetwork.JoinRandomRoom();
            PhotonNetwork.IsMessageQueueRunning = false;
            PhotonNetwork.LoadLevel("4_Main");
        }
    }
    public override void OnConnectedToMaster()
    {
        // PhotonNetwork.NickName = PlayerInfoManager.GetNickname();
        Debug.LogFormat("Connected to Master : {0}", PhotonNetwork.NickName);
        BtnGo.interactable = false;
        PhotonNetwork.JoinRandomRoom();
    }
    //public override void OnDisconnected(DisconnectCause cause)
    //{
    //    Debug.LogWarningFormat("Disconnected: {0}", cause);
    //    BtnGo.interactable = false;
    //    Debug.LogError("Create Room");
    //    PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayerPerRoom });
    //}
    public override void OnJoinedRoom()
    {
        BtnGo.interactable = true;
        isJoined = true;
    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.LogErrorFormat("JoinRandomFailed({0}) : {1}", returnCode, message);
        // BtnGo.interactable = true;
        Debug.Log("Create Room");
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayerPerRoom });
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);

        // ���ο� Ŭ���̾�Ʈ���� ���� �÷��̾ ǥ���ϴ� ���� �߰�
        PhotonView[] photonViews = FindObjectsOfType<PhotonView>();
        foreach (PhotonView photonView in photonViews)
        {
            // ���ο� Ŭ���̾�Ʈ���� PhotonView ����ȭ
            photonView.TransferOwnership(newPlayer);
        }
    }
} // end of class
