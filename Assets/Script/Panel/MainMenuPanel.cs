using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuPanel : BasePanel
{
    public Button btnChangeUser;
    public Text userNameText;
    public Text smallLevelText;
    public Button btnChallenge;

    protected override void Awake() {
        base.Awake();
        btnChangeUser.onClick.AddListener(OnBtnChangeUser);
        btnChallenge.onClick.AddListener(OnBtnChallenge);

        EventCenter.Instance.AddEventListener<UserData>(EventType.EventNewUserCreate, OnEventNewUserCreate);
        EventCenter.Instance.AddEventListener<string>(EventType.EventCurrentUserChange, OnEventCurrentUserChange);
    }

    private void Start() {
        if (BaseManager.instance.currentUserName == "")
        {
            BaseUIManager.Instance.OpenPanel(UIConst.NewUserPanel);
            return;
        }
        userNameText.text = BaseManager.instance.currentUserName;
        UserData userData = LocalConfig.LoadUserData(BaseManager.instance.currentUserName);
        if(userData != null)
        {
            smallLevelText.text = userData.level.ToString();
        }
    }

    void OnEventNewUserCreate(UserData userData)
    {
        userNameText.text = userData.name;
        smallLevelText.text = userData.level.ToString();
    }

    void OnEventCurrentUserChange(string curName)
    {
        userNameText.text = curName;
        if(LocalConfig.LoadUserData(curName) == null)
        {
            smallLevelText.text = "1";
            return;
        }
        smallLevelText.text = LocalConfig.LoadUserData(curName).level.ToString();
    }

    private void OnBtnChallenge()
    {
        SceneManager.LoadScene("Game");
    }

    private void OnBtnChangeUser()
    {
        // todo: 打开用户列表界面
        Debug.Log("OnBtnChangeUser");
        BaseUIManager.Instance.OpenPanel(UIConst.UserPanel);
    }

    public override void ClosePanel()
    {
        base.ClosePanel();
        EventCenter.Instance.RemoveEventListener<UserData>(EventType.EventNewUserCreate, OnEventNewUserCreate);
        EventCenter.Instance.RemoveEventListener<string>(EventType.EventCurrentUserChange, OnEventCurrentUserChange);
    }
}
