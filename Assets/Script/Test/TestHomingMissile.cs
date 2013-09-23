using UnityEngine;
using System.Collections;

public class TestHomingMissile : MonoBehaviour {

    public Transform launchLocation;
    public Transform target;

    private Rigidbody _targetRigidbody;
    private DetectableObject _detectable;

	// Use this for initialization
	void Start () {

        GameObject target = GameObject.Find("Target");
        _detectable = target.GetComponent<DetectableObject>();
        _targetRigidbody = target.GetComponent<Rigidbody>();
	}

    void OnGUI() {
        if (GUI.Button(new Rect(40, 40, 100, 40), "Launch HomingMissile")) {
            MissileController.Launch(launchLocation.position, target.transform, null);
        }
    }
	
	// Update is called once per frame
	void Update () {
        _targetRigidbody.WakeUp();
	}
}
