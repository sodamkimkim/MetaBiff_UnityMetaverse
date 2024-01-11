using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Biff.BackgroundInfo;
public class CreateCharacManager : MonoBehaviour
{
    // ���� ������ ĳ���� 3�� �̻��� create charac���ϰ� ���ƾ���
    // ���� ������ ĳ���� ���� ��ɵ� ������ ��
    [SerializeField]
    private TMP_InputField nicknameField = null;
    [SerializeField]
    private Button btnCheckNickAvailability = null;

    [SerializeField]
    private Button btnCreateNewCharac = null;
    private CameraMoving cameraMoving = null;
    [SerializeField]
    private NewCharacDB db = null;
    [SerializeField]
    private TextMeshProUGUI userIdTxt = null;
    [SerializeField]
    private TextMeshProUGUI resultTxt = null;


    private bool nickNameAvailability = false;
    public struct newCharacInfo
    {
        public string userId { get; set; }
        public string modelName { get; set; }
        public string nickName { get; set; }

        public newCharacInfo(string _userId, string _modelName, string _nickName)
        {
            userId = _userId;
            modelName = _modelName;
            nickName = _nickName;
        } // end of constructor

        public override string ToString()
        {
            return "userId : " + userId + ", modelName : " + modelName + ", nickName : " + nickName;
        }
    } // end of struct newCharacInfo
    private void Awake()
    {
        userIdTxt.text = PlayerInfoManager.GetID();

        btnCreateNewCharac.onClick.AddListener(OnClickBtnCreateNewCharacter);
        cameraMoving = GetComponent<CameraMoving>();
        btnCheckNickAvailability.onClick.AddListener(OnClickBtnChecknickNameAvailablity);
        btnCreateNewCharac.interactable = false;
    }
    private void OnClickBtnChecknickNameAvailablity()
    {
        if (nicknameField.text != "")
        {
            string nickname = nicknameField.text;
            db.CheckNickNameAvailability(nickname);
        }
    }
    public void NickNameIsAvailableFunction(bool _abailability)
    {
        nickNameAvailability = _abailability;
        if (nickNameAvailability)
        {
            // Nick is available �� ���� process
            resultTxt.text = "NickName is available !";
            // �ʷϻ�
            resultTxt.color = new Color(0f, 255f, 0f);
            btnCreateNewCharac.interactable = true;

        }
        else if (nickNameAvailability == false)
        {
            // Nick is not available �� ���� process
            resultTxt.text = "NickName is already exit.";
            resultTxt.color = new Color(255f, 0f, 0f);
            btnCreateNewCharac.interactable = false;
        }
    }
    public void ManageInputData()
    { // nickname inputField OnValueChanged() �ݹ� �Լ�

        string nickname;
        nickname = nicknameField.text.Replace(" ", "");
        nicknameField.text = nickname;
        //btnNewCharac.interactable = true;
    }
    public void OnselectNickNamefield()
    {
        nickNameAvailability = false;
        nicknameField.text = "";

    }
    public void OnClickBtnCreateNewCharacter()
    {
        // �� �����ϰ�
        // �г��� �Է��ؼ� , ���� �г����̸� 
        // create character
        if (nicknameField.text != "")
        {
            PlayerInfoManager.SetModel(cameraMoving.GetNowModelName());
            PlayerInfoManager.SetNickname(nicknameField.text);

            newCharacInfo newCharacInfo = new newCharacInfo(PlayerInfoManager.GetID(), PlayerInfoManager.GetModel(), PlayerInfoManager.GetNickname());
            //Debug.Log(newCharacInfo.ToString());
            db.CreateNewCharacter(newCharacInfo);
            btnCreateNewCharac.interactable = false;
        }

    }
    public void CreateNewCharacSuccessFunction(bool _success)
    {
        if(_success)
        {
        resultTxt.text = "DONE!";
        // �ʷϻ�
        resultTxt.color = new Color(0f, 255f, 0f);
           SceneManager.LoadScene("2_MyCharacters");
        }
        else
        {
            resultTxt.text = "FAIL";
            resultTxt.color = new Color(255f, 0f, 0f);
        }
    }

} // end of class
