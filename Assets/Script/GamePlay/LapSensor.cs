using UnityEngine;
using System.Collections;

namespace GamePlay {

	public class LapSensor : MonoBehaviour {
		
		public ViewController viewController;
		
		// Use this for initialization
		void Start () {
			if (viewController == null ) {
				GameObject go = GameObject.FindGameObjectWithTag("view-controller");
				if (go == null ) {
					Debug.LogError("[GamePlay.Finish] can't find view-controller");
					return;
				}
				
				viewController = go.GetComponent<ViewController>();
			}
		}
		
		// Update is called once per frame
		void Update () {
		
		}
		
		void OnTriggerEnter(Collider other ) {
			Debug.Log ("[Finish.OnTriggerEnter] other:" + other.gameObject.name);
			if (other.gameObject.tag == "Player") {
				viewController.OnPlayerPassThrouthLap(other.gameObject);
			}
		}
	}
}
