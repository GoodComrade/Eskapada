namespace Network
{
    [System.Serializable]
    public class BaseResponse
    {
        public string cmd { get; set; }
        public bool status { get; set; }
        public string err_string { get; set; }
    }
}
