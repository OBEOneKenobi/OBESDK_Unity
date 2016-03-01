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
    
    NSMutableArray *peripherals;
    
    BOOL isConnected, isInitiated;
    float W,X,Y,Z;
}

- (void) initialise;
- (void) startScanning;
- (void) connectToOBE:(int)index;
- (void) disconnectFromOBE;

@end
