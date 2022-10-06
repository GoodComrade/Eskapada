using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;

public class DataController : MonoBehaviour
{
    bool reg_validTel; 
	bool isValidTel; //флаг для того хранит верную/ложную валидацию телефонного номера
	public List<InputField> txt = new List<InputField>(); //лист полей ввода, для смены спрайтов
	public Image img; 
	public List<Sprite> spr = new List<Sprite> (); 
    
	public List<GameObject> screens = new List<GameObject> ();


    private void OnEnable()
    {
        if (PlayerPrefs.HasKey(UserData.Login + ":User name") && PlayerPrefs.HasKey(UserData.Login + ":User phone"))
        {
            Skip();
        }
    }

    //вызывается по нажатию на кнопку назад
    public void BackScreen(){
		screens[0].SetActive (true);
		screens [1].SetActive (false);
	}

	public void NextScreen()
    {
		if (isValidTel) {
			SceneManager.LoadScene (1);
			screens [1].SetActive (false);
		}
	}

	//вызывается по нажатию на кнопку пропустить
	public void Skip()
    {
	    SceneManager.LoadScene(1);
	}

	//сохраняет данные и переходит на следующий экран, если телефон валиден. Вызывается по нажатию на кнопку Сохранить.
	public void save()
    {
		string FIO = txt [0].text;
        string Tel = txt [1].text;

        if (!string.IsNullOrEmpty(FIO) && !string.IsNullOrEmpty(Tel))
        {
            PlayerPrefs.SetString(UserData.Login + ":User name", FIO);
            PlayerPrefs.SetString(UserData.Login + ":User phone", Tel);
            PlayerPrefs.Save();
        }

		SceneManager.LoadScene (1);
	}

	//метод для проверки валидации поля для ввода телефона, вызывается когда пользователь убирает курсорс с поля для ввода телефона. 
	public void ValidationTel()
	{
		Regex myReg = new Regex("[0-9]");
		reg_validTel = myReg.IsMatch (txt[1].text);
		Debug.Log (reg_validTel);
		if (txt[1].text.Length == 10 && reg_validTel)
        {
			img.sprite = spr [0];
			isValidTel = true;
		}
		else
		{
			img.sprite = spr [1];
		}

	}
}
