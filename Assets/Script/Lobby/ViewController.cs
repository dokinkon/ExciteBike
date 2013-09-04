﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Lobby {

	public class ViewController : MonoBehaviour {
		
		public GameObject readyButton;
		public GameObject backButton;
		public GameObject selectTrackButton;
		public GameObject commonPanel;
		public GameObject selectTrackPanel;
		public GameObject selectTrackOKButton;
		public UILabel hostTitleLabel;
        public UILabel countDownLabel;
		public GameObject playerItemPrefab;
		public UITable playerTable;
		
		private List<GameObject> _playerItems = new List<GameObject>();
		
		private FSMSystem _fsm = new FSMSystem();
		public FSMSystem fsm {
			get { return _fsm; }
		}
		
		void Awake() {
			Debug.Log("[Lobby.ViewController.Awake]");
			_fsm.AddState(new State.NormalState(this));
			_fsm.AddState(new State.CountDownState(this));
		}
		
		// Use this for initialization
		void Start () {
			Debug.Log("[Lobby.ViewController.Start]");
			GameManager.OnViewControllerStarted();
			
			selectTrackPanel.SetActive(false);
			
			if (!Network.isClient) {
				selectTrackButton.SetActive(true);
				UIEventListener.Get(selectTrackButton).onClick = OnSelectTrackButtonPressed;
				UIEventListener.Get(selectTrackOKButton).onClick = OnSelectTrackOKButtonPressed;
			} else {
				selectTrackButton.SetActive(false);
			}

            UIEventListener.Get(readyButton).onClick = OnReadyButtonPressed;
			UIEventListener.Get(backButton).onClick = OnBackButtonPressed;
			
			//Client.Instance.EnterLobby();
			
			if (Network.isServer) {
				hostTitleLabel.text = Server.title;
			} else {
				hostTitleLabel.text = Client.hostData.gameName;
			}
			
			
			int count = GameManager.Instance.playersCount;
			for (int i=0;i<count;i++) {
				PlayerInfo playerInfo = GameManager.Instance.GetPlayerInfo(i);
				CreatePlayerItem(playerInfo);
			}
			
			GameManager.Instance.OnPlayerAdded += CreatePlayerItem;
			GameManager.Instance.OnPlayerRemoved += DestroyPlayerItem;
			_fsm.Start ();
		}
		
		void OnEnable() {
			Debug.Log("[Lobby.ViewController.OnEnable]");
            Client.Instance.OnAllPlayersReadyInLobby += OnAllPlayersReadyInLobby;
			
		}
		
		void OnDisable() {
			Debug.Log("[Lobby.ViewController.OnDisable]");
            if (!GameManager.isShutingDown) {
                Client.Instance.OnAllPlayersReadyInLobby -= OnAllPlayersReadyInLobby;
                _fsm.Stop ();
            }
		}

		// Update is called once per frame
		void Update () {
            _fsm.Update();
		}
		
		void OnReadyButtonPressed(GameObject button) {
			Debug.Log("GameLobbyViewController.OnReadyButtonPressed");
			PlayerInfo playerInfo = GameManager.Instance.GetPlayerInfo(Network.player);
			if (playerInfo!=null) {
				playerInfo.status = PlayerInfo.Status.AtLoddyReady;
			}
		}
		
		// for server
		//void OnStartGameButtonPressed(GameObject button) {
			//Server.Instance.LoadGamePlayLevel();
		//}
		
		void OnBackButtonPressed(GameObject button) {
			if (Network.isClient) {
				Network.Disconnect();
				Application.LoadLevel("GameListScene");
			} else if (Network.isServer) {
				Network.Disconnect();
				Application.LoadLevel("HostSettingScene");
			} else {
				
			}
		}

        void OnDisconnectedFromServer(NetworkDisconnection info) {
            Application.LoadLevel("GameListScene");
        }
		
		void OnSelectTrackButtonPressed(GameObject button) {
			commonPanel.SetActive(false);
			selectTrackPanel.SetActive(true);
		}
		
		void OnSelectTrackOKButtonPressed(GameObject button) {
			commonPanel.SetActive(true);
			selectTrackPanel.SetActive(false);
		}
		
		void CreatePlayerItem(PlayerInfo playerInfo) {
			Debug.Log ("GameLobbyViewController.CreatePlayerItem");
			GameObject playerItemGo = (GameObject)Instantiate(playerItemPrefab, Vector3.zero, Quaternion.identity);
			playerItemGo.transform.parent = playerTable.transform;
			playerItemGo.transform.localPosition = Vector3.zero;
			playerItemGo.transform.localRotation = Quaternion.identity;
			playerItemGo.transform.localScale = Vector3.one;
			playerTable.Reposition();
			
			PlayerItem playerItem = playerItemGo.GetComponent<PlayerItem>();
			playerItem.playerInfo = playerInfo;
			_playerItems.Add (playerItemGo);
		}
		
		void DestroyPlayerItem(PlayerInfo playerInfo) {
			Debug.Log ("GameLobbyViewController.DestroyPlayerItem");
			
			foreach (GameObject playerItemGo in _playerItems) {
				PlayerItem playerItem = playerItemGo.GetComponent<PlayerItem>();
				if (playerItem.playerInfo.networkPlayer == playerInfo.networkPlayer) {
					GameObject.Destroy(playerItemGo);
					_playerItems.Remove(playerItemGo);
					return;
				}
			}	
		}

        private static bool _isShutingDown = false;
        void OnApplicationQuit() {
            Debug.Log("[GameLobby.OnApplicationQuit]");
            _isShutingDown = true;
        }
		
		void OnDestroy() {
			Debug.Log("[GameLobbyViewController.OnDestroy] remove event delegates");
            if (!_isShutingDown) {
                GameManager.Instance.OnPlayerAdded -= CreatePlayerItem;
                GameManager.Instance.OnPlayerRemoved -= DestroyPlayerItem;
            }
		}

        void OnSelectedBikeChanged(string bikeName) {
            GameManager.Instance.localPlayerInfo.bikeName = bikeName;
        }

        public void StartCountDownAnimation() {
            StartCoroutine(AnimateCountDownLabel());
        }

        public void StopCountDownAnimation() {
            StopCoroutine("AnimateCountDownLabel");
        }

		IEnumerator AnimateCountDownLabel() {
            int countDownSecond = 3;
			while (countDownSecond > 0) {
				countDownLabel.text = countDownSecond.ToString();
                countDownSecond--;
				yield return new WaitForSeconds(1);
			}
		}

        void OnAllPlayersReadyInLobby() {
            _fsm.PerformTransition(State.Transistions.CountDown);
        }
	}

}
