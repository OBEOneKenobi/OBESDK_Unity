//
//  OBEHandler.h
//  OBEPlugin_OSX
//
//  Created by Henry Serrano on 1/26/16.
//  Copyright © 2016 Machina Wearable Technology SAPI de CV. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <IOBluetooth/IOBluetooth.h>

@interface OBEHandler : NSObject<CBCentralManagerDelegate, CBPeripheralDelegate>{
    CBCentralManager *manager;
    CBPeripheral *obePeripheral;
    
    CBCharacteristic *obeQuaternionCh_Left, *obeQuaternionCh_Right,
        *obeQuaternionCh_Center, *obeHapticCh;
    
    NSMutableArray *peripherals;
    
    //NSTimer *testTimer;
    float lastFloat;
    int counter; bool turnOn; bool shouldUpdateMotor; bool hasFinishedUpdate;
}

@property bool isScanning, isConnected, isEnabled;
@property bool OBEFound;
@property int isReadyToScan;
@property float wLeft, xLeft, yLeft, zLeft;
@property float wRight, xRight, yRight, zRight;
@property float wCenter, xCenter, yCenter, zCenter;

@property float axLeft, ayLeft, azLeft;
@property float gxLeft, gyLeft, gzLeft;
@property float mxLeft, myLeft, mzLeft;

@property float axRight, ayRight, azRight;
@property float gxRight, gyRight, gzRight;
@property float mxRight, myRight, mzRight;

@property float axCenter, ayCenter, azCenter;
@property float gxCenter, gyCenter, gzCenter;
@property float mxCenter, myCenter, mzCenter;

@property float Motor1, Motor2, Motor3, Motor4;

@property int Buttons;

- (void) initVariables;
- (void) startScan;
- (void) stopScan;
- (void) connectToOBE:(int)index;
- (void) disconnectFromOBE;
- (void) updateMotorState;

@end
