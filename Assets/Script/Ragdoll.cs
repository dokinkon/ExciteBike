using UnityEngine;
using System.Collections;

public class Ragdoll : MonoBehaviour {

    public Rigidbody[] rigidbodies;

    public void SetVelocity(Vector3 v) {
        foreach ( Rigidbody b in rigidbodies ) {
            b.velocity = v;
        }
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
