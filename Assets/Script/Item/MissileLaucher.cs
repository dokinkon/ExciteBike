using UnityEngine;
using System.Collections;

namespace Item {

    public class MissileLaucher : MonoBehaviour {

        // Use this for initialization
        void Start () {
        
        }
        
        // Update is called once per frame
        void Update () {
        
        }

        public void Launch() {
            GameObject clone = (GameObject)Instantiate(Resources.Load("HomingMissile"));
            clone.transform.position = transform.position;
            

        }
    }
}
