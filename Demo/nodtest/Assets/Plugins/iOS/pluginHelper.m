#import "pluginHelper.h"


@implementation pluginHelper

-(id) initWithBluetooth: (OpenSpatialBluetooth*) blue
{
    self = [super init];
    if(self)
    {
        self.bluetooth = blue;
        self.bluetooth.delegate = self;
        self.nods = [[NSMutableArray alloc] init];
        self.pos2Ds = [[NSMutableArray alloc] init];
        self.gestures = [[NSMutableArray alloc] init];
        self.accels = [[NSMutableArray alloc] init];
        self.gyros = [[NSMutableArray alloc] init];
        self.analogs = [[NSMutableArray alloc] init];
        self.orientations = [[NSMutableArray alloc] init];
        self.buttonStates = [[NSMutableArray alloc] init];
        self.batteryPercentages = [[NSMutableArray alloc] init];
    }
    return self;
}

-(void) didReadBatteryLevel:(NSInteger)level forRingNamed:(NSString *)name
{
    int index = [self indexFromName:name];
    [self.batteryPercentages replaceObjectAtIndex:index withObject:@(level)];
}

-(void) didConnectToNod:(CBPeripheral *)peripheral
{
    NSLog(@"Connected");
    int i = 0;
    bool found = false;
    for(CBPeripheral* perph in self.nods)
    {
        if([perph.name isEqualToString:peripheral.name])
        {
            [self.nods replaceObjectAtIndex:i withObject:peripheral];
            found = true;
        }
        i++;
    }
    if(!found)
    {
        [self.nods addObject:peripheral];
        RelativeXYData* pointerEvent = [[RelativeXYData alloc] init];
        EulerData* rotationEvent = [[EulerData alloc] init];
        GestureData* gestureEvent = [[GestureData alloc] init];
        NSMutableArray* buttonEvent = [[NSMutableArray alloc] initWithObjects:@(0),@(0),@(0),@(0),@(0),@(0),@(0),@(0),@(0),@(0), nil];
        GyroscopeData* gyro = [[GyroscopeData alloc] init];
        AccelerometerData* accel = [[AccelerometerData alloc]init];
        AnalogData* analog = [[AnalogData alloc] init];
        [self.pos2Ds addObject:pointerEvent];
        [self.gestures addObject:gestureEvent];
        [self.orientations addObject:rotationEvent];
        [self.buttonStates addObject:buttonEvent];
        [self.gyros addObject:gyro];
        [self.accels addObject:accel];
        [self.analogs addObject:analog];
        [self.batteryPercentages addObject:@(-1)];
        //TODO: Firmwares, modes
        
        NSLog(@"num connected devices:%d", [self.nods count]);
    }
}

-(NSString*) getNameFromId:(int)id
{
    return [[self.nods objectAtIndex:id] name];
}

-(int) indexFromName: (NSString*) name
{
    for(int i = 0; i < [self.nods count]; i++)
    {
        if([[[self.nods objectAtIndex:i] name] isEqualToString:name])
        {
            return i;
        }
    }
    return -1;
}

-(void) openSpatialDataFired:(OpenSpatialData *)openSpatialData
{
    switch (openSpatialData.dataType) {
        case OS_RELATIVE_XY_TAG:
            [self pointerEventFired:(RelativeXYData*) openSpatialData];
            break;
        case OS_DIRECTION_GESTURE_TAG:
            [self gestureEventFired:(GestureData*) openSpatialData];
            break;
        case OS_EULER_ANGLES_TAG:
            [self rotationEventFired:(EulerData*) openSpatialData];
            break;
        case OS_BUTTON_EVENT_TAG:
            [self buttonEventFired:(ButtonData*) openSpatialData];
            break;
        case OS_RAW_GYRO_TAG:
            [self gyroEventFired:(GyroscopeData*) openSpatialData];
            break;
        case OS_RAW_ACCELEROMETER_TAG:
            [self accelEventFired:(AccelerometerData*) openSpatialData];
            break;
        case OS_ANALOG_DATA_TAG:
            [self analogEventFired:(AnalogData*)openSpatialData];
            break;
        default:
            break;
    }
}

-(void) pointerEventFired:(RelativeXYData*)pointerEvent
{
    int index = [self indexFromName:pointerEvent.peripheral.name];
    [self.pos2Ds replaceObjectAtIndex:index withObject:pointerEvent];
}

-(void) gestureEventFired:(GestureData*)gestureEvent
{
    int index = [self indexFromName:gestureEvent.peripheral.name];
    [self.gestures replaceObjectAtIndex:index withObject:gestureEvent];
}

-(void) rotationEventFired:(EulerData*)rotationEvent
{
    int index = [self indexFromName:rotationEvent.peripheral.name];
    [self.orientations replaceObjectAtIndex:index withObject:rotationEvent];
}

-(void) buttonEventFired:(ButtonData*)buttonEvent
{
    ButtonData* buttonE = (ButtonData*) buttonEvent;
    int index = [self indexFromName:buttonEvent.peripheral.name];
    int buttonID = buttonE.buttonID;
    int state;
    if(buttonE.buttonState == UP)
    {
        state = 0;
    }
    else
    {
        state = 1;
    }
    [[self.buttonStates objectAtIndex:index] replaceObjectAtIndex:buttonID withObject:@(state)];
}

