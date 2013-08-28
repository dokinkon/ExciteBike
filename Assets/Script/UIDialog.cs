using UnityEngine;
using System.Collections;

public class UIDialog : MonoBehaviour {
	
	public UILabel titleLabel;
	public UILabel messageLabel;
	public GameObject okButton;
	
	public static void Create(GameObject panel, string title, string message) {
		GameObject go = (GameObject)Object.Instantiate(Resources.Load("Dialog"), Vector3.zero, Quaternion.identity);
		go.transform.parent = panel.transform;
		go.transform.localPosition = Vector3.zero;
		go.transform.localRotation = Quaternion.identity;
		go.transform.localScale = Vector3.one;
		UIDialog comp = go.GetComponent<UIDialog>();
		comp.titleLabel.text = title;
		comp.messageLabel.text = message;
		
	}
	
	// Use this for initialization
	void Start () {
		UIEventListener.Get(okButton).onClick = OnOKButtonPressed;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnOKButtonPressed(GameObject button) {
		GameObject.Destroy(gameObject);
	}
}
