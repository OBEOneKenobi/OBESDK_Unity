using UnityEngine;
using System.Collections;

public class NodMenuSceneSelectorRing : MonoBehaviour {

	public void OrientationButtonPressed()
	{
		Application.LoadLevel("Example_OrientationAndButtons_Ring");
	}

	public void GesturesButtonPressed()
	{
		Application.LoadLevel("Example_GesturesAndMouseRing");
	}
		
	public void Update()
	{
		//So Android can exit out when the user hits the back button
		if (Input.GetKeyDown (KeyCode.Escape)) {
			Application.LoadLevel("NodExample");
			return;
		}
	}
}
