using UnityEngine;
using System.Collections;

namespace Profile {


    public class ViewController : MonoBehaviour {

        public GameObject accountOptionWindow;
        public GameObject createAccountWindow;
        public GameObject viewAccountWindow;
        public UILabel createAccountNameLabel;
        public UILabel accountNameLabel;
        public UITexture profileTexture;

        // Use this for initialization
        void Start () {

			FacebookSNSAgent.OnUserLoggedIn += OnUserLoggedIn;
			FacebookSNSAgent.OnUserInfoArrived += OnUserInfoArrived;
			FacebookSNSAgent.OnUserProfilePictureArrived += OnUserProfilePictureArrived;

            if (PlayerInfo.hasAccoutCreated) {
                viewAccountWindow.SetActive(true);
                accountOptionWindow.SetActive(false);
                createAccountWindow.SetActive(false);
                accountNameLabel.text = GameManager.playerName;
            } else {
                viewAccountWindow.SetActive(false);
                accountOptionWindow.SetActive(true);
                createAccountWindow.SetActive(false);
            }
        }
        
        // Update is called once per frame
        void Update () {
        
        }

        void OnDestroy() {
            if (!GameManager.isShutingDown) {
                FacebookSNSAgent.OnUserLoggedIn -= OnUserLoggedIn;
                FacebookSNSAgent.OnUserInfoArrived -= OnUserInfoArrived;
                FacebookSNSAgent.OnUserProfilePictureArrived -= OnUserProfilePictureArrived;
            }
        }

        void OnCreateAccountButtonPressed(GameObject button) {
            createAccountWindow.SetActive(true);
            accountOptionWindow.SetActive(false);
        }

        void OnUseFacebbokButtonPressed(GameObject button) {
            Debug.Log("[Profile.ViewController.OnUserFacebookButtonPressed]");
            FacebookSNS.Instance().Login();
        }

        void OnCreateAccountOKButtonPressed(GameObject button) {
            PlayerInfo.CreateAccount(createAccountNameLabel.text);
            accountNameLabel.text = createAccountNameLabel.text;
            createAccountWindow.SetActive(false);
            viewAccountWindow.SetActive(true);
        }

        void OnCreateAccountCancelButtonPressed(GameObject button) {
            createAccountWindow.SetActive(false);
            accountOptionWindow.SetActive(true);
        }

        void OnBackToMainMenuButtonPressed(GameObject button) {
            Application.LoadLevel("MainMenuScene");
        }

        void OnUserLoggedIn() {
            Debug.Log("[Profile.ViewController.OnUserLoggedIn]");
        }

        void OnUserInfoArrived(FacebookUserInfo userInfo) {
            Debug.Log("[Profile.ViewController.OnUserInfoArrived]");
            /// Your code here...
            FacebookSNS.Instance().GetUserProfilePicture(userInfo.id);
			PlayerInfo.CreateAccount(userInfo.userName);
			accountNameLabel.text = userInfo.userName;
			createAccountWindow.SetActive(false);
			accountOptionWindow.SetActive(false);
			viewAccountWindow.SetActive(true);
        }
		
		void OnUserProfilePictureArrived(string userId, string pictureUrl) {
    		StartCoroutine(FetchUserPicture(pictureUrl));
		}
		
		IEnumerator FetchUserPicture(string pictureUrl) {
			WWW www = new WWW(pictureUrl);
			yield return www;
			profileTexture.mainTexture = www.texture;
			Debug.Log ("[PlayerInfo.FetchUserPicture] OK");
		}
        
    }
}
