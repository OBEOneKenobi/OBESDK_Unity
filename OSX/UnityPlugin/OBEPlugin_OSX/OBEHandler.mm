//
//  OBEHandler.m
//  OBEPlugin_OSX
//
//  Created by Henry Serrano on 1/26/16.
//  Copyright © 2016 Machina Wearable Technology SAPI de CV. All rights reserved.
//

#import "OBEHandler.h"
#import "Plugin.pch"

#define OBEService @"0003cbbb-0000-1000-8000-00805F9B0131"
#define OBEQuaternionCharacteristic_Left @"0003cbb2-0000-1000-8000-00805F9B0131"
#define OBEQuaternionCharacteristic_Right @"0003cbb3-0000-1000-8000-00805F9B0131"
#define OBEQuaternionCharacteristic_Center @"0003cbb4-0000-1000-8000-00805F9B0131"
#define OBEHapticCharacteristic @"0003cbb1-0000-1000-8000-00805F9B0131"

#define OBEQuaternionLeft 0
#define OBEQuaternionRight 1
#define OBEQuaternionCenter 2

#define OBEMPUDataSize 20
#define OBEHapticDataSize 7

void SendQuaternion(float w, float x, float y, float z, int identifier);
//void SendQuaternion_Right(float w, float x, float y, float z);
void NotifyFoundOBE(const char *obeName, int index);
void NotifyOBEConnected(const char *obeName);
void NotifyFoundOBEService(const char *obeService);
void NotifyFoundOBECharacteristic(const char *obeCharacteristic);

union {
    float float_variable;
    Byte temp_array[4];
} wQuaternion;

union {
    float float_variable;
    Byte temp_array[4];
} xQuaternion;

union {
    float float_variable;
    Byte temp_array[4];
} yQuaternion;

union {
    float float_variable;
    Byte temp_array[4];
} zQuaternion;

@implementation OBEHandler

- (void) initVariables{
    peripherals = [[NSMutableArray alloc] init];
    manager = [[CBCentralManager alloc] initWithDelegate:self queue:nil];
}

#pragma mark - Start/Stop Scan methods

/*
 Uses CBCentralManager to check whether the current platform/hardware supports Bluetooth LE. An alert is raised if Bluetooth LE is not enabled or is not supported.
 */
- (BOOL) isLECapableHardware{
    NSString * state = nil;
    
    switch ([manager state]){
        case CBCentralManagerStateUnsupported:
            state = @"Bluetooth 4.0 unsupported!";
            _isReadyToScan = 1;
            break;
        case CBCentralManagerStateUnauthorized:
            state = @"Application doesn't have permission to use Bluetooth...";
            _isReadyToScan = 2;
            break;
        case CBCentralManagerStatePoweredOff:
            state = @"Bluetooth turned off!";
            _isReadyToScan = 3;
            break;
        case CBCentralManagerStatePoweredOn:
            _isReadyToScan = 4;
            return TRUE;
        case CBCentralManagerStateUnknown:
            _isReadyToScan = 5;
        default:
            return FALSE;
            
    }
    
    return FALSE;
}

/*
 Request CBCentralManager to scan for heart rate peripherals using service UUID 0x180D
 */
- (void) startScan{
    //[manager scanForPeripheralsWithServices:[NSArray arrayWithObject:[CBUUID UUIDWithString:@"180D"]] options:nil];
    [manager scanForPeripheralsWithServices:@[[CBUUID UUIDWithString:OBEService]] options:nil];
    _isScanning = true;
    
    // make sure we stop scanning after 5 seconds if no device was found
    [NSTimer scheduledTimerWithTimeInterval:5.0 target:self selector:@selector(stopScanning:) userInfo:nil repeats:NO];
    
    //testTimer = [NSTimer scheduledTimerWithTimeInterval:2.0f target:self selector:@selector(stopScanning:) userInfo:nil repeats:YES];
}

/*
 Request CBCentralManager to stop scanning for heart rate peripherals
 */
- (void) stopScan{
    [manager stopScan];
    //[testTimer invalidate];
    //testTimer = nil;
    _isScanning = NO;
}

