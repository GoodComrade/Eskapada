using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using WebSocket4Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Network;

using Assets.SimpleAndroidNotifications;

public class Requester4net : MonoBehaviour
{
    public static event Action OnAllDevicesLoaded = delegate { };

    public static bool AlreadyGetDataFromAllDevices { get { return dataRecivedFromNDevices >= neededDevicesCount; } }

    private static WebSocket websocket = new WebSocket("ws://185.41.161.132:8002");

    private static string _connectionUri = "ws://185.41.161.132:8002";

    public static string Token;

    public delegate void InvalidLoginPwd();
    public static event InvalidLoginPwd invalidLoginPwd;
    public delegate void SuccessfulAuth();
    public static event SuccessfulAuth successfulAuth;
    public delegate void OnSocketConnected();
    public static event SuccessfulAuth onSocketConnected;
    public delegate void SuccessfulGetDevices();
    public static event SuccessfulGetDevices onSuccessfulGetDevices;

    private static bool isInvalidLoginPwd;
    private static bool isSuccessfulAuth;
    private static bool isSocketConnected;
    private static bool isSuccessfulGetDevices;
    
    public static NotificationController _notificationController;
    public static Requester4net Instance;


    public delegate void GetDataResponse();
    public static event GetDataResponse OnDataResponse;

    private static int neededDevicesCount;
    private static int dataRecivedFromNDevices = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start ()
    {
        DontDestroyOnLoad(gameObject);
        UserData.Init();

        onSocketConnected += TryLoginFromPlayerPrefs;
        successfulAuth += SaveLoginToPlayerPrefs;

        ConnectToServer(_connectionUri);

        _notificationController = GetComponent<NotificationController>();

        
    }

    private void Update()
    {
        if (isInvalidLoginPwd && invalidLoginPwd != null) { invalidLoginPwd(); isInvalidLoginPwd = false; }
        if (isSuccessfulAuth && successfulAuth != null) { successfulAuth(); isSuccessfulAuth = false; }
        if (isSocketConnected && onSocketConnected != null) { onSocketConnected(); isSocketConnected = false; }
        if (isSuccessfulGetDevices && onSuccessfulGetDevices != null) { onSuccessfulGetDevices(); isSuccessfulGetDevices = false; }
    }

    private static void ClearData()
    {
        _connectionUri = "ws://185.41.161.132:8002";
        Token = string.Empty;
        UserData.Devices.Clear();
        UserData.Gateways.Clear();
        UserData.Users.Clear();
        UserData.CurrentUser = null;
        UserData.data_WasLoad = false;
        UserData.ActiveDefenceIsOn = false;
    }

    private static void ConnectToServer(string connectionUri)
    {
        Instance.StartCoroutine(Instance.ConnectToServerRoutine(connectionUri));
    }

    private IEnumerator ConnectToServerRoutine(string connectionUri)
    {
        if (websocket == null)
        {
            websocket = new WebSocket(connectionUri);
        }
        else
        {
            while (websocket.State == WebSocketState.Connecting || websocket.State == WebSocketState.Closing)
            {
                Debug.Log(websocket.State);
                yield return new WaitForEndOfFrame();
            }
        }

        switch (websocket.State)
        {
            case WebSocketState.Closed:
            case WebSocketState.None:

                Debug.Log("Connected to WS");

                // можно присоединяться к серверу
                websocket = new WebSocket(connectionUri);
                websocket.Opened += new EventHandler(websocket_Opened);
                websocket.Error += websocket_Error;
                websocket.Closed += new EventHandler(websocket_Closed);
                websocket.MessageReceived += websocket_MessageReceived;
                websocket.Open();

                while (websocket.State == WebSocketState.Connecting || websocket.State == WebSocketState.Closing)
                {
                    yield return new WaitForEndOfFrame();
                }

                if (websocket.State != WebSocketState.Open)
                {
                    Debug.Log("Error " + websocket.State);
                    UIDebug.Log("Connection is not opened.");
                }

                break;
        }
    }

