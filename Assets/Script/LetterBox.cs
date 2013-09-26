using UnityEngine;
using System.Collections;

public class LetterBox : MonoBehaviour {

    public UITexture textureTop;
    public UITexture textureBottom;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void PlayFlyInAnimation() {
        animation.Play("LetterBoxIn");
    }

    public void PlayFlyOutAnimation() {
        animation.Play("LetterBoxOut");
    }
}
