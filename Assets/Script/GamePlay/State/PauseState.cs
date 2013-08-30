using UnityEngine;
using System.Collections;

namespace GamePlay.State {

	public class PauseState : FSMState {
	
		private ViewController _viewController;
		
		public PauseState(ViewController viewController) {
			_viewController = viewController;
			_stateName = "PauseState";
			AddTransition(Transitions.Playing, "PlayState");
            AddTransition(Transitions.CountDown, "CountDownState");
		}
		
		public override void DoBeforeEntering() {
			_viewController.pauseMenuPanel.SetActive(true);
            _viewController.localBike.engine.volume = 0;
			
			Time.timeScale = 0;
		}
		
		// Update is called once per frame
		public override void Update () {
		
		}
		
		public override void DoBeforeLeaving() {
			if (_viewController.pauseMenuPanel) {
				_viewController.pauseMenuPanel.SetActive(false);
			}
			Time.timeScale = 1;
		}
	}
}
