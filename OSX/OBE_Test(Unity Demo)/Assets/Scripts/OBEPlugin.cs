using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;

public class OBEPlugin {

	public Quaternion QuaternionLeft;
	public Quaternion QuaternionRight;
	public Quaternion QuaternionCenter;

	private float w, x, y, z;
	private bool isInitiated = false;
	//private bool isOBEConnected = false;
	private static int OBEQuaternionLeft = 0;
	private static int OBEQuaternionRight = 1;
	private static int OBEQuaternionCenter = 2;

	public delegate void QuaternionCallbackDelegate(float w, float x, float y, float z, int identifier);
	public delegate void FoundOBECallbackDelegate (IntPtr obeName, int index);
	public delegate void DidConnectOBECallbackDelegate (IntPtr obeName);
	public delegate void DidFindOBEServiceDelegate (IntPtr obeService);
	public delegate void DidFindOBECharacteristicDelegate (IntPtr obeCharacteristic);

	public OBEPluginCallback callback;

	#if UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX

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

	[DllImport ("OBEPlugin_OSX")]
	public static extern void Init();

	[DllImport ("OBEPlugin_OSX")]
	public static extern void StartScanning();

	[DllImport ("OBEPlugin_OSX")]
	public static extern void StopScanning();

	[DllImport ("OBEPlugin_OSX")]
	public static extern void ConnectToOBE (int index);

	[DllImport ("OBEPlugin_OSX")]
	public static extern void DisconnectFromOBE();

	[DllImport ("OBEPlugin_OSX")]
	public static extern int isConnected();

	[DllImport ("OBEPlugin_OSX")]
	public static extern float getQW(int identifier);

	[DllImport ("OBEPlugin_OSX")]
	public static extern float getQX(int identifier);

	[DllImport ("OBEPlugin_OSX")]
	public static extern float getQY(int identifier);

	[DllImport ("OBEPlugin_OSX")]
	public static extern float getQZ(int identifier);

	#elif UNITY_IOS



	#elif UNITY_ANDROID
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
		if (isConnected() == 1) {
			w = getQW (OBEQuaternionLeft); x = getQX (OBEQuaternionLeft);
			y = getQY (OBEQuaternionLeft); z = getQZ (OBEQuaternionLeft);
			QuaternionLeft.Set (x,y,z,w);

			w = getQW (OBEQuaternionRight); x = getQX (OBEQuaternionRight);
			y = getQY (OBEQuaternionRight); z = getQZ (OBEQuaternionRight);
			QuaternionRight.Set (x,y,z,w);

			w = getQW (OBEQuaternionCenter); x = getQX (OBEQuaternionCenter);
			y = getQY (OBEQuaternionCenter); z = getQZ (OBEQuaternionCenter);
			QuaternionCenter.Set (x,y,z,w);
		}
	}

	public void init(){
		if (!isInitiated) {
			QuaternionLeft = new Quaternion (0,0,0,1);
			QuaternionRight = new Quaternion (0,0,0,1);
			QuaternionCenter = new Quaternion (0,0,0,1);

			Init ();

			isInitiated = true;
		}
	}

	//private void setCallback(OBEPluginCallback cb){
	//	callback = cb;
	//}

	public void startScanning(){
		StartScanning ();
	}

	public void stopScanning(){
		StopScanning ();
	}

	public void connectToOBE(int index){
		ConnectToOBE (index);
	}

	public void disconnectFromOBE(){
		DisconnectFromOBE ();
	}

	/*abstract public void onQuaternionUpdated (float w, float x, float y, float z, int identifier);

	abstract public void FoundOBE (string name, int index);

	abstract public void FoundOBEService (string serviceName);

	abstract public void FoundOBECharacteristic (string characteristicName);
	*/
}
