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

int isReadyToScan(){
    if(handler == nil){
        return 0;
    }else{
        return handler.isReadyToScan;
    }
}

void UpdateMotors(){
    if(handler != nil){
        [handler updateMotorState];
    }
}

int getButtons(){
    if(handler != nil){
        return handler.Buttons;
    }else{
        return 0;
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

float getAX(int identifier){
    float auxFloat = 0.0f;
    if(handler == nil){
        return auxFloat;
    }
    switch(identifier){
        case OBEQuaternionLeft:
            auxFloat = handler.axLeft;
            break;
        case OBEQuaternionRight:
            auxFloat = handler.axRight;
            break;
        case OBEQuaternionCenter:
            auxFloat = handler.axCenter;
            break;
    }
    return auxFloat;
}

float getAY(int identifier){
    float auxFloat = 0.0f;
    if(handler == nil){
        return auxFloat;
    }
    switch(identifier){
        case OBEQuaternionLeft:
            auxFloat = handler.ayLeft;
            break;
        case OBEQuaternionRight:
            auxFloat = handler.ayRight;
            break;
        case OBEQuaternionCenter:
            auxFloat = handler.ayCenter;
            break;
    }
    return auxFloat;
}

float getAZ(int identifier){
    float auxFloat = 0.0f;
    if(handler == nil){
        return auxFloat;
    }
    switch(identifier){
        case OBEQuaternionLeft:
            auxFloat = handler.azLeft;
            break;
        case OBEQuaternionRight:
            auxFloat = handler.azRight;
            break;
        case OBEQuaternionCenter:
            auxFloat = handler.azCenter;
            break;
    }
    return auxFloat;
}

float getGX(int identifier){
    float auxFloat = 0.0f;
    if(handler == nil){
        return auxFloat;
    }
    switch(identifier){
        case OBEQuaternionLeft:
            auxFloat = handler.gxLeft;
            break;
        case OBEQuaternionRight:
            auxFloat = handler.gxRight;
            break;
        case OBEQuaternionCenter:
            auxFloat = handler.gxCenter;
            break;
    }
    return auxFloat;
}

float getGY(int identifier){
    float auxFloat = 0.0f;
    if(handler == nil){
        return auxFloat;
    }
    switch(identifier){
        case OBEQuaternionLeft:
            auxFloat = handler.gyLeft;
            break;
        case OBEQuaternionRight:
            auxFloat = handler.gyRight;
            break;
        case OBEQuaternionCenter:
            auxFloat = handler.gyCenter;
            break;
    }
    return auxFloat;
}

float getGZ(int identifier){
    float auxFloat = 0.0f;
    if(handler == nil){
        return auxFloat;
    }
    switch(identifier){
        case OBEQuaternionLeft:
            auxFloat = handler.gzLeft;
            break;
        case OBEQuaternionRight:
            auxFloat = handler.gzRight;
            break;
        case OBEQuaternionCenter:
            auxFloat = handler.gzCenter;
            break;
    }
    return auxFloat;
}

float getMX(int identifier){
    float auxFloat = 0.0f;
    if(handler == nil){
        return auxFloat;
    }
    switch(identifier){
        case OBEQuaternionLeft:
            auxFloat = handler.mxLeft;
            break;
        case OBEQuaternionRight:
            auxFloat = handler.mxRight;
            break;
        case OBEQuaternionCenter:
            auxFloat = handler.mxCenter;
            break;
    }
    return auxFloat;
}

float getMY(int identifier){
    float auxFloat = 0.0f;
    if(handler == nil){
        return auxFloat;
    }
    switch(identifier){
        case OBEQuaternionLeft:
            auxFloat = handler.myLeft;
            break;
        case OBEQuaternionRight:
            auxFloat = handler.myRight;
            break;
        case OBEQuaternionCenter:
            auxFloat = handler.myCenter;
            break;
    }
    return auxFloat;
}

float getMZ(int identifier){
    float auxFloat = 0.0f;
    if(handler == nil){
        return auxFloat;
    }
    switch(identifier){
        case OBEQuaternionLeft:
            auxFloat = handler.mzLeft;
            break;
        case OBEQuaternionRight:
            auxFloat = handler.mzRight;
            break;
        case OBEQuaternionCenter:
            auxFloat = handler.mzCenter;
            break;
    }
    return auxFloat;
}

void setMotor1(float speed){
    if(handler != nil){
        //[handler setMotor1:speed];
        handler.Motor1 = setFloatBounds(speed);
    }
}

void setMotor2(float speed){
    if(handler != nil){
        [handler setMotor2:setFloatBounds(speed)];
    }
}

void setMotor3(float speed){
    if(handler != nil){
        //[handler setMotor3:speed];
        handler.Motor3 = setFloatBounds(speed);
    }
}

void setMotor4(float speed){
    if(handler != nil){
        [handler setMotor4:setFloatBounds(speed)];
    }
}

float setFloatBounds(float var){
    if(var > 1.0f){
        return 1.0f;
    }else if(var < 0.0f){
        return 0.0f;
    }
    return var;
}
