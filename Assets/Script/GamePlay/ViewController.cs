using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace GamePlay {

	public class ViewController : MonoBehaviour {
	
        // panels
		public GameObject gamePlayPanel;
		public GameObject pauseMenuPanel;
		public GameObject waitingPanel;
		public GameObject finishPanel;
		public GameObject countDownPanel;
        public GameObject debugPanel;

        // buttons
		public GameObject pauseButton;
        public GameObject restartButton;
		public GameObject resumeButton;
		public GameObject exitButton;
		public GameObject backToLobbyButton;

        // labels
		public UILabel countDownLabel;
		public UILabel totalTimeLabel;
		public UILabel lapTimeLabel;
		public UILabel bestTimeLabel;
        public UILabel speedLabel;
        public UILabel lapLabel;
		public UILabel rankLabel;

        public UILabel[] racePositionLabels;

        public LocationSprite[] locationSprites;

        public Texture[] userProfilePictures;

        public SmoothFollow smoothFollow;

        private Vector3 _cameraInitPosition;
		
		private Bike _localBike = null;
		public Bike localBike {
			get { return _localBike; }
		}

        private List<Bike> _npcBikes = new List<Bike>();

        private bool _isPlaying = false;
        public bool isPlaying {
            set { _isPlaying = value; }
            get { return _isPlaying; }
        }
        
        private float _lapTimer = 0.0f;
        private float _bestLapTime = 100000.0f;
        private float _totalTimer = 0;
			
		FSMSystem _fsm = new FSMSystem();
		public FSMSystem fsm {
			get { return _fsm; }
		}
		
		private bool _shouldEnterFinishState = false;
		private int _lapCount = 1;

        private float _trackTotalDistance;
        public float trackTotalDistance {
            get { return _trackTotalDistance; }
        }

        private float _trackStartLocation = -110000.0f;
        public float trackStartLocation {
            get { 
                if (_trackStartLocation < -10000.0f) {
                    GameObject spawn = GameObject.FindGameObjectWithTag("init_spawn_0");
                    _trackStartLocation = spawn.transform.position.z;
                }
                return _trackStartLocation;
            }
        }
		
		// Use this for initialization
		void Start () {
			Debug.Log ("[GamePlayViewController.Start]");
			
			// Distable UI Panels.
			waitingPanel.SetActive(false);
			pauseMenuPanel.SetActive(false);
			gamePlayPanel.SetActive(false);
			finishPanel.SetActive(false);
            UpdateLapText();
            
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

            PlayerInfo playerInfo = GameManager.Instance.localPlayerInfo;
			
			_localBike = GameManager.Instance.SpawnBike(playerInfo.bikeName, playerInfo.trackIndex);
            _localBike.name = "Bike-Local";
            _localBike.isLocal = true;
            SpawnNPCBikes(); // TEST

			Client.Instance.OnGamePlayReadyStart += OnPlayerReadyStart;
            _trackTotalDistance = CalculateTrackDistance();
            //Debug.Log("TrackDistance:" + _trackTotalDistance);


            if ( Application.platform == RuntimePlatform.IPhonePlayer ) {
                GameObject go = GameObject.Find("IOSController");
                if (go!=null) {
                    IOSController controller = go.GetComponent<IOSController>();
                    controller.SetBike(_localBike);
                }
            } else {
                GameObject keyboardControllerGo = GameObject.Find("KeyboardController");
                if (keyboardControllerGo != null) {
                    KeyboardController keyboardController = keyboardControllerGo.GetComponent<KeyboardController>();
                    keyboardController.SetBike(_localBike);
                }
            }

			_fsm.Start();
		}

        private void SpawnNPCBikes() {
            Debug.Log("[GamePlayViewController.SpawnNPCBike]");
            for (int i=1;i<4;i++) {
                Bike bike = GameManager.Instance.SpawnBike("Bike", i);
                bike.gameObject.AddComponent<NPCController>();
                bike.name = "Bike-NPC-" + i;
                bike.isNPC = true;
                NPCController controller = bike.GetComponent<NPCController>();
                controller.localBike = _localBike;
            }
        }

        public void StartBikes() {
            if ( Network.isServer) {
                //foreach ( Bike bike in _npcBikes ) {
                    //bike.StartEngine();
                    //bike.engine.volume = 0.8f;
                    //bike.engine.GearPosition = 1;
                //}
            }
        }
		
		// Update is called once per frame
		void Update () {
			if (_shouldEnterFinishState) {
				_shouldEnterFinishState = false;
				_fsm.PerformTransition(State.Transitions.Finish);
			}
			_fsm.Update();
            float speed = _localBike.rigidbody.velocity.z;
            speedLabel.text = speed.ToString();

            if (_isPlaying) {
                _lapTimer += Time.deltaTime;
                _totalTimer += Time.deltaTime;
                UpdateTimerText("LAP", _lapTimer, lapTimeLabel);
                UpdateTimerText("TOTAL", _totalTimer, totalTimeLabel);
            }
		}

        void UpdateTimerText(string prefix, float t, UILabel label) {

            int sec = (int)t;
            int min = (int)(sec / 60);
            if (min > 99 ) {
                min = 99;
            }
            int msec = (int)((t - sec)*100);
            sec = sec % 60;
            
            label.text = System.String.Format("{3} {0:00}:{1:00}:{2:00}", min, sec, msec, prefix);
        }
		
		void OnPlayerReadyStart(float deltaTime) {
            Debug.Log("[GamePlay.ViewController.OnPlayerReadyStart]");
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
			Time.timeScale = 1;
            Network.RemoveRPCs( _localBike.networkView.viewID );
			if (Network.isServer) {
				Server.Instance.LoadLevel("GameLobbyScene");
			} else if (Network.isClient) {
				Network.Destroy(_localBike.gameObject);
				GameManager.Instance.localPlayerInfo.status = PlayerInfo.Status.Loading;
				Application.LoadLevel("GameLobbyScene");
			}
		}

        void OnDebugButtonPressed(GameObject button) {

            if (pauseMenuPanel!=null) {
                pauseMenuPanel.SetActive(false);
            }

            if (debugPanel!=null) {
                debugPanel.SetActive(true);
            }
        }

        private static bool _isShutingDown = false;
        void OnApplicationQuit() {
            Debug.Log("[GameLobby.OnApplicationQuit]");
            _isShutingDown = true;
        }

        void OnDisable() {
			Debug.Log ("[GamePlay.ViewController.OnDisable]");
			//_fsm.Stop();
            if (!_isShutingDown) {
                Client.Instance.OnGamePlayReadyStart -= OnPlayerReadyStart;
            }
        }

		
		void OnDestroy() {
			Debug.Log ("[GamePlay.ViewController.OnDestroy]");
		}
		
		public void OnPlayerPassThrouthLap(GameObject bikeGo) {
			if (bikeGo.networkView.owner == Network.player) {
				Debug.Log("[GamePlay.ViewController.OnPlayerFinished]");
				_lapCount++;

                if (_lapTimer < _bestLapTime) {
                    _bestLapTime = _lapTimer;
                }
                UpdateTimerText("BEST", _bestLapTime, bestTimeLabel);
                _lapTimer = 0;

                UpdateLapText();
				if ( _lapCount > GameManager.Instance.totalLaps ) {
					_shouldEnterFinishState = true;
				}
			}
		}

        void UpdateLapText() {
            if (lapLabel!=null) {
                lapLabel.text = "LAP:" + _lapCount.ToString();
            }
        }

        void OnFollowDistanceChanged(float v) {
            Debug.Log("OnFollowDistanceChanged:" + v);
            // 0:20, 1:80
            float dist = v * 60.0f + 20.0f;
            smoothFollow.distance = dist;
        }

        void OnFollowHeightChanged(float h) {
            float height = h * 50.0f + 10.0f;
            smoothFollow.height = height;
        }

        void OnFOVChanged(float v) {
            Camera.main.fieldOfView = v * 50.0f + 10.0f;
        }

        void OnLookAtOffsetChanged(float v) {
            if (_localBike != null) {
                _localBike.lookAtOffset = 5.0f + v * 8.0f;
            }
        }

        void OnDebugOKButtonPressed(GameObject button) {
            if (debugPanel!=null) {
                debugPanel.SetActive(false);
            }

            if (pauseMenuPanel!=null) {
                pauseMenuPanel.SetActive(true);
            }
        }


        void OnMusicEnableChanged(bool enable) {
            //bool prevValue = PlayerPrefs.GetInt();
            GameObject go = GameObject.Find("Main Camera");
            AudioSource audioSource = go.GetComponent<AudioSource>();
            audioSource.enabled = enable;
        }

        private float CalculateTrackDistance() {
            GameObject spawn = GameObject.FindGameObjectWithTag("init_spawn_0");
            GameObject[] lapSensors = GameObject.FindGameObjectsWithTag("lap");
            float totalDistance = -1000.0f;
            foreach (GameObject lapSensor in lapSensors) {
                float distance = lapSensor.transform.position.z - spawn.transform.position.z;
                if (distance > totalDistance) {
                    totalDistance = distance;
                }
            }
            return totalDistance;
        }

        public LocationSprite GetLocationSprite(int index) {
            if (index < 0 || index > 3)
                return null;

            return locationSprites[index];
        }

        public void SetRacePosition(PlayerInfo playerInfo, int racePosition) {
            racePositionLabels[racePosition].text = playerInfo.playerName;
        }

	}
}
