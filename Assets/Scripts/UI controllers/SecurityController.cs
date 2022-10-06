using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecurityController : MonoBehaviour {


	public List<GameObject> screens = new List<GameObject> ();
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	public void MoveActiveSecurity(){
		screens [0].SetActive (true);
		screens [6].SetActive (false);
	}
	public void MovePassiveSecurity(){
		screens [1].SetActive (true);
		screens [6].SetActive (false);
	}/*
	public void MoveBarrier(){
		screens [2].SetActive (true);
		screens [6].SetActive (false);
	}
	public void MoveGate(){
		screens [3].SetActive (true);
		screens [6].SetActive (false);
	}
	public void MoveDoorphone(){
		screens [3].SetActive (true);
		screens [6].SetActive (false);
	}*/
	public void BackScreen(){
		screens [4].SetActive (true);
		screens [5].SetActive (false);
		screens [6].SetActive (false);
	}

}
