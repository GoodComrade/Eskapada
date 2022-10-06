using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using UnityEngine;
using UnityEngine.UI;

public class authorization : MonoBehaviour
{
    public List<InputField> txt = new List<InputField> ();  // лист полей ввода, чтобы брать текст ввденный пользователем
	public List<Image> img = new List<Image> (); //лист полей ввода, для смены спрайтов
	public List<Sprite> spr = new List<Sprite> (); 
	public GameObject currentScreen; 
	public GameObject nextScreen;
	public GameObject recoveryscreen;


    private void Awake()
    {
        Requester4net.successfulAuth += NextScreen;
        Requester4net.invalidLoginPwd += WrongPassword;
    }

	/// <summary>
	/// Попытаться авторизоваться на сервере.
	/// </summary>
	public void TryLogin()
	{
		string login = txt[0].text;
		string password = txt[1].text;

		Requester4net.Authorization(login, password);
		Invoke("WrongPassword", 1.4f);
	}

	/// <summary>
	/// При успешной авторизации.
	/// </summary>
	public void NextScreen()
	{
        currentScreen.SetActive(false);

        StartCoroutine(NextScreenDelay());
    }

    private IEnumerator NextScreenDelay()
    {
        yield return new WaitForSeconds(1);

        img[0].sprite = spr[0];
        img[1].sprite = spr[0];

        nextScreen.SetActive(true);
        
        Requester4net.successfulAuth -= NextScreen;
        Requester4net.invalidLoginPwd -= WrongPassword;
    }

	/// <summary>
	/// При неудачной авторизации.
	/// </summary>
	public void WrongPassword()
	{
		if (string.IsNullOrEmpty(Requester4net.Token))
		{
			img[0].sprite = spr[1];
			img[1].sprite = spr[1];
		}
	}
	//переход на экран восстановления пароля
	public void recoveryScreen()
    {
		recoveryscreen.SetActive (true);
		currentScreen.SetActive (false);
	}

}
