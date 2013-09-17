using UnityEngine;
using System.Collections;

public class TrackAlign : MonoBehaviour {

    public int index = -1;

	// Use this for initialization
	void Start () {
        Vector3 p = transform.position;
        p.x = Track.GetLocationX(index);
        transform.position = p;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
