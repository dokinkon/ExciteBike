using UnityEngine;
using System.Collections;

public class ColorButton : MonoBehaviour {

    public UISprite sprite;
    public UIButton button;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void SetColor(Color color) {
        sprite.color = color;
        button.hover = color;
    }
}
