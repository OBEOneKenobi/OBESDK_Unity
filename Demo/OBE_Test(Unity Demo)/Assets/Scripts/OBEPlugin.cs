using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;

public class OBEPlugin {

	//private float w, x, y, z;
	private bool isInitiated = false, isScanning = false;
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

	public delegate void QuaternionCallbackDelegate(float w, float x, float y, float z, int identifier);
	public delegate void FoundOBECallbackDelegate (IntPtr obeName, int index);
	public delegate void DidConnectOBECallbackDelegate (IntPtr obeName);
	public delegate void DidFindOBEServiceDelegate (IntPtr obeService);
	public delegate void DidFindOBECharacteristicDelegate (IntPtr obeCharacteristic);

	public OBEPluginCallback callback;

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

	[DllImport (pluginName)]
	public static extern int getButtons();
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
	/*
	public static void QuaternionUpdated(QuaternionCallbackDelegate callbackMethod);

	class OBECallback : AndroidJavaProxy
	{
		public OBECallback() : base("com.machina.OBECallback") { }

		void onQuaternionUpdated(float w, float x, float y, float z){
			
		}

	}*/

	#endif

	public void fetch(){
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

			buttons = getButtons ();
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

			buttons = obeJava.Call<int>("getButtons");
			#endif

			//Debug.Log (axLeft.ToString() + "," + ayLeft.ToString() + "," + azLeft.ToString());
			//Debug.Log (buttons.ToString ());
			button1 = ((buttons & 0x01) > 0) ? true : false;
			button2 = ((buttons & 0x02) > 0) ? true : false;
			button3 = ((buttons & 0x04) > 0) ? true : false;
			button4 = ((buttons & 0x08) > 0) ? true : false;
			//Debug.Log (button1.ToString() + button2.ToString() + button3.ToString() + button4.ToString());

			float rollLeftAux = calculateRoll (-1.0f * azLeft, axLeft);
			float pitchLeftAux = -1.0f * calculatePitch (ayLeft, axLeft, -1.0f * azLeft);
			rollLeft = alpha * rollLeftAux + (1.0f - alpha) * rollLeft;
			pitchLeft = alpha * pitchLeftAux + (1.0f - alpha) * pitchLeft;

			//Debug.Log (axLeft.ToString() + "," + ayLeft.ToString() + "," + azLeft.ToString());
			/*Debug.Log ("Pitch " + pitchLeft.ToString() 
				+ ", Roll: " + rollLeft.ToString());*/

			calculateQuaternion (pitchLeft, rollLeft, 0.0f, OBEQuaternionLeft);


			//rollLeft = calculateRoll (azLeft, -1.0f * axLeft);
			//pitchLeft = -1.0f * calculatePitch (ayLeft, axLeft, azLeft);

			float rollRightAux = calculateRoll (azRight, -1.0f * axRight);
			float pitchRightAux = -1.0f * calculatePitch (ayRight, axRight, azRight);
			rollRight = alpha * rollRightAux + (1.0f - alpha) * rollRight;
			pitchRight = alpha * pitchRightAux + (1.0f - alpha) * pitchRight;

			//Debug.Log (axLeft.ToString() + "," + ayLeft.ToString() + "," + azLeft.ToString());
			//Debug.Log ("Pitch " + pitchRight.ToString() 
			//+ ", Roll: " + rollRight.ToString());

			calculateQuaternion (pitchRight, rollRight, 0.0f, OBEQuaternionRight);

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

	public void init(){
		if (!isInitiated) {
			
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

	//private void setCallback(OBEPluginCallback cb){
	//	callback = cb;
	//}

	public void startScanning(){
		#if UNITY_IOS || UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
		StartScanning ();
		#elif UNITY_ANDROID
		//obeJava.Call("startBluetooth");
		#endif
		isScanning = true;
	}

	public void stopScanning(){
		#if UNITY_IOS || UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
		StopScanning ();
		#endif
	}

	public void connectToOBE(int index){
		#if UNITY_IOS || UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
		ConnectToOBE (index);
		#endif
	}

	public void disconnectFromOBE(){
		#if UNITY_IOS || UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
		DisconnectFromOBE ();
		#endif
	}

	public void updateMotors(){
		if ((OBEMotor1 != OBEMotor1_old) || (OBEMotor2 != OBEMotor2_old) || (OBEMotor3 != OBEMotor3_old) ||
			(OBEMotor4 != OBEMotor4_old)) {
		
			#if UNITY_IOS || UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
			if (isConnected () == 1) {
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
			}
			#endif
		}
	}

	/*abstract public void onQuaternionUpdated (float w, float x, float y, float z, int identifier);

	abstract public void FoundOBE (string name, int index);

	abstract public void FoundOBEService (string serviceName);

	abstract public void FoundOBECharacteristic (string characteristicName);
	*/

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

	/*******************************************************************************
	* Function Name: calculatePitch()
	********************************************************************************
	*   
	*   Summary:    This function calculates a Nerv's pitch. Integer to float is
	*               recommended and employed to keep data resolution.
	*   Parameters: ax  - x axis accelerometer value as a int16
	*               ay  - y axis accelerometer value as a int16
	*               az  - z axis accelerometer value as a int16
	*   Return:     The calculated pitch expressed in radians, as a float value 
	*
	*   TODO:       Optimise function
	*
	********************************************************************************/
	private float calculatePitch(float ax, float ay, float az){
		float localPitch = 0.0f, squareResult = 0.0f;

		squareResult = Mathf.Sqrt(ay * ay + az * az);
		localPitch = Mathf.Atan2(-ax, squareResult); // pitch in radians
		//localPitch = localPitch * 57.2957f; // pitch in degrees

		return localPitch;
	}

	/*******************************************************************************
	* Function Name: calculateRoll()
	********************************************************************************
	*   
	*   Summary:    This function calculates a Nerv's roll. Integer to float is
	*               recommended and employed to keep data resolution.
	*   Parameters: ay  - y axis accelerometer value as a int16
	*               az  - z axis accelerometer value as a int16
	*   Return:     The calculated roll expressed in radians, as a float value 
	*
	*   TODO:       Optimise function
	*
	********************************************************************************/
	private float calculateRoll(float ay, float az){
		float localRoll = 0.0f;

		localRoll = Mathf.Atan2(ay, az); // roll in radians
		//localRoll = localRoll * 57.2957f; // roll in degrees

		return localRoll;
	}

	/*******************************************************************************
	* Function Name: calculateYaw()
	********************************************************************************
	*   
	*   Summary:    This function calculates a Nerv's yaw. Integer to float is
	*               recommended and employed to keep data resolution.
	*   Parameters: roll    - roll expressed in radians, as a float value
	pitch   - pitch expressed in radians, as a float value
	*               mx      - x axis magnetometer value as a int16
	*               my      - y axis magnetometer value as a int16
	*               mz      - z axis magnetometer value as a int16
	*   Return:     The calculated yaw expressed in radians, as a float value 
	*
	*   TODO:       Optimise function
	*
	********************************************************************************/
	private float calculateYaw(float roll, float pitch, float mx, float my,
		float mz){
		float localYaw = 0.0f, upper = 0.0f, lower = 0.0f, sinRoll = 0.0f, cosRoll = 0.0f, 
			sinPitch = 0.0f, cosPitch = 0.0f;

		sinRoll = Mathf.Sin(roll);
		cosRoll = Mathf.Cos(roll); // / 57.2957f
		sinPitch = Mathf.Sin(pitch);
		cosPitch = Mathf.Cos(pitch);

		upper = mz * sinRoll - my * cosRoll;
		lower = mx * cosPitch + my * sinPitch * sinRoll + 
			mz * sinPitch * cosRoll;
		localYaw = Mathf.Atan2(upper, lower); // yaw in radians
		//localYaw = localYaw * 57.2957f; // yaw in angles

		return localYaw;
	}
}
