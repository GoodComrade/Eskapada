using System;
using System.Collections;
using System.Collections.Generic;
using Firebase;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PersonalKabinetController : MonoBehaviour {

	public List<GameObject> screens = new List<GameObject> ();

    [SerializeField]
    private Toggle pushToggle;

    void Start()
    {
        pushToggle.onValueChanged.AddListener(PushToggleChanged);
    }

    private void PushToggleChanged(bool newValue)
    {
        if (FirebaseDatabaseController.Instance.userPushSettings != null && FirebaseDatabaseController.Instance.userPushSettings.usePush != newValue)
        {
            FirebaseDatabaseController.Instance.SetPushSettings(UserData.Login, newValue, FirebaseMessagingController.deviceToken);
        }
    }

    // Use this for initialization
	void OnEnable ()
	{
	    pushToggle.isOn = FirebaseDatabaseController.Instance.userPushSettings == null || FirebaseDatabaseController.Instance.userPushSettings.usePush;
	}

	public void BackScreen()
    {
		screens [0].SetActive (true);
		screens [1].SetActive (false);
		screens [2].SetActive (false);
	}

	public void ChangeUsers(){
		SceneManager.LoadScene (0);
	}
}
