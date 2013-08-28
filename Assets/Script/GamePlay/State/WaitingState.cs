using UnityEngine;
using System.Collections;

namespace GamePlay.State {

	public class WaitingState : FSMState {
		private ViewController _viewController;
		private bool _allReady = false;
		
		public WaitingState(ViewController viewController) {
			_stateName = "WaitingState";
			_viewController = viewController;
			AddTransition(State.Transitions.CountDown, "CountDownState");
		}
		
		// Use this for initialization
		void Start () {
		
		}
		
		public override void DoBeforeEntering () {
			_viewController.waitingPanel.SetActive(true);
			_viewController.gamePlayPanel.SetActive(false);
			GameManager.Instance.localPlayerInfo.status = PlayerInfo.Status.PlayingReady;
		}
				
		// Update is called once per frame
		public override void Update () {
			
			if (!Network.isServer)
				return;
			
			// TODO add a timeout
			if (!_allReady) {
				_allReady = Server.Instance.PollAllPlayersInReadyState();
			}
		}
				
		public override void DoBeforeLeaving() {
			_viewController.waitingPanel.SetActive(false);
		}
		
		
	}
		
} // namespace GamePlay.State
