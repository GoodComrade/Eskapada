using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnergyController : MonoBehaviour
{
	public List<GameObject> screens = new List<GameObject> ();
	public List<GameObject> screensBlur = new List<GameObject> ();
    public Toggle toggle_ColdWater;
    public Toggle toggle_HotWater;
    public Toggle toggle_Electricity;
    public Toggle toggle_Light;
    public Toggle toggle_Heat;

    public Dropdown temperatureSelector;
    public int MinTemperature = 18;
    public int MaxTemperature = 30;

    public float time;
    public float heatTime;
    public float waterTime;
    float maxtime;
    float maxtime1;
    float maxtime2;

    private void Start()
    {
        AddTemperatureDropdownListValues();
        maxtime = time;
        maxtime1 = heatTime;
        maxtime2 = waterTime;
        time = 0;
        heatTime = 0;
        waterTime = 0;
    }

    void Update()
    {
        if(time > 0)
        {
            time -= Time.deltaTime;

            if(time > 0)
            {
                screensBlur[8].SetActive(true);
            }
            else if(time <= 0)
            {
                screensBlur[8].SetActive(false);
            }
        }

        if (heatTime > 0)
        {
            heatTime -= Time.deltaTime;

            if (heatTime > 0)
            {
                screensBlur[8].SetActive(true);
            }
            else if (heatTime <= 0)
            {
                screensBlur[8].SetActive(false);
            }
        }

        if (waterTime > 0)
        {
            waterTime -= Time.deltaTime;

            if (waterTime > 0)
            {
                screensBlur[8].SetActive(true);
            }
            else if (waterTime <= 0)
            {
                screensBlur[8].SetActive(false);
            }
        }
    }

    private void AddTemperatureDropdownListValues()
    {
        List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();

        for (int i = MinTemperature; i <= MaxTemperature; i++)
        {
            options.Add(new Dropdown.OptionData(i.ToString()));
        }

        temperatureSelector.AddOptions(options);
    }

    public void BackScreen(){
		screens [0].SetActive (true);
		screens [1].SetActive (false);
		screens [2].SetActive (false);
	}
	public void changeOpenClose(bool coldWater)
    {
        if(waterTime <= 0)
        {
            bool activate = toggle_ColdWater.isOn;
            Requester4net.DataControl(2, activate, 4);
            string data = activate ? "030105" : "030205";

            if (coldWater)
            {
                Requester4net.DataControl(2, activate, 4);
                Requester4net.SendData(2, data, 4);
            }
            else
            {
                Requester4net.DataControl(2, activate, 2);
                Requester4net.SendData(2, data, 2);
            }
            screensBlur[activate ? 0 : 1].SetActive(true);

            waterTime = maxtime2;
        }
    }

	public void CloseBlure(){
		foreach (GameObject go in screensBlur)
        {
			if (go.activeInHierarchy) {
				go.SetActive (false);
			}
		}
	}

    /// <summary>
    /// Включить/выключить электричество.
    /// </summary>
    public void TurnElectricity()
    {
        bool activate = toggle_Electricity.isOn;
        
        if(time <= 0)
        {
            string data = activate ? "030100" : "0401";
            Requester4net.SendData(2, data, 1);
            screensBlur[activate ? 2 : 3].SetActive(true);
            time = maxtime;
        }
    }

    /// <summary>
    /// Включить/выключить свет.
    /// </summary>
    public void TurnLight()
    {
        bool activate = toggle_Light.isOn;

        if(time <= 0)
        {
            string data = activate ? "030200" : "0402";
            Requester4net.SendData(2, data, 1);
            screensBlur[activate ? 4 : 5].SetActive(true);
            time = maxtime;
        }
    }

    /// <summary>
    /// Включить/выключить тепло.
    /// </summary>
    public void TurnHeat()
    {
        bool activate = toggle_Heat.isOn;
        

        if(time <= 0)
        {
            string data = activate ? "0401" : "030100";
            Requester4net.SendData(2, data, 5);
            screensBlur[activate ? 6 : 7].SetActive(true);
            time = maxtime;
        }
    }

    public void OnTemperatureChanged()
    {
        int selectedValue = temperatureSelector.value + MinTemperature;
        
        if(heatTime <= 0)
        {
            Requester4net.ChangeTemperature(selectedValue);
            heatTime = maxtime1;
        }
        
    }
}