    public static void ChangeTemperature(int selectedTemperature)
    {
        Instance.StartCoroutine(Instance.ChangeTemperatureIE(selectedTemperature));
    }

    private IEnumerator ChangeTemperatureIE(int selectedTemperature)
    {

        if (selectedTemperature < 15)
        {
            Debug.LogWarning("Слишком маленькая температура!");
            yield break;
        }
        else
        {
            string devEui = GetDevEuiFromType(5);

            if (!string.IsNullOrEmpty(devEui))
            {
                int dataT = selectedTemperature - 18;
                int data2th = dataT / 7;
                int data3th = dataT % 7 + 3;

                switch(selectedTemperature)
                {
                    case 18:
                        yield return new WaitForSeconds(5f);

                        string data18 = "030202";
                        websocket.Send(Message.Data(devEui, data18, 2));
                        DebugDataMessage(Message.Data(devEui, data18, 2));

                        yield return new WaitForSeconds(7f);

                        data18 = "030203";
                        websocket.Send(Message.Data(devEui, data18, 2));
                        DebugDataMessage(Message.Data(devEui, data18, 2));

                        yield return new WaitForSeconds(7f);

                        for(int i = 0; i < 3; i++)
                        {
                            data18 = "030203";
                            websocket.Send(Message.Data(devEui, data18, 2));
                            DebugDataMessage(Message.Data(devEui, data18, 2));

                            yield return new WaitForSeconds(10f);
                        }
                        break;

                    case 19:
                        yield return new WaitForSeconds(5f);

                        string data19 = "030202";
                        websocket.Send(Message.Data(devEui, data19, 2));
                        DebugDataMessage(Message.Data(devEui, data19, 2));

                        yield return new WaitForSeconds(7f);

                        data19 = "030203";
                        websocket.Send(Message.Data(devEui, data19, 2));
                        DebugDataMessage(Message.Data(devEui, data19, 2));

                        yield return new WaitForSeconds(7f);

                        for (int i = 0; i < 3; i++)
                        {
                            data19 = "030204";
                            websocket.Send(Message.Data(devEui, data19, 2));
                            DebugDataMessage(Message.Data(devEui, data19, 2));
                            yield return new WaitForSeconds(10f);
                        }
                        break;

                    case 20:
                        yield return new WaitForSeconds(5f);

                        string data20 = "030202";
                        websocket.Send(Message.Data(devEui, data20, 2));
                        DebugDataMessage(Message.Data(devEui, data20, 2));

                        yield return new WaitForSeconds(7f);

                        data20 = "030203";
                        websocket.Send(Message.Data(devEui, data20, 2));
                        DebugDataMessage(Message.Data(devEui, data20, 2));

                        yield return new WaitForSeconds(7f);

                        for (int i = 0; i < 3; i++)
                        {
                            data20 = "030205";
                            websocket.Send(Message.Data(devEui, data20, 2));
                            DebugDataMessage(Message.Data(devEui, data20, 2));
                            yield return new WaitForSeconds(10f);
                        }
                        break;

                    case 21:
                        yield return new WaitForSeconds(5f);

                        string data21 = "030202";
                        websocket.Send(Message.Data(devEui, data21, 2));
                        DebugDataMessage(Message.Data(devEui, data21, 2));

                        yield return new WaitForSeconds(7f);

                        data21 = "030203";
                        websocket.Send(Message.Data(devEui, data21, 2));
                        DebugDataMessage(Message.Data(devEui, data21, 2));

                        yield return new WaitForSeconds(7f);

                        for (int i = 0; i < 3; i++)
                        {
                            data21 = "030206";
                            websocket.Send(Message.Data(devEui, data21, 2));
                            DebugDataMessage(Message.Data(devEui, data21, 2));
                            yield return new WaitForSeconds(10f);
                        }
                        break;

                    case 22:
                        yield return new WaitForSeconds(5f);

                        string data22 = "030202";
                        websocket.Send(Message.Data(devEui, data22, 2));
                        DebugDataMessage(Message.Data(devEui, data22, 2));

                        yield return new WaitForSeconds(7f);

                        data22 = "030203";
                        websocket.Send(Message.Data(devEui, data22, 2));
                        DebugDataMessage(Message.Data(devEui, data22, 2));

                        yield return new WaitForSeconds(7f);

                        for (int i = 0; i < 3; i++)
                        {
                            data22 = "030207";
                            websocket.Send(Message.Data(devEui, data22, 2));
                            DebugDataMessage(Message.Data(devEui, data22, 2));
                            yield return new WaitForSeconds(10f);
                        }
                        break;

                    case 23:
                        yield return new WaitForSeconds(5f);

                        string data23 = "030202";
                        websocket.Send(Message.Data(devEui, data23, 2));
                        DebugDataMessage(Message.Data(devEui, data23, 2));

                        yield return new WaitForSeconds(7f);

                        data23 = "030203";
                        websocket.Send(Message.Data(devEui, data23, 2));
                        DebugDataMessage(Message.Data(devEui, data23, 2));

                        yield return new WaitForSeconds(7f);

                        for (int i = 0; i < 3; i++)
                        {
                            data23 = "030208";
                            websocket.Send(Message.Data(devEui, data23, 2));
                            DebugDataMessage(Message.Data(devEui, data23, 2));
                            yield return new WaitForSeconds(10f);
                        }
                        break;

                    case 24:
                        yield return new WaitForSeconds(5f);

                        string data24 = "030202";
                        websocket.Send(Message.Data(devEui, data24, 2));
                        DebugDataMessage(Message.Data(devEui, data24, 2));

                        yield return new WaitForSeconds(7f);

                        data24 = "030203";
                        websocket.Send(Message.Data(devEui, data24, 2));
                        DebugDataMessage(Message.Data(devEui, data24, 2));

                        yield return new WaitForSeconds(7f);

                        for (int i = 0; i < 3; i++)
                        {
                            data24 = "030209";
                            websocket.Send(Message.Data(devEui, data24, 2));
                            DebugDataMessage(Message.Data(devEui, data24, 2));
                            yield return new WaitForSeconds(10f);
                        }
                        break;

                    case 25:
                        yield return new WaitForSeconds(5f);

                        string data25 = "030202";
                        websocket.Send(Message.Data(devEui, data25, 2));
                        DebugDataMessage(Message.Data(devEui, data25, 2));

                        yield return new WaitForSeconds(7f);

                        data25 = "030204";
                        websocket.Send(Message.Data(devEui, data25, 2));
                        DebugDataMessage(Message.Data(devEui, data25, 2));

                        yield return new WaitForSeconds(7f);

                        for (int i = 0; i < 3; i++)
                        {
                            data25 = "030203";
                            websocket.Send(Message.Data(devEui, data25, 2));
                            DebugDataMessage(Message.Data(devEui, data25, 2));
                            yield return new WaitForSeconds(10f);
                        }
                        break;

                    case 26:
                        yield return new WaitForSeconds(5f);

                        string data26 = "030202";
                        websocket.Send(Message.Data(devEui, data26, 2));
                        DebugDataMessage(Message.Data(devEui, data26, 2));

                        yield return new WaitForSeconds(7f);

                        data26 = "030204";
                        websocket.Send(Message.Data(devEui, data26, 2));
                        DebugDataMessage(Message.Data(devEui, data26, 2));

                        yield return new WaitForSeconds(7f);

                        for (int i = 0; i < 3; i++)
                        {
                            data26 = "030204";
                            websocket.Send(Message.Data(devEui, data26, 2));
                            DebugDataMessage(Message.Data(devEui, data26, 2));
                            yield return new WaitForSeconds(10f);
                        }
                        break;

                    case 27:
                        yield return new WaitForSeconds(5f);

                        string data27 = "030202";
                        websocket.Send(Message.Data(devEui, data27, 2));
                        DebugDataMessage(Message.Data(devEui, data27, 2));

                        yield return new WaitForSeconds(7f);

                        data27 = "030204";
                        websocket.Send(Message.Data(devEui, data27, 2));
                        DebugDataMessage(Message.Data(devEui, data27, 2));

                        yield return new WaitForSeconds(7f);

                        for (int i = 0; i < 3; i++)
                        {
                            data27 = "030205";
                            websocket.Send(Message.Data(devEui, data27, 2));
                            DebugDataMessage(Message.Data(devEui, data27, 2));
                            yield return new WaitForSeconds(10f);
                        }
                        break;

                    case 28:
                        yield return new WaitForSeconds(5f);

                        string data28 = "030202";
                        websocket.Send(Message.Data(devEui, data28, 2));
                        DebugDataMessage(Message.Data(devEui, data28, 2));

                        yield return new WaitForSeconds(7f);

                        data28 = "030204";
                        websocket.Send(Message.Data(devEui, data28, 2));
                        DebugDataMessage(Message.Data(devEui, data28, 2));

                        yield return new WaitForSeconds(7f);

                        for (int i = 0; i < 3; i++)
                        {
                            data28 = "030206";
                            websocket.Send(Message.Data(devEui, data28, 2));
                            DebugDataMessage(Message.Data(devEui, data28, 2));
                            yield return new WaitForSeconds(10f);
                        }
                        break;

                    case 29:
                        yield return new WaitForSeconds(5f);

                        string data29 = "030202";
                        websocket.Send(Message.Data(devEui, data29, 2));
                        DebugDataMessage(Message.Data(devEui, data29, 2));

                        yield return new WaitForSeconds(7f);

                        data29 = "030204";
                        websocket.Send(Message.Data(devEui, data29, 2));
                        DebugDataMessage(Message.Data(devEui, data29, 2));

                        yield return new WaitForSeconds(7f);

                        for (int i = 0; i < 3; i++)
                        {
                            data29 = "030207";
                            websocket.Send(Message.Data(devEui, data29, 2));
                            DebugDataMessage(Message.Data(devEui, data29, 2));
                            yield return new WaitForSeconds(10f);
                        }
                        break;

                    case 30:
                        yield return new WaitForSeconds(5f);

                        string data30 = "030202";
                        websocket.Send(Message.Data(devEui, data30, 2));
                        DebugDataMessage(Message.Data(devEui, data30, 2));

                        yield return new WaitForSeconds(7f);

                        data30 = "030204";
                        websocket.Send(Message.Data(devEui, data30, 2));
                        DebugDataMessage(Message.Data(devEui, data30, 2));

                        yield return new WaitForSeconds(7f);

                        for (int i = 0; i < 3; i++)
                        {
                            data30 = "030208";
                            websocket.Send(Message.Data(devEui, data30, 2));
                            DebugDataMessage(Message.Data(devEui, data30, 2));
                            yield return new WaitForSeconds(10f);
                        }
                        break;

                    default:
                        break;
                }
            }
        }
    }

