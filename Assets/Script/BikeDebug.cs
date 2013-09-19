using UnityEngine;
using System.Collections;

public class BikeDebug : MonoBehaviour {

    public int lap;
    private Bike _bike;
	// Use this for initialization
	void Start () {
        _bike = GetComponent<Bike>();
	
	}
	
	// Update is called once per frame
	void Update () {
        lap = _bike.lap;
	}
}
