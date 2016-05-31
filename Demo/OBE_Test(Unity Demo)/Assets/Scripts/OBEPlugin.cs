using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;

public class OBEPlugin : MonoBehaviour{

	public bool isInitiated = false, isScanning = false;
	public bool readyToScan = false;
	public int bleState = 0;
	private int counter = 0;
	//private bool isOBEConnected = false;
	private static int OBEQuaternionLeft = 0;
	private static int OBEQuaternionRight = 1;
	private static int OBEQuaternionCenter = 2;
	private static float alpha = 0.1f; // factor to tune

	public float axLeft, ayLeft, azLeft;
	public float gxLeft, gyLeft, gzLeft;
	public float mxLeft, myLeft, mzLeft;
	public float rollLeft, pitchLeft, yawLeft;
	public Quaternion QuaternionLeft;

	public Boolean button1, button2, button3, button4;
	public Boolean LeftButton1, LeftButton2, LeftButton3, LeftButton4;
	public Boolean RightButton1, RightButton2, RightButton3, RightButton4;
	public Boolean LogoButton;

	public float axRight, ayRight, azRight;
	public float gxRight, gyRight, gzRight;
	public float mxRight, myRight, mzRight;
	public float rollRight, pitchRight, yawRight;
	public Quaternion QuaternionRight;

	public float axCenter, ayCenter, azCenter;
	public float gxCenter, gyCenter, gzCenter;
	public float mxCenter, myCenter, mzCenter;
	public float rollCenter, pitchCenter, yawCenter;
	public Quaternion QuaternionCenter;

	public float OBEMotor1, OBEMotor2, OBEMotor3, OBEMotor4;
	private float OBEMotor1_old, OBEMotor2_old, OBEMotor3_old, OBEMotor4_old;

	public float BatteryLevel;

	private static OBEPlugin _instance = null;

	private float[] q = new float[4];

	/*public delegate void QuaternionCallbackDelegate(float w, float x, float y, float z, int identifier);
	public delegate void FoundOBECallbackDelegate (IntPtr obeName, int index);
	public delegate void DidConnectOBECallbackDelegate (IntPtr obeName);
	public delegate void DidFindOBEServiceDelegate (IntPtr obeService);
	public delegate void DidFindOBECharacteristicDelegate (IntPtr obeCharacteristic);

	public OBEPluginCallback callback;*/

	#if UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX || UNITY_IOS

	#if UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
	public const String pluginName = "OBEPlugin_OSX";
	#elif UNITY_IOS
	public const String pluginName = "__Internal";
	#endif

	/*
	[DllImport ("OBEPlugin_OSX")]
	public static extern void QuaternionUpdated ([MarshalAs(UnmanagedType.FunctionPtr)]QuaternionCallbackDelegate callbackMethod);
	//[DllImport ("OBEPlugin_OSX")]
	//public static extern void QuaternionUpdated_Right ([MarshalAs(UnmanagedType.FunctionPtr)]QuaternionCallbackDelegate callbackMethod);
	[DllImport ("OBEPlugin_OSX")]
	public static extern void FoundOBE ([MarshalAs(UnmanagedType.FunctionPtr)]FoundOBECallbackDelegate callbackMethod);
	[DllImport ("OBEPlugin_OSX")]
	public static extern void OBEConnected ([MarshalAs(UnmanagedType.FunctionPtr)]DidConnectOBECallbackDelegate callbackMethod);
	[DllImport ("OBEPlugin_OSX")]
	public static extern void FoundOBEService ([MarshalAs(UnmanagedType.FunctionPtr)]DidFindOBEServiceDelegate callbackMethod);
	[DllImport ("OBEPlugin_OSX")]
	public static extern void FoundOBECharacteristic ([MarshalAs(UnmanagedType.FunctionPtr)]DidFindOBECharacteristicDelegate callbackMethod);
	*/

