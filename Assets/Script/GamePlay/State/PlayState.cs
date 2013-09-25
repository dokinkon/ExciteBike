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
			
            _viewController.localBike.engine.volume = 1.0f;
            _viewController.localBike.engine.gearPosition = 1;
            //_viewController.StartBikes();
            _viewController.isPlaying = true;
		}
		
		// Update is called once per frame
		public override void Update () {
			// Update Current Rank
            Bike[] bikes = _viewController.GetBikes();
            Array.Sort( bikes, delegate(Bike bike1, Bike bike2) {
                return bike2.transform.position.z.CompareTo(bike1.transform.position.z);
            });

            for (int racePosition = 0; racePosition < bikes.Length; ++racePosition) {
                _viewController.SetRacePosition(bikes[racePosition], racePosition);
            }

            float beginZ = _viewController.trackStartLocation;
            float dist = _viewController.trackTotalDistance;
            foreach (Bike bike in bikes) {
                LocationSprite locationSprite = _viewController.GetLocationSprite(bike.playerIndex);
                if (locationSprite!= null) {
                    locationSprite.normalizedLocation = (bike.transform.position.z - beginZ) / dist;
                }
            }
		}
		
		public override void DoBeforeLeaving() {
			_viewController.gamePlayPanel.SetActive(false);
            _viewController.localBike.engine.gearPosition = 0;
            _viewController.isPlaying = false;
		}
	}
}
