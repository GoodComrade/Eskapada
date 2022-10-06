using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

/// <summary>
/// Для того, кто будет читать этот код:
/// Я сильно извиняюсь за то, что все наименования классов/функций/переменных с маленьких букв и нет описаний. 
/// Данный код - адаптация web-приложения, которое писал какой-то рукожоп.
/// Пока я разбирался с кодом на js, у меня не хватило времени переписать все названия.
/// Удачи тебе.
/// </summary>
public class cell_history : MessageHelper
{
    public bool ack;
    public string data; //?
    public string macData; //?
    public string dr;
    public int fcnt;
    public int freq;
    public string gatewayId; //?
    public int port;
    public int rssi;
    public float snr;
    public long ts;
    public int type;
    public string packetStatus; //?
    public int charge;
    public int sensor;
    public float temperature;
    public int time;
    public List<string> switch_device = new List<string>();

    public cell_history(JObject mess, DataLora data, int num_sensor)
    {
        jObject = mess;
        
        try
        {
            this.ts = GetJValueLong("ts");
            this.ack = GetJValueBool("ack");
            this.fcnt = GetJValueInt("fcnt");
            this.port = GetJValueInt("port");
            this.freq = GetJValueInt("freq");
            this.dr = GetJValue("dr");
            this.temperature = data.temperature;
            this.rssi = GetJValueInt("rssi");
            this.snr = GetJValueFloat("snr");
            this.charge = data.charge;
            this.switch_device = data.switch_device;
            this.time = data.time;
            this.type = data.type_package;
            this.sensor = int.Parse(data.sensors["sensor_" + num_sensor]);
        }
        catch
        {
            
        }
    }

    public string get_timeStr()
    {
        return UserData.IntToDate(this.time).ToString();
        //return moment.unix(this.time).format("LLL").replace(',', "");
    }

    public DateTime get_time()
    {
        return UserData.IntToDate(this.time);
        //return moment.unix(this.time).format("LLL").replace(',', "");
    }

    public string get_ts()
    {
        return UserData.IntToDate(this.time).ToString();
        //return moment(this.ts).format('LLL').replace(',', '');
    }

    public float get_value(float init_val, int division, int kt)
    {
        return round_size(((init_val * division + this.sensor) / division) * kt, 3);
    }

    public float lite_get_value(float init, int kt, int division)
    {
        return round_size(((sensor / division) + init) * kt, 3);
    }

    public float round_size(float num, float dec)
    {
        if (dec <= 0)
        {
            dec = 1;
        }
        dec = Mathf.Pow(10, dec);
        return (Mathf.Round(num * dec) / dec);
    }
}