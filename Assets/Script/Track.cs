using UnityEngine;
using System.Collections;

public class Track : MonoBehaviour {

    public GameObject grassPrefab;
    public GameObject trackSidePrefab;
    public GameObject defaultRoadPrefab;

    public static float GetLocationX(int trackIndex) {

        if (trackIndex < 0 || trackIndex > 3) {
            System.Random random = new System.Random( 1 );
            trackIndex = random.Next(4);
        }

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

    public static Color GetColor(int trackIndex) {
        if (trackIndex == 0) {
            return new Color(1.0f, 0.6f, 0.6f);
        } else if (trackIndex == 1) {
            return new Color(0.6f, 1.0f, 0.6f);
        } else if (trackIndex == 2) {
            return new Color(0.6f, 0.6f, 1.0f);
        } else {
            return new Color(1.0f, 1.0f, 0.6f);
        }
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
