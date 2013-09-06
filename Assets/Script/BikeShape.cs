using UnityEngine;
using System.Collections;

public class BikeShape : MonoBehaviour {

    public Renderer skinRenderer;

	// Use this for initialization
	void Start () {
        skinRenderer.material.color = Color.red;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
