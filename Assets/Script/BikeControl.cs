using UnityEngine;
using System.Collections;

public class BikeControl : MonoBehaviour {

    private BikePitch _pitch;
    private BikeSteer _steer;
    public BikeEngine _engine;
    private BikeCrash _crashHandler;

	// Use this for initialization
	void Start () {
        _pitch = GetComponent<BikePitch>();
        _steer = GetComponent<BikeSteer>();
        _crashHandler = GetComponent<BikeCrash>();
        //_engine = GetComponent<BikeEngine>();
	}
	
	// Update is called once per frame
	void Update () {
		if (!networkView.isMine)
			return;

        if (_crashHandler.isCrashed) {
            _steer.ResetSteer();
            _pitch.ResetPitch();
        } else {
            UpdateInputControlWithKeyboard();
        }
        /*
        if (controlByNPC) {
            engine.SetThrottle(1.0f);
        } else {
            if ( _runtimePlatform == RuntimePlatform.IPhonePlayer ) {
                UpdateInputControlWithVirtualJoystick();
            } else {
                UpdateInputControlWithKeyboard();
            }
        }
        */
	
	}

	void UpdateInputControlWithKeyboard() {
		if ( Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W)) {
			_steer.TurnLeft();
		} else if ( Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S) ) {
			_steer.TurnRight();
		} else {
			_steer.ResetSteer();
		}
		
		if ( Input.GetKey (KeyCode.LeftArrow) || Input.GetKey ( KeyCode.A) ) {
			_pitch.PitchUp();
		} else if ( Input.GetKey (KeyCode.RightArrow) || Input.GetKey ( KeyCode.D) ) {
			_pitch.PitchDown();
		} else {
			_pitch.ResetPitch();
		}	

        if ( Input.GetKey(KeyCode.Space) ) {
            _engine.SetThrottle(1);
        } else {
            _engine.SetThrottle(0);
        }
	}
	
	void UpdateInputControlWithVirtualJoystick() {
		
        /*
		if (!_joystick) {
			_joystick = (Joystick)GameObject.FindWithTag("joystick").GetComponent("Joystick");
			if (!_joystick) {
				Debug.LogError("Can't grad joystick");
			}
		}
		
		if ( _joystick ) {
			//Debug.Log ("Joystick.x:" + _joystick.position.x);
			if (_joystick.position.x < -0.7 ) {
				_pitch.();
			} else if (_joystick.position.x > 0.7 ) {
				TiltDown();
			} else {
				ResetTilt();
			}
			
			if (_joystick.position.y < -0.7 ) {
				_bikeSteer.TurnRight();
			} else if (_joystick.position.y > 0.7 ) {
				_bikeSteer.TurnLeft();
			} else {
				_bikeSteer.ResetSteer();
			}
		}

        engine.SetThrottle(1.0f);
        */
	}
}