- (void) stopScanning:(id)sender{
    NSTimer *timer = (NSTimer *)sender;
    [timer invalidate];
    
    [self stopScan];
    //lastFloat += 0.5f;
    //SendQuaternion(lastFloat, lastFloat * 10.0f, lastFloat / 2.0f, lastFloat + 5.0f);
}

- (void) connectToOBE:(int)index{
    if(_isConnected){
        return;
    }
    if([peripherals count] > index){
        
        [self stopScan];
        
        obePeripheral = [peripherals objectAtIndex:index];
        [manager connectPeripheral:obePeripheral options:nil];
    }
}

- (void) disconnectFromOBE{
    if(obePeripheral != nil){
        [manager cancelPeripheralConnection:obePeripheral];
        [obePeripheral setDelegate:nil];
        obePeripheral = nil;
    }
}

- (void) updateMotorState{
    if((obePeripheral != nil) && (obeHapticCh != nil)){
        shouldUpdateMotor = YES;
    }
}

#pragma mark - CBCentralManager delegate methods

/*
 Invoked whenever the central manager's state is updated.
 */
- (void) centralManagerDidUpdateState:(CBCentralManager *)central{
    //[self isLECapableHardware];
    if([central state] == CBCentralManagerStatePoweredOn){
        //_isReadyToScan = YES;
        [self isLECapableHardware];
    }
}

/*
 Invoked when the central discovers heart rate peripheral while scanning.
 */
- (void) centralManager:(CBCentralManager *)central didDiscoverPeripheral:(CBPeripheral *)aPeripheral advertisementData:(NSDictionary *)advertisementData RSSI:(NSNumber *)RSSI{
    // NSMutableArray *peripherals = [self mutableArrayValueForKey:@"vimiMonitors"];
    //if( ![vimiMonitors containsObject:aPeripheral] ){
    
    [peripherals addObject:aPeripheral];
    //const char *name = [[aPeripheral name] cStringUsingEncoding:[NSString defaultCStringEncoding]];
    //NotifyFoundOBE(name, (int)([peripherals count] - 1));
    
    //stop at the sight of the first device
    //[self stopScan];
    //}
    
    /* Retreive already known devices */
    /*if(autoConnect){
     [manager retrievePeripherals:[NSArray arrayWithObject:(id)aPeripheral.UUID]];
    }*/
}

/*
 Invoked when the central manager retrieves the list of known peripherals.
 Automatically connect to first known peripheral
 */
- (void)centralManager:(CBCentralManager *)central didRetrievePeripherals:(NSArray *)peripherals{
    //NSLog(@"Retrieved peripheral: %lu - %@", [peripherals count], peripherals);
    
    //[self stopScan];
    
    /* If there are any known devices, automatically connect to it.*/
    /*if([peripherals count] >=1){
     peripheral = [peripherals objectAtIndex:0];
     
     [manager connectPeripheral:peripheral options:[NSDictionary dictionaryWithObject:[NSNumber numberWithBool:YES] forKey:CBConnectPeripheralOptionNotifyOnDisconnectionKey]];
     }*/
}

/*
 Invoked whenever a connection is succesfully created with the peripheral.
 Discover available services on the peripheral
 */
- (void) centralManager:(CBCentralManager *)central didConnectPeripheral:(CBPeripheral *)aPeripheral{
    [aPeripheral setDelegate:self];
    [aPeripheral discoverServices:nil];
    
    //const char *name = "Connected";
    //NotifyOBEConnected(name);
    
    //self.connected = @"Connected";
    _isConnected = true;
    hasFinishedUpdate = YES;
}

/*
 Invoked whenever an existing connection with the peripheral is torn down.
 Reset local variables
 */
- (void)centralManager:(CBCentralManager *)central didDisconnectPeripheral:(CBPeripheral *)aPeripheral error:(NSError *)error{
    //NSLog(@"Disconnected");
    _isConnected = false;
}

/*
 Invoked whenever the central manager fails to create a connection with the peripheral.
 */
- (void)centralManager:(CBCentralManager *)central didFailToConnectPeripheral:(CBPeripheral *)aPeripheral error:(NSError *)error{
    //NSLog(@"Fail to connect to peripheral: %@ with error = %@", aPeripheral, [error localizedDescription]);
    
    if( obePeripheral ){
        [obePeripheral setDelegate:nil];
        obePeripheral = nil;
    }
}

