using UnityEngine;
using System.Collections;

public class CameraFacingBillboard : MonoBehaviour {
	
	public Camera referenceCamera;
	
	// Use this for initialization
	void Start () {
		if (!referenceCamera) 
			referenceCamera = Camera.main;
	
	}
	
	// Update is called once per frame
	void Update () {
		
		transform.LookAt(transform.position + referenceCamera.transform.rotation * Vector3.back,
            referenceCamera.transform.rotation * Vector3.up);
	
	}
}
