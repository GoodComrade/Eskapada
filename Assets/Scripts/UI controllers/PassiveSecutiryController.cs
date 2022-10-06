using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassiveSecutiryController : MonoBehaviour {


	public List<GameObject> screens = new List<GameObject> ();
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	public void BackScreen(){
		screens [0].SetActive (true);
		screens [1].SetActive (false);
		screens [2].SetActive (false);
	}
}
