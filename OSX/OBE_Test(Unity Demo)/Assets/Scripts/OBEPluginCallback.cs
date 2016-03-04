using UnityEngine;
using System.Collections;

public interface OBEPluginCallback {
	void QuaternionUpdated(float w, float x, float y, float z, int identifier);
	void FoundOBE(string name, int index);
	void FoundOBEService(string serviceName);
	void FoundOBECharacteristic(string characteristicName);
}
