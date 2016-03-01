//
//  Plugin.cpp
//  OBEPlugin_OSX
//
//  Created by Henry Serrano on 1/26/16.
//  Copyright © 2016 Machina Wearable Technology SAPI de CV. All rights reserved.
//

#include <stdio.h>
#include "Plugin.pch"
#include "OBEHandler.h"

QuaternionCallback qCallback_Left = NULL;
QuaternionCallback qCallback_Right = NULL;
FoundOBECallback foCallback = NULL;
DidConnectOBECallback dcoCallback = NULL;
DidFindOBEService dfosCallback = NULL;
DidFindOBECharacteristic dfocCallback = NULL;

OBEHandler *handler = nil;

void FoundOBE(FoundOBECallback callbackMethod){
    foCallback = callbackMethod;
}

void NotifyFoundOBE(const char *obeName, int index){
    if(foCallback != NULL){
        foCallback(obeName, index);
    }
}

void OBEConnected(DidConnectOBECallback callbackMethod){
    dcoCallback = callbackMethod;
}

void NotifyOBEConnected(const char *obeName){
    if(dcoCallback != NULL){
        dcoCallback(obeName);
    }
}

void FoundOBEService(DidFindOBEService callbackMethod){
    dfosCallback = callbackMethod;
}

void NotifyFoundOBEService(const char *obeService){
    if(dfosCallback != NULL){
        dfosCallback(obeService);
    }
}

void FoundOBECharacteristic(DidFindOBECharacteristic callbackMethod){
    dfocCallback = callbackMethod;
}

void NotifyFoundOBECharacteristic(const char *obeCharacteristic){
    if(dfocCallback != NULL){
        dfocCallback(obeCharacteristic);
    }
}

// LEFT

void QuaternionUpdated_Left(QuaternionCallback callbackMethod){
    qCallback_Left = callbackMethod;
}

void SendQuaternion_Left(float w, float x, float y, float z){
    if(qCallback_Left != NULL){
        qCallback_Left(w, x, y, z);
    }
}

// RIGHT

void QuaternionUpdated_Right(QuaternionCallback callbackMethod){
    qCallback_Right = callbackMethod;
}

void SendQuaternion_Right(float w, float x, float y, float z){
    if(qCallback_Right != NULL){
        qCallback_Right(w, x, y, z);
    }
}

//

void Init(){
    handler = [[OBEHandler alloc] init];
    [handler initVariables];
}

void StartScanning(){
    if(handler != nil){
        [handler startScan];
    }
}

void StopScanning(){
    if(handler != nil){
        [handler stopScan];
    }
}

void ConnectToOBE(int index){
    if(handler != nil){
        [handler connectToOBE:index];
    }
}

void DisconnectFromOBE(){
    if(handler != nil){
        [handler disconnectFromOBE];
    }
}



