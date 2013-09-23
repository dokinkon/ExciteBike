using UnityEngine;
using System.Collections;

public class TestBomb : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
    void OnGUI() {
        if (GUI.Button(new Rect(40, 40, 100, 40), "Test")) {
            Item.BombController.Use(new Vector3(0, 2, 0), Vector3.zero);
        }
    }
	// Update is called once per frame
	void Update () {
	
	}
}
