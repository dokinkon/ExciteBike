﻿using UnityEngine;
using System.Collections;

namespace GamePlay {

	public class ViewController : MonoBehaviour {
	
		public GameObject gamePlayPanel;
		public GameObject pauseMenuPanel;
		public GameObject pauseButton;
        public GameObject restartButton;
		public GameObject resumeButton;
		public GameObject exitButton;
		public GameObject joystickPane;
		public GameObject joystick;
		public GameObject waitingPanel;
		public GameObject finishPanel;
		public GameObject backToLobbyButton;
		
		public UILabel countDownLabel;
		public GameObject countDownPanel;
		
		public UILabel totalTimeLabel;
		public UILabel lapTimeLabel;
		public UILabel bestTimeLabel;
		
		public UILabel rankLabel;

        private Vector3 _cameraInitPosition;
		
		private Bike _localBike = null;
		public Bike localBike {
			get { return _localBike; }
		}
			
		
		FSMSystem _fsm = new FSMSystem();
		public FSMSystem fsm {
			get { return _fsm; }
		}
		
		private bool _shouldEnterFinishState = false;
		private int _lapCount = 0;
		
	
		// Use this for initialization
		void Start () {
			Debug.Log ("[GamePlayViewController.Start]");
			
			// Distable UI Panels.
			waitingPanel.SetActive(false);
			pauseMenuPanel.SetActive(false);
			gamePlayPanel.SetActive(false);
			finishPanel.SetActive(false);
            
            _cameraInitPosition = Camera.main.transform.position;
			
			// Init FSM
			_fsm.AddState(new State.WaitingState(this));
			_fsm.AddState(new State.CountDownState(this));
			_fsm.AddState(new State.PlayState(this));
			_fsm.AddState(new State.PauseState(this));
			_fsm.AddState(new State.FinishState(this));
				
				
			GameManager.OnViewControllerStarted();
			UIEventListener.Get(pauseButton).onClick = OnPauseButtonPressed;
			UIEventListener.Get(resumeButton).onClick = OnResumeButtonPressed;
			UIEventListener.Get(restartButton).onClick = OnRestartButtonPressed;
			UIEventListener.Get(exitButton).onClick = OnExitButtonPressed;
			UIEventListener.Get(backToLobbyButton).onClick = OnExitButtonPressed;
			
			_localBike = GameManager.Instance.SpawnBike(GameManager.Instance.localPlayerInfo.trackIndex);
			Client.Instance.OnGamePlayReadyStart += OnPlayerReadyStart;
			
			
			_fsm.Start();
		}
		
		// Update is called once per frame
		void Update () {
			if (_shouldEnterFinishState) {
				_shouldEnterFinishState = false;
				_fsm.PerformTransition(State.Transitions.Finish);
			}
			_fsm.Update();
		}
		
		void OnPlayerReadyStart(float deltaTime) {
			_fsm.PerformTransition(State.Transitions.CountDown);
		}
		
		void OnPauseButtonPressed(GameObject button) {
			_fsm.PerformTransition(State.Transitions.Paused);
		}
		
		void OnResumeButtonPressed(GameObject button) {
			_fsm.PerformTransition(State.Transitions.Playing);
		}

        void OnRestartButtonPressed(GameObject button) {
            Camera.main.transform.position = _cameraInitPosition;
            GameObject spawnGo = GameObject.FindGameObjectWithTag("init_spawn_0");
            _localBike.gameObject.transform.position = spawnGo.transform.position;
            _localBike.rigidbody.velocity = Vector3.zero;
            _localBike.rigidbody.angularVelocity = Vector3.zero;
            _fsm.PerformTransition(State.Transitions.CountDown);
        }
		
		void OnExitButtonPressed(GameObject button) {
			//GameManager.Instance.SetPaused(false);
			if (Network.isServer) {
				Server.Instance.LoadLevel("GameLobbyScene");
			} else if (Network.isClient) {
				Network.Destroy(_localBike.gameObject);
				GameManager.Instance.localPlayerInfo.status = PlayerInfo.Status.Loading;
				Application.LoadLevel("GameLobbyScene");
			}
			//Application.LoadLevel("GameLobbyScene");
		}

        void OnDisable() {
			Debug.Log ("[GamePlay.ViewController.OnDisable]");
			_fsm.Stop();
			Client.Instance.OnGamePlayReadyStart -= OnPlayerReadyStart;
        }

		
		void OnDestroy() {
			Debug.Log ("[GamePlay.ViewController.OnDestroy]");
		}
		
		public void OnPlayerPassThrouthLap(GameObject bikeGo) {
			if (bikeGo.networkView.owner == Network.player) {
				Debug.Log("[GamePlay.ViewController.OnPlayerFinished]");
				_lapCount++;
				if ( _lapCount >= GameManager.Instance.totalLaps ) {
					_shouldEnterFinishState = true;
				}
			}
		}
	}

}
