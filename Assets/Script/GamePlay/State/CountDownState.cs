using UnityEngine;
using System.Collections;

namespace GamePlay.State {

	public class CountDownState : FSMState {
		
		private ViewController _viewController;
		private int _currentSec = 3;
		private float _delayTimer;
		private float _localTimer;
		
		
		public CountDownState(ViewController viewController) {
			_viewController = viewController;
			_stateName = "CountDownState";
			AddTransition(Transitions.Playing, "PlayState");
			UpdateLabel();
		}
		
		// Use this for initialization
		void Start () {
	
		}
		
		void UpdateLabel() {
			if (_currentSec <=0 ) {
				_viewController.countDownLabel.text = "GO!!!";
			} else {
				_viewController.countDownLabel.text = _currentSec.ToString();
			}
		}
	
		// Update is called once per frame
		public override void Update () {
			_delayTimer += Time.deltaTime;
			if (_delayTimer > 1) {
				_currentSec--;
				UpdateLabel();
				_delayTimer = 0;
			}
			
			_localTimer += Time.deltaTime;
			if (_localTimer >= 4 ) {
				_viewController.fsm.PerformTransition(Transitions.Playing);
			}
		}
		
		public override void DoBeforeEntering() {
			_viewController.countDownPanel.SetActive(true);
		
		}
		
		public override void DoBeforeLeaving() {
			_viewController.countDownPanel.SetActive(false);
		}
	}
	
}
