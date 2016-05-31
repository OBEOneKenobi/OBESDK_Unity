using UnityEngine;
using System.Collections;

public class NodBackRIng : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	public void Update()
	{
		//So Android can exit out when the user hits the back button
		if (Input.GetKeyDown (KeyCode.Escape)) {
			Application.LoadLevel("NodExampleScenePickerRing");
		}
	}
}