	[DllImport (pluginName)]
	public static extern void Init();
	[DllImport (pluginName)]
	public static extern void StartScanning();
	[DllImport (pluginName)]
	public static extern void StopScanning();
	[DllImport (pluginName)]
	public static extern void ConnectToOBE (int index);
	[DllImport (pluginName)]
	public static extern void DisconnectFromOBE();
	[DllImport (pluginName)]
	public static extern int isConnected();
	[DllImport (pluginName)]
	public static extern int isReadyToScan();
	[DllImport (pluginName)]
	public static extern void UpdateMotors();
	[DllImport (pluginName)]
	public static extern void setMotor1(float speed);
	[DllImport (pluginName)]
	public static extern void setMotor2(float speed);
	[DllImport (pluginName)]
	public static extern void setMotor3(float speed);
	[DllImport (pluginName)]
	public static extern void setMotor4(float speed);

	//[DllImport (pluginName)]
	//public static extern int getButtons();
	[DllImport (pluginName)]
	public static extern int getButtons(int identifier); //TODO: implement this function in iOS
	[DllImport (pluginName)]
	public static extern int getBattery();
	[DllImport (pluginName)]
	public static extern float getQW(int identifier);
	[DllImport (pluginName)]
	public static extern float getQX(int identifier);
	[DllImport (pluginName)]
	public static extern float getQY(int identifier);
	[DllImport (pluginName)]
	public static extern float getQZ(int identifier);

	[DllImport (pluginName)]
	public static extern float getAX(int identifier);
	[DllImport (pluginName)]
	public static extern float getAY(int identifier);
	[DllImport (pluginName)]
	public static extern float getAZ(int identifier);
	[DllImport (pluginName)]
	public static extern float getGX(int identifier);
	[DllImport (pluginName)]
	public static extern float getGY(int identifier);
	[DllImport (pluginName)]
	public static extern float getGZ(int identifier);
	[DllImport (pluginName)]
	public static extern float getMX(int identifier);
	[DllImport (pluginName)]
	public static extern float getMY(int identifier);
	[DllImport (pluginName)]
	public static extern float getMZ(int identifier);


	#elif UNITY_ANDROID

	AndroidJavaObject obeJava = null;
	AndroidJavaObject activityContext = null;

	#endif

	#region Singleton Constructors
	static OBEPlugin(){ }

	OBEPlugin(){ }

	public static OBEPlugin Instance {
		get {
			if (_instance == null) {
				_instance = new GameObject ("OBEPlugin").AddComponent<OBEPlugin>();
			}

			return _instance;
		}
	}
	#endregion

