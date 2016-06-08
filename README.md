# OBESDK_Unity

This repository contains all the source code related to OBE's SDK for the Unity platform.
The SDK has a native plugin for OSX, iOS and Android operative systems.

**Installation**

In order to add OBE's SDK into your project, you must download the repository and add all the files contained on the 'Drag&Drop Files' folder in your project's assets folder.

**Use**

Using OBE's SDK is not complicated. It is configured to work as a singleton.
You can initialise the OBE plugin in any part of your project like this:

	OBEPlugin.Instance.init();
	
In order to read data from an OBE jacket. The following piece of code must be implemented inside an Update function:

	void Update() {

		if (OBEPlugin.Instance.isInitiated) { // make sure the singleton is initiated

			// try to connect to the first OBE that's nearby
			// if an OBE is connected, the following line does nothing
			OBEPlugin.Instance.connectToOBE (0);

			// fetch current data sent from OBE
			OBEPlugin.Instance.fetch ();
			
		}
		
		// the rest of your code
		// ...
	}
	
There are several addressable properties, such as:

* Motor1 (Float - Left hand Motor)
* Motor2 (Float - Right hand Motor)
* Motor3 (Float - Logo Motor)
* Motor4 (Float - Cerebrum Motor)
* QuaternionLeft (Quaternion - Left Hand)
* QuaternionRight (Quaternion - Right Hand)
* QuaternionCenter (Quaternion - Cerebrum)
* LeftButton1 (Boolean - Left Button on Left Hand)
* LeftButton2 (Boolean - Right Button on Left Hand)
* LeftButton3 (Boolean - Up Button on Left Hand)
* LeftButton4 (Boolean - Down Button on Left Hand)
* RightButton1 (Boolean - Left Button on Right Hand)
* RightButton2 (Boolean - Right Button on Right Hand)
* RightButton3 (Boolean - Up Button on Right Hand)
* RightButton4 (Boolean - Down Button on Right Hand)
* LogoButton (Boolean - Logo button)

