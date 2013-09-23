using UnityEngine;
using System.Collections;

public class BikeShape : MonoBehaviour {

    public Renderer skinRenderer;

    private static Color[] _availableColors = new Color[] {
        Color.black,
        Color.blue,
        Color.cyan,
        Color.gray,
        Color.green,
        Color.grey,
        Color.magenta,
        Color.red,
        Color.white,
        Color.yellow
    };

    public static Color[] GetAvailableColors() {
        return _availableColors;
    }

	// Use this for initialization
	void Start () {
        //skinRenderer.material.color = Color.red;
	}

    public void SetColor(Color color) {
        skinRenderer.material.color = color;
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
