using UnityEngine;
using System.Collections;

public class NodMainMenuSceneSelector : MonoBehaviour {

	public void BackspinButtonPressed()
	{
		Application.LoadLevel("NodExampleScenePicker");
	}

	public void RingButtonPressed()
	{
		Application.LoadLevel("NodExampleScenePickerRing");
	}

	public void Update()
	{
		//So Android can exit out when the user hits the back button
		if (Input.GetKeyDown (KeyCode.Escape)) {
			Application.Quit();
		}
	}
}
