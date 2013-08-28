using UnityEngine;
using System.Collections;

namespace MainMenu {

	public class ViewController : MonoBehaviour {
		
		public GameObject quickRaceButton;
		public GameObject multiplayerButton;
		public GameObject optionButton;
		public GameObject singlePlayButton;
		public GameObject loginFacebookButton;
		public UILabel playerNameLabel;
		public GameObject panel;
		//private FacebookUserInfo _fbUserInfo;
		public SplashScreen splashScreen;
		
		void Awake() {
				
		}
		
		void Start() {
			
			GameManager.OnViewControllerStarted();
			//playerNameLabel.text = GameManager.playerName;
			UIEventListener.Get(quickRaceButton).onClick = OnQuickRaceButtonPressed;
			UIEventListener.Get(multiplayerButton).onClick = OnMultiplayerButtonPressed;
			UIEventListener.Get(optionButton).onClick = OnOptionButtonPressed;
			
		}
			
		void OnUserLoggedIn() {
			Debug.Log("MainMenu.ViewController.OnUserLoggedIn");
		}
		
		void OnEnable() {
			Debug.Log("MainMenu.ViewController.OnEnable");
			FacebookSNSAgent.OnUserLoggedIn += OnUserLoggedIn;
			FacebookSNSAgent.OnUserInfoArrived += OnUserInfoArrived;
		}
		
		void OnUserInfoArrived(FacebookUserInfo userInfo) {
			GameManager.Instance.facebookUserInfo = userInfo;
		}
		
		void OnDisable() {
			Debug.Log("MainMenu.ViewController.OnEnable");
			FacebookSNSAgent.OnUserLoggedIn -= OnUserLoggedIn;
			FacebookSNSAgent.OnUserInfoArrived -= OnUserInfoArrived;
			
		}
		
		void OnQuickRaceButtonPressed(GameObject button) {
			//Debug.Log ("OnButtonHostGamePressed:" + button.ToString());
			//splashScreen.levelToLoad = "HostSettingScene";
			//splashScreen.StartSplash();
			//Application.LoadLevel("HostSettingScene");
			//UIDialog.Create (panel, "text", "this is message");
			
		}
		
		void OnMultiplayerButtonPressed(GameObject button) {
			Application.LoadLevel("GameListScene");
		}
		
		void OnOptionButtonPressed(GameObject button) {
		}
		
		void OnLoginFacebookButtonPressed(GameObject button) {
			if (Application.platform == RuntimePlatform.IPhonePlayer ) {
				FacebookSNS.Instance().Login();
			}
		}
		
		void OnPlayerNameCommit() {
			GameManager.playerName = playerNameLabel.text;
		}
	}
}