using UnityEngine;
using System.Collections;

public class BikeFollow : MonoBehaviour {

    public ParticleSystem boostParticleEmitter;
    private Transform _bikeTransform;
    public void SetBikeTransform(Transform t) {
        _bikeTransform = t;
    }
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = _bikeTransform.position;
	}
}
