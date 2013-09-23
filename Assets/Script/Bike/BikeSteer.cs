using UnityEngine;
using System.Collections;
using System;

public class BikeSteer : MonoBehaviour {

    public float speed = 4.0f;

    private float _steerValue = 0; // -1 ~ 1
    public float steerValue {
        get { return _steerValue; }
    }
    private int _steerState = BikeSteerState.NoSteer;
    public int state {
        get { return _steerState; }
    }

    public void TurnLeft() {
        _steerState = BikeSteerState.Left;
    }

    public void TurnRight() {
        _steerState = BikeSteerState.Right;
    }

    public void ResetSteer() {
        _steerState = BikeSteerState.NoSteer;
    }

	// Use this for initialization
	void Start () {
        _steerValue = 0.0f;
        _steerState = BikeSteerState.NoSteer;
	}
	
	// Update is called once per frame
	void Update () {

        float step = Time.deltaTime * speed;

        if (_steerState == BikeSteerState.Left) {
            _steerValue -= step;
        } else if ( _steerState == BikeSteerState.Right) {
            _steerValue += step;
        } else {

            if (Math.Abs(_steerValue) < step) {
                _steerValue = 0.0f;
            } else {
                if (_steerValue > 0) {
                    _steerValue -= step;
                } else {
                    _steerValue += step;
                }
            }
        }

        _steerValue = Math.Min(Math.Max(_steerValue, -1), 1);
	}
}
