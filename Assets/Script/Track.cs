using UnityEngine;
using System.Collections;

public class Track : MonoBehaviour {

    public static float GetLocationX(int trackIndex) {
        if (trackIndex == 0) {
            return 3.0f;
        } else if (trackIndex == 1) {
            return 1.0f;
        } else if ( trackIndex == 2 ) {
            return -1.0f;
        } else if (trackIndex == 3) {
            return -3.0f;
        } else {
            return 3.0f;
        }
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
