using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;

public class OBEController : MonoBehaviour {

	public GameObject obeCube;

	private float W = 1, X = 1, Y = 1, Z = 1;

	//private OBE obe;
	private OBEPlugin plugin;

	void Awake() {
		Application.targetFrameRate = 30;
	}

	// Use this for initialization
	void Start () {
		

		plugin = new OBEPlugin ();
		plugin.init ();

		/*
		OBEPlugin.QuaternionUpdated ((float w, float x, float y, float z, int identifier) => {
			//Debug.Log(w.ToString() + "," + x.ToString() + "," + y.ToString() + "," + z.ToString());
			printQuaternion(w, x, y, z, identifier);
		});

		//OBEPlugin_OSX.QuaternionUpdated_Right ((float w, float x, float y, float z) => {
			//Debug.Log(w.ToString() + "," + x.ToString() + "," + y.ToString() + "," + z.ToString());
		//	printQuaternion(w,x,y,z);
		//});

		OBEPlugin.FoundOBE ((obeName, index) => {
			string name = Marshal.PtrToStringAuto(obeName);

			Debug.Log("Found: " + name);
			plugin.connectToOBE(0);
		});

		OBEPlugin.FoundOBEService((obeService) => {
			//string name = Marshal.PtrToStringAuto(obeService);

			//Debug.Log("Service Found: " + name);
		});

		OBEPlugin.FoundOBECharacteristic((obeCharacteristic) => {
			//string name = Marshal.PtrToStringAuto(obeCharacteristic);

			//Debug.Log("Service Characteristic: " + name);
		});
*/

		//plugin.setCallback (this);
		plugin.startScanning ();


		//obe = new OBE ();
		//obe.init (this);
		//obe.startScanning ();

		//init ();
		//startScanning ();
	}

	// Update is called once per frame
	void Update () {
		//rotateCube (W,X,Y,Z);

		// try to connect to the first OBE that's nearby
		// if connected the following line does nothing
		plugin.connectToOBE(0);

		// fetch current data sent from OBE
		plugin.fetch ();



		//Quaternion q22 = new Quaternion (0.5, 0.5, 0.5, 0.5);
		//obeCube.transform.rotation = plugin.QuaternionLeft * obeCube.transform.rotation;
		//obeCube.transform.Rotate (plugin.QuaternionLeft.eulerAngles.x, plugin.QuaternionLeft.eulerAngles.y,
		//	plugin.QuaternionLeft.eulerAngles.z);

		//obeCube.transform.rotation = plugin.QuaternionLeft;
		//obeCube.transform.rotation = q22 * plugin.QuaternionLeft;

		//obeCube.transform.rotation.Set (plugin.QuaternionLeft.x, plugin.QuaternionLeft.y, 
		//	plugin.QuaternionLeft.z, plugin.QuaternionLeft.w);

		printQuaternion (plugin.QuaternionLeft.w, plugin.QuaternionLeft.x, plugin.QuaternionLeft.y, 
			plugin.QuaternionLeft.z, 0);
	}

	void printQuaternion(float w, float x, float y, float z, int identifier){
		Debug.Log(identifier.ToString() + ": " + w.ToString() + "," + x.ToString() + 
			"," + y.ToString() + "," + z.ToString());

		W = w; X = x; Y = y; Z = z;
		//obeCube.transform.rotation.Set (w, x, y, z);
	}

	public void rotateCube(float w, float x, float y, float z){
		obeCube.transform.rotation.Set (w, x, y, z);
	}

	//
	void OnApplicationQuit(){
		//obe.disconnect ();
		//plugin.disconnectFromOBE();
		Debug.Log ("On Application Quit");
	}

	// override
	/*
	// Interface for receiving updated quaternions
	public void QuaternionUpdated(float w, float x, float y, float z, int identifier){
		Debug.Log(w.ToString() + "," + x.ToString() + "," + y.ToString() + "," + z.ToString());
	}

	public void FoundOBE(string name, int index){
		Debug.Log("Found: " + name);
		//obe.connect(0);
		//connect(0);
		plugin.connectToOBE(0);
	}

	public void FoundOBEService(string serviceName){
		Debug.Log("Service Found: " + serviceName);
	}

	public void FoundOBECharacteristic(string characteristicName){
		Debug.Log("Service Found: " + characteristicName);
	}*/
}
