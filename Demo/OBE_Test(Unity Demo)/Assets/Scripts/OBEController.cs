using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;

public class OBEController : MonoBehaviour {

	public GameObject obeCube;
	public GameObject obeCubeBody;
	public GameObject obeCube2;
	public GameObject obeCube2Body;
	public GameObject obeCube3;

	private float W = 1, X = 1, Y = 1, Z = 1;

	private Boolean shouldUpdate = false;

	//private OBE obe;
	private OBEPlugin plugin;
	private Quaternion auxQ, auxQ2, auxQ3, O1, O2, O3;

	void Awake() {
		Application.targetFrameRate = 24;
	}

	// Use this for initialization
	void Start () {

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

		//plugin.startScanning ();


		//obe = new OBE ();
		//obe.init (this);
		//obe.startScanning ();

		//init ();
		//startScanning ();

		auxQ = new Quaternion (0,0,0,1);
		auxQ2 = new Quaternion (0,0,0,1);
		auxQ3 = new Quaternion (0,0,0,1);

		O1 = obeCube.transform.rotation;
		O2 = obeCube2.transform.rotation;
		O3 = obeCube3.transform.rotation;

		Invoke ("startOBE", 1.0f); // start function 1 second after

		//Renderer rend = obeCubeBody.gameObject.GetComponent<Renderer>();
		//rend.material.color = new Color(1.0f, 0.0f, 0.0f);
	}

	void startOBE(){
		if (plugin == null) {
			plugin = new OBEPlugin ();
			plugin.init ();
		}
	}

	// Update is called once per frame
	void Update () {
		//rotateCube (W,X,Y,Z);

		if (plugin != null) {
			// try to connect to the first OBE that's nearby
			// if an OBE is connected, the following line does nothing
			plugin.connectToOBE (0);

			// fetch current data sent from OBE
			plugin.fetch ();

			//Debug.Log (plugin.bleState.ToString());

			if ((auxQ.w != plugin.QuaternionLeft.w) || (auxQ.x != plugin.QuaternionLeft.x) ||
			  (auxQ.y != plugin.QuaternionLeft.y) || (auxQ.z != plugin.QuaternionLeft.z)) {
				obeCube.transform.rotation = O1 * plugin.QuaternionLeft;

				//printQuaternion(plugin.QuaternionLeft.eulerAngles.x, plugin.QuaternionLeft.eulerAngles.y,
				//	plugin.QuaternionLeft.eulerAngles.z,0,0);
			}

			if ((auxQ2.w != plugin.QuaternionRight.w) || (auxQ2.x != plugin.QuaternionRight.x) ||
			  (auxQ2.y != plugin.QuaternionRight.y) || (auxQ2.z != plugin.QuaternionRight.z)) {
				obeCube2.transform.rotation = O2 * plugin.QuaternionRight;

				//printQuaternion(plugin.QuaternionLeft.eulerAngles.x, plugin.QuaternionLeft.eulerAngles.y,
				//	plugin.QuaternionLeft.eulerAngles.z,0,0);
			}

			if ((auxQ3.w != plugin.QuaternionCenter.w) || (auxQ3.x != plugin.QuaternionCenter.x) ||
				(auxQ3.y != plugin.QuaternionCenter.y) || (auxQ3.z != plugin.QuaternionCenter.z)) {
				obeCube3.transform.rotation = O3 * plugin.QuaternionCenter;

				//printQuaternion(plugin.QuaternionLeft.eulerAngles.x, plugin.QuaternionLeft.eulerAngles.y,
				//	plugin.QuaternionLeft.eulerAngles.z,0,0);
			}

			auxQ = plugin.QuaternionLeft;
			auxQ2 = plugin.QuaternionRight;
			auxQ3 = plugin.QuaternionCenter;

			// The following code was made to show whether a button is pressed or not
			shouldUpdate = !shouldUpdate;
			if (shouldUpdate) {

				if (Input.GetKey (KeyCode.A)) {
					plugin.OBEMotor1 = 1.0f;
				} else {
					plugin.OBEMotor1 = 0.0f;
				}

				if (Input.GetKey (KeyCode.S)) {
					plugin.OBEMotor2 = 1.0f;
				} else {
					plugin.OBEMotor2 = 0.0f;
				}

				if (Input.GetKey (KeyCode.D)) {
					plugin.OBEMotor3 = 1.0f;
				} else {
					plugin.OBEMotor3 = 0.0f;
				}

				if (Input.GetKey (KeyCode.W)) {
					plugin.OBEMotor4 = 1.0f;
				} else {
					plugin.OBEMotor4 = 0.0f;
				}
				//Debug.Log (plugin.OBEMotor1.ToString());
				plugin.updateMotors ();
			}

			if (plugin.button1) {
				Renderer rend = obeCube2Body.gameObject.GetComponent<Renderer> ();
				rend.material.color = new Color (0.0f, 1.0f, 0.0f);
			} else {
				if (plugin.button2) {
					Renderer rend = obeCube2Body.gameObject.GetComponent<Renderer> ();
					rend.material.color = new Color (1.0f, 0.0f, 0.0f);
				} else {
					Renderer rend = obeCube2Body.gameObject.GetComponent<Renderer> ();
					rend.material.color = new Color (1.0f, 1.0f, 1.0f);
				}
			}

			if (plugin.button3) {
				Renderer rend = obeCubeBody.gameObject.GetComponent<Renderer> ();
				rend.material.color = new Color (0.0f, 0.0f, 1.0f);
			} else {
				if (plugin.button4) {
					Renderer rend = obeCubeBody.gameObject.GetComponent<Renderer> ();
					rend.material.color = new Color (1.0f, 0.0f, 1.0f);
				} else {
					Renderer rend = obeCubeBody.gameObject.GetComponent<Renderer> ();
					rend.material.color = new Color (1.0f, 1.0f, 1.0f);
				}
			}

		}

		//obeCube.transform.rotation.Set (plugin.QuaternionLeft.x, plugin.QuaternionLeft.y, 
		//	plugin.QuaternionLeft.z, plugin.QuaternionLeft.w);

		//printQuaternion (plugin.QuaternionLeft.w, plugin.QuaternionLeft.x, plugin.QuaternionLeft.y, 
		//	plugin.QuaternionLeft.z, 0);

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
