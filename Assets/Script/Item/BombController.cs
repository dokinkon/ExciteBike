using UnityEngine;
using System.Collections;

namespace Item {
    public class BombController : MonoBehaviour {

        public Vector3 initVelocity;
        public ParticleSystem explosionEffect;
        private Renderer _renderer;
        private bool _isExplosion = false;

        // Use this for initialization
        void Start () {
            _renderer = GetComponentInChildren<Renderer>();
            rigidbody.velocity = initVelocity;
            if (networkView.isMine)
                StartCoroutine(ExplosionTicker());
        }
        
        // Update is called once per frame
        void Update () {
        
        }

        IEnumerator ExplosionTicker() {
            yield return new WaitForSeconds(10);
            StartExplosion();
        }

        IEnumerator ExplosionCoroutine() {
            yield return new WaitForSeconds(1);
            if (Network.isServer || Network.isClient) {
                if (networkView.isMine) {
                    Network.Destroy(gameObject);
                }
            } else {
                Destroy(gameObject);
            }
        }

        void StartExplosion() {
            if (_isExplosion)
                return;

            _isExplosion = true;
            networkView.RPC("RPCExplorsionStarted", RPCMode.All);
            StartCoroutine(ExplosionCoroutine());
        }

        void OnTriggerEnter(Collider collider) {
            if (!networkView.isMine)
                return;

            if (_isExplosion)
                return;

            if (collider.tag == "road" || collider.tag.Contains("player")) {
                StartExplosion();
            }
        }

        [RPC]
        void RPCExplorsionStarted() {
            _renderer.enabled = false;
            explosionEffect.Play();
            audio.Play();
        }
    }
}

