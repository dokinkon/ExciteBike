using UnityEngine;
using System.Collections;

public class GameStateMultiPlayerMenu : FSMState {
	
	//MultiPlayerMenu _menu;
	
	public GameStateMultiPlayerMenu(/*MultiPlayerMenu menu*/) {
		_stateName = "GameStateMultiPlayerMenu";
		AddTransition("GoToHostSetting", "GameStateHostSetting");
		//_menu = menu;
	}
	
	public override void DoBeforeEntering() { 
		//_menu.SetVisible(true);
	}
	
	public override void Update() {
	}
	
	public override void DoBeforeLeaving() {
		//_menu.SetVisible(false);
	}
}
