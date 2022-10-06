using System.Linq;
using System.Collections.Generic;

using UnityEngine;

using Newtonsoft.Json.Linq;

[System.Serializable]
public class Device : MessageHelper
{
    public string devEui;
    public string devName;
    public string deviceType;
    public string fcnt_down;
    public string fcnt_up;
    public long last_data_ts;
    public int last_data_charge;
    public string name;
    public string adress1;

    public List<Channel> channels = new List<Channel>();

    public Device(JObject jtDevice)
    {
        jObject = jtDevice;

        devEui = GetJValue("devEui");
        devName = GetJValue("devName");
        deviceType = GetJValue("device_type");
        fcnt_down = GetJValue("fcnt_down");
        fcnt_up = GetJValue("fcnt_up");
        last_data_ts = GetJValueLong("last_data_ts");
        name = GetJValue("name");
        adress1 = GetJValue("adress1");

        if (!string.IsNullOrEmpty(deviceType))
        {
            int countChannels = UserData.GetChannelsCount(int.Parse(deviceType));
            if (countChannels > 50) countChannels = 50;

            for (int i = 1; i < countChannels + 1; i++)
            {
                JToken jt = jObject["other_info_" + i];
                if (jt != null)
                {
                    JObject jo = JObject.Parse(jt.ToString()) as JObject;
                    if (jo != null && jo.HasValues)
                    {
                        Channel ch = new Channel(jo);
                        channels.Add(ch);
                    }
                }
            }
        }
    }

    public void add_history(JObject obj)
    {
        jObject = obj;

        var newData = new DataLora();
        string ddta = GetJValue("data");
        newData.set_data(ddta);

        switch (newData.type_package)
        {
            case 1:
                for (int i = 0; i < channels.Count; i++)
                {
                    var res = new cell_history(obj, newData, channels[i].num_channel);
                    var hasHistory = channels[i].history.Where(x => x.get_timeStr() == res.get_timeStr()).FirstOrDefault() != null;

                    if (!hasHistory)
                    {
                        channels[i].history.Add(res);
                    }

                    if (res.ts >= last_data_ts)
                    {
                        last_data_ts = res.ts;
                        if (res.charge != 0)
                        {
                            last_data_charge = res.charge;
                        }
                    }
                }
                break;
            case 2:
                for (int i = 0; i < channels.Count; i++)
                {
                    var res = new cell_history(obj, newData, i + 1);
                    var hasHistory = channels[i].history.Where(x => x.get_timeStr() == res.get_timeStr()).FirstOrDefault() != null;

                    if (!hasHistory)
                    {
                        channels[i].history.Add(res);
                    }
                }
                break;
            case 3:
                int iterator = 0;
                foreach (var dict in newData.archive)
                {
                    newData.sensors["sensor_" + (newData.num_channel)] = dict.Key;
                    newData.time = dict.Value;

                    var res = new cell_history(obj, newData, iterator + 1);
                    var hasHistory = channels[iterator].history.Where(x => x.get_timeStr() == res.get_timeStr()).FirstOrDefault() != null;

                    if (!hasHistory)
                    {
                        channels[iterator].history.Add(res);
                    }

                    iterator++;
                }
                break;
            default:
                // не умеет ещё парсить такой пакет
                break;
        }

        FillUIWithDataFromChannel();
        FillUserInfoFromFirstDevice();

        UserData.data_WasLoad = true;
    }

