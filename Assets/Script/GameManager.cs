using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class GameManager : MonoSingleton< GameManager > {
	
	public delegate void PlayerInfoDelegate(PlayerInfo playerInfo);
	public PlayerInfoDelegate OnPlayerAdded;
	public PlayerInfoDelegate OnPlayerRemoved;
	
	private FacebookUserInfo _facebookUserInfo;
	public FacebookUserInfo facebookUserInfo {
		get { return _facebookUserInfo; }
		set { 
			_facebookUserInfo = value;
			playerName = _facebookUserInfo.userName;
		}
	}
	
	private string _trackName = "Track03";
	public string trackName {
		get { return _trackName; }
		set { _trackName = value; }
	}
	
	private int _totalLaps = 1;
	public int totalLaps {
		get { return _totalLaps; }
		set { _totalLaps = value; }
	}
	
	public static string playerName {
		get {
			string n = PlayerPrefs.GetString("Player.Name");
			if ( n == "" ) {
				n = "Player_" + GameManager.RandomString(3);
			}
			return PlayerPrefs.GetString("Player.Name");
		}
		set { PlayerPrefs.SetString("Player.Name", value); }
	}
	
	public static  int majorVersion {
		get { return 0; }
	}
	
	public static int minorVersion {
		get { return 0; }
	}
	
	public static int microVersion {
		get { return 1; }
	}
	
	public static string version {
		get {
			return System.String.Format("{0}.{1}.{2}", majorVersion, minorVersion, microVersion);
		}
	}
	
	public static string gameName {
		get {
			return System.String.Format("ExciteBike_{0}", version);
		}
	}
	
	
	
	private List<PlayerInfo> _playerInfos = new List<PlayerInfo>();
	public int playersCount {
		get {
			return _playerInfos.Count;
		}
	}
	
	private PlayerInfo _localPlayerInfo;
	public PlayerInfo localPlayerInfo {
		get {
			return _localPlayerInfo;
		}
	}
	
	//private Dictionary<NetworkPlayer, bool> _levelGamePlayLoadedDict;
	//private Dictionary<NetworkPlayer, bool> _playerReadyDict;
	private Dictionary<NetworkPlayer, Bike> _bikes;
	private Bike _localBike;
	private bool _gamePlayStarted;
	private int _currentRank;
	private bool _isPaused;
	private FSMSystem _fsm;
	
	public override void Init() {
		Debug.Log("[GameManager] Init");
		_fsm = new FSMSystem();
		GameStateHostSetting hostSettingState = new GameStateHostSetting();
		GameStateLobby lobbyState = new GameStateLobby();
		_fsm.AddState(hostSettingState);
		_fsm.AddState(lobbyState);
	}
	
	public static void OnViewControllerStarted() {
		Create();
		CreateNetworkNode();
	}
	
	void Awake() {
		Debug.Log ("[GameManager.Awake]");
		if ( Application.platform == RuntimePlatform.IPhonePlayer ) {
			playerName = "Player_" + GameManager.RandomString(3);
			Screen.sleepTimeout = SleepTimeout.NeverSleep;
		}
		
		
	}
	
	// Use this for initialization
	void Start () {
		_bikes = new Dictionary<NetworkPlayer, Bike>();
		_isPaused = false;
		
		
	}
	
	// Update is called once per frame
	void Update () {
		if (_gamePlayStarted) {
			// update current Rank
			_currentRank = 1;
			foreach (KeyValuePair<NetworkPlayer, Bike> item in _bikes) {
				if (item.Key != Network.player) {
					Bike otherBike = item.Value;
					if (otherBike.transform.position.z > _localBike.transform.position.z ) {
						_currentRank++;
					}
				}
			}
		}
		
		//_fsm.Update();
	}
	
	public void SetPaused(bool paused) {
		_isPaused = paused;
		if (_isPaused) {
			Time.timeScale = 0;
		} else {
			Time.timeScale = 1;
		}
	}
	
	public void HostGame() {
		_fsm.PerformTransition("GoToHostSetting");
	}
	
	public void Restart() {
	}
	
	public bool IsPaused() {
		return _isPaused;
	}
		
	public Bike GetLocalBike() {
		return _localBike;
	}
	
	void OnDestroy() {
		print ("[GameManager] OnDestroy");
	}
	
	static public void Create() {
		GameObject go = GameObject.Find ("Game Manager");
		if (go == null ) {
			go = new GameObject("Game Manager");
			go.AddComponent<GameManager>();
			GameObject.DontDestroyOnLoad(go);
		}
	}
	
	static public void CreateNetworkNode() {
		GameObject go = GameObject.Find ("Network");
		if (go==null) {
			go = new GameObject("Network");
			NetworkView nv = go.AddComponent<NetworkView>();
			nv.stateSynchronization = NetworkStateSynchronization.Off;
			
			go.AddComponent<Server>();
			go.AddComponent<Client>();
			GameObject.DontDestroyOnLoad(go);
		}
	}
	
	//private void OnNetworkLoadedLevel() {
	//	
	//	Debug.Log("GameManager.OnNetworkLoadedLevel");
	//	Debug.Log("LocalPlayer:" + Network.player.ToString());
		
	//	if ( Network.isServer ) {
	//		Debug.Log("[GameManager] isServer");
	//	} else if ( Network.isClient ) {
	//		Debug.Log ( "[GameManager] isClient");
	//	} else {
	//		Debug.Log ( "[GameManager] unknown");
	//	}
		
	//	if (Network.isClient) {
	//		networkView.RPC ("NotifyLevelLoaded", RPCMode.Server, Network.player);
	//	} else if (Network.isServer) {
	//		NotifyLevelLoaded(Network.player);
	//	}
		
	//	Debug.Log ("[GameManager] Connections:" + Network.connections.Length);
	//}
	
	void OnLevelWasLoaded(int level) {
		Debug.Log ("[GameManager] OnLevelWasLoaded:" + level);
	}
	
	public void RegisterBike(Bike bike) {
		//_bikes.Add(bike.GetNetworkPlayer(), bike);
	}
	
	public Bike SpawnBike(int track) {
		
		string tagName = "init_spawn_" + track;
		GameObject spawn = GameObject.FindWithTag(tagName);
		if (spawn == null ) {
			Debug.LogError("[GameManager] can not find:" + tagName);
			return null;
		}
		
		GameObject bikeGo = null;
		if (Network.isClient || Network.isServer ) {
			bikeGo = (GameObject)Network.Instantiate(Resources.Load("Bike"), spawn.transform.position, Quaternion.identity, 0);
		} else {
			bikeGo = (GameObject)Instantiate(Resources.Load("Bike"), spawn.transform.position, Quaternion.identity);
		}
		return bikeGo.GetComponent<Bike>();
	}
		
	public void AddPlayer(PlayerInfo playerInfo) {
		if (!ContainsPlayer(playerInfo.networkPlayer)) {
			Debug.Log("[GameManager.AddPlayer] " + playerInfo.networkPlayer.ToString());
			_playerInfos.Add (playerInfo);
			if (playerInfo.networkPlayer == Network.player) {
				_localPlayerInfo = playerInfo;
			}
			playerInfo.transform.parent = transform;
			if (OnPlayerAdded!=null) {
				OnPlayerAdded(playerInfo);
			}
		} else {
			Debug.LogError("[GameManager.AddPlayer] Failed to add player:" + playerInfo.networkPlayer.ToString());
		}
	}
	
	public bool ContainsPlayer(NetworkPlayer player) {
		foreach (PlayerInfo playerInfo in _playerInfos) {
			if (playerInfo.networkPlayer == player)
				return true;
		}
		return false;
	}
	
	public PlayerInfo GetPlayerInfo(NetworkPlayer player) {
		foreach (PlayerInfo playerInfo in _playerInfos) {
			if (playerInfo.networkPlayer == player)
				return playerInfo;
		}
		return null;
	}
	
	public PlayerInfo GetPlayerInfo(int index) {
		return _playerInfos[index];
	}
	
	public void RemovePlayer(NetworkPlayer player) {
		PlayerInfo playerInfo = GetPlayerInfo(player);
		RemovePlayer(playerInfo);
	}
	
	public void RemovePlayer(int index) {
		PlayerInfo playerInfo = _playerInfos[index];
		RemovePlayer(playerInfo);
	}
	
	public void RemovePlayer(PlayerInfo playerInfo) {
		_playerInfos.Remove(playerInfo);
		GameObject.Destroy(	playerInfo.gameObject );
		
		if (OnPlayerRemoved!=null) {
			OnPlayerRemoved(playerInfo);
		}
	}
	
	public void RemoveAllPlayers() {
		
		int count = playersCount;
		while (count > 0 ) {
			RemovePlayer(0);
			count--;
		}
		_playerInfos.Clear();
		_localPlayerInfo = null;
	}
	
	public void GoToLobby() {
		Application.LoadLevel("GameLobbyScene");
	}
	
	public void DestroyBike() {
		if (_localBike) {
			Network.Destroy(_localBike.gameObject);
			_localBike = null;
		}
	}
	
	//[RPC]
	//void SetTrackIndex(int trackIndex) {
	//	Debug.Log("[RPC]SetInitialTrack:" + trackIndex);
		/*
		string tagName = "init_spawn_" + trackIndex;
		GameObject spawn = GameObject.FindWithTag(tagName);
		if (spawn == null ) {
			Debug.LogError("[GameManager] can not find:" + tagName);
			return;
		}
		
		GameObject localBikeGo = (GameObject)Network.Instantiate(bikePrefab, spawn.transform.position, Quaternion.identity, 0);
		_localBike = localBikeGo.GetComponent<Bike>();
		*/
		//_localBike = SpawnBike(trackIndex);
		//_localBike.StartEngine();
		
		//if (Network.isClient) {
		//	networkView.RPC ("RPC2Server_NotifyReadyToStart", RPCMode.Server, Network.player);
		//} else if (Network.isServer) {
		//	RPC2Server_NotifyReadyToStart(Network.player);
		//}
	//}
	
	private static System.Random random = new System.Random((int)DateTime.Now.Ticks);//thanks to McAden
	public static string RandomString(int size) {
        StringBuilder builder = new StringBuilder();
        char ch;
        for (int i = 0; i < size; i++)
        {
            ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));                 
            builder.Append(ch);
        }

        return builder.ToString();
    }
	
	//[RPC]
	//void RPC2All_NotifyCountDownStart() {
	//	_localBike.StartEngine();
	//}
	
	//[RPC]
	//void RPC2Server_NotifyReadyToStart(NetworkPlayer player) {
	//	if (Network.isServer) {
	//		Debug.Log("[GameManager] NofityServerReady, player:" + player.ToString() + "LocalTime:" + Time.timeSinceLevelLoad);
	//	}
	//}
	
	//[RPC]
	//void NotifyLevelLoaded(NetworkPlayer player) {
		// Server Side RPC		
	//	if (Network.isServer) {
	//		Debug.Log ("[GameManager] NotifyLevelLoaded. NetworkPlayer:" + player.ToString() + " guid:" + player.guid);
	//		if (!_levelGamePlayLoadedDict.ContainsKey(player)) {
	//			Debug.LogError("[GameManager] No such player:" + player.ToString());
	//			return;
	//		}
	//		_levelGamePlayLoadedDict[player] = true;
	//		bool allLevelReady = true;
	//		foreach ( KeyValuePair<NetworkPlayer, bool> item in _levelGamePlayLoadedDict ) {
	//			if (!item.Value) {
	//				allLevelReady = false;
	//			}
	//		}
			
	//		if (allLevelReady) {
	//			int trackIndex = 1;
	//			foreach ( NetworkPlayer item in Network.connections ) {
	//				networkView.RPC ("SetTrackIndex", item, trackIndex);
	//				trackIndex++;
	//			}
				
				// for Server player
	//			SetTrackIndex(0);
	//			_gamePlayStarted = true;
	//		}	
			
	//	} else {
	//		Debug.LogWarning("[GameManager] NotifyLevelLoaded was called, but is not in Server Side"); 
	//	}
	//}
	
	//void OnPlayerConnected(NetworkPlayer player) {
	//	Debug.Log ("[GameManager] OnPlayerConnected:" + player.ToString() + " guid:" + player.guid );
	//}	
}
