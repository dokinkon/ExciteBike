using UnityEngine;
using System.Collections;

namespace Lobby.State {

	public class CountDownState : FSMState {

        private ViewController _viewController;
        private float _countDownTimer = 3.0f;
	
		public CountDownState(ViewController viewController) {
			_stateName = "CountDownState";
            _viewController = viewController;
		}
		
		public override void DoBeforeEntering() {
            Debug.Log("[Lobby.State.CountDownState.DoBeforeEntering]");
            _viewController.StartCountDownAnimation();
		}
		
		// Update is called once per frame
		public override void Update () {
            _countDownTimer -= Time.deltaTime;
            if (_countDownTimer < 0) {
                _viewController.StopCountDownAnimation();
                if (Network.isServer) {
                    Server.Instance.LoadGamePlayLevel();
                }
            }
		}
		
		public override void DoBeforeLeaving() {
		}

	}
	
}
