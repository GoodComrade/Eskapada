using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

using UnityEngine;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class DataLora
{
    public int type_package;
    public string comment;
    public string hex;
    public List<string> hex_array;
    public int charge;
    public List<string> switch_device;
    public int time;
    public float temperature;
    public Dictionary<string, string> sensors; // ???
    public int num_channel;
    public int count;
    public int type_archive;
    public int last_time;
    public Dictionary<string, int> archive;

    public DataLora()
    {
        hex_array = new List<string>();
        switch_device = new List<string>();
    }

    public bool _set_last_time()
    {
        try
        {
            var time = this.hex_array[9].ToString() + this.hex_array[8].ToString() + this.hex_array[7].ToString() + this.hex_array[6].ToString();
            this.last_time = Convert.ToInt32(time, 16);
            return true;
        }
        catch 
        {
            return false;
        }
    }

    public bool _set_hex(string hex)
    {
        this.hex = hex;
        this.hex_array = new List<string>();
        for (var i = 0; i < this.hex.Length - 1; i = i + 2)
        {
            this.hex_array.Add(this.hex.Substring(i, 2));
        }
        try
        {
            this.type_package = Convert.ToInt32(this.hex_array[0], 16);
            return true;
        }
        catch 
        {
            return false;
        }
    }

    public bool _set_type_archive()
    {
        try
        {
            this.type_archive = Convert.ToInt32(this.hex_array[5], 16);
            return true;
        }
        catch 
        {
            return false;
        }
    }

    public bool _set_charge()
    {
        try
        {
            this.charge = Convert.ToInt32(this.hex_array[1], 16);
            return true;
        }
        catch 
        {
            return false;
        }
    }
    public bool _set_num_channel()
    {
        try
        {
            this.num_channel = Convert.ToInt32(this.hex_array[3], 16);
            return true;
        }
        catch 
        {
            return false;
        }
    }
    public bool _set_count()
    {
        try
        {
            this.count = Convert.ToInt32(this.hex_array[4], 16);
            return true;
        }
        catch 
        {
            return false;
        }
    }
    public bool _set_temperature()
    {
        try
        {
            this.temperature = Convert.ToInt32(this.hex_array[7], 16);
            //Debug.Log("Temperature - " + temperature);
            if (this.temperature > 127)
            {
                var hex = this.hex_array[7];
                if (hex.Length % 2 != 0)
                {
                    hex = "0" + hex;
                }
                float num = Convert.ToInt32(hex, 16);
                float maxVal = (float)Math.Pow(2, hex.Length / 2 * 8);
                if (num > maxVal / 2 - 1)
                {
                    num = num - maxVal;
                }
                this.temperature = num;
            }
            return true;
        }
        catch 
        {
            return false;
        }
    }
    public bool _set_sensors_opt()
    {
        try
        {
            sensors = new Dictionary<string, string>();

            sensors.Clear();
            sensors.Add("sensor_1", Convert.ToInt32(this.hex_array[7] + this.hex_array[6] + this.hex_array[5] + this.hex_array[4], 16).ToString());
            sensors.Add("sensor_2", Convert.ToInt32(this.hex_array[11] + this.hex_array[10] + this.hex_array[9] + this.hex_array[8], 16).ToString());
            sensors.Add("sensor_3", Convert.ToInt32(this.hex_array[15] + this.hex_array[14] + this.hex_array[13] + this.hex_array[12], 16).ToString());
            sensors.Add("sensor_4", Convert.ToInt32(this.hex_array[19] + this.hex_array[18] + this.hex_array[17] + this.hex_array[16], 16).ToString());
            return true;
        }
        catch 
        {
            return false;
        }
    }
    public bool _set_sensors()
    {
        try
        {
            sensors = new Dictionary<string, string>();

            sensors.Clear();
            sensors.Add("sensor_1", Convert.ToInt32(this.hex_array[11] + this.hex_array[10] + this.hex_array[9] + this.hex_array[8], 16).ToString());
            sensors.Add("sensor_2", Convert.ToInt32(this.hex_array[15] + this.hex_array[14] + this.hex_array[13] + this.hex_array[12], 16).ToString());
            sensors.Add("sensor_3", Convert.ToInt32(this.hex_array[19] + this.hex_array[18] + this.hex_array[17] + this.hex_array[16], 16).ToString());
            sensors.Add("sensor_4", Convert.ToInt32(this.hex_array[23] + this.hex_array[22] + this.hex_array[21] + this.hex_array[20], 16).ToString());
            return true;
        }
        catch 
        {
            return false;
        }
    }
    public bool _set_switch_device()
    {
        try
        {
            //this.switch_device = Convert.ToInt32(this.hex_array[2], 16).ToString(2).split("").reverse().splice(0, 6);
            var chrArr = Convert.ToByte(this.hex_array[2], 16).ToString().ToCharArray();
            this.switch_device.Clear();

            for (int i = chrArr.Length - 1; i >= 0; i--)
            {
                if (this.switch_device.Count < 6)
                {
                    this.switch_device.Add(chrArr[i].ToString());
                }
            }
            
            if (this.switch_device.Count < 6)
            {
                while (this.switch_device.Count < 6)
                {
                    this.switch_device.Add("0");
                }
            }

            return true;
        }
        catch 
        {
            return false;
        }
    }
    public bool _set_archive()
    {
        archive = new Dictionary<string, int>();

        for (var i = 10; i < this.hex_array.Count - 3; i = i + 4)
        {
            int time = this.last_time * 1000;

            if (this.type_archive == 0)
            {
                
                DateTime dt = UserData.IntToDate(time).Subtract(new TimeSpan(0, 1, 0, 0, 0));
                time = UserData.DateToInt(dt);

                //time = moment(time).subtract((i - 10) / 4, "hour").unix();
            }
            else if (this.type_archive == 1)
            {
                DateTime dt = UserData.IntToDate(time).Subtract(new TimeSpan(1, 0, 0, 0, 0));
                time = UserData.DateToInt(dt);

                //time = moment(time).subtract((i - 10) / 4, "day").unix();
            }
            else if (this.type_archive == 2)
            {
                DateTime dt = UserData.IntToDate(time).Subtract(new TimeSpan(30, 0, 0, 0, 0));
                time = UserData.DateToInt(dt);

                //time = moment(time).subtract((i - 10) / 4, "month").unix();
            }
            else if (this.type_archive == 3)
            {

            }
            else
            {
                return false;
            }
            this.archive.Add(this.hex_array[i + 3].ToString() + this.hex_array[i + 2].ToString() + this.hex_array[i + 1].ToString() + this.hex_array[i].ToString(), time);
        }

        return true;
    }

    public bool _set_time()
    {
        try
        {
            var time = this.hex_array[6].ToString() + this.hex_array[5].ToString() + this.hex_array[4].ToString() + this.hex_array[3].ToString();
            this.time = Convert.ToInt32(time, 16);
            return true;
        }
        catch 
        {
            return false;
        }
    }
    public bool set_package_3()
    {
        if (this._set_charge())
        {
            if (this._set_switch_device())
            {
                if (this._set_num_channel())
                {
                    if (this._set_count())
                    {
                        if (this._set_type_archive())
                        {
                            if (this._set_last_time())
                            {
                                this._set_archive();
                                //this.comment = JsonConvert.SerializeObject(this);
                            }
                            else
                            {
                                return false;
                            }
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
        return true;
    }
    public bool set_package_1()
    {
        if (this._set_charge())
        {
            if (this._set_switch_device())
            {
                if (this._set_time())
                {
                    if (this._set_temperature())
                    {
                        if (this._set_sensors())
                        {
                            //this.comment = JsonConvert.SerializeObject(this);
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

        }
        else
        {
            return false;
        }
        return true;
    }
    public bool set_package_2()
    {
        if (this._set_charge())
        {
            if (this._set_switch_device())
            {
                if (this._set_num_channel())
                {
                    //this.comment = JsonConvert.SerializeObject(this);
                    this._set_sensors_opt();
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
        return true;
    }
    public bool back_door(string text)
    {
        this.comment = text;
        return true;
    }
    public bool set_data(string hex)
    {
        if (this._set_hex(hex))
        {

            switch (this.type_package)
            {
                case 1:
                    return this.set_package_1();
                case 2:
                    return this.set_package_2();
                case 3:
                    return this.set_package_3();
                default:
                    return false;
            }
        }
        else
        {
            return false;
        }
    }
    public DataLora get_data()
    {
        var result = new DataLora();

        switch (this.type_package)
        {
            case 1:
                result.type_package = this.type_package;
                result.charge = this.charge;
                result.switch_device = this.switch_device;
                result.time = this.time;
                result.temperature = this.temperature;
                result.sensors = this.sensors;
                result.hex = this.hex;
                return result;
            case 2:
                result.type_package = this.type_package;
                result.charge = this.charge;
                result.switch_device = this.switch_device;
                result.num_channel = this.num_channel;
                result.hex = this.hex;
                return result;
            case 3:
                result.type_package = this.type_package;
                result.charge = this.charge;
                result.switch_device = this.switch_device;
                result.num_channel = this.num_channel;
                result.count = this.count;
                result.type_archive = this.type_archive;
                result.last_time = this.last_time;
                result.hex = this.hex;
                result.archive = this.archive;
                return result;
            default:
                return null;
        }
    }

}
