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
	
	private string _trackName = "Track01";
	public string trackName {
		get { return _trackName; }
		set { _trackName = value; }
	}
	
	private int _totalLaps = 3;
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
		get { return 2; }
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

    private static bool _isShutingDown = false;
    public static bool isShutingDown {
        get { return _isShutingDown; }
    }
	
	private Dictionary<NetworkPlayer, Bike> _bikes;
	private Bike _localBike;
	private bool _gamePlayStarted;
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
	
	void OnLevelWasLoaded(int level) {
		Debug.Log ("[GameManager] OnLevelWasLoaded:" + level);
	}
	
	public Bike SpawnBike(string bikeName, int track) {
		
		string tagName = "init_spawn_" + track;
		GameObject spawn = GameObject.FindWithTag(tagName);
		if (spawn == null ) {
			Debug.LogError("[GameManager] can not find:" + tagName);
			return null;
		}
		
		GameObject bikeGo = null;
		if (Network.isClient || Network.isServer ) {
			bikeGo = (GameObject)Network.Instantiate(Resources.Load(bikeName), spawn.transform.position, Quaternion.identity, 0);
		} else {
			bikeGo = (GameObject)Instantiate(Resources.Load(bikeName), spawn.transform.position, Quaternion.identity);
		}
        Utility.SetLayerRecursively(bikeGo, 21 + track);
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

    public PlayerInfo[] GetPlayerInfos() {
        return _playerInfos.ToArray();
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
        if (Network.isServer) {
            Network.RemoveRPCs( playerInfo.networkView.viewID );
        }
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

    void OnApplicationQuit() {
        _isShutingDown = true;
    }
}
