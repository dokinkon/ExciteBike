using UnityEngine;
using System.Collections;

public class BikeWheel : MonoBehaviour {
	
	private int _touchCount;
	
	// Use this for initialization
	void Start () {
	
	}
	
	public bool IsTouchingTheRoad() {
		return _touchCount != 0;
	}

    /*
    void OnCollisionEnter( Collider other) {
        if (other.gameObject.tag == "road") {
            _touchCount++;
        }
    }

    void OnCollisionExit( Collider other ) {
        if (other.gameObject.tag == "road") {
            _touchCount--;
        }
    }
    */
		
	void OnTriggerEnter ( Collider other ) {
		if ( other.gameObject.tag == "road" ) {
			_touchCount++;
		}
	}
	
	void OnTriggerExit ( Collider other ) {
		if ( other.gameObject.tag == "road" ) {
			_touchCount--;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
