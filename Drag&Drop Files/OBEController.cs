using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;
using UnityOSC;
using System.Collections.Generic;

public class OBEController : MonoBehaviour {

	public GameObject obeCube;
	public GameObject obeCubeBody;
	public GameObject obeCube2;
	public GameObject obeCube2Body;
	public GameObject obeCube3;

	private float W = 1, X = 1, Y = 1, Z = 1;

	private Boolean shouldUpdate = false;

	private string address;

	//private OBE obe;
	//private OBEPlugin plugin;
	private Quaternion auxQ, auxQ2, auxQ3, O1, O2, O3;
	private long testCounter;

	public String up = "off", down = "off", 
		left = "off", right = "off", attack = "off";

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

		OSCHandler.Instance.Init();

		Invoke ("startOBE", 1.0f); // start function 1 second after

		//Renderer rend = obeCubeBody.gameObject.GetComponent<Renderer>();
		//rend.material.color = new Color(1.0f, 0.0f, 0.0f);
	}

	void startOBE(){
//		if (plugin == null) {
//			plugin = new OBEPlugin ();
//			plugin.init ();
//		}
		OBEPlugin.Instance.init();
	}

	// Update is called once per frame
	void Update () {
		//rotateCube (W,X,Y,Z);

		if (OBEPlugin.Instance.isInitiated) {
		//if(plugin != null){
			// Send OSC message every frame
			//OSCHandler.Instance.SendMessageToClient("mainClient", "hello/",
			//	testCounter++);

			// TODO: in order to read, it's necessary to pass data from
			// 		 handler to this class...

			// try to connect to the first OBE that's nearby
			// if an OBE is connected, the following line does nothing
			OBEPlugin.Instance.connectToOBE (0);

			// fetch current data sent from OBE
			OBEPlugin.Instance.fetch ();

			//Debug.Log (plugin.bleState.ToString());

			if ((auxQ.w != OBEPlugin.Instance.QuaternionLeft.w) || (auxQ.x != OBEPlugin.Instance.QuaternionLeft.x) ||
				(auxQ.y != OBEPlugin.Instance.QuaternionLeft.y) || (auxQ.z != OBEPlugin.Instance.QuaternionLeft.z)) {
				obeCube.transform.rotation = O1 * OBEPlugin.Instance.QuaternionLeft;

				//printQuaternion(plugin.QuaternionLeft.eulerAngles.x, plugin.QuaternionLeft.eulerAngles.y,
				//	plugin.QuaternionLeft.eulerAngles.z,0,0);
			}

			if ((auxQ2.w != OBEPlugin.Instance.QuaternionRight.w) || (auxQ2.x != OBEPlugin.Instance.QuaternionRight.x) ||
				(auxQ2.y != OBEPlugin.Instance.QuaternionRight.y) || (auxQ2.z != OBEPlugin.Instance.QuaternionRight.z)) {
				obeCube2.transform.rotation = O2 * OBEPlugin.Instance.QuaternionRight;

				//printQuaternion(plugin.QuaternionLeft.eulerAngles.x, plugin.QuaternionLeft.eulerAngles.y,
				//	plugin.QuaternionLeft.eulerAngles.z,0,0);
			}

			if ((auxQ3.w != OBEPlugin.Instance.QuaternionCenter.w) || (auxQ3.x != OBEPlugin.Instance.QuaternionCenter.x) ||
				(auxQ3.y != OBEPlugin.Instance.QuaternionCenter.y) || (auxQ3.z != OBEPlugin.Instance.QuaternionCenter.z)) {
				obeCube3.transform.rotation = O3 * OBEPlugin.Instance.QuaternionCenter;

				//printQuaternion(plugin.QuaternionLeft.eulerAngles.x, plugin.QuaternionLeft.eulerAngles.y,
				//	plugin.QuaternionLeft.eulerAngles.z,0,0);
			}

			auxQ = OBEPlugin.Instance.QuaternionLeft;
			auxQ2 = OBEPlugin.Instance.QuaternionRight;
			auxQ3 = OBEPlugin.Instance.QuaternionCenter;

			// The following code was made to show whether a button is pressed or not
			shouldUpdate = !shouldUpdate;
			if (shouldUpdate) {

				if (Input.GetKey (KeyCode.A)) {
					OBEPlugin.Instance.OBEMotor1 = 1.0f;
				} else {
					OBEPlugin.Instance.OBEMotor1 = 0.0f;
				}

				if (Input.GetKey (KeyCode.S)) {
					OBEPlugin.Instance.OBEMotor2 = 1.0f;
				} else {
					OBEPlugin.Instance.OBEMotor2 = 0.0f;
				}

				if (Input.GetKey (KeyCode.D)) {
					OBEPlugin.Instance.OBEMotor3 = 1.0f;
				} else {
					OBEPlugin.Instance.OBEMotor3 = 0.0f;
				}

				if (Input.GetKey (KeyCode.W)) {
					OBEPlugin.Instance.OBEMotor4 = 1.0f;
				} else {
					OBEPlugin.Instance.OBEMotor4 = 0.0f;
				}

				// OSC detection
				if (OSCHandler.Instance.shouldVibrate) {
					OBEPlugin.Instance.OBEMotor1 = 1.0f; OBEPlugin.Instance.OBEMotor2 = 1.0f;
					OBEPlugin.Instance.OBEMotor3 = 1.0f; OBEPlugin.Instance.OBEMotor4 = 1.0f;
				} else {
					OBEPlugin.Instance.OBEMotor1 = 0.0f; OBEPlugin.Instance.OBEMotor2 = 0.0f;
					OBEPlugin.Instance.OBEMotor3 = 0.0f; OBEPlugin.Instance.OBEMotor4 = 0.0f;
				}

				//Debug.Log (plugin.OBEMotor1.ToString());
				OBEPlugin.Instance.updateMotors ();
			}

			// LEFT
			// ---------
			//if (plugin.button1) {
			if (OBEPlugin.Instance.LeftButton1) {
				Renderer rend = obeCube2Body.gameObject.GetComponent<Renderer> ();
				rend.material.color = new Color (0.0f, 1.0f, 0.0f);

				up = "on"; down = "off"; left = "off"; right = "off";
			} else {
				//if (plugin.button2) {
				if (OBEPlugin.Instance.LeftButton2) {
					Renderer rend = obeCube2Body.gameObject.GetComponent<Renderer> ();
					rend.material.color = new Color (1.0f, 0.0f, 0.0f);

					up = "off"; down = "on"; left = "off"; right = "off";
				} else {
					//if (plugin.button3) {
					if (OBEPlugin.Instance.LeftButton3) {
						Renderer rend = obeCube2Body.gameObject.GetComponent<Renderer> ();
						rend.material.color = new Color (0.0f, 0.0f, 1.0f);

						up = "off"; down = "off"; left = "on"; right = "off";
					} else {
						//if (plugin.button4) {
						if (OBEPlugin.Instance.LeftButton4) {
							Renderer rend = obeCube2Body.gameObject.GetComponent<Renderer> ();
							rend.material.color = new Color (1.0f, 0.0f, 1.0f);

							up = "off"; down = "off"; left = "off"; right = "on";
						} else {
							Renderer rend = obeCube2Body.gameObject.GetComponent<Renderer> ();
							rend.material.color = new Color (1.0f, 1.0f, 1.0f);

							up = "off"; down = "off"; left = "off"; right = "off";
						}
					}
					//Renderer rend = obeCube2Body.gameObject.GetComponent<Renderer> ();
					//rend.material.color = new Color (1.0f, 1.0f, 1.0f);
				}
			}
			//if (plugin.button3) {
			/*if (plugin.LeftButton3) {
				Renderer rend = obeCube2Body.gameObject.GetComponent<Renderer> ();
				rend.material.color = new Color (0.0f, 0.0f, 1.0f);
			} else {
				//if (plugin.button4) {
				if (plugin.LeftButton4) {
					Renderer rend = obeCube2Body.gameObject.GetComponent<Renderer> ();
					rend.material.color = new Color (1.0f, 0.0f, 1.0f);
				} else {
					Renderer rend = obeCube2Body.gameObject.GetComponent<Renderer> ();
					rend.material.color = new Color (1.0f, 1.0f, 1.0f);
				}
			}*/
			//---------

			//if (plugin.button1) {
			if (OBEPlugin.Instance.RightButton1) {
				Renderer rend = obeCubeBody.gameObject.GetComponent<Renderer> ();
				rend.material.color = new Color (0.0f, 1.0f, 0.0f);

				attack = "on";
			} else {

				attack = "off";
				//if (plugin.button2) {
				if (OBEPlugin.Instance.RightButton2) {
					Renderer rend = obeCubeBody.gameObject.GetComponent<Renderer> ();
					rend.material.color = new Color (1.0f, 0.0f, 0.0f);
				} else {
					//Renderer rend = obeCubeBody.gameObject.GetComponent<Renderer> ();
					//rend.material.color = new Color (1.0f, 1.0f, 1.0f);
					//if (plugin.button3) {
					if (OBEPlugin.Instance.RightButton3) {
						Renderer rend = obeCubeBody.gameObject.GetComponent<Renderer> ();
						rend.material.color = new Color (0.0f, 0.0f, 1.0f);
					} else {
						//if (plugin.button4) {
						if (OBEPlugin.Instance.RightButton4) {
							Renderer rend = obeCubeBody.gameObject.GetComponent<Renderer> ();
							rend.material.color = new Color (1.0f, 0.0f, 1.0f);
						} else {
							Renderer rend = obeCubeBody.gameObject.GetComponent<Renderer> ();
							rend.material.color = new Color (1.0f, 1.0f, 1.0f);
						}
					}
				}
			}

			// Update Battery

			address = up + "," + down + "," + left + "," + right + "," + attack;
			OSCHandler.Instance.SendMessageToClient ("mainClient", address, 1);


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