#pragma mark - CBPeripheral delegate methods
/*
 Invoked upon completion of a -[discoverServices:] request.
 Discover available characteristics on interested services
 */
- (void) peripheral:(CBPeripheral *)aPeripheral didDiscoverServices:(NSError *)error{
    for (CBService *aService in aPeripheral.services){
        //NSLog(@"Service found with UUID: %@", aService.UUID);
        
        /* Device Information Service */
        if ([aService.UUID isEqual:[CBUUID UUIDWithString:@"180A"]]){
            [aPeripheral discoverCharacteristics:nil forService:aService];
        }
        
        /* GAP (Generic Access Profile) for Device Name */
        if ( [aService.UUID isEqual:[CBUUID UUIDWithString:CBUUIDGenericAccessProfileString]] ){
            [aPeripheral discoverCharacteristics:nil forService:aService];
        }
        
        /* BG Cable Replacement */
        if([aService.UUID isEqual:[CBUUID UUIDWithString:OBEService]]){
            [aPeripheral discoverCharacteristics:nil forService:aService];
        }
        
        //NSString *nameString = [aService.UUID UUIDString];
        //const char *name = [nameString cStringUsingEncoding:[NSString defaultCStringEncoding]];
        //NotifyFoundOBEService(name);
    }
}

/*
 Invoked upon completion of a -[discoverCharacteristics:forService:] request.
 Perform appropriate operations on interested characteristics
 */
- (void) peripheral:(CBPeripheral *)aPeripheral didDiscoverCharacteristicsForService:(CBService *)service error:(NSError *)error{
    
    if ( [service.UUID isEqual:[CBUUID UUIDWithString:CBUUIDGenericAccessProfileString]] ){
        for (CBCharacteristic *aChar in service.characteristics){
            /* Read device name */
            if ([aChar.UUID isEqual:[CBUUID UUIDWithString:CBUUIDDeviceNameString]]){
                //[aPeripheral readValueForCharacteristic:aChar];
                //NSLog(@"Found a Device Name Characteristic");
                
            }
            
            //NSString *nameString = [aChar.UUID UUIDString];
            //const char *name = [nameString cStringUsingEncoding:[NSString defaultCStringEncoding]];
            //NotifyFoundOBECharacteristic(name);
        }
    }
    if ([service.UUID isEqual:[CBUUID UUIDWithString:@"180A"]]){
        for (CBCharacteristic *aChar in service.characteristics){
            /* Read manufacturer name */
            if ([aChar.UUID isEqual:[CBUUID UUIDWithString:@"2A29"]]){
                //[aPeripheral readValueForCharacteristic:aChar];
                //NSLog(@"Found a Device Manufacturer Name Characteristic");
                
            }
            
            //NSString *nameString = [aChar.UUID UUIDString];
            //const char *name = [nameString cStringUsingEncoding:[NSString defaultCStringEncoding]];
            //NotifyFoundOBECharacteristic(name);
        }
    }
    
    
    if ([service.UUID isEqual:[CBUUID UUIDWithString:OBEService]]){
        for (CBCharacteristic *aChar in service.characteristics){
            /* Read DATA Characteristic */
            if ([aChar.UUID isEqual:[CBUUID UUIDWithString:OBEQuaternionCharacteristic_Left]]){
                //[aPeripheral readValueForCharacteristic:aChar];
                [obePeripheral setNotifyValue:YES forCharacteristic:aChar];
                //NSLog(@"Found BG Cable Replacement DATA Characteristic");
                
                obeQuaternionCh_Left = aChar;
            }/*else if ([aChar.UUID isEqual:[CBUUID UUIDWithString:OBEQuaternionCharacteristic_Right]]){
                //[aPeripheral readValueForCharacteristic:aChar];
                [obePeripheral setNotifyValue:YES forCharacteristic:aChar];
                //NSLog(@"Found BG Cable Replacement DATA Characteristic");
                
                obeQuaternionCh_Right = aChar;
            }else if ([aChar.UUID isEqual:[CBUUID UUIDWithString:OBEQuaternionCharacteristic_Center]]){
                //[aPeripheral readValueForCharacteristic:aChar];
                [obePeripheral setNotifyValue:YES forCharacteristic:aChar];
                //NSLog(@"Found BG Cable Replacement DATA Characteristic");
                
                obeQuaternionCh_Center = aChar;
            }*/else if ([aChar.UUID isEqual:[CBUUID UUIDWithString:OBEHapticCharacteristic]]){
                //[aPeripheral readValueForCharacteristic:aChar];
                //NSLog(@"Found BG Cable Replacement DATA Characteristic");
                
                obeHapticCh = aChar;
            }
            
            //NSString *nameString = [aChar.UUID UUIDString];
            //const char *name = [nameString cStringUsingEncoding:[NSString defaultCStringEncoding]];
            //NotifyFoundOBECharacteristic(name);
        }
    }
    
    
    
}

