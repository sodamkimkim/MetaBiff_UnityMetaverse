using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class JoinManager : MonoBehaviour
{
    // id �Է�
    [SerializeField]
    private GameObject canvasCreateNewAccout = null;
    [SerializeField]
    private TMP_InputField idField = null;

    // id �ߺ�üũ
    [SerializeField]
    private Button btnIdAvailabilityCheck = null;
    [SerializeField]
    private TextMeshProUGUI idStatusMsgTxt = null;

    //Pw �Է�
    [SerializeField]
    private TMP_InputField pwField = null;

    // result message
    [SerializeField]
    private TextMeshProUGUI resultMsgTxt = null;

    // ȸ������ �ϱ� ��ư
    [SerializeField]
    private Button btnCreateNewAccount = null;

    // �α��� â���� ���ư���
    [SerializeField]
    private Button btnGotnLogin = null;

    [SerializeField]
    private DB db = null;

    private bool idAvailability = false;

    public struct UserJoinData
    {
        public string userId { get; set; }
        public string userPw { get; set; }
        public UserJoinData(string _id, string _pw)
        {
            userId = _id;
            userPw = _pw;
        } // end of constructor
    }// end of struct UserJoinData


    private void Awake()
    {

        idAvailability = false;
        // canvasCreateNewAccout.SetActive(false);
        btnIdAvailabilityCheck.onClick.AddListener(OnClickCheckIdAvailability);
        btnCreateNewAccount.onClick.AddListener(OnClickCreateNewAccBtn);
        btnGotnLogin.onClick.AddListener(OpenCanvasLogin);
    }


    private void OnClickCheckIdAvailability()
    {
        string id;
        id = idField.text.Replace(" ", "");
        idField.text = id;
        // ID Availability üũ idAvailability�÷��� ���� �� �ٲ���
        idStatusMsgTxt.text = "...Checking...";

        if (idField.text.Equals(""))
        {
            idStatusMsgTxt.text = "Please Enter ID!";
            idStatusMsgTxt.color = new Color(255f, 0f, 0f);
        }
        else
        {
            idStatusMsgTxt.color = new Color(0f, 255f, 0f);
            // db���� id��ȸ
            db.SearchUserID(id);
        }
    }
    public bool CheckIDAvailability(bool _idAvailability)
    {
        // ���̵� �ߺ�üũ ��ư �ݹ��Լ� -> db���� ���ؼ� ���� ���� �ٲ�
        return idAvailability = _idAvailability;
    }
    public void IDIsAvailableFunction(bool _idAvailability)
    {
        idAvailability = _idAvailability;
        if (idAvailability)
        {
            // ID is available �� ���� process
            idStatusMsgTxt.text = "Your ID is available !";
            // �ʷϻ�
            idStatusMsgTxt.color = new Color(0f, 255f, 0f);

        }
        else if (idAvailability == false)
        {
            // ID is not available �� ���� process
            idStatusMsgTxt.text = "The ID is already exit.";
            idStatusMsgTxt.color = new Color(255f, 0f, 0f);
        }
    }

    public void OpenCanvasCreateAccount()
    {
        canvasCreateNewAccout.SetActive(true);
    }
    public void OpenCanvasLogin()
    {
        canvasCreateNewAccout.SetActive(false);
        LoginManager.instance.OpenCanvasLogin();
    }
    public void OnClickCreateNewAccBtn()
    {
        // id�ߺ�üũ �������
        // CreateNewAccount Callback�Լ�
        string id = idField.text;
        string pw = pwField.text;
        UserJoinData userData = new UserJoinData(id, pw);
        if (idAvailability)
        {
            resultMsgTxt.text = "...Creating Your Account...";
            resultMsgTxt.color = new Color(0f, 255f, 0f);
            db.CreateNewAccount(userData);
            resultMsgTxt.text = "Done!";
            resultMsgTxt.color = new Color(0f, 255f, 0f);
        }
        else if(idAvailability==false)
        {
            resultMsgTxt.text = "Please Check ID!!";
            idStatusMsgTxt.color = new Color(255f, 0f, 0f);
        }
    }
    public void IdFieldOnSelect()
    {
        idAvailability = false;
        idField.text = "";
    }
    public void pwFieldOnSelect()
    {
        pwField.text = "";
    }




} // end of class
