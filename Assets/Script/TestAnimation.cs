using UnityEngine;
using System.Collections;

public class TestAnimation : MonoBehaviour {

    AnimationState _leanLeft;
    AnimationState _leanRight;
    AnimationState _viberation;

	// Use this for initialization
	void Start () {
        _leanLeft = animation["BikeTiltLeft"];
        _leanRight = animation["BikeTiltRight"];
        _viberation = animation["BikeViberation"];
        _viberation.wrapMode = WrapMode.Loop;

        _leanLeft.enabled = true;
        _leanRight.enabled = true;

        _leanLeft.speed = 0;
        _leanRight.speed = 0;

        _leanLeft.weight = 1.0f;
        _leanRight.weight = 1.0f;

        _leanLeft.layer = 10;
        _leanRight.layer = 10;

        //_leanLeft.blendMode = AnimationBlendMode.Additive;
        //_leanRight.blendMode = AnimationBlendMode.Additive;

        _leanLeft.wrapMode = WrapMode.ClampForever;
        _leanRight.wrapMode = WrapMode.ClampForever;
        //_leanLeft.blendMode = 
        animation.Play("BikeViberation");

        //animation.Play("BikeTiltLeft");
	
	}
	
	// Update is called once per frame
	void Update () {
        float lean = Input.GetAxis("Horizontal");
        _leanLeft.normalizedTime = lean;
        _leanRight.normalizedTime = -lean;
        Debug.Log("Lean:" + lean + "anim-time:" + _leanLeft.normalizedTime);
	
	}
}
