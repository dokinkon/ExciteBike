using UnityEngine;
using System.Collections;

namespace GamePlay.State {

	public class CountDownState : FSMState {
		
		private ViewController _viewController;
		private int _currentSec = 3;
		private float _delayTimer;
		private float _localTimer;

        private IOSController _iosController;
		
		
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
                TweenScale ts = _viewController.countDownLabel.gameObject.GetComponent<TweenScale>();
                ts.Play(true);
                //_viewController.countDownLabel.transform.localScale = new Vector3(30, 30, 1);
                //TweenScale.Begin(_viewController.countDownLabel.gameObject, 0.8f, new Vector3(80.2f, 80.2f, 1.2f));
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
            Time.timeScale = 1;
            Debug.Log("[GamePlay.State.CountDownState.DoBeforeEntering]");
			_viewController.countDownPanel.SetActive(true);
            _viewController.localBike.engine.isStarted = true;
            _viewController.localBike.engine.gearPosition = 0;
            GameObject go = GameObject.Find("IOSController");
            if (go != null) {
                _iosController = go.GetComponent<IOSController>();
                if (_iosController!= null) {
                    _iosController.StartMeasureAcceleration();
                }
            }
		}
		
		public override void DoBeforeLeaving() {
            _viewController.LetterBoxOut();
			_viewController.countDownPanel.SetActive(false);
            if (_iosController!=null) {
                _iosController.StopMeasureAcceleration();
            }

            for (int i=0;i<4;i++) {
                string tag = "player-" + i;
                GameObject ob = GameObject.FindGameObjectWithTag(tag);
                if (ob!=null) {
                    ob.SendMessage("OnRaceStarted");
                }
            }
		}
	}
	
}