    public static void DataControl(int port, bool on, int typeChannel)
    {
        string devEui = GetDevEuiFromType(typeChannel);

        if (!string.IsNullOrEmpty(devEui))
        {
            string end = (typeChannel == 2 || typeChannel == 4) ? "05" : (typeChannel == 1) ? "03" : "00";
            string waterData = "030" + (@on ? 1 : 2) + end;
            DebugDataMessage(Message.Data(devEui, waterData, port));
            websocket.Send(Message.Data(devEui, waterData, port));
        }

    }

    public static void SendData(int port, string data, int typeChannel)
    {
        string devEui = GetDevEuiFromType(typeChannel);

        if (!string.IsNullOrEmpty(devEui))
        {
            DebugDataMessage(Message.Data(devEui, data, port));
            websocket.Send(Message.Data(devEui, data, port));
        }
    }

    private static void DebugDataMessage(string message)
    {
        Debug.Log("<color=#0044b2>" + "Отправлено сообщение: " + message + "</color>");
    }

    public static void TryLoginFromPlayerPrefs()
    {
        if (PlayerPrefs.HasKey("login:password"))
        {
            string[] userAuthData = PlayerPrefs.GetString("login:password").Split(':');
            //UIDebug.Log(userAuthData[0] + ":" + userAuthData[1]);
            if (userAuthData.Length == 2)
            {
                Authorization(userAuthData[0], userAuthData[1]);
                UserData.ActiveDefenceIsOn = PlayerPrefs.HasKey(userAuthData[0] + ":ActiveDefence") && (PlayerPrefs.GetString(userAuthData[0] + ":ActiveDefence") == "true");
            }
        }
    }