/*
 Invoked upon completion of a -[readValueForCharacteristic:] request or on the reception of a notification/indication.
 */
- (void) peripheral:(CBPeripheral *)aPeripheral didUpdateValueForCharacteristic:(CBCharacteristic *)characteristic error:(NSError *)error{
    //dispatch_async(dispatch_get_global_queue(2,0), ^{//máxima prioridad
        /* Value for device Name received */
        if ([characteristic.UUID isEqual:[CBUUID UUIDWithString:CBUUIDDeviceNameString]]){
            //NSString * deviceName = [[NSString alloc] initWithData:characteristic.value encoding:NSUTF8StringEncoding];
            //NSLog(@"Device Name = %@", deviceName);
        }
        /* Value for manufacturer name received */
        else if ([characteristic.UUID isEqual:[CBUUID UUIDWithString:@"2A29"]]){
            //self.manufacturer = [[NSString alloc] initWithData:characteristic.value encoding:NSUTF8StringEncoding];
            //NSLog(@"Manufacturer Name = %@", self.manufacturer);
        }else if ([characteristic.UUID isEqual:[CBUUID UUIDWithString:OBEQuaternionCharacteristic_Left]]){
            
            NSData *quaternion = [characteristic value];
            if([quaternion length] == OBEMPUDataSize){
                
                Byte *buffer = (Byte *)malloc(sizeof(Byte) * OBEMPUDataSize);
                [quaternion getBytes:buffer length:OBEMPUDataSize];
                
                //assignBufferToStruct(buffer);
                [self assignBuffer:buffer withIdentifier:buffer[18]];
                
                free(buffer);
                
                if(buffer[18] == OBEQuaternionCenter){
                    _Buttons = buffer[19] & 0xFF;
                    if(shouldUpdateMotor){
                        if(hasFinishedUpdate){
                            hasFinishedUpdate = NO;
                            shouldUpdateMotor = NO;
                            [self motorUpdate];
                        }
                    }
                }
                
                //[self assignFloatQuaternion:OBEQuaternionLeft];
                //SendQuaternion(wQuaternion.float_variable, xQuaternion.float_variable, yQuaternion.float_variable, zQuaternion.float_variable, OBEQuaternionLeft);
            }
            
            //lastFloat += 0.5f;
            //SendQuaternion(lastFloat, lastFloat * 10.0f, lastFloat / 2.0f, lastFloat + 5.0f);
        }/*else if ([characteristic.UUID isEqual:[CBUUID UUIDWithString:OBEQuaternionCharacteristic_Right]]){
            
            NSData *quaternion = [characteristic value];
            if([quaternion length] == OBEMPUDataSize){
                
                Byte *buffer = (Byte *)malloc(sizeof(Byte) * OBEMPUDataSize);
                [quaternion getBytes:buffer length:OBEMPUDataSize];
                
                //assignBufferToStruct(buffer);
                [self assignBuffer:buffer withIdentifier:OBEQuaternionRight];
                
                free(buffer);
                
                //[self assignFloatQuaternion:OBEQuaternionRight];
                //SendQuaternion(wQuaternion.float_variable, xQuaternion.float_variable, yQuaternion.float_variable, zQuaternion.float_variable, OBEQuaternionRight);
            }
            
        }else if ([characteristic.UUID isEqual:[CBUUID UUIDWithString:OBEQuaternionCharacteristic_Center]]){
            
            NSData *quaternion = [characteristic value];
            if([quaternion length] == OBEMPUDataSize){
                
                Byte *buffer = (Byte *)malloc(sizeof(Byte) * OBEMPUDataSize);
                [quaternion getBytes:buffer length:OBEMPUDataSize];
                
                //assignBufferToStruct(buffer);
                [self assignBuffer:buffer withIdentifier:OBEQuaternionCenter];
                
                free(buffer);
                
                //[self assignFloatQuaternion:OBEQuaternionCenter];
                //dispatch_async(dispatch_get_main_queue(), ^{
                //    SendQuaternion(wQuaternion.float_variable, xQuaternion.float_variable, yQuaternion.float_variable, zQuaternion.float_variable, OBEQuaternionCenter);
                //});
                //SendQuaternion(wQuaternion.float_variable, xQuaternion.float_variable, yQuaternion.float_variable, zQuaternion.float_variable, OBEQuaternionCenter);
            }
            
        }*/
    //});
}

