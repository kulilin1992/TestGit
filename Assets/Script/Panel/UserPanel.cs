using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserPanel : BasePanel
{
    public Button btnOk;
    public Button btnCancel;
    public Button btnDelete;
    public ScrollRect scroll;
    public GameObject UserNamePrefab;
    private List<UserData> testData;
    private Dictionary<string, UserNameItem> menuNameItems;
    private string curUserName;
    public string CurName
    {
        get { return curUserName; }
        set
        {
            curUserName = value;
            RefreshSelectState();
        }
    }
    public void RefreshSelectState()
    {
        // todo: 刷新所有子节点的选中状态
        foreach (UserNameItem item in menuNameItems.Values)
        {
            item.RefreSelect();
        }
    }

    protected override void Awake()
    {
        base.Awake();
        btnOk.onClick.AddListener(OnBtnOk);
        btnCancel.onClick.AddListener(OnBtnCancel);
        btnDelete.onClick.AddListener(OnBtnDelete);
        // testData = new List<UserData>();
        // testData.Add(new UserData("小棋1", 1));
        // testData.Add(new UserData("小棋2", 2));
        EventCenter.Instance.AddEventListener<UserData>(EventType.EventNewUserCreate, OnEventNewUserCreate);
        EventCenter.Instance.AddEventListener<UserData>(EventType.EventUserDelete, OnEventUserDelete);
        EventCenter.Instance.AddEventListener<string>(EventType.EventCurrentUserChange, OnEventCurrentUserChange);
    }
    private void Start()
    {
        RefreshMainPanel();
        // select item
        CurName = BaseManager.instance.currentUserName;
    }

    void OnEventNewUserCreate(UserData userData)
    {
        RefreshMainPanel();
    }

    void OnEventUserDelete(UserData userData)
    {
        RefreshMainPanel();
    }

    void OnEventCurrentUserChange(string curName)
    {
        CurName = curName;
    }

    void RefreshMainPanel()
    {
        // remove all children
        print(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>");
        foreach (Transform child in scroll.content)
        {
            // print(child);
            Destroy(child.gameObject);
        }
        // init all children
        menuNameItems = new Dictionary<string, UserNameItem>();
        foreach (UserData userData in LocalConfig.LoadAllUseData())
        {
            Transform prefab = Instantiate(UserNamePrefab).transform;
            prefab.SetParent(scroll.content, false);
            prefab.localPosition = Vector3.zero;
            prefab.localScale = Vector3.one;
            prefab.localRotation = Quaternion.identity;
            prefab.GetComponent<UserNameItem>().InitItem(userData, this);
            menuNameItems.Add(userData.name, prefab.GetComponent<UserNameItem>());
        }
        // add new user item
        Transform newPrefab = Instantiate(UserNamePrefab).transform;
        newPrefab.SetParent(scroll.content, false);
        newPrefab.localPosition = Vector3.zero;
        newPrefab.localScale = Vector3.one;
        newPrefab.localRotation = Quaternion.identity;
        newPrefab.GetComponent<UserNameItem>().InitNewUserItem();
    }

    private void OnBtnOk()
    {
        Debug.Log(">>>>>>>>> on btn ok");
        if (CurName != "")
        {
            BaseManager.instance.SetCurrentUserName(CurName);
            ClosePanel();
        }
        else
        {
            print(">>>>>>>>>>>>>>>>> Cur name is empty!");
        }
    }

    private void OnBtnCancel()
    {
        Debug.Log(">>>>>>>>> on btn cancel");
        ClosePanel();
    }

    private void OnBtnDelete()
    {
        Debug.Log(">>>>> OnBtnDelete");
        if (CurName == "")
        {
            return;
        }
        bool isSuccess = LocalConfig.ClearUserData(CurName);
        if (isSuccess && CurName == BaseManager.instance.currentUserName)
        {
            List<UserData> users = LocalConfig.LoadAllUseData();
            if (users.Count > 0)
            {
                BaseManager.instance.SetCurrentUserName(users[0].name);
            }
            else
            {
                BaseManager.instance.SetCurrentUserName("");
                BaseUIManager.Instance.OpenPanel(UIConst.NewUserPanel);
            }
        }
    }

    public override void ClosePanel()
    {
        base.ClosePanel();
        EventCenter.Instance.RemoveEventListener<UserData>(EventType.EventNewUserCreate, OnEventNewUserCreate);
        EventCenter.Instance.RemoveEventListener<UserData>(EventType.EventUserDelete, OnEventUserDelete);
        EventCenter.Instance.RemoveEventListener<string>(EventType.EventCurrentUserChange, OnEventCurrentUserChange);
    }

}