    private static void SaveLoginToPlayerPrefs()
    {
        PlayerPrefs.SetString("login:password", UserData.Login + ":" + UserData.Password);
        PlayerPrefs.Save();
    }

    public static void ChangeUser()
    {
        ClearLoginInPlayerPrefs();
        ClearUserData();
        CloseConnection();
    }

    private static void ClearUserData()
    {
        UserData.Devices.Clear();
        UserData.Users.Clear();
        UserData.Gateways.Clear();

        UserData.CurrentUser = null;

        UserData.Login = null;
        UserData.Password = null;

        UserData.ActiveDefenceIsOn = false;

        UserData.type_device_list.Clear();
        UserData.type_channel_list.Clear();
        UserData.status_channel_list.Clear();

        UserData.data_Electricity = new ResourceData();
        UserData.data_ColdWater = new ResourceData();
        UserData.data_HotWater = new ResourceData();
        UserData.data_Gas = new ResourceData();

        UserData.Adress = null;
        UserData.UserName = null;

        UserData.data_WasLoad = false;

        UserData.Init();
    }

    public static void ClearLoginInPlayerPrefs()
    {
        PlayerPrefs.DeleteKey("login:password");
        PlayerPrefs.Save();
    }

    public static void Authorization(string login = "", string password = "")
    {
        UserData.Login = login;
        UserData.Password = password;
        if (websocket == null || websocket.State != WebSocketState.Open)
        {
            ConnectToServer(_connectionUri);
            websocket.Opened += WebsocketOnOpenedAuthorize;
        }
        else
        {
            websocket.Send(Message.Auth(UserData.Login, UserData.Password));
        }
    }