	/*******************************************************************************
	* @name fetch()
	********************************************************************************
	*   
	*   Summary:    This function loads all current values from the native plugin
	* 				libraries. On iOS and OSX, this function will start if there is
	* 				no active scanning happening in background.
	*
	*   @todo       Optimize function.
	*
	********************************************************************************/
	public void fetch(){

		int auxLeftButtons = 0, auxRightButtons, auxLogoButtons = 0;

		#if UNITY_IOS || UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
		if (isConnected () == 1) {
		#elif UNITY_ANDROID



		#endif
			//isScanning = false;
			int buttons;
			#if UNITY_IOS || UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
			axLeft = getAX (OBEQuaternionLeft); ayLeft = getAY (OBEQuaternionLeft);
			azLeft = getAZ (OBEQuaternionLeft);
			gxLeft = getGX (OBEQuaternionLeft); gyLeft = getGY (OBEQuaternionLeft);
			gzLeft = getGZ (OBEQuaternionLeft);
			mxLeft = getMX (OBEQuaternionLeft); myLeft = getMY (OBEQuaternionLeft);
			mzLeft = getMZ (OBEQuaternionLeft);

			axRight = getAX (OBEQuaternionRight); ayRight = getAY (OBEQuaternionRight);
			azRight = getAZ (OBEQuaternionRight);
			gxRight = getGX (OBEQuaternionRight); gyRight = getGY (OBEQuaternionRight);
			gzRight = getGZ (OBEQuaternionRight);
			mxRight = getMX (OBEQuaternionRight); myRight = getMY (OBEQuaternionRight);
			mzRight = getMZ (OBEQuaternionRight);

			axCenter = getAX (OBEQuaternionCenter); ayCenter = getAY (OBEQuaternionCenter);
			azCenter = getAZ (OBEQuaternionCenter);
			gxCenter = getGX (OBEQuaternionCenter); gyCenter = getGY (OBEQuaternionCenter);
			gzCenter = getGZ (OBEQuaternionCenter);
			mxCenter = getMX (OBEQuaternionCenter); myCenter = getMY (OBEQuaternionCenter);
			mzCenter = getMZ (OBEQuaternionCenter);

			//Debug.Log(gxRight.ToString() + "," + gyRight.ToString() + "," + gzRight.ToString());

			//buttons = getButtons ();
			auxLeftButtons = getButtons(OBEQuaternionLeft);
			auxRightButtons = getButtons(OBEQuaternionRight);
			auxLogoButtons = getButtons(OBEQuaternionCenter); // Center applies to Logo in this case

			BatteryLevel = getBattery();

			#elif UNITY_ANDROID
			axLeft = obeJava.Call<float>("getAX", OBEQuaternionLeft); 
			ayLeft = obeJava.Call<float>("getAY", OBEQuaternionLeft);
			azLeft = obeJava.Call<float>("getAZ", OBEQuaternionLeft);
			/*gxLeft = obeJava.Call<float>("getGX", OBEQuaternionLeft); 
			gyLeft = obeJava.Call<float>("getGY", OBEQuaternionLeft);
			gzLeft = obeJava.Call<float>("getGZ", OBEQuaternionLeft);
			mxLeft = obeJava.Call<float>("getMX", OBEQuaternionLeft); 
			myLeft = obeJava.Call<float>("getMY", OBEQuaternionLeft);
			mzLeft = obeJava.Call<float>("getMZ", OBEQuaternionLeft);*/

			axRight = obeJava.Call<float>("getAX", OBEQuaternionRight); 
			ayRight = obeJava.Call<float>("getAY", OBEQuaternionRight);
			azRight = obeJava.Call<float>("getAZ", OBEQuaternionRight);
			/*gxRight = obeJava.Call<float>("getGX", OBEQuaternionRight); 
			gyRight = obeJava.Call<float>("getGY", OBEQuaternionRight);
			gzRight = obeJava.Call<float>("getGZ", OBEQuaternionRight);
			mxRight = obeJava.Call<float>("getMX", OBEQuaternionRight); 
			myRight = obeJava.Call<float>("getMY", OBEQuaternionRight);
			mzRight = obeJava.Call<float>("getMZ", OBEQuaternionRight);*/

			/*axCenter = obeJava.Call<float>("getAX", OBEQuaternionCenter); 
			ayCenter = obeJava.Call<float>("getAY", OBEQuaternionCenter);
			azCenter = obeJava.Call<float>("getAZ", OBEQuaternionCenter);
			gxCenter = obeJava.Call<float>("getGX", OBEQuaternionCenter); 
			gyCenter = obeJava.Call<float>("getGY", OBEQuaternionCenter);
			gzCenter = obeJava.Call<float>("getGZ", OBEQuaternionCenter);
			mxCenter = obeJava.Call<float>("getMX", OBEQuaternionCenter); 
			myCenter = obeJava.Call<float>("getMY", OBEQuaternionCenter);
			mzCenter = obeJava.Call<float>("getMZ", OBEQuaternionCenter);*/

			//buttons = obeJava.Call<int>("getButtons");
			auxLeftButtons = obeJava.Call<int>("getButtons", OBEQuaternionLeft);
			auxRightButtons = obeJava.Call<int>("getButtons", OBEQuaternionRight);
			auxLogoButtons = obeJava.Call<int>("getButtons", OBEQuaternionCenter); // Center applies to Logo in this case

			BatteryLevel = obeJava.Call<float>("getBattery");
			#endif

			parseButtons (auxLeftButtons, OBEQuaternionLeft);
			parseButtons (auxRightButtons, OBEQuaternionRight);
			parseButtons (auxLogoButtons, OBEQuaternionCenter); // Center applies to Logo in this case

			//Debug.Log (axLeft.ToString() + "," + ayLeft.ToString() + "," + azLeft.ToString());
			//Debug.Log (buttons.ToString ());
			//Debug.Log (button1.ToString() + button2.ToString() + button3.ToString() + button4.ToString());

			//LEFT
			//float rollLeftAux = OBEMath.calculateRoll (-1.0f * azLeft, axLeft);
			//float pitchLeftAux = -1.0f * OBEMath.calculatePitch (ayLeft, axLeft, -1.0f * azLeft);
			float rollLeftAux = OBEMath.calculateRoll (-1.0f * azLeft, -1.0f * ayLeft);
			float pitchLeftAux = -1.0f * OBEMath.calculatePitch (axLeft, ayLeft, -1.0f * azLeft);
			rollLeft = alpha * rollLeftAux + (1.0f - alpha) * rollLeft;
			pitchLeft = alpha * pitchLeftAux + (1.0f - alpha) * pitchLeft;

			//Debug.Log (axLeft.ToString() + "," + ayLeft.ToString() + "," + azLeft.ToString());
			/*Debug.Log ("Pitch " + pitchLeft.ToString() 
				+ ", Roll: " + rollLeft.ToString());*/

			calculateQuaternion (-pitchLeft, -rollLeft, 0.0f, OBEQuaternionLeft);


			//rollLeft = calculateRoll (azLeft, -1.0f * axLeft);
			//pitchLeft = -1.0f * calculatePitch (ayLeft, axLeft, azLeft);

			/*
			MadgwickQuaternionNoMag (axRight, ayRight, azRight, gxRight * Mathf.PI / 180.0f, 
				gyRight * Mathf.PI / 180.0f, gzRight * Mathf.PI / 180.0f);
			QuaternionRight.Set (q[0], q[1], q[2], q[3]);
			*/

			//RIGHT
			//float rollRightAux = OBEMath.calculateRoll (azRight, -1.0f * axRight);
			//float pitchRightAux = -1.0f * OBEMath.calculatePitch (ayRight, axRight, azRight);
			float rollRightAux = OBEMath.calculateRoll (azRight, ayRight);
			float pitchRightAux = -1.0f * OBEMath.calculatePitch (axRight, ayRight, azRight);
			rollRight = alpha * rollRightAux + (1.0f - alpha) * rollRight;
			pitchRight = alpha * pitchRightAux + (1.0f - alpha) * pitchRight;

			//Debug.Log (axLeft.ToString() + "," + ayLeft.ToString() + "," + azLeft.ToString());
			//Debug.Log ("Pitch " + pitchRight.ToString() 
			//+ ", Roll: " + rollRight.ToString());

			// 2.5 , 0, -5

			calculateQuaternion (-pitchRight, -rollRight, 0, OBEQuaternionRight);


			//CENTER
			float rollCenterAux = OBEMath.calculateRoll (azCenter, -1.0f * axCenter);
			float pitchCenterAux = -1.0f * OBEMath.calculatePitch (ayCenter, axCenter, azCenter);
			rollCenter = alpha * rollCenterAux + (1.0f - alpha) * rollCenter;
			pitchCenter = alpha * pitchCenterAux + (1.0f - alpha) * pitchCenter;

			//Debug.Log (axLeft.ToString() + "," + ayLeft.ToString() + "," + azLeft.ToString());
			//Debug.Log ("Pitch " + (pitchRight * 180 / Mathf.PI).ToString() 
			//	+ ", Roll: " + (rollRight * 180 / Mathf.PI).ToString());

			//calculateQuaternion (pitchCenter , rollCenter, 0.0f, OBEQuaternionCenter);

			/*w = getQW (OBEQuaternionLeft); x = getQX (OBEQuaternionLeft);
			y = getQY (OBEQuaternionLeft); z = getQZ (OBEQuaternionLeft);
			QuaternionLeft.Set (x,y,z,w);

			w = getQW (OBEQuaternionRight); x = getQX (OBEQuaternionRight);
			y = getQY (OBEQuaternionRight); z = getQZ (OBEQuaternionRight);
			QuaternionRight.Set (x,y,z,w);

			w = getQW (OBEQuaternionCenter); x = getQX (OBEQuaternionCenter);
			y = getQY (OBEQuaternionCenter); z = getQZ (OBEQuaternionCenter);
			QuaternionCenter.Set (x,y,z,w);*/
		#if UNITY_IOS || UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
		} else { // if not connected, try to scan
			
			if ((isReadyToScan() == 4) && (!isScanning)) {
				startScanning ();
			}

			/*if (isReadyToScan () == 1) {
				readyToScan = true;
			}*/
			//bleState = isReadyToScan ();
		}
		#endif
	}

