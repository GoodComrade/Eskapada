using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SchetchikiController : MonoBehaviour
{
    public List<GameObject> screens = new List<GameObject> ();

    [SerializeField]
    private Text hotWater;
    [SerializeField]
    private Text coldWater;
    [SerializeField]
    private Text energy;
    [SerializeField]
    private Text gas;

    private bool needUpdate;
    private bool unsuscribed;

	// Use this for initialization
	void Start ()
    {
        if (Requester4net.AlreadyGetDataFromAllDevices)
        {
            hotWater.text = UserData.data_HotWater.GetAll().ToString("F3");
            coldWater.text = UserData.data_ColdWater.GetAll().ToString("F3");
            energy.text = UserData.data_Electricity.GetAll().ToString("F3");
            gas.text = UserData.data_Gas.GetAll().ToString("F3");
        }
        else
        {
            Requester4net.OnAllDevicesLoaded += Requester4NetOnOnAllDevicesLoaded;
        }
    }

    private void Requester4NetOnOnAllDevicesLoaded()
    {
        Requester4net.OnAllDevicesLoaded -= Requester4NetOnOnAllDevicesLoaded;
        needUpdate = true;
        unsuscribed = true;
    }

    // Update is called once per frame
	void Update ()
    {
        if (needUpdate)
        {
            hotWater.text = UserData.data_HotWater.GetAll().ToString("F3");
            coldWater.text = UserData.data_ColdWater.GetAll().ToString("F3");
            energy.text = UserData.data_Electricity.GetAll().ToString("F3");
            gas.text = UserData.data_Gas.GetAll().ToString("F3");
            needUpdate = false;
        }
    }

	public void MoveAnalitics(){
		screens [0].SetActive (true);
		screens [3].SetActive (false);
	}	

	public void MoveAddHard(){
		screens [4].SetActive (true);
		screens [3].SetActive (false);
	}	


	public void MoveMainMenu(){
		screens [1].SetActive (true);
		screens [2].SetActive (false);
		screens [3].SetActive (false);
	}

    protected void OnDestroy()
    {
        if(!unsuscribed)
            Requester4net.OnAllDevicesLoaded -= Requester4NetOnOnAllDevicesLoaded;
    }
}