    private static void WebsocketOnOpenedAuthorize(object sender, EventArgs eventArgs)
    {
        // Авторизуемся на сервере
        websocket.Send(Message.Auth(UserData.Login, UserData.Password));
    }

    public static string GetDevEuiFromType(int typeChannel)
    {
        string devEui = string.Empty;

        foreach (Device device in UserData.Devices)
        {
            foreach (Channel channel in device.channels)
            {
                if (channel.type_channel == typeChannel)
                {
                    devEui = device.devEui;
                    break;
                }
            }
            if (!string.IsNullOrEmpty(devEui))
            {
                break;
            }
        }

        if (string.IsNullOrEmpty(devEui))
        {
            Debug.Log("<color=red>Не найдено такого девайса! Всего устройств " + UserData.Devices.Count + "</color>");
        }

        return devEui;
    }

    [ContextMenu("Получить информацию о всех устройствах")]
    public void ПолучитьИнформациюОВсехУстройствах()
    {
        // запрос полной информации о устройствах
        DateTime dateFrom = DateTime.UtcNow.Subtract(new TimeSpan(50, 0, 0, 0, 0));
        DateTime dateTo = DateTime.UtcNow.AddHours(10);

        ПолучитьИнформациюОВсехУстройствах(dateFrom, dateTo);
    }

    public static void ПолучитьИнформациюОВсехУстройствах(DateTime dateFrom, DateTime dateTo)
    {
        string dateFromString = UserData.DateToString(dateFrom);
        string dateToString = UserData.DateToString(dateTo);

        Debug.Log("Получение данных с устройств.");

        UserData.Devices.ForEach(x => 
        {
            string message = Message.GetDeviceData(x.devEui, new { date_from = dateFromString, date_to = dateToString, limit = 50000 });
            //Debug.Log("<color=yellow>" + message + "</color>");
            websocket.Send(message);
        });
    }

    
    /// <summary>
    /// Вызывается при удачном соединении с сервером.
    /// </summary>
    private static void websocket_Opened(object sender, EventArgs e)
    {
        UIDebug.Log("Удачное подключение к серверу.");
        isSocketConnected = true;
    }