    /// <summary>
    /// Временное решение. Считаем, что у пользователя только по одному датчику каждого типа.
    /// </summary>
    public void FillUIWithDataFromChannel()
    {
        for (int i = 0; i < channels.Count; i++)
        {
            switch (channels[i].type_channel)
            {
                case 1:
                    if (channels[i].history.Count > 0)
                    {
                        var last = channels[i].history.OrderByDescending(h => h.get_time()).First();
                        /*float initVal = last.get_value(channels[i].init_value, channels[i].division,
                            (channels[i].kt == 0) ? 1 : channels[i].kt);
                        UserData.data_Electricity.Add(devEui, last.get_time(), 
                            last.lite_get_value(initVal,(channels[i].kt == 0) ? 1 : channels[i].kt, channels[i].division));
                        */
                        UserData.data_Electricity.Add(devEui, last.get_time(),
                            last.get_value(channels[i].init_value, channels[i].division, (channels[i].kt == 0) ? 1 : channels[i].kt));
                        //UIDebug.Log("Электричество: " + channels[i].history[0].lite_get_value(initVal,
                        //                (channels[i].kt == 0) ? 1 : channels[i].kt, channels[i].division));
                    }
                    
                    break;
                case 2:
                    if (channels[i].history.Count > 0)
                    {
                        /*var last = channels[i].history.OrderByDescending(h => h.get_time()).First();
                        UserData.data_HotWater.Add(devEui, last.get_time(),
                            last.get_value(channels[i].init_value, (channels[i].kt == 0) ? 1 : channels[i].kt, channels[i].division));
                        */
                        var last = channels[i].history.OrderByDescending(h => h.get_time()).First();
                        UserData.data_HotWater.Add(devEui, last.get_time(),
                            last.get_value(channels[i].init_value, channels[i].division, (channels[i].kt == 0) ? 1 : channels[i].kt));
                        //Debug.Log("Горячая вода: " + channels[i].history[0].get_value(channels[i].init_value,
                        //              channels[i].division, (channels[i].kt == 0) ? 1 : channels[i].kt));
                    }
                        

                    break;
                case 4:
                    if (channels[i].history.Count > 0)
                    {
                        var last = channels[i].history.OrderByDescending(h => h.get_time()).First();
                        UserData.data_ColdWater.Add(devEui, last.get_time(),
                            last.get_value(channels[i].init_value,channels[i].division, (channels[i].kt == 0) ? 1 : channels[i].kt));
                        
                        //Debug.Log("Холодная вода: " + channels[i].history[0].get_value(channels[i].init_value,
                        //              channels[i].division, (channels[i].kt == 0) ? 1 : channels[i].kt));
                    }

                    break;
                case 5:
                    
                    if (channels[i].history.Count > 0)
                    {
                        /*var last = channels[i].history.OrderByDescending(h => h.get_time()).First();
                        UserData.data_Gas.Add(devEui, last.get_time(),
                            last.get_value(channels[i].init_value, channels[i].division, (channels[i].kt == 0) ? 1 : channels[i].kt));
                        */
                        var last = channels[i].history.OrderByDescending(h => h.get_time()).First();
                        UserData.data_Gas.Add(devEui, last.get_time(),
                            channels[i].history[0].get_value(channels[i].init_value, channels[i].division, (channels[i].kt == 0) ? 1 : channels[i].kt));

                        //Debug.Log("Найдено устройство газа: " + channels[i].history[0].get_value(channels[i].init_value,
                        //              channels[i].division, (channels[i].kt == 0) ? 1 : channels[i].kt));
                    }
             
                    break;
                default:

                    break;
            }
        }
    }

    public void FillUserInfoFromFirstDevice()
    {
        if (channels.Count > 0)
        {
            UserData.UserName = channels[0].name;
            UserData.Adress = channels[0].address_level_1 + ", " + channels[0].name_level_1 + ", " + channels[0].level_2;
        }
    }
}

[System.Serializable]
public class Channel : MessageHelper
{
    public string address_level_1;
    public string name_level_1;
    public string level_2;
    public string name;
    public int num_channel;
    public int status;
    public int color;
    public int info_channel;
    public float init_value;
    public int kt;
    //public last_date
    public int calib_interval;
    public string model;
    public string serial;
    public int type_channel;
    public cell_history temp_history;
    public List<cell_history> history = new List<cell_history>();
    public int division;
    
    public TypeChannel GetTypeChannel
    {
        get
        {
            var type_Channel = UserData.type_channel_list.Where(x => x.id == type_channel).FirstOrDefault();

            if (type_Channel != null)
            {
                switch (type_Channel.name)
                {
                    case "Электросчетчик": return TypeChannel.Электросчетчик;
                    case "Счетчик горячей воды": return TypeChannel.ГорячаяВода;
                    case "Счетчик холодной воды": return TypeChannel.ХолоднаяВода;
                    case "Cчетчик газа": return TypeChannel.Газ;
                    case "Охранный вход": return TypeChannel.ОхранныйВход;
                    default: return TypeChannel.unknown;
                }
            }
            else
            {
                return TypeChannel.unknown;
            }
        }
    }

    public Channel(JToken _jObject)
    {
        jObject = _jObject as JObject;

        address_level_1 = GetJValue("address_level_1");
        name_level_1 = GetJValue("name_level_1");
        level_2 = GetJValue("level_2");
        name = GetJValue("name");
        num_channel = GetJValueInt("num_channel");
        status = GetJValueInt("status");
        color = GetJValueInt("color");
        info_channel = GetJValueInt("info_channel");
        init_value = GetJValueFloat("init_value");
        kt = GetJValueInt("kt");
        calib_interval = GetJValueInt("calib_interval");
        model = GetJValue("model");
        serial = GetJValue("serial");
        type_channel = GetJValueInt("type_channel");
        division = GetJValueInt("division");
    }
}

public enum TypeChannel
{
    unknown,
    Электросчетчик,
    ГорячаяВода,
    ХолоднаяВода,
    Газ,
    ОхранныйВход
}