	/*******************************************************************************
	* @name init()
	********************************************************************************
	*   
	*   Summary:    This funciton initiates the native plugin.
	*
	*   @todo       Nothing.
	*
	********************************************************************************/
	public void init(){
		if (!isInitiated) {

			q [0] = 1.0f; q [1] = 0.0f; q [2] = 0.0f; q [3] = 0.0f;

			QuaternionLeft = new Quaternion (1,0,0,0);
			QuaternionRight = new Quaternion (1,0,0,0);
			QuaternionCenter = new Quaternion (1,0,0,0);

			#if UNITY_IOS || UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
			Init ();
			#elif UNITY_ANDROID
			if(obeJava == null) {
				using(AndroidJavaClass activityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer")) {
					activityContext = activityClass.GetStatic<AndroidJavaObject>("currentActivity");
				}	

				using(AndroidJavaClass pluginClass = new AndroidJavaClass("com.machina.OBEPlugin")) {
					if(pluginClass != null) {
						obeJava = pluginClass.CallStatic<AndroidJavaObject>("instance");
						obeJava.Call("setContext", activityContext);
						/*activityContext.Call("runOnUiThread", new AndroidJavaRunnable(() => {
							obeJava.Call("showMessage", "INIT");
							
						}));*/

						obeJava.Call("startBluetooth");
					}
				}
			}
			#endif

			isInitiated = true;
		}
	}

