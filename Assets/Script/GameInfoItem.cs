using UnityEngine;
using System.Collections;

public class GameInfoItem : MonoBehaviour {
	
	public UILabel playersLabel;
	public UILabel gameNameLabel;
	public GameObject button;
	private HostData _hostData;
	
	public void SetHostData(HostData hostData) {
		playersLabel.text = System.String.Format("{0}/{1}", hostData.connectedPlayers, hostData.playerLimit);
		gameNameLabel.text = hostData.gameName;
		_hostData = hostData;
	}

	// Use this for initialization
	void Start () {
		UIEventListener.Get(button).onClick = OnButtonPressed;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnButtonPressed(GameObject button) {
		Client.Connect(_hostData);
		//Network.Connect(_hostData);
	}
}
