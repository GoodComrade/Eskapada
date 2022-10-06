namespace Network
{
    [System.Serializable]
    public class DeviceAppdataResponse : BaseResponse
    {

    }

    [System.Serializable]
    public class Device
    {
        public string devEui { get; set; }
        public string devName { get; set; }

    }
}