	/*******************************************************************************
	* @name startScanning()
	********************************************************************************
	*   
	*   Summary:    This function starts scanning for OBE devices.
	*
	*   @todo       Nothing
	*
	********************************************************************************/
	public void startScanning(){
		#if UNITY_IOS || UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
		StartScanning ();
		#elif UNITY_ANDROID
		//obeJava.Call("startBluetooth");
		#endif
		isScanning = true;
	}

	/*******************************************************************************
	* @name stopScanning()
	********************************************************************************
	*   
	*   Summary:    This function stops scanning for OBE devices.
	*
	*   @todo       Nothing
	*
	********************************************************************************/
	public void stopScanning(){
		#if UNITY_IOS || UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
		StopScanning ();
		#endif
	}

	/*******************************************************************************
	* @name connectToOBE()
	********************************************************************************
	*   
	*   Summary:    This function connects to an OBE device. 
	* 
	* 	@param		index - denotes the index of an OBE device found
	*
	*   @todo       Nothing
	*
	********************************************************************************/
	public void connectToOBE(int index){
		#if UNITY_IOS || UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
		ConnectToOBE (index);
		#endif
	}

	/*******************************************************************************
	* @name disconnectFromOBE()
	********************************************************************************
	*   
	*   Summary:    This function starts scanning for OBE devices.
	*
	*   @todo       Nothing
	*
	********************************************************************************/
	public void disconnectFromOBE(){
		#if UNITY_IOS || UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
		DisconnectFromOBE ();
		#endif
	}

	/*******************************************************************************
	* @name updateMotors()
	********************************************************************************
	*   
	*   Summary:    This function updates current motor values.
	*
	*   @todo       Nothing
	*
	********************************************************************************/
	public void updateMotors(){
		//Debug.Log("Trying to Update");
		//if ((OBEMotor1 != OBEMotor1_old) || (OBEMotor2 != OBEMotor2_old) || (OBEMotor3 != OBEMotor3_old) ||
		//	(OBEMotor4 != OBEMotor4_old)) {

			//Debug.Log("Almost Updating");
		
			#if UNITY_IOS || UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
			if (isConnected () == 1) {
				//Debug.Log("About to Update");
				//counter++;
				//Debug.Log (counter.ToString());
				OBEMotor1_old = OBEMotor1;
				OBEMotor2_old = OBEMotor2;
				OBEMotor3_old = OBEMotor3;
				OBEMotor4_old = OBEMotor4;

				setMotor1 (OBEMotor1);
				setMotor2 (OBEMotor2);
				setMotor3 (OBEMotor3);
				setMotor4 (OBEMotor4);
				UpdateMotors ();

				//Debug.Log("Motor Updated");
			}
			#endif
		//}
	}

