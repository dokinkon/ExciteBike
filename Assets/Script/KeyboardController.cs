using UnityEngine;
using System.Collections;

public class KeyboardController : MonoBehaviour {

    public float pitchStrength = 0.4f;

    private Bike _bike;


    public void SetBike(Bike b) {
        _bike = b;
    }

	// Use this for initialization
	void Start () {
        if (Application.platform == RuntimePlatform.IPhonePlayer) {
            gameObject.SetActive(false);
        }
	}
	
	// Update is called once per frame
	void Update () {
        if (_bike == null)
            return;

		if ( Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W)) {
			_bike.steer.TurnLeft();
		} else if ( Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S) ) {
			_bike.steer.TurnRight();
		} else {
			_bike.steer.ResetSteer();
		}
		
		if ( Input.GetKey (KeyCode.LeftArrow) || Input.GetKey ( KeyCode.A) ) {
			_bike.pitch.PitchUp(pitchStrength);
		} else if ( Input.GetKey (KeyCode.RightArrow) || Input.GetKey ( KeyCode.D) ) {
			_bike.pitch.PitchDown(pitchStrength);
		} else {
            if (_bike.isFlying) {
                _bike.pitch.Balance(1);
            } else {
                _bike.pitch.ResetPitch();
            }
		}	

        if ( Input.GetKey(KeyCode.Space) ) {
            _bike.engine.SetThrottle(1);
        } else {
            _bike.engine.SetThrottle(0);
        }
	}
}
