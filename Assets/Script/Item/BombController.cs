using UnityEngine;
using System.Collections;

namespace Item {
    public class BombController : MonoBehaviour {

        public Vector3 initVelocity;
        public ParticleSystem explosionEffect;
        private Renderer _renderer;

        // Use this for initialization
        void Start () {
            _renderer = GetComponentInChildren<Renderer>();
            rigidbody.velocity = initVelocity;
        }
        
        // Update is called once per frame
        void Update () {
        
        }

        IEnumerator Explosion() {
            _renderer.enabled = false;
            explosionEffect.Play();
            audio.Play();
            yield return new WaitForSeconds(1);
            if (networkView.isMine) {
                Network.Destroy(gameObject);
            }
        }

        void OnTriggerEnter(Collider collider) {
            if (collider.tag == "road" || collider.tag.Contains("player")) {
                StartCoroutine(Explosion());
            }
        }
    }
}

