using System;
using System.Collections.Generic;
using Firebase.Database;
using Firebase.Unity.Editor;
using UnityEngine;

namespace Firebase
{
    public class FirebaseDatabaseController
    {
        private static FirebaseDatabaseController instance;
        private DatabaseReference reference;
        
        public static FirebaseDatabaseController Instance
        {
            get { return instance ?? (instance = new FirebaseDatabaseController()); }
        }

        public event Action OnUserPushSettingsLoad = delegate {};
        public UserPushSettings userPushSettings;
        public bool userPushSettingsWasDownload;

        private event Action onConnect = delegate { };

        // Use this for initialization
        FirebaseDatabaseController()
        {
            InitDatabase();
        }

        public void Init() { }

        public void SetDevices(string user, string[] devices)
        {
#if UNITY_EDITOR
            //return;
#endif
            if (reference == null)
                onConnect += () => SetDevicesInternal(user, devices);
            else
                SetDevicesInternal(user, devices);
        }

        public void SetPushSettings(string user, bool usePush, string token)
        {
#if UNITY_EDITOR
            //return;
#endif
            if (userPushSettings == null)
                userPushSettings = new UserPushSettings();

            userPushSettings.usePush = usePush;
            if (!userPushSettings.tokens.Contains(token))
                userPushSettings.tokens.Add(token);

            Debug.Log(reference);
            if (reference == null)
                onConnect += () => SetPushSettingsInternal(user, usePush, token);
            else
                SetPushSettingsInternal(user, usePush, token);
        }

        public void GetPushSettings(string user)
        {
#if UNITY_EDITOR
            //return;
#endif
            if (reference == null)
                onConnect += () => GetPushSettingsInternal(user);
            else
                GetPushSettingsInternal(user);
        }
        
        private void SetDevicesInternal(string user, string[] devices)
        {
#if UNITY_EDITOR
            //return;
#endif
            foreach (string device in devices)
            {
                reference.Child("Devices").Child(device).SetRawJsonValueAsync(JsonUtility.ToJson(new DeviceData { user = user }));
            }
        }

        private void SetPushSettingsInternal(string user, bool usePush, string token)
        {
#if UNITY_EDITOR
            // return;
#endif
            if (userPushSettingsWasDownload)
            {
                reference.Child("Users").Child(user).SetRawJsonValueAsync(JsonUtility.ToJson(userPushSettings));
            }
            else
            {
                OnUserPushSettingsLoad += () => OnOnUserPushSettingsLoad(user, usePush, token);
            }
            
        }

        private void OnOnUserPushSettingsLoad(string user, bool usePush, string newToken)
        {
#if UNITY_EDITOR
            // return;
#endif

            userPushSettings.tokens.Add(newToken);
            userPushSettings.usePush = usePush;
            reference.Child("Users").Child(user).SetRawJsonValueAsync(JsonUtility.ToJson(userPushSettings));
        }

        private void GetPushSettingsInternal(string user)
        {
#if UNITY_EDITOR
            // return;
#endif
            
            reference.Child("Users").GetValueAsync().ContinueWith(task =>
                {
                    if (task.IsCompleted)
                    {
                        if (task.Result.HasChild(user))
                        {
                            userPushSettings = JsonUtility.FromJson<UserPushSettings>(task.Result.Child(user).GetRawJsonValue());
                            if (userPushSettings == null)
                                userPushSettings = new UserPushSettings();
                        }
                        else
                        {
                            userPushSettings = new UserPushSettings();
                        }

                        userPushSettingsWasDownload = true;
                        OnUserPushSettingsLoad();
                    }
                });
        }

        private void InitDatabase()
        {
#if UNITY_EDITOR
            // return;
            FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://eskapada-unity.firebaseio.com/");
            FirebaseApp.DefaultInstance.SetEditorP12FileName("eskapada-unity-b9a65a465468.p12");
            FirebaseApp.DefaultInstance.SetEditorServiceAccountEmail("eskapada-unity@appspot.gserviceaccount.com");
            FirebaseApp.DefaultInstance.SetEditorP12Password("notasecret");

            reference = FirebaseDatabase.DefaultInstance.RootReference;

            onConnect();
#else
            Firebase.Auth.FirebaseAuth auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
            auth.SignInAnonymouslyAsync().ContinueWith(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.LogError("SignInAnonymouslyAsync was canceled.");
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.Log("Autorize faild - " + task.Exception);
                    return;
                }

                reference = FirebaseDatabase.DefaultInstance.RootReference;
                onConnect();
            });
#endif
        }

        [Serializable]
        private struct DeviceData
        {
            public string user;
        }

        [Serializable]
        public class UserPushSettings
        {
            public bool usePush = true;
            public List<string> tokens = new List<string>();
        }
    }
}