- (void) assignFloatQuaternion:(int) identifier{
    switch (identifier) {
        case OBEQuaternionLeft:
            _wLeft = wQuaternion.float_variable;
            _xLeft = xQuaternion.float_variable;
            _yLeft = yQuaternion.float_variable;
            _zLeft = zQuaternion.float_variable;
            break;
        case OBEQuaternionRight:
            _wRight = wQuaternion.float_variable;
            _xRight = xQuaternion.float_variable;
            _yRight = yQuaternion.float_variable;
            _zRight = zQuaternion.float_variable;
            break;
        case OBEQuaternionCenter:
            _wCenter = wQuaternion.float_variable;
            _xCenter = xQuaternion.float_variable;
            _yCenter = yQuaternion.float_variable;
            _zCenter = zQuaternion.float_variable;
            break;
    }
}

void assignBufferToStruct(Byte *buffer){
    wQuaternion.temp_array[0] = buffer[0];
    wQuaternion.temp_array[1] = buffer[1];
    wQuaternion.temp_array[2] = buffer[2];
    wQuaternion.temp_array[3] = buffer[3];
    
    xQuaternion.temp_array[0] = buffer[4];
    xQuaternion.temp_array[1] = buffer[5];
    xQuaternion.temp_array[2] = buffer[6];
    xQuaternion.temp_array[3] = buffer[7];
    
    yQuaternion.temp_array[0] = buffer[8];
    yQuaternion.temp_array[1] = buffer[9];
    yQuaternion.temp_array[2] = buffer[10];
    yQuaternion.temp_array[3] = buffer[11];

    zQuaternion.temp_array[0] = buffer[12];
    zQuaternion.temp_array[1] = buffer[13];
    zQuaternion.temp_array[2] = buffer[14];
    zQuaternion.temp_array[3] = buffer[15];
}

