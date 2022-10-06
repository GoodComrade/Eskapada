using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;

public class RecoveryPassController : MonoBehaviour {

	//
	bool reg_validTel;
	bool isValidLogin = false;
	string true_login = "test";
	public InputField txt;
	public Image img;
	public List<Sprite> spr = new List<Sprite> ();
	public List<GameObject> screens = new List<GameObject> ();
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	//метод валидации поля для ввода телефона, вызывается когда пользователь убирает курсорс с поля для ввода телефона.
	public void ValidationTel()
	{

		Regex myReg = new Regex("[0-9]");
		reg_validTel = myReg.IsMatch (txt.text);

		if ((txt.text.Length == 10 && reg_validTel) || txt.text == true_login) {
			img.sprite = spr [0];
			isValidLogin = true;
		}

		else
		{
			img.sprite = spr [1];
		}

	}
	//вызывается по нажатию на кнопку назад
	public void BackScreen(){
		screens[0].SetActive (true);
		screens [1].SetActive (false);
	}
	//Вызывается по нажатию на кнопку Далее
	public void nextscreen(){
		if (isValidLogin == true) {
			screens [2].SetActive (true);
			screens [1].SetActive (false);
	}
}
}