using UnityEngine;
using System.Collections;
using System;

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
			
			//_viewController.localBike.StartEngine();
            _viewController.localBike.engine.volume = 1.0f;
            _viewController.localBike.engine.gearPosition = 1;
            _viewController.StartBikes();
            _viewController.isPlaying = true;
		}
		
		// Update is called once per frame
		public override void Update () {
			// Update Current Rank
			
            float beginZ = _viewController.trackStartLocation;
            float dist = _viewController.trackTotalDistance;

			PlayerInfo localPlayer = GameManager.Instance.localPlayerInfo;
			Vector3 position = localPlayer.bike.gameObject.transform.position;
			int rank = 1;
			int count = GameManager.Instance.playersCount;

            PlayerInfo[] playerInfos = GameManager.Instance.GetPlayerInfos();
            Array.Sort( playerInfos, delegate(PlayerInfo info1, PlayerInfo info2) {
                return info2.bike.transform.position.z.CompareTo(info1.bike.transform.position.z);
            });

            for (int rp=0;rp < playerInfos.Length; ++rp) {
                _viewController.SetRacePosition( playerInfos[rp], rp );
            }

			for (int i=0;i<count;++i) {
				PlayerInfo playerInfo = GameManager.Instance.GetPlayerInfo(i);

                LocationSprite locationSprite = _viewController.GetLocationSprite(i);
                if (locationSprite!= null) {
                    locationSprite.normalizedLocation = (playerInfo.bike.gameObject.transform.position.z - beginZ) / dist;
                }

				if (playerInfo.networkPlayer != localPlayer.networkPlayer) {
					Vector3 otherPosition = playerInfo.bike.transform.position;
					if ( otherPosition.z > position.z ) {
						rank++;
					}
				}
			}

            /*
			if (rank==1) {
				_viewController.rankLabel.text = "1st";
			} else if (rank == 2) {
				_viewController.rankLabel.text = "2nd";
			} else if (rank == 3) {
				_viewController.rankLabel.text = "3rd";
			} else {
				_viewController.rankLabel.text = rank.ToString() + "th";
			}
            */

		}
		
		public override void DoBeforeLeaving() {
			_viewController.gamePlayPanel.SetActive(false);
            _viewController.localBike.engine.gearPosition = 0;
            _viewController.isPlaying = false;
		}
	}
}
