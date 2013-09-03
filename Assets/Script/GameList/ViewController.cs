using UnityEngine;
using System.Collections;

namespace GameList {

	public class ViewController : MonoBehaviour {
		
		public GameObject hostButton;
		public GameObject backButton;
		public GameObject gameInfoItemPrefab;
		public GameObject gameListRoot;
		public UILabel searchingLabel;
		
		private float _lastHostListRequest = -1000.0f;
		private float _hostListRefreshTimeout = 20.0f;
		private bool _pollingHostList = false;
		
		
		// Use this for initialization
		void Start () {
            Debug.Log("[GameList.ViewController.Start]");
			GameManager.OnViewControllerStarted();
			UIEventListener.Get(hostButton).onClick = OnHostButtonPressed;
			UIEventListener.Get(backButton).onClick = OnBackButtonPressed;
			RequestHostList();

		}
		
		// Update is called once per frame
		void Update () {
			
			if ( _lastHostListRequest + _hostListRefreshTimeout < Time.time) {
				RequestHostList();
			}
			
			if (MasterServer.PollHostList().Length != 0) {
				Debug.Log ("[GameList.ViewController] Found Servers");
				
				// Delete Current GameInfoItems
				int count = gameListRoot.transform.childCount;
				while (count > 0) {
					Transform t = gameListRoot.transform.GetChild(0);
					GameObject.Destroy(t.gameObject);
					count--;
				}
				
				
	            HostData[] hostData = MasterServer.PollHostList();
	            int i = 0;
	            while (i < hostData.Length) {
	                Debug.Log("Game name: " + hostData[i].gameName);
					GameObject gameInfoItemGo = (GameObject)Instantiate(gameInfoItemPrefab, Vector3.zero, Quaternion.identity);
					gameInfoItemGo.transform.parent = gameListRoot.transform;
					gameInfoItemGo.transform.localPosition = Vector3.zero;
					gameInfoItemGo.transform.localRotation = Quaternion.identity;
					gameInfoItemGo.transform.localScale = Vector3.one;
					
					GameInfoItem infoItem = gameInfoItemGo.GetComponent<GameInfoItem>();
					infoItem.SetHostData(hostData[i]);
					
					UITable table = gameListRoot.GetComponent<UITable>();
					table.Reposition();
					
	                i++;
	            }
	            MasterServer.ClearHostList();
				_pollingHostList = false;
	        }
			
			if (_pollingHostList) {
				searchingLabel.enabled = true;
			} else {
				searchingLabel.enabled = false;
			}
		}
		
		IEnumerator AnimateSearchingStatus() {
			int dotCount = 0;
			while (_pollingHostList) {
				dotCount++;
				dotCount = dotCount % 4;
				string dotString = "";
				for (int i=0;i<dotCount;i++) {
					dotString += ".";
				}
				searchingLabel.text = "SEARCHING FOR GAMES" + dotString;
				yield return new WaitForSeconds(0.5f);
			}
		}
		
		void RequestHostList() {
			MasterServer.ClearHostList();
			MasterServer.RequestHostList (GameManager.gameName);
			_pollingHostList = true;
			_lastHostListRequest = Time.time;
			StartCoroutine(AnimateSearchingStatus());
			Debug.Log ("[GameList.ViewController.RequestHostList]");
		}
		
		void OnHostButtonPressed(GameObject button) {
			Application.LoadLevel("HostSettingScene");
			//RequestHostList();
		}
		
		void OnBackButtonPressed(GameObject button) {
			Application.LoadLevel("MasterServerScene");
		}
	}
}
