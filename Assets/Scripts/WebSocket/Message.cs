using UnityEngine;
using Newtonsoft.Json;

public class Message : MonoBehaviour
{
    public static string Auth (string userLogin, string userPassword)
    {
        return JsonConvert.SerializeObject(new
        {
            login = userLogin,
            password = userPassword,
            cmd = "auth_req"
        });
    }

    public static string TokenAuth
    {
        get
        {
            return JsonConvert.SerializeObject(new
            {
                cmd = "token_auth_req",
                token = Requester4net.Token
            });
        }
    }

    public static string Disconnect
    {
        get
        {
            return JsonConvert.SerializeObject(new
            {
                cmd = "close_auth_req",
                token = Requester4net.Token
            });
        }
    }

    public static string GetUsers
    {
        get
        {
            return JsonConvert.SerializeObject(new KeywordMessage() { cmd = "get_users_req", keyword = new string[] { "no_command_and_devEui" } });
        }
    }

    public static string GetDevices
    {
        get
        {
            return JsonConvert.SerializeObject(new KeywordMessage() { cmd = "get_device_appdata_req", keyword = new string[] { "add_data_info" } });
        }
    }

    public static string GetGateways
    {
        get
        {
            return JsonConvert.SerializeObject(new { cmd = "get_gateways_req" });
        }
    }

    public static string GetDeviceData(string _devEui, object _select)
    {
        object reqObj = new { cmd = "get_data_req", devEui = _devEui };
        if (_select != null)
        {
            reqObj = new { cmd = "get_data_req", devEui = _devEui, select = _select };
        }
        return JsonConvert.SerializeObject(reqObj);
    }

    public static string Data(string _devEui, string _data, int _port)
    {
        object reqObj = new { cmd = "send_data_req",
                              data_list = new object[] {
                                  new { devEui = _devEui, data = _data, port = _port, ack = false }
                              } };

        return JsonConvert.SerializeObject(reqObj);
    }
}
