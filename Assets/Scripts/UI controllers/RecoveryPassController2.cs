using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;


public class RecoveryPassController2 : MonoBehaviour {


	public List<GameObject> screens = new List<GameObject> (); //лист хранит три экрана прошлый, текущий, следующий
	public List<InputField> txt = new List<InputField>();
	public List<Image> img = new List<Image>(); //лист полей ввода, для смены спрайтов
	public List<Sprite> spr = new List<Sprite>();

	//флаги для хранения верной или не верной валидации
	bool blValidNewPass; 
	bool blValidAcceptPass; 
	bool blValidTemporaryPass;
	bool is_valid = false;

	//переменная для хранения времени задержки заблюренного экрана
	float t= 1.5f;

	//временный пароль
	string temporary_pass = "1234";

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (is_valid) {
			t -= Time.deltaTime;
		}
	}


	//Валидация на временный пароль
	public void isValidTemporaryPass(){
		if (txt [0].text == temporary_pass) {
			img [0].sprite = spr [0];
			blValidTemporaryPass = true;
		} else {
			img [0].sprite = spr [1];
		}
	}

	//Валидация на создание нового пароля
	public void isValidNewPass(){
		
		if (txt [1].text.Length >= 8 && Regex.IsMatch (txt [1].text, "[a-z][0-9]")) {
			img [1].sprite = spr [0];
			blValidNewPass = true;
		} else {
			img [1].sprite = spr [1];
		}
	}

	//Валидация на подтверждающий пароль
	public void isValidAcceptPass(){
		if (txt [1].text == txt [2].text) {
			img [2].sprite = spr [0];
			blValidAcceptPass = true;
		} else {
			img [2].sprite = spr [1];
		}
	}

	//если все поля введены верно, то открываем заблюренный экран
	public void isValid(){
		if (blValidNewPass && blValidAcceptPass && blValidTemporaryPass) {
			screens [2].SetActive (true);
		}
	}
	//метод вызывается по нажатию на кнопку закрыть на заблюренном экране
	public void closBlueScreen(){
		screens[3].SetActive (true);
		screens [2].SetActive (false);
		screens [1].SetActive (false);
	}
	//метод вызывается по нажатию на кнопку назад
	public void BackScreen(){
		screens[0].SetActive (true);
		screens [1].SetActive (false);
	}
}