    /// <summary>
    /// Вызывается при возникновении ошибки общения с сервером / подключении к нему.
    /// </summary>
    private static void websocket_Error(object sender, SuperSocket.ClientEngine.ErrorEventArgs e)
    {
        UIDebug.Log("Web socket was error: " + e.Exception);
    }

    /// <summary>
    /// Вызывается при закрытии соединения с сервером.
    /// </summary>
    private static void websocket_Closed(object sender, EventArgs e)
    {
        UIDebug.Log("Web socket was close.");
    }

    /// <summary>
    /// Вызывается при получении сообщения от сервера.
    /// </summary>
    private static void websocket_MessageReceived(object sender, MessageReceivedEventArgs e)
    {
        //UIDebug.Log("Message: " + e.Message);

        JObject responseObj = JsonConvert.DeserializeObject(e.Message) as JObject;
        if (!responseObj.HasValues) return;

        BaseResponse baseResponse = JsonConvert.DeserializeObject<BaseResponse>(e.Message);

        if (!baseResponse.status)
        {
            return;
        }

        switch (baseResponse.cmd)
        {
            // АВТОРИЗАЦИЯ
            // --------------------------------------------------------------------------------------------------------
            case "auth_resp":
                JToken error;
                if (responseObj.TryGetValue("err_string", out error))
                {
                    UIDebug.Log("Auth error: " + error.ToString());
                    isInvalidLoginPwd = true;
                }
                else
                {
                    UIDebug.Log("Авторизация прошла успешно.");
                    isSuccessfulAuth = true;

                    Token = responseObj.GetValue("token").ToString();

                    User user = new User(responseObj);
                    UserData.CurrentUser = user;

                    // запрашиваем информацию о устройствах (получаем список устройств)
                    if (user.CheckRights("get_device_appdata")) websocket.Send(Message.GetDevices);
                    if (user.CheckRights("get_gateways")) websocket.Send(Message.GetGateways);
                    if (user.CheckRights("get_users")) websocket.Send(Message.GetUsers);
                }
                break;

            // ИНФОРМАЦИЯ О УСТРОЙСТВАХ
            // --------------------------------------------------------------------------------------------------------
            case "get_device_appdata_resp":
                UIDebug.Log("Устрво: " + e.Message);
                // пришла информация об устройствах
                UIDebug.Log("Получена информация об устройствах.");

                JToken jtDevices = responseObj["devices_list"];
                foreach (JToken jtDevice in jtDevices)
                {
                    Device device = new Device(jtDevice as JObject);

                    if (!string.IsNullOrEmpty(device.devEui) && !string.IsNullOrEmpty(device.deviceType))
                    {
                        // добавляем устройство в список устройств
                        UserData.Devices.Add(device);
                    }
                    else
                    {
                        //UIDebug.Log("Device devEui is empty!");
                    }
                }

                isSuccessfulGetDevices = true;
                // запрос полной информации о устройствах
                DateTime dateFrom = DateTime.UtcNow.Subtract(new TimeSpan(90, 0, 0, 0, 0));
                DateTime dateTo = DateTime.UtcNow.AddHours(10);

                neededDevicesCount = UserData.Devices.Count;
                ПолучитьИнформациюОВсехУстройствах(dateFrom, dateTo);

                Debug.Log("<color=blue>Получено устройств " + UserData.Devices.Count + "</color>");

                break;

            // ДОП. ИНФОРМАЦИЯ О УСТРОЙСТВЕ.
            // --------------------------------------------------------------------------------------------------------
            case "get_data_resp":
                // пришла информация по устройству
                dataRecivedFromNDevices++;
                
                string _devEui = responseObj["devEui"].ToString();
                //Debug.Log(_devEui + " - " + dataRecivedFromNDevices);

                for (int i = 0; i < UserData.Devices.Count; i++)
                {
                    if (UserData.Devices[i].devEui == _devEui)
                    {
                        try
                        {
                            JToken data_listToken = responseObj["data_list"];
                            
                            if (data_listToken != null)
                            {
                                int data_listCount = 0;
                                foreach (JToken jt in data_listToken)
                                    data_listCount++;

                                if (data_listCount > 0)
                                {
                                    if (data_listCount == 50000)
                                    {
                                        UIDebug.Log("Мы загрузили только часть данных за вабранный период, поскольку их слишком много. Попробуйте выбрать меньший промежуток времени.");
                                    }

                                    foreach (JToken jt in data_listToken)
                                    {
                                        UserData.Devices[i].add_history(jt as JObject);
                                    }

                                    //Debug.Log("<color=#ffff00>" + UserData.Devices[i].channels.Count + "</color>");
                                    //Debug.Log("<color=#ffff00>" + UserData.Devices[i].channels[0].history.Count + "</color>");

                                }
                                else
                                {
                                    //UIDebug.Log("Data list is empty.");
                                }
                            }
                        }
                        catch (Exception exception)
                        {
                            Debug.Log("<color=#ff00ff>" + exception.Message + "</color>" + Environment.NewLine + exception.StackTrace);
                        }

                        break;
                    }
                }

                if (dataRecivedFromNDevices >= neededDevicesCount)
                    OnAllDevicesLoaded();

                //Debug.Log("<color=#ffff00>OnDataResponse</color>");

                if (OnDataResponse != null)
                {
                    OnDataResponse();
                }
                else
                {
                    //Debug.Log("NUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUULL");
                }

                break;

            // ИНФОРМАЦИЯ О ШЛЮЗАХ.
            // --------------------------------------------------------------------------------------------------------
            case "get_gateways_resp":
                UIDebug.Log("Получена информация о шлюзах.");

                // UIDebug.Log("Шлюз: " + e.Message);

                JToken jtGateways = responseObj["gateway_list"];
                foreach (JToken jtDevice in jtGateways)
                {
                    Gateway device = new Gateway(jtDevice as JObject);
                    UserData.Gateways.Add(device);
                }
                break;

            // СООБЩЕНИЕ КОНСОЛИ.
            // --------------------------------------------------------------------------------------------------------
            case "console":

                break;

            // ИНФОРМАЦИЯ О ПОЛЬЗОВАТЕЛЯХ.
            // --------------------------------------------------------------------------------------------------------
            case "get_users_resp":
                // к нам пришёл список пользователей
                UIDebug.Log("Получена информация о пользователях.");

                JToken jtUsers = responseObj["user_list"];
                foreach (JToken jtUser in jtUsers)
                {
                    User user = new User(jtUser as JObject);
                    UserData.Users.Add(user);
                }
                break;

            // СООБЩЕНИЕ О ВЫХОДЕ ИЗ ПОЛЬЗОВАТЕЛЯ.
            // --------------------------------------------------------------------------------------------------------
            case "close_auth_resp":
                // сообщение о завершении авторизации на сервере
                ClearData();
                break;
            case "send_data_resp":
                UIDebug.Log(e.Message);
                break;

            case "rx":
                UIDebug.Log(e.Message);
                break;
            default:
                UIDebug.Log(baseResponse.cmd);
                break;
        }
    }

    public static void CloseConnection()
    {
        Instance.StartCoroutine(Instance.CloseConnectionRoutine());
    }

    private IEnumerator CloseConnectionRoutine()
    {
        while (websocket.State == WebSocketState.Connecting || websocket.State == WebSocketState.Closing)
        {
            yield return new WaitForEndOfFrame();
        }

        if (websocket != null && websocket.State == WebSocketState.Open && !string.IsNullOrEmpty(Token))
        {
            websocket.Send(Message.Disconnect);
        }

        yield return new WaitForEndOfFrame();

        if (websocket != null)
        {
            websocket.Close();
        }

        websocket = null;
    }

    private void OnApplicationQuit()
    {
        CloseConnection();
    }
}
