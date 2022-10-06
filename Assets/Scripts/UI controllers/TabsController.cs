using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabsController : MonoBehaviour {
	
	public List<GameObject> screens = new List<GameObject> ();
	public List<GameObject> screensAll = new List<GameObject> ();
	public List<Sprite> spr = new List<Sprite> ();
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void MoveSchetchiki() {
		foreach (GameObject go in screensAll) {
			if (go.activeInHierarchy) {
				Debug.Log (go.name);
				go.SetActive (false);
			}
		}
		screens [1].SetActive (true);
	}
	public void MoveSecurity() {
		foreach (GameObject go in screensAll) {
			if (go.activeInHierarchy) {
				go.SetActive (false);
			}
		}
		screens [2].SetActive (true);

	}
	public void MoveFavorite() {
		foreach (GameObject go in screensAll) {
			if (go.activeInHierarchy) {
				go.SetActive (false);
			}
		}
		screens [3].SetActive (true);
	}
	public void MoveMainMenu() {
		foreach (GameObject go in screensAll) {
			if (go.activeInHierarchy) {
				go.SetActive (false);
			}
		}
		screens [0].SetActive (true);
		gameObject.SetActive (false);
	}
}
