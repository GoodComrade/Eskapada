using System.Linq;
using Firebase.Messaging;
using UnityEngine;

namespace Firebase
{
    public class FirebaseMessagingController : MonoBehaviour
    {
        private static FirebaseMessagingController instance;

        public static string deviceToken = null;

        private bool loadToken;

        // Use this for initialization
        void Awake ()
        {
#if UNITY_EDITOR
            //return;
#endif
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(gameObject);
            
            Requester4net.successfulAuth += Requester4NetOnSuccessfulAuth;
            Requester4net.onSuccessfulGetDevices += Requester4NetOnOnSuccessfulGetDevices;


#if UNITY_ANDROID
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
                var dependencyStatus = task.Result;
                if (dependencyStatus == DependencyStatus.Available)
                {
                    FirebaseDatabaseController.Instance.Init();
                    FirebaseMessaging.TokenReceived += OnTokenReceived;
                    FirebaseMessaging.MessageReceived += OnMessageReceived;
                }
                else
                {
                    Debug.LogError(string.Format(
                        "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                }
            });
#else
            FirebaseDatabaseController.Instance.Init();
            FirebaseMessaging.TokenReceived += OnTokenReceived;
            FirebaseMessaging.MessageReceived += OnMessageReceived;
#endif

#if UNITY_EDITOR
            OnTokenReceived(this, new TokenReceivedEventArgs("EDITOR1"));
#endif
        }

        private void Requester4NetOnOnSuccessfulGetDevices()
        {
            var devices = UserData.Devices.Select(d => d.devEui).ToArray();
            FirebaseDatabaseController.Instance.SetDevices(UserData.Login, devices);
        }

        private void Requester4NetOnSuccessfulAuth()
        {
            FirebaseDatabaseController.Instance.GetPushSettings(UserData.Login);
            FirebaseDatabaseController.Instance.OnUserPushSettingsLoad += InstanceOnOnUserPushSettingsLoad;
        }

        private void InstanceOnOnUserPushSettingsLoad()
        {
            if (string.IsNullOrEmpty(deviceToken))
            {
                loadToken = true;
                return;
            }

            if (FirebaseDatabaseController.Instance.userPushSettingsWasDownload)
            {
                if(FirebaseDatabaseController.Instance.userPushSettings.tokens.Any(t=>t == deviceToken))
                    return;
            }

            FirebaseDatabaseController.Instance.SetPushSettings(UserData.Login, true, deviceToken);
        }

        public void OnTokenReceived(object sender, Messaging.TokenReceivedEventArgs token)
        {
            Debug.Log("Received Registration Token: " + token.Token);

            deviceToken = token.Token;
            if (loadToken)
            {
                loadToken = false;
                InstanceOnOnUserPushSettingsLoad();
            }
        }

        public void OnMessageReceived(object sender, Messaging.MessageReceivedEventArgs e)
        {
            Debug.Log("Received a new message from: " + e.Message.From);
        }
    }
}
