using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

abstract public class OBE : MonoBehaviour {

	#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
	private OBEPlugin plugin;
	#endif

	//public OBEPluginCallback callback;

	public void init(){
		#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX

		//callback = cb;

		plugin = new OBEPlugin ();
		plugin.init ();
		/*
		OBEPlugin.QuaternionUpdated ((float w, float x, float y, float z, int identifier) => {
			//Debug.Log(w.ToString() + "," + x.ToString() + "," + y.ToString() + "," + z.ToString());
			//if(callback != null){
				//QuaternionUpdated(w, x, y, z);
			//}
		});

		OBEPlugin.FoundOBE ((obeName, index) => {
			string name = Marshal.PtrToStringAuto(obeName);

			//Debug.Log("Found: " + name);
			//plugin.connectToOBE(0);
			//if(callback != null){
				//FoundOBE(name, index);
			//}
		});

		OBEPlugin.FoundOBEService((obeService) => {
			string name = Marshal.PtrToStringAuto(obeService);

			//Debug.Log("Service Found: " + name);
			//if(callback != null){
				//FoundOBEService(name);
			//}
		});

		OBEPlugin.FoundOBECharacteristic((obeCharacteristic) => {
			string name = Marshal.PtrToStringAuto(obeCharacteristic);

			//Debug.Log("Service Characteristic: " + name);
			//if(callback != null){
				//FoundOBECharacteristic(name);
			//}
		});*/

		plugin.startScanning ();
		#endif
	}

	public void startScanning(){
		#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
		if (plugin != null) {
			plugin.startScanning ();
		}
		#endif
	}

	public void stopScanning(){
		#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
		if (plugin != null) {
			plugin.stopScanning ();
		}
		#endif
	}

	public void connect(int index){
		#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
		if(plugin != null){
			plugin.connectToOBE(index);
		}
		#endif
	}

	public void disconnect(){
		#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
		if(plugin != null){
			plugin.disconnectFromOBE();
		}
		#endif
	}

	abstract public void QuaternionUpdated (float w, float x, float y, float z, int identifier);

	abstract public void FoundOBE (string name, int index);

	abstract public void FoundOBEService (string serviceName);

	abstract public void FoundOBECharacteristic (string characteristicName);
}
