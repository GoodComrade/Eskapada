using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChannelFillerUI : MonoBehaviour {
    
    public Text ui_UserName;
    public Text ui_Adress;

    public Toggle ui_ActiveDefenceToggle;


    private IEnumerator Start()
    {
        int wait = 5;
        while (!UserData.data_WasLoad || wait > 0)
        {
            yield return new WaitForSeconds(1);
            wait--;
        }

        ui_UserName.text = UserData.UserName;
        ui_Adress.text = UserData.Adress;

        ui_ActiveDefenceToggle.isOn = UserData.ActiveDefenceIsOn;

        StartService();
    }

    private void StartService()
    {
        try
        {
            string devType = string.Empty;
            foreach (Device device in UserData.Devices)
            {
                if (!string.IsNullOrEmpty(device.devEui))
                {
                    
                    foreach (Channel channel in device.channels)
                    {
                        if (channel.type_channel == 6)
                        {
                            devType += device.devEui + "|";
                            break;
                        }
                    }
                }
            }
            if (devType.Length > 0)
            {
                devType = devType.Substring(0, devType.Length - 1);
            }
            //UIDebug.Log(devType);
            ServiceStarter.StartService(UserData.Login, UserData.Password, UserData.ActiveDefenceIsOn, devType);
        }
        catch (Exception exception)
        {
            UIDebug.Log(exception.Message);
        }
    }

    public void ClearUserAuthData()
    {
        Requester4net.ChangeUser();
    }

    public void OnChangeActiveDefenceToggle()
    {
        bool isDefence = ui_ActiveDefenceToggle.isOn;
        UserData.ActiveDefenceIsOn = isDefence;
        PlayerPrefs.SetString((UserData.Login + ":ActiveDefence"), isDefence ? "true" : "false");
        PlayerPrefs.Save();

        StartService();
    }

    public void WaterHotOn(int port, bool activate)
    {
        Requester4net.DataControl(2, true, 2);
    }
}
