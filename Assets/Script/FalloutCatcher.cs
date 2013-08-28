using UnityEngine;
using System.Collections;

public class FalloutCatcher : MonoBehaviour {
	
	public Transform safeLocation; 

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnTriggerEnter(Collider other) {
		if (other.gameObject.tag=="Player") {
			
			Vector3 position = Vector3.zero;
			
			if ( safeLocation == null ) {
				position = other.gameObject.transform.position;
				position.x = 0;
				position.y = 3;
			} else {
				position = safeLocation.position;
			}
			
			
    		//var spawn : GameObject = GameObject.FindGameObjectWithTag("Respawn");
    		other.gameObject.transform.position = position;
    		other.gameObject.rigidbody.velocity = Vector3.zero;
    		other.gameObject.rigidbody.angularVelocity = Vector3.zero;
    	}
	}
}
