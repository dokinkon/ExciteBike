using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
	
	public Bike bike;
	
	// Use this for initialization
	void Start () {
	
	}
	
	void UpdateIPhonePlayer() {
		Vector3 acc = Input.acceleration;
		if (acc.x < -3 ) {
			//bike.TiltUp();
		} else if (acc.x > 3 ) {
			//bike.TiltDown();
		} else {
			//bike.ResetTilt();
		}
		
		if (acc.y < -0.85 ) {
			//bike.TurnRight();
		} else if ( acc.y > -0.65) {
			//bike.TurnLeft();
		} else {
			//bike.ResetSteer();
		}
	}
	
	// Update is called once per frame
	void Update () {
		
		if ( !bike )
			return;
		
		/*
		if (Application.platform == RuntimePlatform.IPhonePlayer) {
			//UpdateIPhonePlayer();
		} else {
			if ( Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W)) {
				bike.TurnLeft();
			} else if ( Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S) ) {
				bike.TurnRight();
			} else {
				bike.ResetSteer();
			}
		
		
			if ( Input.GetKey (KeyCode.LeftArrow) || Input.GetKey ( KeyCode.A) ) {
				bike.TiltUp();
			} else if ( Input.GetKey (KeyCode.RightArrow) || Input.GetKey ( KeyCode.D) ) {
				bike.TiltDown();
			} else {
				bike.ResetTilt();
			}	
		}
		*/
	}
}
