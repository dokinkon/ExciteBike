using UnityEngine;
using System.Collections;

namespace Item {
    [RequireComponent(typeof(NetworkView))]
    public class Spike : MonoBehaviour {

        public float moveSpeed = 1.0f;

        private bool _foundPlace = false;
        private bool _arrived = false;
        private Vector3 _placeLocation = Vector3.zero;
        private Vector3 _placeNormal = Vector3.up;
        

        // Use this for initialization
        void Start () {
            collider.enabled = false;
            FoundPlace();
        }

        // Update is called once per frame
        void Update () {
            if (!_foundPlace) {
                FoundPlace();
            } else {
                if (!_arrived) {
                    Vector3 offset = _placeLocation - transform.position;
                    Vector3 step = offset.normalized * moveSpeed * Time.deltaTime;
                    if ( offset.magnitude < step.magnitude ) {
                        transform.position = _placeLocation;
                        _arrived = true;
                        collider.enabled = true;
                    } else {
                        Vector3 position = transform.position;
                        position += step;
                        transform.position = position;
                    }
                }
            }
        }

        private void FoundPlace() {
            RaycastHit hitInfo;
            if ( Physics.Raycast( transform.position, Vector3.down, out hitInfo, 10 ) ) {
                if (hitInfo.collider.tag == "road") {
                    _placeLocation = hitInfo.point;
                    _placeNormal = hitInfo.normal.normalized;
                    _foundPlace = true;
                }
            }
        }

        private void Destruct() {
            if (Network.isServer || Network.isClient) {
                if (networkView.isMine) {
                    Network.Destroy(gameObject);
                }
            } else {
                Destroy(gameObject);
            }
        }

        void OnTriggerEnter(Collider collider) {
            if (collider.tag.Contains("player")) {
                Destruct();
            }
        }
    }
}
