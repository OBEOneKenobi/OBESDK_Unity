Current version 1.8
Published 10/20/2015

Getting Started:
Check out our example scenes in Assets\Nod\Examples\Scenes\
You will need to pair your Nod device to your system through its bluetooth settings.  Windows and Android need to have the Nod Config app installed on your system, see platform notes below for details.

Platforms notes:
Android: 
Our plugin requires some modifications to the AndroidManifest.xml file to enable bluetooth support, and reference the nod service app.
There should be a preexisting AndroidManifest.xml you can use as under: Assets\Plugins\Android\
You will need a companion app to use our Android plugin for Unity you can get it here: 
https://play.google.com/apps/testing/com.nod_labs.mainapp

Windows: You will need to have our windows service installed to run on windows. You can find the service installer here: 
https://github.com/openspatial/openspatial-windows/blob/master/Nod%20Installer/setup-1686.exe

Nod Unity SDK basics:
Since bandwidth over Bluetooth LE is limited you should conditionally subscribe only to what you need, both to save battery on the device and to speed up communication over bluetooth.
Here is a break down of what services you can subscribe to from a Nod Devices:
ButtonMode - Sends buttons. See Nod.ButtonIDs.Backspin for a list of button IDs for the backspin.  Can be read by calling nodDevice.GetNodButton(Nod.ButtonIDs.Ring.Touch0) for example.
EulerMode - The orientation of the device.  Reported in nodDevice.rotation
GameControlMode - backspin only. Joystick and trigger values. Read them from nodDevice.triggerPressure (range 0-1) and nodDevice.joyStickPosition (Values should be in the range of -1.0f to 1.0f for x and y)
GestureMode - ring only.  See website for how to perform gestures.  Will report for one frame the most recent gesture.  Up, Down, Left, Right, Clockwise, Counter Clockwise.  Ring only at the moment but will be exposed for the backspin soon.  Reported as nodDevice.gestureState
PointerMode - ring only.  Mouse emulation.  gets reported as nodDevice.position2D which is relative deltas since the last update.
AccelMode - raw Acceleration values.  Should be normalized around gravity.  read from nodDevice.acceleration.  Maybe useful to see if you are gently pushing something or punching something hard.
GyroMode - raw gyroscopic values.  Don't use these to track orientation use EulerMode and nodDevice.rotation for that.  This is useful for figuring out angular momentum, not angular position.  Maybe useful to detect a wave or a slap.

The orientation the backspin reports through EulerMode is one of the most powerful and interesting aspects of what your Nod device reports.  It can also be the most confusing to figure out how to read usefully.
As an example you could bind the orientation from your nod device to the orientation of a shoulder joint on a rigged skinned mesh to have a model move its arm around.
There are a couple of tricks you need to be aware of to get the Orientation data out of the backspin to make sense.
Take a look at the "Example_OrientationAndButtons" scene under the NodUnitySDK, under Assets/Nod/Examples/Scenes/
To get the motions of the backspin to effect the model of the nod ring in a similar manner we have to establish a baseline frame of reference for how the data from the device, which is in the real world, affects the unity orientation of the object you are applying it to.  
To do this in the orientation demo start the demo by clicking play in unity, then hold the backspin such that the USB charge port is facing you, and the joystick if pointing up.  Now click or press the reset button.  
At this point the rotations of the backspin in the real world should have a similar rotational effect on the model in the scene.

The reset buttons calls "recenter" to establish a frame of reference where your rotations make sense.
In NodOrientationExample.cs we do this with the recenter call:
	private void recenter()
	{
		inverseInitialRotation = Quaternion.Inverse(nodDevice.rotation);
	}
Which gets applied in the Update call when reading the current rotation of the device:
	public void Update()
	{
...
		//Example of applying the nod devices orientation to the local transform.
		transform.localRotation = inverseInitialRotation * nodDevice.rotation;
	}

The rotation is applied to the local rotation, and the parent actually has a non zero rotation and that was critical to get things to line up for that example.  The parent had a Euler rotation of (90, 90, 0).

There is a trick that can help get orientations to line up.  Bring in the model of the ring and parent it to the GameObject that I want to apply a rotation to.  For this discussion lets call the GameObject "gizmo".  
Zero "Gizmo"'s position, and rotate it such that its rest position is parallel to the rotation of how the controller will be held in the real world and the target display.
Disable the ring model in the unity scene so its not visible in your demos and use the transform of the gizmo as a rotational frame of reference like this:
	public Transform gizmo; //The transform of the gizmo linked up in the scene
	public void Awake()
	{
		startRot = transform.rotation;

		gizmoRotation = gizmo.rotation;
		inverseGizmoRotation = Quaternion.Inverse(gizmoRotation);
	}
	public void Update()
	{
...
		//Always start with the rotation as supplied from Unity
		transform.rotation = startRot;

		//relative change of the ring from our defined ring origin
		Quaternion ringDelta = inverseInitialRotation * ring.rotation;

		//Ring rotation within the frame of the gizmo
		Quaternion gizmoDelta = gizmoRotation * ringDelta * inverseGizmoRotation;
		transform.rotation *= gizmoDelta;
...
	}

This will let you use the Unity editor to help you visualize a frame of reference for where your rotations of the nod device will make sense in your scene.