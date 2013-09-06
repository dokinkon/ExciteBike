using UnityEngine;
using System.Collections;

public class BikePitch : MonoBehaviour {

    private int _state = BikePitchState.NoPitch;
    public int state {
        get { return _state; }
    }

    public float rotateSpeed = 0.1f;
    public float maxUpAngle = -60;
    public float maxDownAngle = 60;

    private BikeCrash _crash;

	// Use this for initialization
	void Start () {
        _state = BikePitchState.NoPitch;
        _crash = GetComponent<BikeCrash>();
	}

    void FixedUpdate() {

        if (_crash.isCrashed)
            return;

		Vector3 v = rigidbody.angularVelocity;
		if ( _state == BikePitchState.Up ) {
            Quaternion targetRotation = Quaternion.AngleAxis(maxUpAngle, Vector3.right);
            float dist = Quaternion.Angle( rigidbody.rotation, targetRotation );
            v.x = -dist * rotateSpeed;
		} else if ( _state == BikePitchState.Down ) {
            Quaternion targetRotation = Quaternion.AngleAxis(maxDownAngle, Vector3.right);
            float dist = Quaternion.Angle( rigidbody.rotation, targetRotation );
			v.x = dist * rotateSpeed;
		} 
		rigidbody.angularVelocity = v;
    }

	// Update is called once per frame
	void Update () {
	
	}

    public void PitchUp() {
        _state = BikePitchState.Up;
    }

    public void PitchDown() {
        _state = BikePitchState.Down;
    }

    public void ResetPitch() {
        _state = BikePitchState.NoPitch;
    }
}
