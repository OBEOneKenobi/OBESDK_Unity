using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Nod;

public class NodOrientationExample : NodExampleBase
{
	//Rotation to get the Nod device from where it started to where it should be once we recenter
	private Quaternion inverseInitialRotation = Quaternion.identity;

	public void Awake()
	{
		nodSubscribtionList = new NodSubscriptionType []
		{
			NodSubscriptionType.EulerMode,
			NodSubscriptionType.ButtonMode
		};

		//This will create a GameObject in your Hierarchy called "NodController" which will manage
		//interactions with all connected nod devices.  It will presist between scene loads.  Only
		//one instance will be created if you request a nod interface from multiple locations
		//in your code.

		nod = NodController.GetNodInterface();
	}

	public void Update()
	{
		if (!NodDeviceConnectedAndInitialized())
			return;

		UpdateUGUIDisplay();
	

		if (Input.GetKeyDown(KeyCode.Space))
			recenter();

		//Example of applying the nod devices orientation to the local transform.
		transform.localRotation = inverseInitialRotation * nodDevice.rotation;
	}

	public void ResetOrientatinButtonPressed()
	{
		recenter();
	}

	private void recenter()
	{
		inverseInitialRotation = Quaternion.Inverse(nodDevice.rotation);
	}

	public Text [] uGUILabels;

	private string[] ringButtonNames = { "A", "B", "X", "Y", "NodLogo", "Joystick", "Left", "Right", "Bumper", "Grip"};

	private void UpdateUGUIDisplay()
	{
		//Once the device is connected display the pressed status of each button
		if (nodDeviceConnected) {
			//Display the status of each button
			string [] buttonPressStatus = {
				nodDevice.GetNodButton(Nod.ButtonIDs.Backspin.A) ? "Down" : "Up",
				nodDevice.GetNodButton(Nod.ButtonIDs.Backspin.B) ? "Down" : "Up",
				nodDevice.GetNodButton(Nod.ButtonIDs.Backspin.X) ? "Down" : "Up",
				nodDevice.GetNodButton(Nod.ButtonIDs.Backspin.Y) ? "Down" : "Up",
				nodDevice.GetNodButton(Nod.ButtonIDs.Backspin.Logo) ? "Down" : "Up",
				nodDevice.GetNodButton(Nod.ButtonIDs.Backspin.Joystick) ? "Down" : "Up",
				nodDevice.GetNodButton(Nod.ButtonIDs.Backspin.Left) ? "Down" : "Up",
				nodDevice.GetNodButton(Nod.ButtonIDs.Backspin.Right) ? "Down" : "Up",
				nodDevice.GetNodButton(Nod.ButtonIDs.Backspin.Bumper) ? "Down" : "Up",
				nodDevice.GetNodButton(Nod.ButtonIDs.Backspin.Grip) ? "Down" : "Up"
			};

			for (int ndx = 0; ndx < ringButtonNames.Length; ndx++) {
				uGUILabels[ndx].text = ringButtonNames[ndx] + "\n" + buttonPressStatus[ndx];
			}
		}
	}

	public void OnGUI()
	{
		//Deal with displaying error conditions
		BaseNodOnGUI();
	}
}