using UnityEngine;
using System.Collections;

public class PlayerItem : MonoBehaviour {
	
	public UILabel nameLabel;
	public UILabel statusLabel;
    public UILabel ipAndPortLabel;

	private PlayerInfo _playerInfo;
	public PlayerInfo playerInfo {
		set {
			if (_playerInfo!=value) {
				if (_playerInfo!=null) {
					_playerInfo.OnNameChanged -= SetPlayerName;
					_playerInfo.OnStatusChanged -= SetPlayerStatus;
				}
				
				_playerInfo = value;
				if (_playerInfo!=null) {
					_playerInfo.OnNameChanged += SetPlayerName;
					_playerInfo.OnStatusChanged += SetPlayerStatus;
					SetPlayerName(_playerInfo.playerName);
					SetPlayerStatus(_playerInfo.status);

                    NetworkPlayer np = _playerInfo.networkPlayer;
                    SetIpAndPort(np.ipAddress, np.port);
				}
			}	
		}
		
		get { return _playerInfo; }
	}
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void SetPlayerName(string name) {
		if (nameLabel!=null) {
			nameLabel.text = name;
		}
	}
	
	void SetPlayerStatus(PlayerInfo.Status status) {
		if (statusLabel!=null) {
			statusLabel.text = PlayerInfo.StatusString(status);
		}
	}

    void SetIpAndPort(string ip, int port) {

        if (_playerInfo == null)
            return;

        NetworkView nv = _playerInfo.networkView;
        NetworkPlayer np = nv.owner;

        if (ipAndPortLabel != null) {
            ipAndPortLabel.text = np.ipAddress + ":" + np.port.ToString() + "/viewID:" + nv.viewID.ToString();
        }
    }
}
