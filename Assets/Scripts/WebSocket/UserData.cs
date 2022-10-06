using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Network;

public class UserData : MonoBehaviour
{
    public static List<Device> Devices = new List<Device>();
    public static List<User> Users = new List<User>();
    public static List<Gateway> Gateways = new List<Gateway>();

    public static User CurrentUser;

    public static string Login;
    public static string Password;

    public static bool ActiveDefenceIsOn = false;
    
    public static List<Type_device> type_device_list = new List<Type_device>();
    public static List<Type_channel> type_channel_list = new List<Type_channel>();
    public static List<Status_channel> status_channel_list = new List<Status_channel>();

    // Temporary solution to test the 1 hot water, 1 cold water, 1 electricity and 1 gas pipe counters;
    /// <summary>
    /// Diode №2
    /// </summary>
    public static ResourceData data_HotWater = new ResourceData();
    /// <summary>
    /// Diode №4
    /// </summary>
    public static ResourceData data_ColdWater = new ResourceData();
    /// <summary>
    /// Diode №1
    /// </summary>
    public static ResourceData data_Electricity = new ResourceData();
    /// <summary>
    /// Diode №5
    /// </summary>
    public static ResourceData data_Gas = new ResourceData();

    public static string Adress;
    public static string UserName;

    public static bool data_WasLoad;
    

    public static void Init()
    {
        type_device_list = new List<Type_device>()
        {
            new Type_device() { id = 1, name = "СИ-11", count_channels = 4 }
        };
		//YSOCH - Your system of calculus here "m/ph, km/ph"
        type_channel_list = new List<Type_channel>()
        {
            new Type_channel() { id = 1, name = "Electricity", unit = "YSOCH", type = 1, span = "flash", clss = "icon-device-color_energy" },
            new Type_channel() { id = 2, name = "HotWaterCounet", unit = "YSOCH", type = 1, span = "tint", clss = "icon-device-color_water_hot" },
            new Type_channel() { id = 4, name = "ColdWaterCounter", unit = "YSOCH", type = 1, span = "tint", clss = "icon-device-color_water_ice" },
            new Type_channel() { id = 5, name = "GasPipeCounter", unit = "YSOCH", type = 1, span = "free-code-camp", clss = "icon-device-color_gas" },
            new Type_channel() { id = 6, name = "SecurityEnter", unit = "", type = 2, span = "shield", clss = "icon-device-color_protection" }
        };
        status_channel_list = new List<Status_channel>()
        {
            new Status_channel() { id = 1, name = "Online" },
            new Status_channel() { id = 2, name = "Offline" },
            new Status_channel() { id = 3, name = "OnRepair" },
            new Status_channel() { id = 4, name = "OnTets" }
        };
    }

    public static int GetChannelsCount(int deviceType)
    {
        foreach (Type_device td in type_device_list)
        {
            if (td.id == deviceType)
            {
                return td.count_channels;
            }
        }

        return 0;
    }

    public static bool CheckRights(string value)
    {
        foreach (var right in CurrentUser.command_list)
        {
            if (value.ToLower().Trim() == right.ToLower().Trim())
            {
                return true;
            }
        }

        return false;
    }

    public static Int32 DateStartDay()
    {
        DateTime dNow = DateTime.Now;
        TimeSpan ts = dNow - DateTime.UtcNow;
        DateTime startDay = new DateTime(dNow.Year, dNow.Month, dNow.Day, 0, 0, 0);
        startDay -= ts;

        return DateToInt(startDay);
    }

    public static Int32 DateToInt(DateTime date)
    {
        return (Int32)(date.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
    }

    public static string DateToString(DateTime date)
    {
        return DateToInt(date).ToString() + "000";
    }

    public static DateTime IntToDate(int time)
    {
        TimeSpan t = TimeSpan.FromSeconds(time);
        DateTime dt = new DateTime(1970, 1, 1);
        dt += t;

        return dt;
    }

    public static List<GraphData> GetCountersInfoByTime(TypeChannel typeChannel, DateTime from, DateTime to)
    {
        List<GraphData> values = new List<GraphData>();

        foreach (var device in Devices)
        {
            foreach (var channel in device.channels)
            {
                if (channel.GetTypeChannel == typeChannel)
                {
                    var hist = channel.history
                        .Where(x => x.get_time() > from && x.get_time() < to)
                        .ToList();

                    if (hist.Count > 0)
                    {
                        switch (typeChannel)
                        {
                            case TypeChannel.HotWaterCounet:

                                hist.ForEach(x =>
                                {
                                    float val = x.get_value(channel.init_value, channel.division, (channel.kt == 0) ? 1 : channel.kt);

                                    values.Add(new GraphData()
                                    {
                                        Value = val,
                                        Date = x.get_time()
                                    });
                                });

                                break;
                            case TypeChannel.ColdWaterCounter:

                                hist.ForEach(x =>
                                {
                                    float val = x.get_value(channel.init_value, channel.division, (channel.kt == 0) ? 1 : channel.kt);

                                    values.Add(new GraphData()
                                    {
                                        Value = val,
                                        Date = x.get_time()
                                    });
                                });

                                break;
                            case TypeChannel.Electricity:
                                
                                hist.ForEach(x =>
                                {
                                    float val = x.get_value(channel.init_value, channel.division, (channel.kt == 0) ? 1 : channel.kt);
                                    val = x.lite_get_value(val, (channel.kt == 0) ? 1 : channel.kt, channel.division);

                                    values.Add(new GraphData()
                                    {
                                        Value = val,
                                        Date = x.get_time()
                                    });
                                });

                                break;
                            case TypeChannel.GasPipeCounter:

                                hist.ForEach(x =>
                                {
                                    float val = x.get_value(channel.init_value, channel.division, (channel.kt == 0) ? 1 : channel.kt);

                                    values.Add(new GraphData()
                                    {
                                        Value = val,
                                        Date = x.get_time()
                                    });
                                });

                                break;
                        }

                        break;
                    }
                    
                }
            }

            if (values.Count > 0)
            {
                break;
            }
        }

        values = values.OrderBy(x => x.Date).ToList();

        return values;
    }
}

[System.Serializable]
public class Type_device
{
    public int id;
    public string name;
    public int count_channels;
}

[System.Serializable]
public class Type_channel
{
    public int id;
    public string name;
    public string unit;
    public int type;
    public string span;
    public string clss;
}

[System.Serializable]
public class Status_channel
{
    public int id;
    public string name;
}

[System.Serializable]
public class Type_color
{
    public int id;
    public string name;
    public string hex;
}


[System.Serializable]
public class ResourceData
{
    public Dictionary<string, Tuple<DateTime, float>> devicesDatas = new Dictionary<string, Tuple<DateTime, float>>();

    public void Add(string dei, DateTime date, float value)
    {
        if (devicesDatas.ContainsKey(dei))
        {
            if (devicesDatas[dei].Item1 < date)
            {
                devicesDatas[dei] = new Tuple<DateTime, float>(date, value);
            }
        }
        else
        {
            devicesDatas.Add(dei, new Tuple<DateTime, float>(date, value));
        }
    }

    public float GetAll()
    {
        return devicesDatas.Sum(d => d.Value.Item2);
    }
}
