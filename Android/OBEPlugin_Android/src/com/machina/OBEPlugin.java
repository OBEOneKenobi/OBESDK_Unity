/** Copyright (C) Machina Wearable Technology - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Henry Serrano <henry@machina.cc>
 */

package com.machina;

import android.content.Context;
import android.widget.Toast;

import android.bluetooth.BluetoothDevice;
import android.bluetooth.*;
import android.bluetooth.BluetoothAdapter;
import android.util.Log;
import java.util.Set;
import java.util.List;
import java.nio.ByteBuffer;
import java.nio.ByteOrder;
import java.util.HashMap;
import java.util.UUID;

public class OBEPlugin {
	//TODO: Right and Center missing the right characteristic
	private static String QuaternionCharacteristic_Left = "0003cbb2-0000-1000-8000-00805f9b0131";
	private static String QuaternionCharacteristic_Right = "0003cbb2-0000-1000-8000-00805f9b0132";
	private static String QuaternionCharacteristic_Center = "0003cbb2-0000-1000-8000-00805f9b0133";
	
	private static int QuaternionLeft = 0;
	private static int QuaternionRight = 1;
	private static int QuaternionCenter = 2;
	private static int MPUDataSize = 20;
	
    private Context context;
    private static OBEPlugin instance;
    BluetoothAdapter mBtAdapter;
    private Set<BluetoothDevice> pairedDevices;
    BluetoothGatt mBluetoothGatt;
    //OBECallback callback;
    
    private float axLeft, ayLeft, azLeft;
    private float gxLeft, gyLeft, gzLeft;
    private float mxLeft, myLeft, mzLeft;
    private float axRight, ayRight, azRight;
    private float gxRight, gyRight, gzRight;
    private float mxRight, myRight, mzRight;
    private float axCenter, ayCenter, azCenter;
    private float gxCenter, gyCenter, gzCenter;
    private float mxCenter, myCenter, mzCenter;
    private int Buttons;
    
    //BluetoothGattCharacteristic correct_characteristic;
    BluetoothGattCharacteristic write_characteristic;

    //0 - not started
    //1 - connecting
    //2 - connected
    //3 - disconnected
    //4 - no jacket found
    //5 - other error
    /*
		0 NOT_STARTED,
		1 CONNECTING,
		2 CONNECTED,
		3 DISCCONECTED,
		4 NOT_FOUND,
		5 OTHER_ERROR,
		6 CALIBRATING,
		7 CALIBRATED,
		8 CONNECTED_NOT_CALIBRATED,
		9 STARTED_CALIBRATED //ERROR IT WAS ALREADY CALIBRATED WHEN WE STARTED
		*/
    int state = 0;
    byte[] completePacket = new byte[MPUDataSize];

    // original code had no callback
    /*public OBEPlugin(OBECallback cb) {
        this.instance = this;
        this.callback = cb;
    }*/
    
    // original code had no callback
    /*public static OBEPlugin instance(OBECallback cb) {
        if(instance == null) {
            //instance = new OBEPlugin(cb);
        	instance = new OBEPlugin();
        	Log.i("INIT", "YES");
        }
        return instance;
    }*/
    public static OBEPlugin instance() {
        if(instance == null) {
        	instance = new OBEPlugin();
        	Log.i("INIT", "YES");
        }
        return instance;
    }

    public void setContext(Context context) {
        this.context = context;
    }

    public void showMessage(String message) {
        Toast.makeText(this.context, message, Toast.LENGTH_SHORT).show();
    }

    public boolean canBLE(){
        /*if (!getPackageManager().hasSystemFeature(PackageManager.FEATURE_BLUETOOTH_LE)) {
                return false;
        }*/
        return true;
    }

    public void startBluetooth(){
            state = 1;
            mBtAdapter = BluetoothAdapter.getDefaultAdapter();
            Log.i("Discovery", "About to Start");
            this.doDiscovery();
    }

    public byte[] getData()
    {
        return completePacket;
    }

    public int getStatus()
    {
        return state;
    }
    
