//  
//  OBESDK_Unity
//
//  Created by Henry Serrano on 2/29/16.
//  Copyright © 2016 Machina Wearable Technology SAPI de CV. All rights reserved.
//

#import <UIKit/UIKit.h>
#import <CoreBluetooth/CoreBluetooth.h>

@interface OBE : NSObject<CBCentralManagerDelegate, CBPeripheralDelegate>{
    CBCentralManager *manager;
    CBPeripheral *obePeripheral;
    CBCharacteristic *hapticCH;
    
    NSMutableArray *peripherals;
    
    BOOL isConnected;
    float W,X,Y,Z;
}

@property bool isScanning, isConnected, isEnabled;
@property int isReadyToScan;
@property bool OBEFound;

@property float Motor1, Motor2, Motor3, Motor4;

@property float axLeft, ayLeft, azLeft;
@property float gxLeft, gyLeft, gzLeft;
@property float mxLeft, myLeft, mzLeft;

@property float axRight, ayRight, azRight;
@property float gxRight, gyRight, gzRight;
@property float mxRight, myRight, mzRight;

@property float axCenter, ayCenter, azCenter;
@property float gxCenter, gyCenter, gzCenter;
@property float mxCenter, myCenter, mzCenter;

@property int Buttons;

- (void) initVariables;
- (void) startScan;
- (void) stopScan;
- (void) connectToOBE:(int)index;
- (void) disconnectFromOBE;
- (void) updateMotorState;

@end
