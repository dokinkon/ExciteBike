using UnityEngine;
using System.Collections;
using System;

public class BikePitch : MonoBehaviour {

    private int _state = BikePitchState.NoPitch;
    public int state {
        get { return _state; }
    }

    public float rotateSpeed = 0.1f;
    public float maxUpAngle = -60;
    public float maxDownAngle = 60;
    private float _strength = 0.0f;

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
            v.x = -dist * rotateSpeed * _strength;
		} else if ( _state == BikePitchState.Down ) {
            Quaternion targetRotation = Quaternion.AngleAxis(maxDownAngle, Vector3.right);
            float dist = Quaternion.Angle( rigidbody.rotation, targetRotation );
			v.x = dist * rotateSpeed * _strength;
		} else if ( _state == BikePitchState.Balance ) {
            //Debug.Log("Balance");
            //Quaternion targetRotation = Quaternion.AngleAxis(0, Vector3.right);
            //float dist = Quaternion.Angle( rigidbody.rotation, targetRotation );
			//v.x = -dist * rotateSpeed * _strength;
        }
		rigidbody.angularVelocity = v;
    }

	// Update is called once per frame
	void Update () {
	
	}

    public void PitchUp(float strength) {
        _state = BikePitchState.Up;
        _strength = Math.Min(Math.Abs(strength), 1.0f);
    }

    public void PitchDown(float strength) {
        _state = BikePitchState.Down;
        _strength = Math.Min(Math.Abs(strength), 1.0f);
    }

    public void Balance(float strength) {
        _state = BikePitchState.Balance;
        _strength = Math.Min(Math.Abs(strength), 1.0f);
    }

    public void ResetPitch() {
        _state = BikePitchState.NoPitch;
    }
}
