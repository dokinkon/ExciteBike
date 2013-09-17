using UnityEngine;
using System.Collections;
using System;

public class NPCController : MonoBehaviour {

    private Bike _bike;
    private int _targetTrackIndex = -1;
    private bool _hasDeferredMove = false;
    private bool _freeMove = true;
    private bool _raceStarted = false;
    private static System.Random _random = new System.Random();
	// Use this for initialization
	void Start () {
        //Debug.Log("[NPCController.Start]");
        _bike = GetComponent<Bike>();
        _bike.follow.selfIndicator.SetActive(false);
        AudioListener listener = GetComponent<AudioListener>();
        listener.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
        Debug.Log("[NPCController.Update1]");
        if (!_raceStarted)
            return;

        Debug.Log("[NPCController.Update2]");

        _bike.engine.SetThrottle(1);

        if (!_hasDeferredMove && _freeMove) {
            StartCoroutine(DeferredMoveTo(_random.Next(4), 5000, _random.Next(5000)));
        }

        if (_targetTrackIndex != -1) {
            float x = Track.GetLocationX(_targetTrackIndex);
            float dist = transform.position.x - x;
            if (dist > 0.5f) {
                Debug.Log("[NPCController.TurnLeft]");
                _bike.steer.TurnLeft();
            } else if (dist < -0.5f) {
                Debug.Log("[NPCController.TurnRight]");
                _bike.steer.TurnRight();
            } else {
                _bike.steer.ResetSteer();
                Debug.Log("[NPCController.ResetSteer]");
                _targetTrackIndex = -1;
            }
        }
	}

    IEnumerator DeferredMoveTo(int targetIndex, int baseDelayMS, int randomDelayMS) {
        _hasDeferredMove = true;
        float delay = baseDelayMS + _random.Next(randomDelayMS);
        delay *= 0.001f;
        yield return new WaitForSeconds(delay);
        _targetTrackIndex = targetIndex;
        _hasDeferredMove = false;
    }

    void OnTriggerEnter(Collider collider) {
        if (collider.gameObject.tag=="moveto-1") {
            //Debug.Log("[NPCController.OnTriggerEnter] moveto-1");
            _freeMove = false;
            if (_hasDeferredMove) {
                StopCoroutine("DeferredMoveTo");
            }
            StartCoroutine(DeferredMoveTo(1, 200, _random.Next(500)));
        }
    }

    void OnTriggerExit(Collider collider) {
        if (collider.gameObject.tag=="moveto-1") {
            _freeMove = true;
        }
    }

    void OnRaceStarted() {
        _bike.engine.gearPosition = 1;
        _raceStarted = true;
    }

}
