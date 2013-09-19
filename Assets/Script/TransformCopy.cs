using UnityEngine;
using System.Collections;

public class TransformCopy : MonoBehaviour {

    public Transform target;
    public bool copyPosition = false;
    public Vector3 postionOffset;
    public bool copyLocationRotation = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (copyPosition) {
            Vector3 position = target.position;
            position += postionOffset;
            transform.position = position;
        }

        if (copyLocationRotation) {
            transform.localRotation = target.localRotation;
        }
	
	}
}
