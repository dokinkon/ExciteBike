using UnityEngine;
using System.Collections;

public class BikeHead : MonoBehaviour {

    public delegate void VoidDelegate();
    public event VoidDelegate OnHit;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider collider) {
        if (collider.CompareTag("road")) {
            if (OnHit!=null) {
                OnHit();
            }
        }
    }
}
