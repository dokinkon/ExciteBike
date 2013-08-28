using UnityEngine;
using System.Collections;

namespace Lobby.State {

	public class CountDownState : FSMState {
	
		
		public CountDownState() {
			_stateName = "CountDownState";
		}
		
		public override void DoBeforeEntering() {
		}
		
		// Update is called once per frame
		public override void Update () {
		
		}
		
		public override void DoBeforeLeaving() {
		}
	}
	
}
