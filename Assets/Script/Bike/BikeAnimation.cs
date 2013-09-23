using UnityEngine;
using System.Collections;

public class BikeAnimation : MonoBehaviour {

    private AnimationState _viberation;
    private AnimationState _leanLeft;
    private AnimationState _leanRight;

    public BikeSteer steer;

	// Use this for initialization
	void Start () {
        _viberation = animation["BikeViberation"];
        _leanLeft = animation["BikeLeanLeft"];
        _leanRight = animation["BikeLeanRight"];
        _leanLeft.layer = 10;
        _leanRight.layer = 10;
        //_leanLeftAnimState.blendMode = AnimationBlendMode.Additive;
        //_leanRightAnimState.blendMode = AnimationBlendMode.Additive;
        _leanLeft.wrapMode = WrapMode.ClampForever;
        _leanRight.wrapMode = WrapMode.ClampForever;
        _leanLeft.enabled = true;
        _leanRight.enabled = true;
        _leanLeft.weight = 1.0f;
        _leanRight.weight = 1.0f;
        _leanLeft.normalizedTime = 0;
        _leanRight.normalizedTime = 0;
        
        _viberation.wrapMode = WrapMode.Loop;
        if (networkView.isMine) {
            animation.Play("BikeViberation");
        }
	}
	
	// Update is called once per frame
	void Update () {
        if (networkView.isMine) {
            _leanLeft.normalizedTime = -steer.steerValue;
            _leanRight.normalizedTime = steer.steerValue;
        }
	}
}
