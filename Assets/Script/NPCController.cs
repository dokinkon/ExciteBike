using UnityEngine;
using System.Collections;
using System;

public class NPCController : MonoBehaviour {

    private Bike _bike;
    private BikePitch _pitch;
    private BikeCrash _crash;
    private int _targetTrackIndex = -1;
    private bool _hasDeferredMove = false;
    private bool _freeMove = true;
    private bool _raceStarted = false;
    private bool _fakeUpdate = false;
    private float fakeSpeed = 20;
    private float _fakeUpdateDistance = 50;
    private static System.Random _random = new System.Random();
    private Bike _localBike;
    public Bike localBike {
        set { _localBike = value;}
        get { return _localBike; }
    }

	// Use this for initialization
	void Start () {
        // Debug.Log("[NPCController.Start]");
        _bike = GetComponent<Bike>();
        _pitch = GetComponent<BikePitch>();
        _crash = GetComponent<BikeCrash>();
        _bike.isNPC = true;
        _bike.follow.selfIndicator.SetActive(false);
	}

    void FakeUpdate() {
        float distance = transform.position.z - _localBike.transform.position.z;
        if (Math.Abs(distance) < _fakeUpdateDistance) {
            StopFakeUpdate();
        } else {
            float fakeSpeed = 60;
            if (distance > 0) {
                fakeSpeed = 1;
            }

            Vector3 position = transform.position;
            position.z += Time.deltaTime * fakeSpeed;
            position.y = 1;
            transform.position = position;
        }
    }

    private void StartFakeUpdate() {
        if (_fakeUpdate)
            return;
        rigidbody.detectCollisions = false;
        rigidbody.isKinematic = true;
        rigidbody.useGravity = false;
        _pitch.enabled = false;
        _crash.enabled = false;
        _bike.enabled = false;
        _fakeUpdate = true;
    }

    private void StopFakeUpdate() {
        if (!_fakeUpdate)
            return;

        // Use Raycast to test current road height
        RaycastHit hitInfo;
        Vector3 position = transform.position;
        position.y += 10;
        if (!Physics.Raycast(position, Vector3.down, out hitInfo, 45)) {
            Debug.LogError("[NPCController.StopFakeUpdate] failed to raycast to road");
            return;
        }

        position.y = hitInfo.point.y + 0.5f;
        rigidbody.detectCollisions = true;
        rigidbody.isKinematic = false;
        rigidbody.useGravity = true;
        _pitch.enabled = true;
        _crash.enabled = true;
        _bike.enabled = true;
        _bike.SetPositionTo(position, -1);
        _fakeUpdate = false;
    }

    void RealUpdate() {
        if (_fakeUpdate)
            return;

        float distance = transform.position.z - _localBike.transform.position.z;
        if (Math.Abs(distance) > _fakeUpdateDistance) {
            StartFakeUpdate();
        } else {
            _bike.engine.SetThrottle(1);

            if (!_hasDeferredMove && _freeMove) {
                StartCoroutine(DeferredMoveTo(_random.Next(4), 5000, _random.Next(5000)));
            }

            if (_targetTrackIndex != -1) {
                float x = Track.GetLocationX(_targetTrackIndex);
                float dist = transform.position.x - x;
                if (dist > 0.5f) {
                    _bike.steer.TurnLeft();
                } else if (dist < -0.5f) {
                    _bike.steer.TurnRight();
                } else {
                    _bike.steer.ResetSteer();
                    _targetTrackIndex = -1;
                }
            }

            _bike.crash.enabled = (distance > -10);

            // NPC max speed should between 50% ~ 150%
            float ratio = distance * -0.1f;
            ratio = Math.Min(Math.Max(-1, ratio), 1);
            _bike.maxSpeedMultipier = 1.0f + ratio * 0.8f;
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (!_raceStarted)
            return;

        if (_fakeUpdate) {
            FakeUpdate();
        } else {
            RealUpdate();
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