	/*abstract public void onQuaternionUpdated (float w, float x, float y, float z, int identifier);

	abstract public void FoundOBE (string name, int index);

	abstract public void FoundOBEService (string serviceName);

	abstract public void FoundOBECharacteristic (string characteristicName);
	*/

	/*******************************************************************************
	* @name parseButtons()
	********************************************************************************
	*   
	*   Summary:    This function parses and assigns a button value based on its
	*				identifier.
	*
	*	@param		button 		- button value
	*	@param		identifier 	- value that determines which hand owns the button
	*
	*   @todo       Possible function optimization
	*
	********************************************************************************/
	private void parseButtons(int button, int identifier){

		Boolean auxbutton1 = ((button & 0x01) > 0) ? true : false;
		Boolean auxbutton2 = ((button & 0x02) > 0) ? true : false;
		Boolean auxbutton3 = ((button & 0x04) > 0) ? true : false;
		Boolean auxbutton4 = ((button & 0x08) > 0) ? true : false;

		switch (identifier) {
			case 0:
				LeftButton1 = auxbutton1;
				LeftButton2 = auxbutton2;
				LeftButton3 = auxbutton3;
				LeftButton4 = auxbutton4;
				break;
			case 1:
				RightButton1 = auxbutton1;
				RightButton2 = auxbutton2;
				RightButton3 = auxbutton3;
				RightButton4 = auxbutton4;
				break;
			case 2:
				LogoButton = auxbutton1;
				break;
		}
	}

	/*******************************************************************************
	* @name calculateQuaternion()
	********************************************************************************
	*   
	*   Summary:    This function parses and assigns a button value based on its
	*				identifier.
	*
	*	@param		roll 		- roll of the given hand
	*	@param		pitch 		- pitch of the given hand
	*	@param		yaw			- yaw of the given hand
	*	@param		identifier	- value that indicates which hand's values are the 
	*							  ones to be processed
	*
	*   @todo       Possible function optimization.
	*
	********************************************************************************/
	private void calculateQuaternion(float roll, float pitch, float yaw, int identifier){
		float sinHalfYaw = Mathf.Sin(yaw / 2.0f);
		float cosHalfYaw = Mathf.Cos(yaw / 2.0f);
		float sinHalfPitch = Mathf.Sin(pitch/ 2.0f);
		float cosHalfPitch = Mathf.Cos(pitch / 2.0f);
		float sinHalfRoll = Mathf.Sin(roll / 2.0f);
		float cosHalfRoll = Mathf.Cos(roll / 2.0f);

		float x = -cosHalfRoll * sinHalfPitch * sinHalfYaw
			+ cosHalfPitch * cosHalfYaw * sinHalfRoll;
		float y = cosHalfRoll * cosHalfYaw * sinHalfPitch
			+ sinHalfRoll * cosHalfPitch * sinHalfYaw;
		float z = cosHalfRoll * cosHalfPitch * sinHalfYaw
			- sinHalfRoll * cosHalfYaw * sinHalfPitch;
		float w = cosHalfRoll * cosHalfPitch * cosHalfYaw
			+ sinHalfRoll * sinHalfPitch * sinHalfYaw;

		switch (identifier) {
			case 0:
				QuaternionLeft.Set (x, y, z, w);
				break;
			case 1:
				QuaternionRight.Set (x, y, z, w);
				break;
			case 2:
				QuaternionCenter.Set (x, y, z, w);
				break;
		}
	}

