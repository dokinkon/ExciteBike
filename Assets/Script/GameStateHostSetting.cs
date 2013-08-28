using UnityEngine;
using System.Collections;

public class GameStateHostSetting : FSMState {
	
	//private HostSettingMenu _menu = null;
	
	public GameStateHostSetting() {
		_stateName = "GameStateHostSetting";
		AddTransition("GoToLobby", "GameStateLobby");
	}
	
	
	public override void DoBeforeEntering() {
	
		/*
		if (_menu == null) {
			GameObject menuGo = GameObject.FindWithTag("menu");
			if (menuGo == null ) {
				Debug.LogError("");
				return;
			}
			
			_menu = menuGo.GetComponent<HostSettingMenu>();
			if (_menu == null ) {
				Debug.LogError("");
				return;
			}
		}
		_menu.SetVisible(true);
		*/
	}
	
	// Update is called once per frame
	public override void Update () {
	
	}
	
	public override void DoBeforeLeaving() {
		//if (_menu != null ) {
		//	_menu.SetVisible(false);
		//}
	}
}
