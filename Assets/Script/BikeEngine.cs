using UnityEngine;
using System.Collections;
using System;

public class BikeEngine : MonoBehaviour {

    public AudioSource soundRPM1000;
    public AudioSource soundRPM2000;
    public AudioSource soundRPM3000;
    public AudioSource soundRPM4000;
    public AudioSource soundRPM5000;
    public AudioSource soundRPM6000;
    public bool selfTest = true;
    private bool _risingRev = true;
    private float _currentRPM = 0.0f;
    public float _selfTestFactor = 10.0f;
    public float pitchRange = 0.3f;
    public float maxHorsePower = 200;
    public BikeBoost _boost;
    public BikeSlowDown slowdown;

    public int currentGearPosition = 0;
    public int totalGears = 6;

    public float volume = 1.0f;

    private bool _isStarted = false;
    public bool isStarted {
        get { return _isStarted;}
        set { 
            if (_isStarted != value) {
                _isStarted = value;
                if (_isStarted) {
                    _currentRPM = 1000.0f;
                    for (int i=0;i<6;i++) {
                        rpmSounds[i].enabled = true;
                    }
                } else {
                    _currentRPM = 0.0f;
                    for (int i=0;i<6;i++) {
                        rpmSounds[i].enabled = false;
                    }
                }
            }
        }
    }

    private AudioSource[] rpmSounds;
    private int[] _rpmSamples = new int[] { 1000, 2000, 3000, 4000, 5000, 6000};

    public int rpm;

    private float _currentThrottle = 0.0f;
    private float _currentMaxRPM;
    public float increaseRevFactor = 500;
    public float decreaseRevFactor = 400;

	// Use this for initialization
	void Start () {
        rpmSounds = new AudioSource[6];
        rpmSounds[0] = soundRPM1000;
        rpmSounds[1] = soundRPM2000;
        rpmSounds[2] = soundRPM3000;
        rpmSounds[3] = soundRPM4000;
        rpmSounds[4] = soundRPM5000;
        rpmSounds[5] = soundRPM6000;

        for (int i=0;i<6;i++) {
            rpmSounds[i].enabled = false;
        }
	}

    public float GetCurrentPower() {
        if (currentGearPosition == 0) {
            return 0.0f;
        } else {
            return (_currentRPM - 1000.0f) / 5000.0f * maxHorsePower;
        }
    }

    public void SetThrottle(float t) {
        _currentThrottle = t;
        if (_currentThrottle > 1.0f) 
            _currentThrottle = 1.0f;

        if (_currentThrottle < 0.0f)
            _currentThrottle = 0.0f;

        _currentMaxRPM = 1000.0f + 5000.0f * _currentThrottle;
    }
	
	// Update is called once per frame
	void Update () {

        if (networkView.isMine) {

            float targetRPM = _currentMaxRPM;
            float factor = increaseRevFactor;

            if (_boost.isBoosting) {
                targetRPM = 6000;
                factor *= 3.0f;
            }

            if (slowdown.shouldSlowdown) {
                targetRPM = Math.Min(slowdown.rpmLimit, targetRPM);
            }

            if (_currentRPM < targetRPM) {
                _currentRPM += Time.deltaTime * factor;
            } else if ( _currentRPM > targetRPM ) {
                _currentRPM -= Time.deltaTime * factor;
            }

            _currentRPM = Math.Min(Math.Max(1000.0f, _currentRPM), 6000.0f);
        }

        rpm = (int)_currentRPM;

        for (int i=0;i<6;i++) {
            float dist = rpm - _rpmSamples[i];
            rpmSounds[i].volume = volume * (1000.0f - Math.Abs(dist)) / 1000.0f;

            float pitch = 1.0f + dist / 1000.0f * pitchRange;
            if (pitch > 1.0f + pitchRange)
                pitch = 1.0f + pitchRange;

            if (pitch < 1.0f - pitchRange)
                pitch = 1.0f - pitchRange;

            rpmSounds[i].pitch = pitch;
        }
	}

	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info) {
		if (stream.isWriting) {
			float r = _currentRPM;
			stream.Serialize(ref r);
		} else {
			float r = 0;
			stream.Serialize(ref r);
			_currentRPM = r;
		}
	}
}
