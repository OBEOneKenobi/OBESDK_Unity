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

QuaternionCallback qCallback = NULL;
//QuaternionCallback qCallback_Right = NULL;
FoundOBECallback foCallback = NULL;
DidConnectOBECallback dcoCallback = NULL;
DidFindOBEService dfosCallback = NULL;
DidFindOBECharacteristic dfocCallback = NULL;

OBEHandler *handler = nil;

const int OBEQuaternionLeft = 0;
const int OBEQuaternionRight = 1;
const int OBEQuaternionCenter = 2;

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

void QuaternionUpdated(QuaternionCallback callbackMethod){
    qCallback = callbackMethod;
}

void SendQuaternion(float w, float x, float y, float z, int identifier){
    if(qCallback != NULL){
        qCallback(w, x, y, z, identifier);
    }
}

// RIGHT
/*
void QuaternionUpdated_Right(QuaternionCallback callbackMethod){
    qCallback_Right = callbackMethod;
}

void SendQuaternion_Right(float w, float x, float y, float z){
    if(qCallback_Right != NULL){
        qCallback_Right(w, x, y, z);
    }
}*/

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

int isConnected(){
    if(handler == nil){
        return 0;
    }else{
        return handler.isConnected;
    }
}

float getQW(int identifier){
    float auxFloat = 0.0f;
    if(handler == nil){
        return auxFloat;
    }
    switch(identifier){
        case OBEQuaternionLeft:
            auxFloat = handler.wLeft;
            break;
        case OBEQuaternionRight:
            auxFloat = handler.wRight;
            break;
        case OBEQuaternionCenter:
            auxFloat = handler.wCenter;
            break;
    }
    return auxFloat;
}

float getQX(int identifier){
    float auxFloat = 0.0f;
    if(handler == nil){
        return auxFloat;
    }
    switch(identifier){
        case OBEQuaternionLeft:
            auxFloat = handler.xLeft;
            break;
        case OBEQuaternionRight:
            auxFloat = handler.xRight;
            break;
        case OBEQuaternionCenter:
            auxFloat = handler.xCenter;
            break;
    }
    return auxFloat;
}

float getQY(int identifier){
    float auxFloat = 0.0f;
    if(handler == nil){
        return auxFloat;
    }
    switch(identifier){
        case OBEQuaternionLeft:
            auxFloat = handler.yLeft;
            break;
        case OBEQuaternionRight:
            auxFloat = handler.yRight;
            break;
        case OBEQuaternionCenter:
            auxFloat = handler.yCenter;
            break;
    }
    return auxFloat;
}

float getQZ(int identifier){
    float auxFloat = 0.0f;
    if(handler == nil){
        return auxFloat;
    }
    switch(identifier){
        case OBEQuaternionLeft:
            auxFloat = handler.zLeft;
            break;
        case OBEQuaternionRight:
            auxFloat = handler.zRight;
            break;
        case OBEQuaternionCenter:
            auxFloat = handler.zCenter;
            break;
    }
    return auxFloat;
}


