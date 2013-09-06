using UnityEngine;
using System.Collections;

public class BikeCrash : MonoBehaviour {

    public float rotateSpeed = 7.0f;
    public float moveSpeed = 4.0f;
    public float threshold = 0.5f;
    private bool _isCrashed = false;
    private float _crashTimer;
    public BikeEngine _engine;
    public bool isCrashed {
        get { return _isCrashed; }
    }

	// Use this for initialization
	void Start () {
	
	}
	
	void FixedUpdate() {
        if (_isCrashed) {
            Vector3 v = rigidbody.angularVelocity;
            v.x = rotateSpeed;
            rigidbody.angularVelocity = v;

            v = rigidbody.velocity;
            v.z = moveSpeed;
            rigidbody.velocity = v;
        }
    }

	// Update is called once per frame
	void Update () {
        if (_isCrashed) {
            _crashTimer += Time.deltaTime;
            if (_crashTimer > 3) {
                _isCrashed = false;
                _crashTimer = 0;
                _engine.isStarted = true;

                Vector3 p = rigidbody.position;
                rigidbody.rotation = Quaternion.identity;
                rigidbody.velocity = Vector3.zero;
                rigidbody.angularVelocity = Vector3.zero;
                rigidbody.MovePosition(p + Vector3.up);
            }
        }
	}

    void OnCollisionEnter(Collision collision) {
        //Debug.Log("OnCollisionEnter");
        if (_isCrashed)
            return;

        foreach (ContactPoint contact in collision.contacts) {
            float dot = Vector3.Dot(transform.up, contact.normal);
            if (dot < threshold) {
                _isCrashed = true;
                _engine.isStarted = false;
                Debug.Log("OnCrashed:" + dot);

            }
        }
    }
}
