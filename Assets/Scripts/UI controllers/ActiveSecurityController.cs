using System.Collections.Generic;
using UnityEngine;

public class ActiveSecurityController : MonoBehaviour
{

	public List<GameObject> screens = new List<GameObject>();

	public void BackScreen()
	{
		screens[0].SetActive(true);
		screens[1].SetActive(false);
	}
}