    public float getAX(int identifier){
    	float result = 0.0f;
    	switch(identifier){
    		case 0://QuaternionLeft
    			result = axLeft;
    			break;
    		case 1://QuaternionRight
    			result = axRight;
    			break;
    		case 2://QuaternionCenter
    			result = axCenter;
    			break;
    	}
    	return result;
    }
    
    public float getAY(int identifier){
    	float result = 0;
    	switch(identifier){
    		case 0://QuaternionLeft
    			result = ayLeft;
    		break;
    		case 1://QuaternionRight
    			result = ayRight;
    		break;
    		case 2://QuaternionCenter
    			result = ayCenter;
    		break;
    	}
    	return result;
    }
    
    public float getAZ(int identifier){
    	float result = 0;
    	switch(identifier){
    		case 0://QuaternionLeft
    			result = azLeft;
    		break;
    		case 1://QuaternionRight
    			result = azRight;
    		break;
    		case 2://QuaternionCenter
    			result = azCenter;
    		break;
    	}
    	return result;
    }

    public float getGX(int identifier){
    	float result = 0;
    	switch(identifier){
    		case 0://QuaternionLeft
    			result = gxLeft;
    		break;
    		case 1://QuaternionRight
    			result = gxRight;
    		break;
    		case 2://QuaternionCenter
    			result = gxCenter;
    		break;
    	}
    	return result;
    }
    
    public float getGY(int identifier){
    	float result = 0;
    	switch(identifier){
    		case 0://QuaternionLeft
    			result = gyLeft;
    		break;
    		case 1://QuaternionRight
    			result = gyRight;
    		break;
    		case 2://QuaternionCenter
    			result = gyCenter;
    		break;
    	}
    	return result;
    }
    
    public float getGZ(int identifier){
    	float result = 0;
    	switch(identifier){
    		case 0://QuaternionLeft
    			result = gzLeft;
    		break;
    		case 1://QuaternionRight
    			result = gzRight;
    		break;
    		case 2://QuaternionCenter
    			result = gzCenter;
    		break;
    	}
    	return result;
    }
    
    public float getMX(int identifier){
    	float result = 0;
    	switch(identifier){
    		case 0://QuaternionLeft
    			result = mxLeft;
    		break;
    		case 1://QuaternionRight
    			result = mxRight;
    		break;
    		case 2://QuaternionCenter
    			result = mxCenter;
    		break;
    	}
    	return result;
    }
    
    public float getMY(int identifier){
    	float result = 0;
    	switch(identifier){
    		case 0://QuaternionLeft
    			result = myLeft;
    		break;
    		case 1://QuaternionRight
    			result = myRight;
    		break;
    		case 2://QuaternionCenter
    			result = myCenter;
    		break;
    	}
    	return result;
    }
    
    public float getMZ(int identifier){
    	float result = 0;
    	switch(identifier){
    		case 0://QuaternionLeft
    			result = mzLeft;
    		break;
    		case 1://QuaternionRight
    			result = mzRight;
    		break;
    		case 2://QuaternionCenter
    			result = mzCenter;
    		break;
    	}
    	return result;
    }
    
    public int getButtons(){
    	return Buttons;
    }
    