-(void) gyroEventFired:(GyroscopeData*) gyroEvent
{
    int index = [self indexFromName:gyroEvent.peripheral.name];
    [self.gyros replaceObjectAtIndex:index withObject:gyroEvent];
}

-(void) accelEventFired:(AccelerometerData*) accelEvent
{
    int index = [self indexFromName:accelEvent.peripheral.name];
    [self.accels replaceObjectAtIndex:index withObject:accelEvent];
}

-(void) analogEventFired:(AnalogData*)analogEvent
{
    int index = [self indexFromName:analogEvent.peripheral.name];
    [self.analogs replaceObjectAtIndex:index withObject:analogEvent];
}

-(NodPointer) getPointerPosition:(int)ringID
{
    RelativeXYData* pEvent = [self.pos2Ds objectAtIndex:ringID];
    int x = pEvent.x;
    int y = pEvent.y;
    if(x != 0 && y != 0)
    {
        pEvent.x = 0;
        pEvent.y = 0;
    }
    NodPointer pointer;
    pointer.x = x;
    pointer.y = y;
    return pointer;
}

-(NodEulerOrientation) getOrientation:(int)ringID {
    EulerData* rEvent = (EulerData*)[self.orientations objectAtIndex:ringID];
    NodEulerOrientation orientation;
    orientation.pitch = rEvent.pitch;
    orientation.yaw = rEvent.yaw;
    orientation.roll = rEvent.roll;
    return orientation;
}

-(int) getButtonState:(int)ringID forButton:(int)buttonID {
    return [[[self.buttonStates objectAtIndex:ringID] objectAtIndex:buttonID] intValue];
}

-(int) getMostRecentGesture:(int)ringID {
    GestureData* gEvent = [self.gestures objectAtIndex:ringID];
    int ret = gEvent.gestureType;
    gEvent.gestureType = 0;
    return ret;
}

-(NodPointer) getAnalogData: (int) ringID {
    AnalogData* anEv = [self.analogs objectAtIndex:ringID];
    NodPointer point;
    point.x = anEv.x;
    point.y = anEv.y;
    return point;
}

-(int) getTriggerValue: (int) ringID
{
    AnalogData* anEv = [self.analogs objectAtIndex:ringID];
    int trig = anEv.trigger;
    return trig;
}

-(NodAccel) getAccel: (int) ringID
{
    AccelerometerData* mot6d = [self.accels objectAtIndex:ringID];
    NodAccel accel;
    accel.accelX = mot6d.x;
    accel.accelY = mot6d.y;
    accel.accelZ = mot6d.z;
    return accel;
}

-(NodGyro) getGyro: (int) ringID
{
    GyroscopeData* mot6D = [self.gyros objectAtIndex:ringID];
    NodGyro gyro;
    gyro.gyroX = mot6D.x;
    gyro.gyroY = mot6D.y;
    gyro.gyroZ = mot6D.z;
    return gyro;
}

-(int) getBatteryValue: (int) ringID
{
    int battery = [[self.batteryPercentages objectAtIndex:ringID] intValue];
    return battery;
}

-(NodFirmwareVersion) getFirmwareVersion: (int) ringID
{
    //TODO grab from array
    NodFirmwareVersion vers;
    vers.major = -1; vers.minor = -1;vers.subminor = -1;
    return vers;
}
-(NodFirmwareVersion) getLatestFirmware: (int) channel
{
    //TODO grab from array
    NodFirmwareVersion vers;
    vers.major = -1; vers.minor = -1;vers.subminor = -1;
    return vers;
}
-(int) getMode: (int) ringID
{
    //TODO grab from array
    return -1;
}

-(NSArray*) getOTAStatus
{
    //TODO: implement
    NSArray* array = [[NSArray alloc] initWithObjects:@(-2), nil];
    return array;
}

+(NodQuaternionOrientation) eulerToQuaternionYaw:(float)yaw Pitch:(float)pitch Roll:(float)roll
{
    float sinHalfYaw = sin(yaw / 2.0f);
    float cosHalfYaw = cos(yaw / 2.0f);
    float sinHalfPitch = sin(pitch / 2.0f);
    float cosHalfPitch = cos(pitch / 2.0f);
    float sinHalfRoll = sin(roll / 2.0f);
    float cosHalfRoll = cos(roll / 2.0f);
    
    NodQuaternionOrientation result;
    result.x = -cosHalfRoll * sinHalfPitch * sinHalfYaw
    + cosHalfPitch * cosHalfYaw * sinHalfRoll;
    result.y = cosHalfRoll * cosHalfYaw * sinHalfPitch
    + sinHalfRoll * cosHalfPitch * sinHalfYaw;
    result.z = cosHalfRoll * cosHalfPitch * sinHalfYaw
    - sinHalfRoll * cosHalfYaw * sinHalfPitch;
    result.w = cosHalfRoll * cosHalfPitch * cosHalfYaw
    + sinHalfRoll * sinHalfPitch * sinHalfYaw;
    
    return result;
}

@end