	void MadgwickQuaternionNoMag(float ax, float ay, float az, float gx, float gy, float gz){
		float q0 = q[0], q1 = q[1], q2 = q[2], q3 = q[3];   // short name local variable for readability
		float recipNorm;
		float s0, s1, s2, s3;
		float qDot1, qDot2, qDot3, qDot4;
		float _2q0, _2q1, _2q2, _2q3, _4q0, _4q1, _4q2 ,_8q1, _8q2, q0q0, q1q1, q2q2, q3q3;
		float betaDef = 0.1f;

		// Rate of change of quaternion from gyroscope
		qDot1 = 0.5f * (-q1 * gx - q2 * gy - q3 * gz);
		qDot2 = 0.5f * (q0 * gx + q2 * gz - q3 * gy);
		qDot3 = 0.5f * (q0 * gy - q1 * gz + q3 * gx);
		qDot4 = 0.5f * (q0 * gz + q1 * gy - q2 * gx);

		// Compute feedback only if accelerometer measurement valid (avoids NaN in accelerometer normalisation)
		if(!((ax == 0.0f) && (ay == 0.0f) && (az == 0.0f))) {

			// Normalise accelerometer measurement
			recipNorm = invSqrt(ax * ax + ay * ay + az * az);
			ax *= recipNorm;
			ay *= recipNorm;
			az *= recipNorm;

			// Auxiliary variables to avoid repeated arithmetic
			_2q0 = 2.0f * q0;
			_2q1 = 2.0f * q1;
			_2q2 = 2.0f * q2;
			_2q3 = 2.0f * q3;
			_4q0 = 4.0f * q0;
			_4q1 = 4.0f * q1;
			_4q2 = 4.0f * q2;
			_8q1 = 8.0f * q1;
			_8q2 = 8.0f * q2;
			q0q0 = q0 * q0;
			q1q1 = q1 * q1;
			q2q2 = q2 * q2;
			q3q3 = q3 * q3;

			// Gradient decent algorithm corrective step
			s0 = _4q0 * q2q2 + _2q2 * ax + _4q0 * q1q1 - _2q1 * ay;
			s1 = _4q1 * q3q3 - _2q3 * ax + 4.0f * q0q0 * q1 - _2q0 * ay - _4q1 + _8q1 * q1q1 + _8q1 * q2q2 + _4q1 * az;
			s2 = 4.0f * q0q0 * q2 + _2q0 * ax + _4q2 * q3q3 - _2q3 * ay - _4q2 + _8q2 * q1q1 + _8q2 * q2q2 + _4q2 * az;
			s3 = 4.0f * q1q1 * q3 - _2q1 * ax + 4.0f * q2q2 * q3 - _2q2 * ay;
			recipNorm = invSqrt(s0 * s0 + s1 * s1 + s2 * s2 + s3 * s3); // normalise step magnitude
			s0 *= recipNorm;
			s1 *= recipNorm;
			s2 *= recipNorm;
			s3 *= recipNorm;

			// Apply feedback step
			qDot1 -= betaDef * s0;
			qDot2 -= betaDef * s1;
			qDot3 -= betaDef * s2;
			qDot4 -= betaDef * s3;
		}



		float deltat = 0.0166f; // .041

		// Integrate rate of change of quaternion to yield quaternion
		q0 += qDot1 * deltat; 
		q1 += qDot2 * deltat;
		q2 += qDot3 * deltat;
		q3 += qDot4 * deltat;

		// Normalise quaternion
		recipNorm = invSqrt(q0 * q0 + q1 * q1 + q2 * q2 + q3 * q3);
		q0 *= recipNorm;
		q1 *= recipNorm;
		q2 *= recipNorm;
		q3 *= recipNorm;

		q[0] = q0; q[1] = q1; q[2] = q2; q[3] = q3; 
	}

	float invSqrt(float x) {
		/*float halfx = 0.5f * x;
		float y = x;
		long i = *(long*)&y;
		i = 0x5f3759df - (i>>1);
		y = *(float*)&i;
		y = y * (1.5f - (halfx * y * y));
		return y;*/

		return 1 / Mathf.Sqrt(x);
	}
}
