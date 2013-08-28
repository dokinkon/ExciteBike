using UnityEngine;
using System.Collections;

namespace GamePlay.State {

	public class FinishState : FSMState {
		
		private ViewController _viewController; 
		
		public FinishState(ViewController viewController) {
			_stateName = "FinishState";
			_viewController = viewController;
		}
		
		public override void DoBeforeEntering() {
			Debug.Log("[FinishState.DoBeforeEntering]");
			_viewController.finishPanel.SetActive(true);
		}
		
	
		// Update is called once per frame
		public override void Update () {
		
		}
		
		public override void DoBeforeLeaving() {
			Debug.Log("[FinishState.DoBeforeLeaving]");
			_viewController.finishPanel.SetActive(false);
		}
	}
	
}