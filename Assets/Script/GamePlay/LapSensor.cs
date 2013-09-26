using UnityEngine;
using System.Collections;

namespace GamePlay {

	public class LapSensor : MonoBehaviour {
		
        public delegate void IntDelegate(int i);
        public event IntDelegate OnLapChanged;
		private ViewController _viewController = null;
        
        private int _lap = 1;
        public int lap {
            get { return _lap; }
            set { _lap = value; }
        }
		
		// Use this for initialization
		void Start () {
			if (_viewController == null ) {
				GameObject clone = GameObject.FindGameObjectWithTag("view-controller");
				if (clone == null ) {
					Debug.LogError("[LapSensor.Start] can't find view-controller");
					return;
				}
				_viewController = clone.GetComponent<ViewController>();
			}
		}
		
		// Update is called once per frame
		void Update () {
		
		}
		
		//void OnTriggerEnter(Collider other ) {
			//Debug.Log ("[Finish.OnTriggerEnter] other:" + other.gameObject.name);
			//if (other.gameObject.tag == "Player") {
				//viewController.OnPlayerPassThrouthLap(other.gameObject);
			//}
		//}

        void OnTriggerExit(Collider collider) {
            //Debug.Log("[LapSensor.OnTriggerExit] 1 tag:" + collider.tag);
            if (!collider.tag.Contains("player-"))
                return;

            //Debug.Log("[LapSensor.OnTriggerExit] 2");
            Bike bike = collider.GetComponent<Bike>();
            if (!bike.isLocal) 
                return;

            //Debug.Log("[LapSensor.OnTriggerExit] 3");
            if (bike.transform.position.z > transform.position.z) {
                    
                //Debug.Log("[LapSensor.OnTriggerExit] 4");
                if (_lap > bike.lap) {
                    bike.lap = _lap;
                    _viewController.OnLapChanged(bike.lap);
                }
            } else {
                //Debug.Log("[LapSensor.OnTriggerExit] 5");
                bike.lap = _lap - 1;
                _viewController.OnLapChanged(bike.lap);
            }
        }
	}
}

















