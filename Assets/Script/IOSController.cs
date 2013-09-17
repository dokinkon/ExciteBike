using UnityEngine;
using System.Collections;

public class IOSController : MonoBehaviour {

    public GameObject throttleButton;
    public UILabel accelerationLabel;
    public UILabel debugLabel;
    public Transform accelSpriteTransform;
	// Use this for initialization
    private Vector3 _acceleration;
    private int _measureCount = 0;
    private bool _isMeasuring = false;
    private float _lastXDegree = 0;

    private Bike _bike;
    // http://forum.unity3d.com/threads/15029-Iphone-Shaking
    private static float LowPassKernelWidthInSeconds = 1.0f;
    private static float AccelerometerUpdateInterval = 1.0f / 60.0f;
    private static float LowPassFilterFactor = 1.0f / 60.0f / 1.0f; 
    private Vector3 lowPassValue = Vector3.zero;
    private Vector3 IphoneAcc;
    private Vector3 IphoneDeltaAcc;
    private float _maxY = -99.0f;
    private float _minY = 99.0f;


    Vector3 LowPassFilter(Vector3 newSample) {

        lowPassValue = Vector3.Lerp(lowPassValue, newSample, LowPassFilterFactor);
        return lowPassValue;
    }
    
    

// The greater the value of LowPassKernelWidthInSeconds, the slower the filtered value will converge towards current input sample (and vice versa). You should be able to use LowPassFilter() function instead of avgSamples(). 
    

	void Start () {
        _bike = GetComponent<Bike>();

        if (Application.platform !=RuntimePlatform.IPhonePlayer) {
            throttleButton.SetActive(false);
            gameObject.SetActive(false);
        } else {
            Swipe swipe = GetComponent<Swipe>();
            swipe.swipeDelegate += OnSwipe;
            UIEventListener.Get(throttleButton).onPress = OnThrottleButtonPressed;
        }
	}
	
    void OnThrottleButtonPressed(GameObject button, bool pressed) {
        //Debug.Log("OnThrottleButtonPressed:" + pressed.ToString());
        if (pressed)
            _bike.engine.SetThrottle(1);
        else
            _bike.engine.SetThrottle(0);
    }

	// Update is called once per frame
	void Update () {
        Vector3 v = Input.acceleration;
        accelerationLabel.text =  "(" + v.x + ", " + v.y + ", " + v.z + ")";

        if (_isMeasuring) {
            _acceleration += Input.acceleration;
            _measureCount++;
        } else {

            Quaternion q = Quaternion.FromToRotation(_acceleration, Input.acceleration);
            accelSpriteTransform.localRotation = q;

            //Matrix4x4 m = Matrix4x4.TRS(Vector3.zero, q, Vector3.one);
            //float x = m.MultiplyPoint3x4(Vector3.right).x;
            //float y = m.MultiplyPoint3x4(Vector3.up).y;
            //float z = m.MultiplyPoint3x4(Vector3.forward).z;
            float x = q.eulerAngles.x;
            float y = q.eulerAngles.y;
            float z = q.eulerAngles.z;

            IphoneAcc = Input.acceleration;//new Vector3(x, y, z);
            IphoneDeltaAcc = IphoneAcc - LowPassFilter(IphoneAcc);
            if (IphoneDeltaAcc.y < _minY) {
                _minY = IphoneDeltaAcc.y;
            }

            if (IphoneDeltaAcc.y > _maxY) {
                _maxY = IphoneDeltaAcc.y;
            }

            //debugLabel.text = System.String.Format("max:{0:F3} min:{1:F3}", _maxY, _minY);

            

            if (x > 180) {
                x -= 360;
            }

            float diffX = x - _lastXDegree;
            _lastXDegree = x;

            if ( y > 180 )
                y -= 360;

            accelerationLabel.text = System.String.Format("({0:F3}, {1:F3}, {2:F3}",
                    IphoneDeltaAcc.x,
                    IphoneDeltaAcc.y,
                    IphoneDeltaAcc.z);

            if (_bike!=null) {
                /*
                if (IphoneDeltaAcc.y > .4f ) {
                    _bike.steer.TurnLeft();
                } else if ( IphoneDeltaAcc.y < -.4f) {
                    _bike.steer.TurnRight();
                } else {
                    _bike.steer.ResetSteer();
                }
                */

                /*

                if ( x > 30 )
                    _bike.steer.TurnLeft();
                else if ( x < -30)
                    _bike.steer.TurnRight();
                else
                    _bike.steer.ResetSteer();
                    */

                if (_bike.isFlying) {
                    if ( y > 0 ) {
                        _bike.pitch.PitchUp( y / 40 );
                    } else if ( y < 0) {
                        _bike.pitch.PitchDown ( y / 40);
                    } else {
                        _bike.pitch.ResetPitch();
                    }

                } else {
                    if ( y > 30 ) {
                        _bike.pitch.PitchUp( y / 40 );
                    } else if ( y < -30 ) {
                        _bike.pitch.PitchDown ( y / 40);
                    } else {
                        _bike.pitch.ResetPitch();
                    }
                }
            }
        }
	}

    public void StartMeasureAcceleration() {
        if (_isMeasuring)
            return;

        _acceleration = Vector3.zero;
        _measureCount = 0;
        _isMeasuring = true;
    }

    public void StopMeasureAcceleration() {

        if (!_isMeasuring)
            return;

        _acceleration.x /= _measureCount;
        _acceleration.y /= _measureCount;
        _acceleration.z /= _measureCount;
        _acceleration = _acceleration.normalized;

        _isMeasuring = false;
    }

    void OnSwipe(int type) {
        if (type == Swipe.Up) {
            debugLabel.text = "Swipe.Up";
            _bike.steer.TurnLeft();
        } else if (type == Swipe.Down) {
            debugLabel.text = "Swipe.Down";
            _bike.steer.TurnRight();
        }
    }


}
