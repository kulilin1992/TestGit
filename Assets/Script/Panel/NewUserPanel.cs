using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class NewUserPanel : BasePanel
{
    public Button BtnOk;
    public Button BtnCancel;
    public InputField inputField;
    private string inputString = "";
    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
        BtnOk.onClick.AddListener(OnBtnOk);
        BtnCancel.onClick.AddListener(OnBtnCancel);
        inputField.onValueChanged.AddListener(OnInputChange);
    }

    public void OnBtnOk()
    {
        print("OnBtnOk");
        if(inputString.Trim() == "")
        {
            print(">>>>>>>>>>>> input string is empty !!!");
            return;
        }
        else if(LocalConfig.LoadUserData(inputString) != null)
        {
            print(">>>>>>>>>>>> input string has exist !!!");
            return;
        }

        // 创建新用户
        UserData userData = new UserData();
        userData.name = inputString;
        userData.level = 1;
        LocalConfig.SaveUserData(userData);

        // 更改当前的用户选择
        BaseManager.instance.SetCurrentUserName(userData.name);

        // 广播新用户创建的消息
        EventCenter.Instance.EventTrigger<UserData>(EventType.EventNewUserCreate, userData);

        // 关闭界面
        ClosePanel();
    }
    public void OnInputChange(string value)
    {
        inputString = value;
    }
    public void OnBtnCancel()
    {
        print("OnBtnCancel");
        if(BaseManager.instance.currentUserName == "")
        {
            print(">>>>>>>>> BaseManager.instance.currentUserName is empty!!!!!!!");
            return;
        }
        ClosePanel();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
