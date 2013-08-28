using UnityEngine;
using System.Collections;

public class GameStateLobby : FSMState {
	
	//private LobbyMenu _menu = null;
	
	public GameStateLobby() {
		_stateName = "GameStateLobby";
	}
	
	public override void DoBeforeEntering() {
	}

	// Update is called once per frame
	public override void Update () {
	
	}
	
	public override void DoBeforeLeaving() {
	}
}
