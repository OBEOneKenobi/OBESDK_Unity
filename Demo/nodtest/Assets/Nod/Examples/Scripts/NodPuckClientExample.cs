using System;
using Nod;
using UnityEngine;

public class NodPuckClientExample : NodExampleBase
{
	private Quaternion inverseInitialRotation = Quaternion.identity;
	private Vector3 initialPosition;
	private Vector3 initialDevicePosition;

	private void recenter()
	{
		inverseInitialRotation = Quaternion.Inverse(nodDevice.rotation);
		initialDevicePosition = nodDevice.translation;
	}

	public void OnEnable()
	{
		initialPosition = transform.localPosition;
		initialDevicePosition = new Vector3 (0, 0, 0);

		nodSubscribtionList = new NodSubscriptionType []
		{
			NodSubscriptionType.EulerMode,
			NodSubscriptionType.TranslationMode
		};

		nod = NodController.GetNodInterface();
	}

	public void Update()
	{
		if (!NodDeviceConnectedAndInitialized())
			return;

		if (Input.GetKeyDown(KeyCode.Space))
			recenter();

		//Example of applying the nod devices orientation to the local transform.
		transform.localRotation = inverseInitialRotation * nodDevice.rotation;
		transform.localPosition = initialPosition + nodDevice.translation;
		Debug.Log("Translation " + initialPosition + " + " + nodDevice.translation);
	}

	public void OnGUI()
	{
		//Deal with displaying error conditions
		BaseNodOnGUI();
	}
}