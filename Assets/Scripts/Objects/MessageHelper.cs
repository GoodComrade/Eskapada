using Newtonsoft.Json.Linq;

public class MessageHelper
{
    public JObject jObject;

    public string GetJValue(string value)
    {
        JToken jToken;

        if (jObject.TryGetValue(value, out jToken))
        {
            return jToken.ToString();
        }

        return string.Empty;
    }

    public int GetJValueInt(string value)
    {
        string str = GetJValue(value);

        if (!string.IsNullOrEmpty(str))
        {
            int k = 0;
            int.TryParse(str, out k);
            return k;
        }
        else return 0;
    }

    public long GetJValueLong(string value)
    {
        string str = GetJValue(value);

        if (!string.IsNullOrEmpty(str))
        {
            long k = 0;
            long.TryParse(str, out k);
            return k;
        }
        else return 0;
    }

    public float GetJValueFloat(string value)
    {
        string str = GetJValue(value);

        try
        {
            float k = float.Parse(str, System.Globalization.CultureInfo.CurrentCulture);
            return k;
        }
        catch
        {
            return 0.0f;
        }
    }

    public bool GetJValueBool(string value)
    {
        string str = GetJValue(value).ToLower();

        if (str == "true")
        {
            return true;
        }

        return false;
    }
}