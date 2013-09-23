using UnityEngine;
using System.Collections;

namespace Item {

    public class Missile : MonoBehaviour {

        public void Hide() {
            renderer.enabled = false;
        }

        // Use this for initialization
        void Start () {
            animation.Play("MissileSpawn");
        }
        
        // Update is called once per frame
        void Update () {
        
        }

        void OnSpawned() {
            animation.Play("MissileRotation");
        }
    }
}

