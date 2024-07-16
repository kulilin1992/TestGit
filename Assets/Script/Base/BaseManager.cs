using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseManager
{
    private static BaseManager _instance;

    public static BaseManager instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new BaseManager();
            }
            return _instance;
        }
    }

    public ClientData GetClientData()
    {
        return LocalConfig.LoadClientData();
    }

    public string currentUserName = "";
    public BaseManager()
    {
        Debug.Log(">>>>>>>>>>>>>>>>>>>>>>>> GameStart >>>>>>>>>>>>>>>>>>>");
        currentUserName = GetClientData().curUserName;
    }

    public void SetCurrentUserName(string name)
    {
        currentUserName = name;
        // save
        ClientData clientData = GetClientData();
        clientData.curUserName = name;
        LocalConfig.SaveClientData(clientData);

        EventCenter.Instance.EventTrigger<string>(EventType.EventCurrentUserChange, currentUserName);
    }

}
