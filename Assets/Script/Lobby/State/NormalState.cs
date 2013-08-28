using UnityEngine;
using System.Collections;

namespace Lobby.State {

	public class NormalState : FSMState {
	
		private ViewController _viewController;
		
		public NormalState(ViewController viewController) {
			_stateName = "NormalState";
			_viewController = viewController;
			AddTransition(Transistions.CountDown, "CountDownState");
			
		}
		
		public override void DoBeforeEntering() {
			Debug.Log ("Lobby.NormalState.DoBeforeEntering");
			GameManager.Instance.localPlayerInfo.status = PlayerInfo.Status.AtLobby;
		}
		
		// Update is called once per frame
		public override void Update () {
			if (Server.Instance.PollAllPlayerSyncForStatus(PlayerInfo.Status.AtLoddyReady)) {
				_viewController.fsm.PerformTransition(Transistions.CountDown);
			}
		}
		
		public override void DoBeforeLeaving() {
		}
	}
	
}
