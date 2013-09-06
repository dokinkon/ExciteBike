using UnityEngine;
using System.Collections;

public class BikePitch : MonoBehaviour {

    private int _state = BikePitchState.NoPitch;
    public int state {
        get { return _state; }
    }

	// Use this for initialization
	void Start () {
        _state = BikePitchState.NoPitch;
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
