using UnityEngine;
using System.Collections;
using System;

public class Engine : MonoBehaviour {

    public AudioSource soundRPM1000;
    public AudioSource soundRPM2000;
    public AudioSource soundRPM3000;
    public AudioSource soundRPM4000;
    public AudioSource soundRPM5000;
    public AudioSource soundRPM6000;
    public bool selfTest = true;
    private bool _risingRev = true;
    private float _rpmFloat = 1000.0f;
    public float _selfTestFactor = 10.0f;
    public float pitchRange = 0.3f;

    private AudioSource[] rpmSounds;
    private int[] _rpmSamples = new int[] { 1000, 2000, 3000, 4000, 5000, 6000};

    public int rpm;

	// Use this for initialization
	void Start () {
        rpmSounds = new AudioSource[6];
        rpmSounds[0] = soundRPM1000;
        rpmSounds[1] = soundRPM2000;
        rpmSounds[2] = soundRPM3000;
        rpmSounds[3] = soundRPM4000;
        rpmSounds[4] = soundRPM5000;
        rpmSounds[5] = soundRPM6000;
	
	}
	
	// Update is called once per frame
	void Update () {

        if (selfTest) {
            if (_risingRev) {
                _rpmFloat += Time.deltaTime * _selfTestFactor;
                if (_rpmFloat > 6000) {
                    _risingRev = false;
                }
            } else {
                _rpmFloat -= Time.deltaTime * _selfTestFactor;
                if (_rpmFloat < 1000) {
                    _risingRev = true;
                }
            }

            rpm = (int)_rpmFloat;

        }

        for (int i=0;i<6;i++) {
            float dist = rpm - _rpmSamples[i];
            rpmSounds[i].volume = (1000.0f - Math.Abs(dist)) / 1000.0f;

            float pitch = 1.0f + dist / 1000.0f * pitchRange;
            if (pitch > 1.0f + pitchRange)
                pitch = 1.0f + pitchRange;

            if (pitch < 1.0f - pitchRange)
                pitch = 1.0f - pitchRange;

            rpmSounds[i].pitch = pitch;
        }
	
	}
}
