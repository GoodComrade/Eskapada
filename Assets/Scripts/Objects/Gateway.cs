using Newtonsoft.Json.Linq;

[System.Serializable]
public class Gateway : MessageHelper
{
    public string gatewayId;
    public string extraInfo;
    public bool active;
    /// <summary>
    /// If not "active".
    /// </summary>
    public int lastOnline;
    public int latency;
    public int downlinkChannel;
    public int maxPower;
    public bool rxOnly;
    /// <summary>
    /// If "rxOnly"==true.
    /// </summary>
    public string companionGateway;
    public GatewayPosition position;

    public Gateway(JObject jtGateway)
    {
        jObject = jtGateway;

        gatewayId = GetJValue("gatewayId");
        extraInfo = GetJValue("extraInfo");
        active = GetJValueBool("active");
        if (active) lastOnline = GetJValueInt("lastOnline");
        latency = GetJValueInt("latency");
        downlinkChannel = GetJValueInt("downlinkChannel");
        maxPower = GetJValueInt("maxPower");
        rxOnly = GetJValueBool("rxOnly");
        if (rxOnly) companionGateway = GetJValue("companionGateway");

        JObject jtGatewayPosition = jtGateway["position"] as JObject;
        if (jtGatewayPosition != null) position = new GatewayPosition(jtGatewayPosition);
    }
}

[System.Serializable]
public class GatewayPosition : MessageHelper
{
    public float longitude;
    public float latitude;
    public int altitude;

    public GatewayPosition(JObject jtObject)
    {
        jObject = jtObject;

        longitude = GetJValueFloat("longitud");
        latitude = GetJValueFloat("latitude");
        altitude = GetJValueInt("altitude");
    }
}