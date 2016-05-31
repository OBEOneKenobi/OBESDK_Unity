//
//  unityPlugin.h
//  iOSUnity
//
//  Created by Khwaab Dave on 12/9/14.
//  Copyright (c) 2014 Khwaab Dave. All rights reserved.
//
#include <OpenSpatial/OpenSpatialBluetooth.h>
#include "pluginHelper.h"

extern "C"
{
    typedef char nodBool;
    
    OpenSpatialBluetooth* openSpatial;
    pluginHelper* delegate;

    struct NodUniqueID
    {
        char byte0;
        char byte1;
        char byte2;
    };

     nodBool NodInitialize(void);
     nodBool NodShutdown(void);
     nodBool NodRefresh(void);
    
     int  NodNumRings(void);
    const  char* NodGetRingName(int ringID);
    
     nodBool NodSubscribe(int mode, int ringID);
     nodBool NodUnsubscribe(int mode, int ringID);
    
     int NodGetButtons(int ringID);
     int NodGetButtonState(int ringID, int button);
     NodEulerOrientation NodGetEulerOrientation(int ringID);
     NodQuaternionOrientation NodGetQuaternionOrientation(int ringID);
     NodAccel NodGetAccel(int ringID);
     NodGyro NodGetGyro(int ringID);
     int NodGetGesture(int ringID);
     NodPointer NodGetPosition2D(int ringID);
     NodPointer NodGetGamePosition(int ringID);
     int NodGetTrigger(int ringID);
    
     int NodRequestBatteryPercentage(int ringID);
     int NodGetBatteryPercentage(int ringID);
    
     int NodRequestCurrentFirmware(int ringID);
     NodFirmwareVersion NodGetCurrentFirmware(int ringID);
    
     int NodRequestMode(int ringID);
     int NodGetMode(int ringID);
     int NodSetMode(int ringID, int mode);
    
     int NodUpdateRingUniqueID(int ringID);
     NodUniqueID NodGetRingUniqueID(int ringID);

     int NodRequestLatestFirmware(int inputChannel);
     NodFirmwareVersion NodGetLatestFirmware(int inputChannel);
    
     int startOTA(int ringID, int inputChannel);
     int queryOTAStatus();
}