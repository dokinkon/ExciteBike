using UnityEngine;
using System.Collections;

namespace HostSetting {

	public class ViewController : MonoBehaviour {
		
		public GameObject goToLobbyButton;
		public GameObject backButton;
		public GameObject hostTitleInput;
		public GameObject maxPlayersList;
		public GameObject panel;
		
		private UIInput _hostTitleInput;
		private UIPopupList _popupList;
	
		// Use this for initialization
		void Start () {
			
			GameManager.CreateNetworkNode();
			
			UIEventListener.Get(goToLobbyButton).onClick = OnButtonGoToLobbyPressed;
			_hostTitleInput = hostTitleInput.GetComponent<UIInput>();
			_hostTitleInput.label.text = Server.title;
			
			UIEventListener.Get (backButton).onClick = OnBackButtonPressed;
			_popupList = maxPlayersList.GetComponent<UIPopupList>();
			
			Server.Instance.OnLaunchServerFailed += OnLaunchServerFailed;
			
			
		}
		
		void OnLaunchServerFailed(NetworkConnectionError e) {
			UIDialog.Create(panel, "Failed to Launch Server", e.ToString());
		}
		
		// Update is called once per frame
		void Update () {
		
		}

        private static bool _isShuttingDown = false;

        void OnApplicationQuit() {
            _isShuttingDown = true;
        }
		
		void OnDestroy() {
            if (!_isShuttingDown) {
                Server.Instance.OnLaunchServerFailed -= OnLaunchServerFailed;
            }
		}
		
		void OnButtonGoToLobbyPressed(GameObject button) {
			Debug.Log("OnButtonGoToLobbyPressed:" + _hostTitleInput.label.text + ", " + _popupList.selection);
			Server.title =  _hostTitleInput.label.text;
			Server.maxPlayers = 4;//int.Parse(_popupList.selection);
			Server.Instance.Launch(true);
		}
		
		void OnServerInitialized() {
			Application.LoadLevel("GameLobbyScene");
		}
		
		void OnBackButtonPressed(GameObject button) {
			Application.LoadLevel("GameListScene");
		}
	}
}
