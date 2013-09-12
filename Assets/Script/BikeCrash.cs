using UnityEngine;
using System.Collections;

public class BikeCrash : MonoBehaviour {

    public AudioSource _soundEffect;
    public float rotateSpeed = 7.0f;
    public float moveSpeed = 4.0f;
    public float threshold = 0.0f;
    private bool _isCrashed = false;
    private float duration = 4;
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
	}

    void OnCollisionEnter(Collision collision) {
        //Debug.Log("OnCollisionEnter");
        if (_isCrashed)
            return;

        RaycastHit hitInfo;
        int roadLayer = 9;
        if (Physics.Raycast(transform.position + Vector3.up, Vector3.down, out hitInfo, 5)) {
            if (hitInfo.collider.gameObject.tag == "road") {
                float dot = Vector3.Dot(transform.up, hitInfo.normal);
                //Debug.Log("OnCollisionEnter, dot:" + dot);
                //Debug.DrawLine(hitInfo.point, hitInfo.point + hitInfo.normal * 5, Color.red, 0.3f, false);
                if (dot < threshold) {
                    StartCoroutine(DoCrash());
                }

            }

        }

        /*
        foreach (ContactPoint contact in collision.contacts) {
            float dot = Vector3.Dot(transform.up, contact.normal);
            if (dot < threshold) {
                //_engine.isStarted = false;
                //Debug.Log("OnCrashed:" + dot);
                StartCoroutine(DoCrash());
            }
        }
        */
    }

    IEnumerator DoCrash() {
        _isCrashed = true;
        _soundEffect.Play();
        yield return new WaitForSeconds(duration);
        _isCrashed = false;
        Vector3 p = rigidbody.position;
        rigidbody.rotation = Quaternion.identity;
        rigidbody.velocity = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;
        rigidbody.MovePosition(p + Vector3.up);
    }
}
