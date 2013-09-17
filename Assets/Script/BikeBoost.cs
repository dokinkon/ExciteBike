using UnityEngine;
using System.Collections;

public class BikeBoost : MonoBehaviour {

    public ParticleSystem boostParticleEmitter;
    public AudioSource soundEffect1;
    public float duration = 3.0f;
    private BikePitch _pitch;
    private float _delayTimer;
    private bool _isBoosting = false;
    public bool isBoosting {
        get { return _isBoosting; }
    }

    BikeCrash _crashHandler;

	// Use this for initialization
	void Start () {
        _crashHandler = GetComponent<BikeCrash>();
        _pitch = GetComponent<BikePitch>();
	}
	
	// Update is called once per frame
	void Update () {
        if (!_isBoosting)
            return;

        _delayTimer += Time.deltaTime;
        if (_delayTimer > duration ) {
            _delayTimer = 0;
            StopBoost();
        }
	}

	void OnTriggerEnter(Collider other ) {
        if (_crashHandler.isCrashed || _isBoosting)
            return;

		if (other.gameObject.tag == "accelerator" ) {
            StartBoost();
        }
    }

    private void StartBoost() {
        if (_isBoosting)
            return;

        soundEffect1.Play();
        if (boostParticleEmitter!=null) {
            boostParticleEmitter.Play(true);
        }
        _pitch.PitchUp(1);
        _isBoosting = true;
    }

    private void StopBoost() {
        if (!_isBoosting)
            return;

        if (boostParticleEmitter!=null) {
            boostParticleEmitter.Stop();
        }
        _pitch.ResetPitch();
        _isBoosting = false;
    }
}
