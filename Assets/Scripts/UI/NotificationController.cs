using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Assets.SimpleAndroidNotifications;

public class NotificationController : MonoBehaviour {

    public static IEnumerator NotificateMessage(string title, string message)
    {
        yield return new WaitForSeconds(0.1f); 
        NotificationManager.Send(TimeSpan.FromSeconds(0), title, message, new Color(1, 0.3f, 0.15f));
    }
}
