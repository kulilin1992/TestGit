// 用于文件读写
using System.IO;
// 用于json序列化和反序列化
using Newtonsoft.Json;
// Application.persistentDataPath配置在这里
using UnityEngine;
// 修改0：使用字典命名空间
using System.Collections.Generic;

public class LocalConfig
{

    // 修改1：增加usersData缓存数据
    public static Dictionary<string, UserData> usersData = new Dictionary<string, UserData>();
    public static ClientData clientData;
    // 加密1：选择一些用于亦或操作的字符（注意保密）
    public static char[] keyChars = {'a', 'b', 'c', 'd', 'e'};

    // 加密2： 加密方法
    public static string Encrypt(string data)
    {
        char [] dataChars = data.ToCharArray();
        for (int i=0; i<dataChars.Length; i++)
        {
            char dataChar = dataChars[i];
            char keyChar = keyChars[i % keyChars.Length];
            // 重点： 通过亦或得到新的字符
            char newChar = (char)(dataChar ^ keyChar);
            dataChars[i] = newChar;
        }
        return new string(dataChars);
    }

    // 加密3： 解密方法
    public static string Decrypt(string data)
    {
        return Encrypt(data);
    }

    // 保存用户数据文本
    public static void SaveUserData(UserData userData)
    {
        // 在persistentDataPath下创建一个/users文件夹，方便管理
        if(!File.Exists(Application.persistentDataPath + "/users"))
        {
            System.IO.Directory.CreateDirectory(Application.persistentDataPath + "/users");
        }

        // 修改2：保存缓存数据
        usersData[userData.name] = userData;

        // 转换用户数据为JSON字符串
        string jsonData = JsonConvert.SerializeObject(userData);
// #if UNITY_EDITOR
//         // 加密4
//         jsonData = Encrypt(jsonData);
// #endif
        // 将JSON字符串写入文件中（文件名为userData.name）
        File.WriteAllText(Application.persistentDataPath + string.Format("/users/{0}.json", userData.name), jsonData);
    }

    // 读取用户数据到内存
    public static UserData LoadUserData(string userName)
    {
        // 修改3： 率先从缓存中取数据，而不是从文本文件中读取
        if(usersData.ContainsKey(userName))
        {
            return usersData[userName];
        }

        string path = Application.persistentDataPath + string.Format("/users/{0}.json", userName);
        // 检查用户配置文件是否存在
        if(File.Exists(path))
        {
            // 从文本文件中加载JSON字符串
            string jsonData = File.ReadAllText(path);
// #if UNITY_EDITOR
//             // 加密5
//             jsonData = Decrypt(jsonData);
// #endif
            // 将JSON字符串转换为用户内存数据
            UserData userData = JsonConvert.DeserializeObject<UserData>(jsonData);
            return userData;
        }
        else
        {
            return null;
        }
    }

    public static List<UserData> LoadAllUseData()
    {
        string folderPath = Application.persistentDataPath + "/users";
        DirectoryInfo folder = new DirectoryInfo(folderPath);
        List<UserData> users = new List<UserData>();
        FileInfo[] allFiles = folder.GetFiles("*.json");
        // 先检查内存
        if (allFiles.Length == usersData.Count)
        {
            foreach (UserData userData in usersData.Values)
            {
                users.Add(userData);
            }
            return users;
        }
        // 再从硬盘加载
        foreach (FileInfo file in allFiles)
        {
            UserData userData = LoadUserData(file.Name.Split('.')[0]);
            if (userData != null)
            {
                users.Add(userData);
            }
        }
        return users;
    }

    public static bool ClearUserData(string userName)
    {
        string path = Application.persistentDataPath + string.Format("/users/{0}.json", userName);
        if (File.Exists(path))
        {
            UserData oldUseData = LoadUserData(userName);
            File.Delete(path);
            if (usersData.ContainsKey(userName))
            {
                usersData.Remove(userName);
            }
            EventCenter.Instance.EventTrigger<UserData>(EventType.EventUserDelete, oldUseData);
            return true;
        }
        else
        {
            Debug.Log(">>>>>>>>>>>>>>>>>>>>>>>> 删除失败 >>>>>>>>>>>>>");
            return false;
        }
    }

    // 保存用户数据文本
    public static void SaveClientData(ClientData data)
    {
        clientData = data;

        // 转换用户数据为JSON字符串
        string jsonData = JsonConvert.SerializeObject(clientData);
// #if !UNITY_EDITOR
//         // 加密4
//         jsonData = Encrypt(jsonData);
// #endif
        // 将JSON字符串写入文件中（文件名为userData.name）
        File.WriteAllText(Application.persistentDataPath + "/client_data.json", jsonData);
    }

    // 读取用户数据到内存
    public static ClientData LoadClientData()
    {
        // 修改3： 率先从缓存中取数据，而不是从文本文件中读取
        if (clientData != null)
        {
            return clientData;
        }
        string path = Application.persistentDataPath + "/client_data.json";
        // 检查用户配置文件是否存在
        if (File.Exists(path))
        {
            // 从文本文件中加载JSON字符串
            string jsonData = File.ReadAllText(path);
// #if !UNITY_EDITOR
//             // 加密5
//             jsonData = Decrypt(jsonData);
// #endif
            // 将JSON字符串转换为用户内存数据
            ClientData clientData = JsonConvert.DeserializeObject<ClientData>(jsonData);
            return clientData;
        }
        else
        {
            clientData = new ClientData();
            string jsonData = JsonConvert.SerializeObject(clientData);
            File.WriteAllText(Application.persistentDataPath + "/client_data.json", jsonData);
            return clientData;
        }
    }
}


public class UserData
{
    public string name;
    public int level;
    public UserData(string name="", int level=1)
    {
        this.name = name;
        this.level = level;
    }
}

public class ClientData
{
    public string curUserName = "";
    public override string ToString()
    {
        return "curUserName: " + curUserName;
    }
}