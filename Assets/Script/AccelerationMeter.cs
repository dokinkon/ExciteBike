using UnityEngine;
using System.Collections;

public class AccelerationMeter : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 acc = Input.acceleration;
		guiText.text = System.String.Format("accel:{0}, {1}, {2}", acc.x, acc.y, acc.z);
	}
}
