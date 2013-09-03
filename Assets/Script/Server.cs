using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Server : MonoSingleton<Server> {
	
	public delegate void NetworkConnectionErrorDelegate(NetworkConnectionError e);
	public event NetworkConnectionErrorDelegate OnLaunchServerFailed;
	
	public enum ConnectResult : int {
		Welcome = 0,
		Full = 1,
		VersionMismatch = 2
	}
	
	//private Dictionary<NetworkPlayer, PlayerInfo> _playerDict;
	
	public int listenPort = 25002;
	public bool useNat = true;
	private bool _isPasswordProtected = false;
	private static int _maxPlayers = 4;
	public static int maxPlayers
	{
		set { _maxPlayers = value; }
		get { return _maxPlayers; }
	}
	
	public static string title {
		get { return PlayerPrefs.GetString("Server.Title"); }
		set { PlayerPrefs.SetString("Server.Title", value); }
	}
	
	private int _lastLevelPrefix;
	private string[] _trackLockedBy = new string[4];
	
	// Use this for initialization
	void Start () {
		title = "Server_" + GameManager.RandomString(3);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void NotifyStartRace() {
		networkView.RPC ("StartRace", RPCMode.All);
	}
	
	public void Launch(bool registerToMasterServer) {
		
		NetworkConnectionError error = Network.InitializeServer(_maxPlayers - 1, listenPort, useNat);
		if (NetworkConnectionError.NoError!=error) {
			// TODO
			Debug.LogError("Launch Server Fail!:" + error.ToString());
			if (OnLaunchServerFailed!=null) {
				OnLaunchServerFailed(error);
			}
			return;
		}

        if ( registerToMasterServer ) {
            MasterServer.updateRate = 3;
            MasterServer.RegisterHost(GameManager.gameName, title, "profas chat test");	
        }
	}
	
	// Deprecated
	public void LoadGamePlayLevel() {
		networkView.RPC ("LoadLevelRPC", RPCMode.All, "GamePlayScene", _lastLevelPrefix);
		++_lastLevelPrefix;
	}
	
	public void LoadLevel(string level) {
		if (Network.isServer) {
			networkView.RPC ("LoadLevelRPC", RPCMode.All, level, _lastLevelPrefix);
			++_lastLevelPrefix;
		}
	}
	
	public bool IsPasswordProtected() {
		return _isPasswordProtected;
	}
	
	void OnFailedToConnectToMasterServer(NetworkConnectionError info) {
        Debug.Log("Could not connect to master server: " + info);
    }
	
	void ResetTrackLock() {
		for (int i=0;i<4;i++) {
			_trackLockedBy[i] = null;
		}
	}
	
	void LockTrackIndex(NetworkPlayer player) {
		if ( !Network.isServer ) {
			Debug.LogError("[Server.LockTrackIndex] error");
			return;
		}
		
		for (int i=0;i<4;i++) {
			if (_trackLockedBy[i] == null) {
				_trackLockedBy[i] = player.guid;
				Debug.Log ("[Server.LockTrackIndex] index:" + i + " player:" + player.ToString());
				return;
			}
		}
	}
	
	void UnlockTrackIndex(NetworkPlayer player) {
		int index = Array.IndexOf(_trackLockedBy, player);
		if (index!=-1) {
			_trackLockedBy[index] = null;
		}
	}
	
	public int GetTrackIndex(NetworkPlayer player) {
		for (int i=0;i<4;i++) {
			if (_trackLockedBy[i] == player.guid) {
				return i;
			}
		}
		return -1;
	}
	
	
	public bool PollAllPlayerSyncForStatus(PlayerInfo.Status status) {
		if (!Network.isServer) {
			Debug.LogError("[Server.PollAllPlayerSyncForStatus] This function is used by server.");
			return false;
		}
		
		bool allSync = true;
		
		int count = GameManager.Instance.playersCount;
		for (int i=0;i<count;i++) {
			PlayerInfo playerInfo = GameManager.Instance.GetPlayerInfo(i);
			if (playerInfo.status != status) {
				allSync = false;
				break;
			}
		}
		
		return allSync;
	}
	
	public bool PollAllPlayersInReadyState() {
		if (!Network.isServer) {
			Debug.LogError("[Server.PollAllPlayersInReadyState] This function is used by server.");
			return false;
		}
		
		bool allReady = true;
		
		int count = GameManager.Instance.playersCount;
		for (int i=0;i<count;i++) {
			PlayerInfo playerInfo = GameManager.Instance.GetPlayerInfo(i);
			if (playerInfo.status != PlayerInfo.Status.PlayingReady) {
				allReady = false;
				break;
			}
		}
		
		if (allReady) {
			networkView.RPC ("NotifyGamePlayReadyStart", RPCMode.All);
		}
		
		return allReady;
	}
	
	void OnPlayerConnected(NetworkPlayer player) {
		Debug.Log("[Server.OnPlayerConnected] connected from " + player.ipAddress + ":" + player.port);
		LockTrackIndex(player);
    }
	
	void OnPlayerDisconnected(NetworkPlayer player) {
		Debug.Log("[Server.OnPlayerDisconnected]:" + player.ToString());
		UnlockTrackIndex(player);
		GameManager.Instance.RemovePlayer(player);
		Network.RemoveRPCs(player);
        Network.DestroyPlayerObjects(player);
	}
	
	void OnServerInitialized() {
		Debug.Log("[Server.OnServerInitialized]");
		ResetTrackLock();
		LockTrackIndex(Network.player);
		RequestJoinGame(Network.player, GameManager.playerName, GameManager.majorVersion, GameManager.minorVersion, GameManager.microVersion);
	}
	
	void OnConnectedToServer() {
        Debug.Log("[Server.OnConnectedToServer]");
    }
	
	[RPC]
	void RequestJoinGame(NetworkPlayer player, string name, int majorVersion, int minorVersion, int microVersion) {
		string version = System.String.Format("{0}.{1}.{2}", majorVersion, minorVersion, microVersion);
		Debug.Log ("[Server.RequestJoinGame] player:" + player.ToString() + " name:" + name + " version:" + version);
		int trackIndex = GetTrackIndex(player);
		
		if (Network.player != player) {
			networkView.RPC ("ResponseJoinGame", player, (int)ConnectResult.Welcome, trackIndex);
		} else {
			Client.Instance.ResponseJoinGame((int)ConnectResult.Welcome, trackIndex);
		}
	}
}