    private final BluetoothGattCallback mGattCallback =
            new BluetoothGattCallback() {
    	
        @Override
        public void onConnectionStateChange(BluetoothGatt gatt, int status,
                int newState) {
            String intentAction;
            if (newState == BluetoothProfile.STATE_CONNECTED) {
                //intentAction = ACTION_GATT_CONNECTED;
                //mConnectionState = STATE_CONNECTED;
                //broadcastUpdate(intentAction);
                //Log.d("MAIN", "Connected to GATT server."); Log.d("MAIN", "Attempting to start service discovery:" +
                mBluetoothGatt.discoverServices();
                state = 2;
            } else if (newState == BluetoothProfile.STATE_DISCONNECTED) {
                //intentAction = ACTION_GATT_DISCONNECTED;
                //mConnectionState = STATE_DISCONNECTED;
                //Log.d("MAIN", "Disconnected from GATT server.");
                state = 3;
                //broadcastUpdate(intentAction);
            }
        }

        /*public UUID toUuid(long paramLong){
            return new UUID(0x1000 | paramLong << 32, -9223371485494954757L);
        }
        public String toUuid128(long paramLong){
            return toUuid(paramLong).toString();
        }*/

        private void displayGattServices(List<BluetoothGattService> gattServices) {
        	if (gattServices == null) return;
        	String uuid = null;

        	// Loops through available GATT Services.
        	for (BluetoothGattService gattService : gattServices) {
        		HashMap<String, String> currentServiceData =
                    new HashMap<String, String>();
                //Log.d("MAIN", "Service" );
        		uuid = gattService.getUuid().toString();
        		List<BluetoothGattCharacteristic> gattCharacteristics =
                    gattService.getCharacteristics();
        		// Loops through available Characteristics.
        		for (BluetoothGattCharacteristic gattCharacteristic :
                    gattCharacteristics) {

        			uuid = gattCharacteristic.getUuid().toString();
        			//Log.d("MAIN", "Characterisitic" ); //Log.d("MAIN", uuid );

        			// Check if characteristic is writable
        			if (((gattCharacteristic.getProperties() & BluetoothGattCharacteristic.PROPERTY_WRITE) |
        					(gattCharacteristic.getProperties() & BluetoothGattCharacteristic.PROPERTY_WRITE_NO_RESPONSE)) > 0) {
        				// writing characteristic functions
        				//Log.d("MAIN", "Writable" );
        				write_characteristic = gattCharacteristic;
        			}

        			// OBE-Controls Characteristic
        			// e7add780-b042-4876-aae1-112855353cc1
        			//if (uuid.startsWith("0003cbb2-0000-1000-8000-00805f9b0131")) {
        			if(uuid.startsWith(QuaternionCharacteristic_Left) )/*|| uuid.startsWith(QuaternionCharacteristic_Right)
        					|| uuid.startsWith(QuaternionCharacteristic_Center))*/
        			{
        				//Log.d("MAIN", "Activating Notifications" );
        				// ENABLE INDICATION
        				/*BluetoothGattCharacteristic characteristic =gattCharacteristic;
                    	mBluetoothGatt.setCharacteristicNotification(characteristic, true);
                    	BluetoothGattDescriptor localBluetoothGattDescriptor = characteristic.getDescriptor(UUID.fromString(toUuid128(10498L)));
                    	localBluetoothGattDescriptor.setValue(BluetoothGattDescriptor.ENABLE_INDICATION_VALUE);
                    	mBluetoothGatt.writeDescriptor(localBluetoothGattDescriptor);*/
                	
        				// ENABLE NOTIFICATION
        				BluetoothGattCharacteristic characteristic = gattCharacteristic;
        				mBluetoothGatt.setCharacteristicNotification(characteristic, true);
        				BluetoothGattDescriptor localBluetoothGattDescriptor = 
                			characteristic.getDescriptor(UUID.fromString("00002902-0000-1000-8000-00805f9b34fb"));
        				
        				if(localBluetoothGattDescriptor != null) {
        					localBluetoothGattDescriptor.setValue(BluetoothGattDescriptor.ENABLE_NOTIFICATION_VALUE);
        					mBluetoothGatt.writeDescriptor(localBluetoothGattDescriptor);
        					//Log.i("Notification status", "Enabled");
        				} else {
        					//Log.i("Notification status", "Not enabled");
        				}
        			}
        			//OBE-Haptics Characteristic
        			else if(uuid.startsWith("0003cbb1-0000-1000-8000-00805f9b0131")){
        				//write_characteristic = gattCharacteristic;
        			}
        		}
        		//Log.d("MAIN", "Characterisitic" );
        	}
        }

        @Override
        // New services discovered
        public void onServicesDiscovered(BluetoothGatt gatt, int status) {
            if (status == BluetoothGatt.GATT_SUCCESS) {
                //broadcastUpdate(ACTION_GATT_SERVICES_DISCOVERED);
                //Log.d("MAIN", "onServicesDiscovered received!" );
                displayGattServices(gatt.getServices());
            } else {
                //Log.d("MAIN", "onServicesDiscovered received: " + status);
            }
        }

        @Override
        // Result of a characteristic read operation
        public void onCharacteristicRead(BluetoothGatt gatt,
                BluetoothGattCharacteristic characteristic,
                int status) {
            if (status == BluetoothGatt.GATT_SUCCESS) {
                //broadcastUpdate(ACTION_DATA_AVAILABLE, characteristic);
                //Log.d("MAIN", "Characteristic has been read!" );
            }
            else{
                //Log.d("MAIN", "Characteristic has been read but with error!" );
            }
        }

        @Override
        public void onDescriptorRead (BluetoothGatt gatt, BluetoothGattDescriptor descriptor, int status){
            //Log.d("MAIN", "Descriptor has been read!" );
        }

        @Override
        public void onCharacteristicWrite (BluetoothGatt gatt, BluetoothGattCharacteristic characteristic, int status){
            if (status == BluetoothGatt.GATT_SUCCESS) {
                Log.d("MAIN", "Callback: Characteristic has been writen.");
            }
            else{
                Log.d("MAIN", "Callback: Error writing characteristic: "+ status);
                state = 5;
            }
        }

        @Override
        public void onCharacteristicChanged(BluetoothGatt gatt, final BluetoothGattCharacteristic characteristic) {
        	// if callback is null, do nothing
        	/*if(callback == null){
    			return;
    		}*/
        	
        	byte[] arrayOfByte = characteristic.getValue();
        	if(arrayOfByte.length == MPUDataSize){
        		//completePacket = arrayOfByte;
        		
        		int aux = arrayOfByte[MPUDataSize - 2];
        		bufferToFloat(arrayOfByte, aux);
        		if(aux == QuaternionRight){
        			Buttons = arrayOfByte[19] & 0xFF;
        		}
        		
        		// Convert bytes to floats
        		/*float w = ByteBuffer.wrap(arrayOfByte, 0, 4).order(ByteOrder.LITTLE_ENDIAN).getFloat();
        		float x = ByteBuffer.wrap(arrayOfByte, 4, 4).order(ByteOrder.LITTLE_ENDIAN).getFloat();
        		float y = ByteBuffer.wrap(arrayOfByte, 8, 4).order(ByteOrder.LITTLE_ENDIAN).getFloat();
        		float z = ByteBuffer.wrap(arrayOfByte, 12, 4).order(ByteOrder.LITTLE_ENDIAN).getFloat();
        		
        		String uuid = characteristic.getUuid().toString();
        		
        		// Notify callback that a Quaternion has been updated
        		if(uuid.equals(QuaternionCharacteristic_Left)){
        			callback.onQuaternionUpdated(w, x, y, z, Quaternion_Left);
        		}*/
        	}

            /*String str = "a";
            byte[] strBytes = str.getBytes();
            write_characteristic.setValue(strBytes);
            write_characteristic.setWriteType(BluetoothGattCharacteristic.WRITE_TYPE_NO_RESPONSE);
            gatt.writeCharacteristic(write_characteristic);*/
        }


        @Override
        public void onDescriptorWrite(BluetoothGatt gatt, BluetoothGattDescriptor descriptor, int status) {
            if (status == BluetoothGatt.GATT_SUCCESS) {
                Log.d("MAIN", "Callback: Wrote GATT Descriptor successfully.");
            }
            else{
                Log.d("MAIN", "Callback: Error writing GATT Descriptor: "+ status);
                state = 5;
            }
            //mBluetoothGatt.readCharacteristic(correct_characteristic);
        };
    };

