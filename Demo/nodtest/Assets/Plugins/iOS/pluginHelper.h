#import "OpenSpatial/OpenSpatialBluetooth.h"

#ifdef _cplusplus
extern "C"
{
#endif
    typedef struct NodEulerOrientation_
    {
        float pitch;
        float roll;
        float yaw;
    } NodEulerOrientation;
    
    typedef struct NodQuaternionOrientation_
    {
        float x;
        float y;
        float z;
        float w;
    } NodQuaternionOrientation;
    
    typedef struct NodAccel_
    {
        float accelX;
        float accelY;
        float accelZ;
    } NodAccel;
    
    typedef struct NodGyro_
    {
        float gyroX;
        float gyroY;
        float gyroZ;
    } NodGyro;
    
    typedef struct NodPointer_
    {
        int x;
        int y;
    } NodPointer;
    
    //Not exposing these structure externally, just using them to track if data has been reported to the user yet or not
    typedef struct InternalNodGesture_
    {
        int gestureType;
        bool userInspectedValue;
    } InternalNodGesture;
    
    typedef struct InternalNodPointer_
    {
        int x;
        int y;
        bool userInspectedValue;
    } InternalNodPointer;
    
    typedef struct NodFirmwareVersion_
    {
        int major;
        int minor;
        int subminor;
    } NodFirmwareVersion;
    
#ifdef _cplusplus
}
#endif

@interface pluginHelper : NSObject <OpenSpatialBluetoothDelegate>

@property OpenSpatialBluetooth* bluetooth;
@property NSMutableArray* foundNods;
@property NSMutableArray* nods;
@property NSMutableArray* buttonStates;
@property NSMutableArray* orientations;
@property NSMutableArray* gestures;
@property NSMutableArray* pos2Ds;
@property NSMutableArray* gyros;
@property NSMutableArray* accels;
@property NSMutableArray* firwmares;
@property NSMutableArray* modes;
@property NSMutableArray* analogs;
@property NSMutableArray* batteryPercentages;


-(id) initWithBluetooth: (OpenSpatialBluetooth*) blue;
-(NSString*) getNameFromId: (int) id;
+(NodQuaternionOrientation) eulerToQuaternionYaw: (float) yaw Pitch:(float) pitch Roll: (float) roll;
-(int) getButtonState: (int) ringID forButton:(int) buttonID;
-(NodEulerOrientation) getOrientation:(int) ringID;
-(NodPointer) getPointerPosition: (int) ringID;
-(int) getMostRecentGesture: (int) ringID;
-(NodPointer) getAnalogData: (int) ringID;
-(int) getTriggerValue: (int) ringID;
-(NodAccel) getAccel: (int) ringID;
-(NodGyro) getGyro: (int) ringID;
-(int) getBatteryValue: (int) ringID;
-(NodFirmwareVersion) getFirmwareVersion: (int) ringID;
-(NodFirmwareVersion) getLatestFirmware: (int) channel;
-(int) getMode: (int) ringID;
-(NSArray*) getOTAStatus;

@end