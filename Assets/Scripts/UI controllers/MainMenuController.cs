using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuController : MonoBehaviour {

	public List<GameObject> screens = new List<GameObject> ();
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	public void MoveControllerEnegry(){
		screens [0].SetActive (true);
		screens [7].SetActive (true);
		screens [6].SetActive (false);
	}
	public void MoveSchetchiki(){
		screens [1].SetActive (true);
		screens [7].SetActive (true);
		screens [6].SetActive (false);
	}
	public void MoveSecurity(){
		screens [2].SetActive (true);
		screens [7].SetActive (true);
		screens [6].SetActive (false);
	}
	public void MovePersonalKabinet(){
		screens [3].SetActive (true);
		screens [7].SetActive (true);
		screens [6].SetActive (false);
	}
	public void MovePay(){
		screens [4].SetActive (true);
		screens [7].SetActive (true);
		screens [6].SetActive (false);
	}
	public void MoveFavorite(){
		screens [5].SetActive (true);
		screens [7].SetActive (true);
		screens [6].SetActive (false);
	}

}
