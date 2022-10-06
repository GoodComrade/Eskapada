using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PersonalDataController : MonoBehaviour
{
	public List<GameObject> screens = new List<GameObject> (); //лист хранит три экрана прошлый, текущий, следующий
	public Toggle accept;

    private bool БылСогласенСУсловниями;


    private void OnEnable()
    {
        БылСогласенСУсловниями = PlayerPrefs.HasKey(UserData.Login + ":Agrees with conditions") && PlayerPrefs.GetInt(UserData.Login + ":Agrees with conditions") == 1;

        if (БылСогласенСУсловниями)
        {
            accept.isOn = true;
            NextScreen();
        }
    }

    //вызывается по нажатию на кнопку назад
    public void BackScreen(){
		screens[0].SetActive (true);
		screens[1].SetActive(false);
	}

	//вызывается по нажатию на кнопку далее	
	public void NextScreen()
    {
		if (accept.isOn)
        {
            if (!БылСогласенСУсловниями)
            {
                PlayerPrefs.SetInt(UserData.Login + ":Agrees with conditions", 1);
                PlayerPrefs.Save();
            }

			screens [2].SetActive (true);
			screens [1].SetActive (false);
		}
	}
}
