using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashTime : MonoBehaviour {


	public float t;
	public List<GameObject> Screen = new List<GameObject>(); 
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		t -= Time.deltaTime;
		if (t < 0) {
			NextSplash();
		}

	}
	void NextSplash(){
		Screen[1].SetActive (true);
		gameObject.SetActive (false);
	}
}
