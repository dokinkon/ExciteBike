using UnityEngine;
using System.Collections;

namespace GamePlay {

    public class LookAtTarget : MonoBehaviour {

        public Transform target;
        public float offset;

        // Use this for initialization
        void Start () {
        
        }
        
        // Update is called once per frame
        void Update () {
            if (target == null)
                return;

            Vector3 position = Vector3.zero;
            position.z = target.position.z + offset;
            transform.position = position;
        }
    }

}