    void doDiscovery() {
        Log.d("MAIN", "doDiscovery()");

        pairedDevices = mBtAdapter.getBondedDevices();
        // If there are paired devices, add each one to the ArrayAdapter
        if (pairedDevices.size() > 0) {
            for (BluetoothDevice device : pairedDevices) {
                //mPairedDevicesArrayAdapter.add(device.getName() + "\n" + device.getAddress());
                Log.d("MAIN", device.getName() + " -> " + device.getAddress());
                if (device.getName().startsWith("OBE"))
                {
                    Log.d("MAIN", "OBE FOUND");
                    mBluetoothGatt = device.connectGatt(this.context, false, mGattCallback);
                    return;
                }

            }
        }else {
            Log.d("MAIN", "No devices found");
        }

        state = 4;

        //Log.d("MAIN", "Scanning for devices...");

        // Turn on sub-title for new devices
        // findViewById(R.id.title_new_devices).setVisibility(View.VISIBLE);
        // If we're already discovering, stop it
        /*if (mBtAdapter.isDiscovering()) {
            mBtAdapter.cancelDiscovery();
        }

        // Request discover from BluetoothAdapter
        mBtAdapter.startDiscovery();*/
    }

    // Update OBE_Haptics characteristic
    public void writeCharacteristic(byte[] data){
    	if(write_characteristic != null){
    		write_characteristic.setValue(data);
        	write_characteristic.setWriteType(BluetoothGattCharacteristic.WRITE_TYPE_NO_RESPONSE);
        	mBluetoothGatt.writeCharacteristic(write_characteristic);
    	}
    }
    
