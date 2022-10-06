using System.Collections;
using UnityEngine;

public class ServiceStarter : MonoBehaviour {

#if UNITY_ANDROID && !UNITY_EDITOR

        private const string FullClassName = "example.com.wsclient.Controller";
        private const string MainActivityClassName = "com.unity3d.player.UnityPlayerActivity";

#endif

    public static ServiceStarter ss;


    private void Awake()
    {
        ss = this;
    }

    public IEnumerator StartService(string login, string password, bool activeDefence, string devType)
    {
        yield return new WaitForSeconds(0.1f);

#if UNITY_ANDROID && !UNITY_EDITOR

        AndroidJavaClass unityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject unityActivity = unityClass.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaClass customClass = new AndroidJavaClass(FullClassName);

        customClass.CallStatic("StartService", login, password, activeDefence == true ? 1 : 0, unityActivity, devType);

#endif
    }

    public static void StartService(string login, string password, bool activeDefence, string devType)
    {
        ss.StartCoroutine(ss.StartService(login, password, activeDefence, devType));
    }
}
