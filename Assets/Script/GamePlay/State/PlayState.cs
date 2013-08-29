using UnityEngine;
using System.Collections;

namespace GamePlay.State {
	
	
	public class PlayState : FSMState {
		
		private ViewController _viewController;
        private float _lapTimer;
		
		public PlayState(ViewController viewController) {
			_stateName = "PlayState";
			_viewController = viewController;
			AddTransition(Transitions.Paused, "PauseState");
			AddTransition(Transitions.Finish, "FinishState");
		}
		
		public override void DoBeforeEntering() {
			_viewController.gamePlayPanel.SetActive(true);
			
			if ( Application.platform == RuntimePlatform.IPhonePlayer ) {
				_viewController.joystickPane.SetActive(true);
				_viewController.joystick.SetActive(true);
			} else {
				_viewController.joystickPane.SetActive(false);
				_viewController.joystick.SetActive(false);
			}
			_viewController.localBike.StartEngine();
            _viewController.isPlaying = true;
		}
		
		// Update is called once per frame
		public override void Update () {
			// Update Current Rank
			
			PlayerInfo localPlayer = GameManager.Instance.localPlayerInfo;
			Vector3 position = localPlayer.bike.gameObject.transform.position;
			int rank = 1;
			int count = GameManager.Instance.playersCount;
			for (int i=0;i<count;++i) {
				PlayerInfo playerInfo = GameManager.Instance.GetPlayerInfo(i);
				if (playerInfo.networkPlayer != localPlayer.networkPlayer) {
					Vector3 otherPosition = playerInfo.bike.transform.position;
					if ( otherPosition.z > position.z ) {
						rank++;
					}
				}
			}
			
			if (rank==1) {
				_viewController.rankLabel.text = "1st";
			} else if (rank == 2) {
				_viewController.rankLabel.text = "2nd";
			} else if (rank == 3) {
				_viewController.rankLabel.text = "3rd";
			} else {
				_viewController.rankLabel.text = rank.ToString() + "th";
			}
		}
		
		public override void DoBeforeLeaving() {
			_viewController.gamePlayPanel.SetActive(false);
			_viewController.joystick.SetActive(false);
			_viewController.joystickPane.SetActive(false);
			_viewController.localBike.StopEngine();
            _viewController.isPlaying = false;
		}
	}
	
}
