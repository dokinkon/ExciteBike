using UnityEngine;
using System.Collections;

public class BikeControl : MonoBehaviour {

    private BikePitch _pitch;
    private BikeSteer _steer;
    public BikeEngine _engine;
    private BikeCrash _crashHandler;
    private Joystick _joystick;


	// Use this for initialization
	void Start () {
        _pitch = GetComponent<BikePitch>();
        _steer = GetComponent<BikeSteer>();
        _crashHandler = GetComponent<BikeCrash>();
	}
	
	// Update is called once per frame
	void Update () {
		if (!networkView.isMine)
			return;

        if (_crashHandler.isCrashed) {
            _steer.ResetSteer();
            _pitch.ResetPitch();
        } else {
        }
	}
}
