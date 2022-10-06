namespace Network
{
    [System.Serializable]
    public class AuthResponse : BaseResponse
    {
        public string token { get; set; }
        public string device_access { get; set; }
        public bool consoleEnable { get; set; }
        public string[] command_list { get; set; }
        public RXSettings rx_settings { get; set; }
    }

    [System.Serializable]
    public class RXSettings
    {
        public bool unsolicited { get; set; }
        public string direction { get; set; }
        public bool withMacCommands { get; set; }
    }
}