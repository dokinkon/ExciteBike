using UnityEngine;
using System.Collections;
using System;

namespace GamePlay {

    public class LocationSprite : MonoBehaviour {

        public Transform anchorTopLeft;
        public Transform anchorTopRight;

        private float _normalizedLocation = 0.0f;
        public float normalizedLocation {
            set {
                _normalizedLocation = value;
                _normalizedLocation = Math.Min(Math.Max(0, _normalizedLocation), 1);

                Vector3 location = Vector3.zero;
                location.y = -70;
                
                float beginX = anchorTopLeft.localPosition.x;
                float endX   = anchorTopRight.localPosition.x;

                location.x = (endX - beginX) * _normalizedLocation;
                transform.localPosition = location;
                //Debug.Log("p:" + _normalizedLocation);
            }

            get { return _normalizedLocation; }
        }

        // Use this for initialization
        void Start () {
            normalizedLocation = 0;
        }
        
        // Update is called once per frame
        void Update () {
        
        }
    }
}
