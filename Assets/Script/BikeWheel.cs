using UnityEngine;
using System.Collections;

public class BikeWheel : MonoBehaviour {
	
	private int touchCount_;
	
	// Use this for initialization
	void Start () {
	
	}
	
	public bool IsTouchingTheRoad() {
		return touchCount_ != 0;
	}
		
	void OnTriggerEnter ( Collider other ) {
		//Debug.Log("OnTriggerEnter");
		//if ( other.gameObject.tag == "road" ) {
			touchCount_++;
		//}
	}
	
	void OnTriggerExit ( Collider other ) {
		//if ( other.gameObject.tag == "road" ) {
			touchCount_--;
		//}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
