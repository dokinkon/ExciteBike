﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Client : MonoSingleton< Client > {
	
	private static HostData _hostData;
	public static HostData hostData {
		set { _hostData = value; }
		get { return _hostData; }
	}
	
    public delegate void StringDelegate(string s);
    public delegate void VoidDelegate();
	public delegate void FloatDelegate(float f);
	public event FloatDelegate OnGamePlayReadyStart;
    public event VoidDelegate OnAllPlayersReadyInLobby;
    public event StringDelegate OnReceiveChatMessage;
    public event StringDelegate OnCurrentTrackNameChanged;
	
	public override void Init() {
		
	}
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void Connect(string IP, int remotePort) {
		NetworkConnectionError error = Network.Connect(IP, remotePort);
		if (error!=NetworkConnectionError.NoError) {
			Debug.LogError("[Client.Connect] Error:" + error.ToString()); 
		}
	}
	
	public static void Connect(HostData hostData) {
		_hostData = hostData;
		NetworkConnectionError error = Network.Connect(hostData);
		if (error!=NetworkConnectionError.NoError) {
			Debug.LogError("[Client.Connect] Error:" + error.ToString()); 
		}
	}
	
	void OnConnectedToServer() {
		Debug.Log ("[Client.OnConnectToServer]");
		networkView.RPC ("RequestJoinGame", RPCMode.Server, Network.player, GameManager.playerName, 
				GameManager.majorVersion, GameManager.minorVersion, GameManager.microVersion);
	}
	
	void OnDisconnectedFromServer(NetworkDisconnection info) {
		Debug.Log ("[Client.OnDisconnectedFromServer] " + info + " isServer:" + Network.isServer.ToString() + " isClient:" + Network.isClient.ToString());
		GameManager.Instance.RemoveAllPlayers();
	}
	
	public void SpawnPlayerInfo(int trackIndex) {
		Debug.Log("[Client.SpawnPlayerInfo] trackIndex:" + trackIndex);
		GameObject playerInfoGo = (GameObject)Network.Instantiate(Resources.Load("PlayerInfoPrefab"), Vector3.zero, Quaternion.identity, 0);
		PlayerInfo playerInfo = playerInfoGo.GetComponent<PlayerInfo>();
		playerInfo.playerName = GameManager.playerName;
		playerInfo.trackIndex = trackIndex;
		if ( GameManager.Instance.facebookUserInfo != null ) {
			playerInfo.facebookUserId = GameManager.Instance.facebookUserInfo.id;
		}
	}

    public void SendChatMessage(string message) {
        string finalMessage = GameManager.playerName;
        finalMessage += ":";
        finalMessage += message;
        networkView.RPC("RPCSendChatMessage", RPCMode.All, finalMessage);
    }

    [RPC]
    void RPCSendChatMessage(string message) {
        if ( OnReceiveChatMessage != null ) {
            OnReceiveChatMessage (message);
        }
    }

	[RPC]
	public void ResponseJoinGame(int result, int trackIndex) {
		Debug.Log("[Client.ResponseJoinGame]:" + result + " player:" + GameManager.playerName + " trackIndex:" + trackIndex);
		if (Server.ConnectResult.Welcome == (Server.ConnectResult)result) {
			Debug.Log ("[Client.Receive Welcome]");

			SpawnPlayerInfo(trackIndex);		
			GameManager.Instance.GoToLobby();
		} else if ( Server.ConnectResult.Full == (Server.ConnectResult)result ){
			
		} else if ( Server.ConnectResult.VersionMismatch == (Server.ConnectResult)result ) {

		}
	}

	[RPC]
	void LoadGamePlayLevelRPC(string trackName, int levelPrefix ) {
		//Debug.Log("[Client.LoadLevel] level:" + level + " with prefix:" + levelPrefix);

		// There is no reason to send any more data over the network on the default channel,
		// because we are about to load the level, because all those objects will get deleted anyway
		Network.SetSendingEnabled(0, false);	

		// We need to stop receiving because first the level must be loaded.
		// Once the level is loaded, RPC's and other state update attached to objects
        // in the level are allowed to fire
		Network.isMessageQueueRunning = false;
		
		// All network views loaded from a level will get a prefix into their NetworkViewID.
		// This will prevent old updates from clients leaking into a newly created scene.
		Network.SetLevelPrefix(levelPrefix);
		Application.LoadLevel("GamePlayScene");
        Application.LoadLevelAdditive(trackName);

		// Allow receiving data again
		Network.isMessageQueueRunning = true;
		// Now the level has been loaded and we can start sending out data
		Network.SetSendingEnabled(0, true);
	}
	
	[RPC]
	void LoadLevelRPC(string level, int levelPrefix ) {
		//Debug.Log("[Client.LoadLevel] level:" + level + " with prefix:" + levelPrefix);

		// There is no reason to send any more data over the network on the default channel,
		// because we are about to load the level, because all those objects will get deleted anyway
		Network.SetSendingEnabled(0, false);	

		// We need to stop receiving because first the level must be loaded.
		// Once the level is loaded, RPC's and other state update attached to objects
        // in the level are allowed to fire
		Network.isMessageQueueRunning = false;
		
		// All network views loaded from a level will get a prefix into their NetworkViewID.
		// This will prevent old updates from clients leaking into a newly created scene.
		Network.SetLevelPrefix(levelPrefix);
		Application.LoadLevel(level);

		// Allow receiving data again
		Network.isMessageQueueRunning = true;
		// Now the level has been loaded and we can start sending out data
		Network.SetSendingEnabled(0, true);
	}
	
	[RPC]
	void NotifyGamePlayReadyStart(NetworkMessageInfo info) {
		double transitTime = Network.time - info.timestamp;
		Debug.Log ("[Client.NotifyGamePlayReadyStart] transitTime:" + transitTime);
		
		if ( OnGamePlayReadyStart != null ) {
			OnGamePlayReadyStart( 1.0f - (float)transitTime );
		}
	}

    [RPC]
    void RPCAllPlayersReadyInLobby(NetworkMessageInfo info) {
        if ( OnAllPlayersReadyInLobby != null ) {
            OnAllPlayersReadyInLobby();
        }
    }

    [RPC]
    void NotifyPlayerDisconnected(NetworkPlayer player) {
		GameManager.Instance.RemovePlayer(player);
    }

    [RPC]
    void SyncCurrentTrackName(string trackName) {
        GameManager.Instance.trackName = trackName;
        
        if (OnCurrentTrackNameChanged!=null) {
            OnCurrentTrackNameChanged (trackName);
        }
    }
}

