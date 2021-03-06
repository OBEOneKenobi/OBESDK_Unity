//
//  Plugin.pch
//  OBEPlugin_OSX
//
//  Created by Henry Serrano on 1/26/16.
//  Copyright © 2016 Machina Wearable Technology SAPI de CV. All rights reserved.
//

/*
#ifndef Plugin_pch
#define Plugin_pch

// Include any system framework and library headers here that should be included in all compilation units.
// You will also need to set the Prefix Header build setting of one or more of your targets to reference this file.

#endif*/ /* Plugin_h */

extern "C"{
    
    typedef void (* QuaternionCallback) (float w, float x, float y, float z, int identifier);
    typedef void (* FoundOBECallback) (const char *obeName, int index);
    typedef void (* DidConnectOBECallback) (const char *obeName);
    typedef void (* DidFindOBEService) (const char *obeService);
    typedef void (* DidFindOBECharacteristic) (const char *obeCharacteristic);
    
    void FoundOBE(FoundOBECallback callbackMethod);
    void NotifyFoundOBE(const char *obeName, int index);
    
    void OBEConnected(DidConnectOBECallback callbackMethod);
    void NotifyOBEConnected(const char *obeName);
    
    void FoundOBEService(DidFindOBEService callbackMethod);
    void NotifyFoundOBEService(const char *obeService);
    
    void FoundOBECharacteristic(DidFindOBECharacteristic callbackMethod);
    void NotifyFoundOBECharacteristic(const char *obeCharacteristic);
    
    // LEFT
    void QuaternionUpdated(QuaternionCallback callbackMethod);
    //void SendQuaternion_Left(float w, float x, float y, float z);
    
    // RIGHT
    //void QuaternionUpdated_Right(QuaternionCallback callbackMethod);
    //void SendQuaternion_Right(float w, float x, float y, float z);
    
    
    void Init();
    void StartScanning();
    void StopScanning();
    void ConnectToOBE(int index);
    void DisconnectFromOBE();
    int isConnected();
    int isReadyToScan();
    void UpdateMotors();
    
    int getButtons();
    
    float getQW(int identifier); float getQX(int identifier);
    float getQY(int identifier); float getQZ(int identifier);
    
    float getAX(int identifier); float getAY(int identifier); float getAZ(int identifier);
    float getGX(int identifier); float getGY(int identifier); float getGZ(int identifier);
    float getMX(int identifier); float getMY(int identifier); float getMZ(int identifier);
    
    void setMotor1(float speed);
    void setMotor2(float speed);
    void setMotor3(float speed);
    void setMotor4(float speed);
    
    float setFloatBounds(float var);
}
