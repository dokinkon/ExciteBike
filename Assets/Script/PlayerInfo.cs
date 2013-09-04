using UnityEngine;
using System.Collections;

public class PlayerInfo : MonoBehaviour {
	
	public delegate void StringDelegate(string s);
	public delegate void StatusDelegate(Status s);
	
	public event StringDelegate OnNameChanged;
	public event StatusDelegate OnStatusChanged;
	
	private string _facebookUserId;
	public string facebookUserId {
		get { return _facebookUserId; }
		set { 
			if ( value != _facebookUserId ) {
				_facebookUserId = value;
				networkView.RPC ("SyncFacebookUserId", RPCMode.OthersBuffered, _facebookUserId);
				LoadProfilePicture();
			}
		}
	}
	
	private void LoadProfilePicture() {
		FacebookSNS.Instance().GetUserProfilePicture(_facebookUserId, 128, 128);
	}
	
	public static int groudID {
		get { return 3; }
	}
	
	private Texture2D _profilePicture;
	public Texture2D profilePicture {
		get { return _profilePicture; }
	}

    private string _bikeName = "Bike";
    public string bikeName {
        get { return _bikeName; }
        set { 
            if (_bikeName != value) {
                _bikeName = value;
                networkView.RPC ("SyncBikeName", RPCMode.OthersBuffered, _bikeName);

            }
        }
    }
	
	public enum Status : int {
		Unknown = 0,
		Loading = 1,
		Playing = 2,
		PlayingReady = 3,
		PlayingPaused = 4,
		PlayingIdle = 5,
		AtLobby = 6,
		AtLoddyReady = 7
	}
	
	public static string StatusString(Status status) {
		if (Status.Unknown == status ) {
			return "Unknown";
		} else if (Status.Loading == status ) {
			return "Loading";
		} else if (Status.Playing == status ) {
			return "Playing";
		} else if (Status.PlayingReady == status ) {
			return "PlayingReady";
		} else if (Status.PlayingIdle == status ) {
			return "PlayingIdle";
		} else if (Status.PlayingPaused == status ) {
			return "PlayingPaused";
		} else if (Status.AtLobby == status ) {
			return "AtLobby";
		} else if (Status.AtLoddyReady == status ) {
			return "AtLoddyReady";
		}
		
		return "";
	}

    private NetworkPlayer _owner;
	public NetworkPlayer networkPlayer {
		get { return _owner; }
	}
	
	private string _playerName;
	public string playerName {
		set {
			if (_playerName!=value) {
				_playerName = value;
				if (OnNameChanged!=null) {
					OnNameChanged(_playerName);
				}
				
				if (networkView.isMine) {
					name = "PlayerInfo (" + _playerName + ")";
				} else {
					name = "PlayerInfo_Remote (" + _playerName +")";
				}
				
				networkView.RPC ("SyncPlayerName", RPCMode.OthersBuffered, _playerName);
			}
		}
		get { return _playerName; }
	}
	
	private Status _status;
	public Status status {
		set {
			if (_status!= value) {
				_status = value;
				if (OnStatusChanged!=null) {
					OnStatusChanged(_status);
				}
			}
			
		}
		get { return _status; }
	}
	
	private int _trackIndex = -1;
	public int trackIndex {
		set {
			if (_trackIndex != value) {
				_trackIndex = value;
				Debug.Log ("[PlayerInfo (" + _playerName + ") trackIndex:" + _trackIndex);
			}
		}
		
		get {
			return _trackIndex;
		}
	}
	
	private Bike _bike;
	public Bike bike {
		set { _bike = value; }
		get { return _bike; }
	}

    public string GetIPAndPort() {
        NetworkPlayer np = networkView.viewID.owner;
        return np.ipAddress + ":" + np.port.ToString();
    }

    void Awake() {
        Debug.Log("[PlayerInfo.Awake] " + GetIPAndPort());
    }

    void Start() {
        Debug.Log("[PlayerInfo.Start]" + GetIPAndPort());
    }

	void OnNetworkInstantiate(NetworkMessageInfo info) {
        //Note: The networkView attribute inside the NetworkMessageInfo is not used inside OnNetworkInstantiate
		
		Debug.Log("[PlayerInfo.OnNetworkInstantiate] group:" + networkView.group 
                + " ip:" + networkView.owner.ipAddress + " port:" + networkView.owner.port.ToString() 
                + " viewID:" + networkView.viewID.ToString() + " player:" + networkView.owner.ToString());
		
		networkView.group = groudID;
		
		GameObject.DontDestroyOnLoad(gameObject);
		//GameManager.Instance.AddPlayer(this);
		if (Network.isServer) {
			trackIndex = Server.Instance.GetTrackIndex(networkView.owner);
			Debug.Log ("[PlayerInfo.OnNetworkInstantiate] trackIndex:"+trackIndex);
		}

        if (networkView.isMine) {
            networkView.RPC("SetOwner", RPCMode.AllBuffered, Network.player);
        }
	}

    [RPC]
    void SetOwner(NetworkPlayer player) {
        _owner = player;
		GameManager.Instance.AddPlayer(this);
    }
	
	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info) {
		if (stream.isWriting) {
			int s = (int)_status;
			int i = trackIndex;
			stream.Serialize(ref s);
			stream.Serialize(ref i);
		} else {
			int s = 0;
			int i = 0;
			stream.Serialize(ref s);
			stream.Serialize(ref i);
			status = (Status)s;
			trackIndex = i;
		}
	}
	
	IEnumerator FetchUserPicture(string pictureUrl) {
		WWW www = new WWW(pictureUrl);
		yield return www;
		_profilePicture = www.texture;
		Debug.Log ("[PlayerInfo.FetchUserPicture] OK");
	}
	
	void OnUserProfilePictureArrived(string userId, string pictureUrl) {
		if (_facebookUserId == userId ) {
			Debug.Log("[PlayerInfo.OnUserProfilePictureArrived] " + pictureUrl);
			StartCoroutine(FetchUserPicture(pictureUrl));
		}
	}
	
	void OnEnable() {
		FacebookSNSAgent.OnUserProfilePictureArrived += OnUserProfilePictureArrived;
	}
	
	void OnDisable() {
		FacebookSNSAgent.OnUserProfilePictureArrived -= OnUserProfilePictureArrived;
	}
	
	[RPC]
	void SyncPlayerName(string n, NetworkMessageInfo info) {
		playerName = n;
	}
	
	[RPC]
	void SyncFacebookUserId(string facebookUserId) {
		if (_facebookUserId!=facebookUserId) {
			_facebookUserId = facebookUserId;
			LoadProfilePicture();
		}
	}

    [RPC]
    void SyncBikeName(string n) {
        bikeName = n;
    }
	
	
	
	
	

	
}