    private void bufferToFloat(byte[] buffer, int identifier){
    	switch(identifier){
    		case 0://QuaternionLeft
    			axLeft = twoBytesToFloat(buffer[0], buffer[1]);
    			ayLeft = twoBytesToFloat(buffer[2], buffer[3]);
    			azLeft = twoBytesToFloat(buffer[4], buffer[5]);
    			gxLeft = ((float)((short)((buffer[6] << 8) | buffer[7]))) / 32768.0f;
    			gyLeft = ((float)((short)((buffer[8] << 8) | buffer[9]))) / 32768.0f;
    			gzLeft = ((float)((short)((buffer[10] << 8) | buffer[11]))) / 32768.0f;
    			mxLeft = ((float)((short)((buffer[12] << 8) | buffer[13]))) / 32768.0f;
    			myLeft = ((float)((short)((buffer[14] << 8) | buffer[15]))) / 32768.0f;
    			mzLeft = ((float)((short)((buffer[16] << 8) | buffer[17]))) / 32768.0f;
    			break;
    		case 1://QuaternionRight
    			axRight = twoBytesToFloat(buffer[0], buffer[1]);
    			ayRight = twoBytesToFloat(buffer[2], buffer[3]);
    			azRight = twoBytesToFloat(buffer[4], buffer[5]);
    			gxRight = ((float)((short)((buffer[6] << 8) | buffer[7]))) / 32768.0f;
    			gyRight = ((float)((short)((buffer[8] << 8) | buffer[9]))) / 32768.0f;
    			gzRight = ((float)((short)((buffer[10] << 8) | buffer[11]))) / 32768.0f;
    			mxRight = ((float)((short)((buffer[12] << 8) | buffer[13]))) / 32768.0f;
    			myRight = ((float)((short)((buffer[14] << 8) | buffer[15]))) / 32768.0f;
    			mzRight = ((float)((short)((buffer[16] << 8) | buffer[17]))) / 32768.0f;
    			break;
    		case 2://QuaternionRight
    			axCenter = ((float)((short)((buffer[0] << 8) | buffer[1]))) / 32768.0f;
    			ayCenter = ((float)((short)((buffer[2] << 8) | buffer[3]))) / 32768.0f;
    			azCenter = ((float)((short)((buffer[4] << 8) | buffer[5]))) / 32768.0f;
    			gxCenter = ((float)((short)((buffer[6] << 8) | buffer[7]))) / 32768.0f;
    			gyCenter = ((float)((short)((buffer[8] << 8) | buffer[9]))) / 32768.0f;
    			gzCenter = ((float)((short)((buffer[10] << 8) | buffer[11]))) / 32768.0f;
    			mxCenter = ((float)((short)((buffer[12] << 8) | buffer[13]))) / 32768.0f;
    			myCenter = ((float)((short)((buffer[14] << 8) | buffer[15]))) / 32768.0f;
    			mzCenter = ((float)((short)((buffer[16] << 8) | buffer[17]))) / 32768.0f;
    			break;
    	}
    }
    
    private float twoBytesToFloat(byte var1, byte var2){
    	float auxF = 0.0f;
    	short auxS = (short)(((var1 << 8) | var2) & 0xFFFF);
    	auxF = auxS;
    	auxF /= 32768.0f;
    	
    	return auxF;
    }
}
