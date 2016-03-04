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
    
    CBCharacteristic *obeQuaternionCh_Left, *obeQuaternionCh_Right;
    
    NSMutableArray *peripherals;
    
    //NSTimer *testTimer;
    float lastFloat;
}

@property bool isScanning, isConnected, isEnabled;
@property bool OBEFound;
@property float wLeft, xLeft, yLeft, zLeft;
@property float wRight, xRight, yRight, zRight;
@property float wCenter, xCenter, yCenter, zCenter;

- (void) initVariables;
- (void) startScan;
- (void) stopScan;
- (void) connectToOBE:(int)index;
- (void) disconnectFromOBE;

@end
