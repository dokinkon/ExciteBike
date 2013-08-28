using UnityEngine;
using System.Collections;

public class ShakeCamera : MonoBehaviour {
	
	public float shakePower = 2.0f; 
	public float shakeDelay = 0.05f;
	public float shakeAngle = 10;
	
	IEnumerator UpDownShake () {
		transform.Translate(Vector3.up * shakePower);
		yield return new WaitForSeconds(shakeDelay);
		transform.Translate(Vector3.up * -shakePower);
	}

	IEnumerator LeftRightShake () {
		transform.Translate(Vector3.right * -shakePower);
		yield return new WaitForSeconds(shakeDelay);
		transform.Translate(Vector3.right * shakePower);
	}

	IEnumerator ForwardBackShake () {
		transform.Translate(Vector3.forward * shakePower);
		yield return new WaitForSeconds(shakeDelay);
		transform.Translate(Vector3.forward * -shakePower);
	}

	IEnumerator XRotateShake () {
 		transform.Rotate(Vector3.right * shakeAngle);
 		yield return new WaitForSeconds(shakeDelay);
 		transform.Rotate(Vector3.right * -shakeAngle);
	}

	IEnumerator YRotateShake () {
 		transform.Rotate(Vector3.up * shakeAngle);
 		yield return new WaitForSeconds(shakeDelay);
 		transform.Rotate(Vector3.up * -shakeAngle);
	}

	IEnumerator ZRotateShake () {
 		transform.Rotate(Vector3.forward * shakeAngle);
 		yield return new WaitForSeconds(shakeDelay);
 		transform.Rotate(Vector3.forward * -shakeAngle);
	}
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
