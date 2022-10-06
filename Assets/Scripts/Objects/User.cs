using Newtonsoft.Json.Linq;
using System.Collections.Generic;

[System.Serializable]
public class User : MessageHelper
{
    public string login;
    public string device_access;
    public bool consoleEnable;
    public List<string> devEui_list = new List<string>();
    public List<string> command_list = new List<string>();
    public RX_Settings rx_settings;

    public User(JObject jtObject)
    {
        jObject = jtObject;

        login = GetJValue("login");
        device_access = GetJValue("device_access");
        consoleEnable = GetJValueBool("consoleEnable");

        JToken jToken = jObject["devEui_list"];
        if (jToken != null)
        {
            foreach (string jtDevice in jToken)
            {
                devEui_list.Add(jtDevice);
            }
        }

        jToken = jObject["command_list"];
        if (jToken != null)
        {
            foreach (string jtDevice in jToken)
            {
                command_list.Add(jtDevice);
            }
        }

        JObject jtRXSettings = jtObject["rx_settings"] as JObject;
        if (jtRXSettings != null) rx_settings = new RX_Settings(jtRXSettings);
    }

    public bool CheckRights(string right)
    {
        if (command_list.Contains(right))
        {
            return true;
        }

        return false;
    }
}

[System.Serializable]
public class RX_Settings : MessageHelper
{
    public bool unsolicited;
    public string direction;
    public bool withMacCommands;

    public RX_Settings(JObject jtObject)
    {
        jObject = jtObject;

        unsolicited = GetJValueBool("unsolicited");
        direction = GetJValue("direction");
        withMacCommands = GetJValueBool("withMacCommands");
    }
}
