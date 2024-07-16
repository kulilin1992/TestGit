using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserNameItem : MonoBehaviour
{
    private GameObject select;
    private Text txt;
    public UserData userData;
    private Button btn;
    private UserPanel parent;
    // Start is called before the first frame update

    // name / new
    public string ItemType = "name";

    private void Awake()
    {
        txt = transform.Find("Name").GetComponent<Text>();
        select = transform.Find("Select").gameObject;
        select.SetActive(false);
        btn = GetComponent<Button>();
        btn.onClick.AddListener(OnBtnNameItem);
    }

    public void InitItem(UserData userData, UserPanel userPanel)
    {
        this.userData = userData;
        txt.text = userData.name;
        parent = userPanel;
    }

    public void InitNewUserItem()
    {
        ItemType = "new";
        txt.text = "创建新用户";
    }

    void OnBtnNameItem()
    {
        if (ItemType == "name")
        {
            // todo: 修改列表的选中状态
            parent.CurName = userData.name;
        }
        else
        {
            BaseUIManager.Instance.OpenPanel(UIConst.NewUserPanel);
            // todo: 打开新用户界面
            // print("todo: 打开新用户界面！！！");
        }

    }
    public void RefreSelect()
    {
        select.SetActive(userData.name == parent.CurName);
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
