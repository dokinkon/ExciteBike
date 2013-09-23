using UnityEngine;
using System.Collections;

public class BikeSlowDown : MonoBehaviour {

    public float speedLimit = 5;
    public float rpmLimit = 2000;
    private bool _shouldSlowdown;
    public bool shouldSlowdown {
        get { return _shouldSlowdown; }
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider other ) {
		if ( other.gameObject.tag == "slowdown" ) {
			_shouldSlowdown = true;
		}
    }

	void OnTriggerExit(Collider other) {
		if ( other.gameObject.tag == "slowdown" ) {
			_shouldSlowdown = false;
		}
    }
}
