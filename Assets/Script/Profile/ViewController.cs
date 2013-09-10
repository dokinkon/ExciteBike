using UnityEngine;
using System.Collections;

namespace Profile {


    public class ViewController : MonoBehaviour {

        public GameObject accountOptionWindow;
        public GameObject createAccountWindow;
        public GameObject viewAccountWindow;
        public UILabel createAccountNameLabel;
        public UILabel accountNameLabel;

        // Use this for initialization
        void Start () {

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

        void OnCreateAccountButtonPressed(GameObject button) {
            createAccountWindow.SetActive(true);
            accountOptionWindow.SetActive(false);
        }

        void OnUseFacebbokButtonPressed(GameObject button) {

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
    }
}
