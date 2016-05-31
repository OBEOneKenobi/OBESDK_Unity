#include "unityPlugin.h"


extern "C"
{
    nodBool NodInitialize(void)
    {
        //Initialize SDK classes
        NSLog(@"initialize");
        openSpatial = [OpenSpatialBluetooth sharedBluetoothServ];
        delegate = [[pluginHelper alloc] initWithBluetooth:openSpatial];
        [openSpatial connectToNodDevices];
        return (openSpatial != NULL) && (delegate != NULL);
    }
    
    nodBool NodShutdown(void)
    {
        return true;
    }
    
    nodBool NodRefresh(void)
    {
        return true;
    }
    
    int NodNumRings(void)
    {
        return (int) [delegate.nods count];
    }
    
    const char* NodGetRingName(int ringID)
    {
        if(ringID >= [delegate.nods count])
        {
            return "index outside range";
        }
            
        return [[[delegate.nods objectAtIndex:ringID] name]
                cStringUsingEncoding:NSUTF8StringEncoding];
    }
    NSArray* modeToTagMapping = @[@(0),
                                  @(OS_BUTTON_EVENT_TAG),
                                  @(OS_RAW_ACCELEROMETER_TAG),
                                  @(OS_EULER_ANGLES_TAG),
                                  @(OS_ANALOG_DATA_TAG),
                                  @(OS_DIRECTION_GESTURE_TAG),
                                  @(OS_RELATIVE_XY_TAG),
                                  @(OS_SLIDER_GESTURE_TAG),
                                  @(0),
                                  @(OS_TRANSLATIONS_TAG),
                                  @(OS_RAW_GYRO_TAG)];
    
    nodBool NodSubscribe(int mode, int ringID)
    {
        NSLog(@"%d subscribe to %d", ringID, mode);
        NSString* name = [delegate getNameFromId:ringID];
        
        [openSpatial subscribeToEvents:name
                         forEventTypes:@[[modeToTagMapping objectAtIndex:mode]]];
        return true;
    }
    
    nodBool NodUnsubscribe(int mode, int ringID)
    {
        NSLog(@"%d unsubscribe to %d", ringID, mode);
        NSString* name = [delegate getNameFromId:ringID];
        [openSpatial unsubscribeFromEvents:name
                         forEventTypes:@[[modeToTagMapping objectAtIndex:mode]]];
        return true;
    }
    
    int NodGetButtons(int ringID)
    {
        return 10;
    }
    
    int NodGetButtonState(int ringID, int button)
    {
        return [delegate getButtonState:ringID forButton:button];
    }
    
    NodEulerOrientation NodGetEulerOrientation(int ringID)
    {
        return [delegate getOrientation:ringID];
    }
    
    NodQuaternionOrientation NodGetQuaternionOrientation(int ringID)
    {
        NodEulerOrientation euler = [delegate getOrientation:ringID];
        NodQuaternionOrientation quat = [pluginHelper eulerToQuaternionYaw:euler.yaw Pitch:euler.pitch Roll:euler.roll];
        return quat;
    }
    
    NodGyro NodGetGyro(int ringID) {
        return [delegate getGyro:ringID];
    }
    
    NodAccel NodGetAccel(int ringID)
    {
        return [delegate getAccel:ringID];
    }
    
    NodPointer NodGetPosition2D(int ringID)
    {
        return [delegate getPointerPosition:ringID];
    }
    
    int NodGetGesture(int ringID)
    {
        return [delegate getMostRecentGesture:ringID];
    }
    
    NodPointer NodGetGamePosition(int ringID)
    {
        return [delegate getAnalogData:ringID];
    }
    
    int NodGetTrigger(int ringID)
    {
        return [delegate getTriggerValue:ringID];
    }

    //Request and Get for Ring info
    int NodRequestBatteryPercentage(int ringID)
    {
        NSString* name = [delegate getNameFromId:ringID];
        [openSpatial readBatteryLevel:name];
        return 0;
    }
    int NodGetBatteryPercentage(int ringID)
    {
        return [[delegate.batteryPercentages objectAtIndex:ringID] intValue];
    }
    
    int NodRequestCurrentFirmware(int ringID)
    {
        NSString* name = [delegate getNameFromId:ringID];
        //TODO: Add read firmware once implemented
        return -1;
    }
    
    NodFirmwareVersion NodGetCurrentFirmware(int ringID)
    {
        return [delegate getFirmwareVersion:ringID];
    }
    
    int NodRequestLatestFirmware(int inputChannel)
    {
        //TODO: add when implemented
        return -1;
    }
    
    NodFirmwareVersion NodGetLatestFirmware(int inputChannel)
    {
        return [delegate getLatestFirmware:inputChannel];
    }
    
    int NodRequestMode(int ringID)
    {
        //TODO: add when implemented
        return -1;
    }
    
    int NodGetMode(int ringID)
    {
        return [delegate getMode:ringID];
    }
    
    int NodUpdateRingUniqueID(int ringID){return 0;}
    NodUniqueID NodGetRingUniqueID(int ringID){
        NodUniqueID uid;
        uid.byte0 = -1;
        uid.byte1 = -1;
        uid.byte2 = -1;
        return uid;
    }
    
    //TODO: Incorperate OTA
    int startOTA(int ringID, int inputChannel){return 0;}
    int queryOTAStatus(){return 0;}
    
}


