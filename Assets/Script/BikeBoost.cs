﻿using UnityEngine;
using System.Collections;

public class BikeBoost : MonoBehaviour {

    public ParticleEmitter boostParticleEmitter;
    public AudioSource soundEffect1;
    public float duration = 3.0f;
    private float _delayTimer;
    private bool _isBoosting = false;
    public bool isBoosting {
        get { return _isBoosting; }
    }

    BikeCrash _crashHandler;

	// Use this for initialization
	void Start () {
        _crashHandler = GetComponent<BikeCrash>();
	
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
        if (_crashHandler.isCrashed)
            return;

		if (other.gameObject.tag == "accelerator" ) {
            StartBoost();
        }
    }

    private void StartBoost() {
        _isBoosting = true;
        soundEffect1.Play();
        if (boostParticleEmitter!=null) {
            boostParticleEmitter.emit = true;
        }
    }

    private void StopBoost() {
        _isBoosting = false;
        if (boostParticleEmitter!=null) {
            boostParticleEmitter.emit = false;
        }
    }
}