- (void) assignBuffer:(Byte *)buffer withIdentifier:(int) identifier{
    switch(identifier){
        case OBEQuaternionLeft:
            _axLeft = (float)((int16_t)((buffer[0] << 8) | buffer[1])); _axLeft /= 32768.0f;
            _ayLeft = (float)((int16_t)((buffer[2] << 8) | buffer[3])); _ayLeft /= 32768.0f;
            _azLeft = (float)((int16_t)((buffer[4] << 8) | buffer[5])); _azLeft /= 32768.0f;
            _gxLeft = (float)((int16_t)((buffer[6] << 8) | buffer[7])); _gxLeft /= 32768.0f;
            _gyLeft = (float)((int16_t)((buffer[8] << 8) | buffer[9])); _gyLeft /= 32768.0f;
            _gzLeft = (float)((int16_t)((buffer[10] << 8) | buffer[11])); _gzLeft /= 32768.0f;
            _mxLeft = (float)((int16_t)((buffer[12] << 8) | buffer[13])); _mxLeft /= 32768.0f;
            _myLeft = (float)((int16_t)((buffer[14] << 8) | buffer[15])); _myLeft /= 32768.0f;
            _mzLeft = (float)((int16_t)((buffer[16] << 8) | buffer[17])); _mzLeft /= 32768.0f;
            break;
        case OBEQuaternionRight:
            _axRight = (float)((int16_t)((buffer[0] << 8) | buffer[1])); _axRight /= 32768.0f;
            _ayRight = (float)((int16_t)((buffer[2] << 8) | buffer[3])); _ayRight /= 32768.0f;
            _azRight = (float)((int16_t)((buffer[4] << 8) | buffer[5])); _azRight /= 32768.0f;
            _gxRight = (float)((int16_t)((buffer[6] << 8) | buffer[7])); _gxRight /= 32768.0f;
            _gyRight = (float)((int16_t)((buffer[8] << 8) | buffer[9])); _gyRight /= 32768.0f;
            _gzRight = (float)((int16_t)((buffer[10] << 8) | buffer[11])); _gzRight /= 32768.0f;
            _mxRight = (float)((int16_t)((buffer[12] << 8) | buffer[13])); _mxRight /= 32768.0f;
            _myRight = (float)((int16_t)((buffer[14] << 8) | buffer[15])); _myRight /= 32768.0f;
            _mzRight = (float)((int16_t)((buffer[16] << 8) | buffer[17])); _mzRight /= 32768.0f;
            break;
        case OBEQuaternionCenter:
            _axCenter = (float)((int16_t)((buffer[0] << 8) | buffer[1])); _axCenter /= 32768.0f;
            _ayCenter = (float)((int16_t)((buffer[2] << 8) | buffer[3])); _ayCenter /= 32768.0f;
            _azCenter = (float)((int16_t)((buffer[4] << 8) | buffer[5])); _azCenter /= 32768.0f;
            _gxCenter = (float)((int16_t)((buffer[6] << 8) | buffer[7])); _gxCenter /= 32768.0f;
            _gyCenter = (float)((int16_t)((buffer[8] << 8) | buffer[9])); _gyCenter /= 32768.0f;
            _gzCenter = (float)((int16_t)((buffer[10] << 8) | buffer[11])); _gzCenter /= 32768.0f;
            _mxCenter = (float)((int16_t)((buffer[12] << 8) | buffer[13])); _mxCenter /= 32768.0f;
            _myCenter = (float)((int16_t)((buffer[14] << 8) | buffer[15])); _myCenter /= 32768.0f;
            _mzCenter = (float)((int16_t)((buffer[16] << 8) | buffer[17])); _mzCenter /= 32768.0f;
            break;
    }
}

- (void) motorUpdate{
    dispatch_async(dispatch_get_global_queue(0,0), ^{//normal priority
    
    Byte motor1 = (Byte)(_Motor1 * 255.0f);
     Byte motor2 = (Byte)(_Motor2 * 255.0f);
     Byte motor3 = (Byte)(_Motor3 * 255.0f);
     Byte motor4 = (Byte)(_Motor4 * 255.0f);
    
    /*Byte motor1 = turnOn ? 0xFF : 0x00;
    Byte motor2 = turnOn ? 0xFF : 0x00;
    Byte motor3 = turnOn ? 0xFF : 0x00;
    Byte motor4 = turnOn ? 0xFF : 0x00;*/
    
    Byte auxByte[OBEHapticDataSize];
    auxByte[0] = 0x7E;
    auxByte[1] = motor1;
    auxByte[2] = motor2;
    //auxByte[3] = turnOn ? 0xFF : 0x00;
    auxByte[3] = motor3;
    auxByte[4] = motor4;
    auxByte[5] = 0xFF;
    auxByte[6] = 0x00;
    
    /*counter += 5;
     if(counter > 255){
     turnOn = !turnOn;
     counter = 0;
     }*/
    
    turnOn = !turnOn;
    
    NSData *auxData = [NSData dataWithBytes:auxByte length:OBEHapticDataSize];
    
    [obePeripheral writeValue:auxData forCharacteristic:obeHapticCh type:CBCharacteristicWriteWithoutResponse];
    
    //auxData = nil;
        hasFinishedUpdate = YES;
    });
}

@end
