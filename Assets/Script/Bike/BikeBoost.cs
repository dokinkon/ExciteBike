using UnityEngine;
using System.Collections;

public class BikeBoost : MonoBehaviour {

    public static int TypeNormal = 0;
    public static int TypeItem   = 1;
    public static int TypeRamp   = 2;


    public delegate void VoidDelegate();
    public event VoidDelegate OnBoostBegan;
    public event VoidDelegate OnBoostEnded;

    public ParticleSystem boostParticleEmitter;
    public AudioSource soundEffect1;
    public float duration = 3.0f;
    private BikePitch _pitch;
    private float _delayTimer;
    private bool _isBoosting = false;
    public bool isBoosting {
        get { return _isBoosting; }
    }

    private bool _shouldOverwriteVelocity = false;
    public bool shouldOverwriteVelocity {
        get { return _shouldOverwriteVelocity; }
    }

    private TrailRenderer _trailRenderer;

    private Vector3 _velocity = Vector3.zero;
    public Vector3 velocity {
        get { return _velocity; }
    }

    private static Vector3 _rampVelocity = new Vector3(0, 1, 2).normalized * 30;
    private static Vector3 _normalVelocity = new Vector3(0, 0, 1) * 30;

    BikeCrash _crashHandler;

	// Use this for initialization
	void Start () {
        _crashHandler = GetComponent<BikeCrash>();
        _pitch = GetComponent<BikePitch>();
        _trailRenderer = GetComponent<TrailRenderer>();
        _trailRenderer.enabled = false;
        //_rampVelocity = new Vector3(0, 1, 2);
        //_rampVelocity = _rampVelocity.normalized * 28.0f;
	}
	
	// Update is called once per frame
	void Update () {
	}

    void OnCrashBegan() {

    }

    void StopBoost() {
        if (!_isBoosting)
            return;

        networkView.RPC("RPCBoostFinished", RPCMode.All);
        _pitch.ResetPitch();
        _isBoosting = false;
    }

	void OnTriggerEnter(Collider collider ) {

        if (!networkView.isMine)
            return;

        if (_crashHandler!=null) {
            if (_crashHandler.isCrashed)
                return;
        }

		if (collider.tag == "accelerator" ) {
            StartBoost(TypeNormal);
        } else if (collider.tag == "ramp") {
            StartBoost(TypeRamp);
        }
    }

    IEnumerator VelocityOverwrite() {
        _shouldOverwriteVelocity = true;
        yield return new WaitForSeconds(0.5f);
        _shouldOverwriteVelocity = false;
    }

    IEnumerator BoostCoroutine() {
        _pitch.PitchUp(1);
        _isBoosting = true;
        networkView.RPC("RPCBoostStarted", RPCMode.All);

        yield return new WaitForSeconds(duration);

        StopBoost();
    }

    public void StartBoost(int type) {
        if (!networkView.isMine) {
            Debug.LogError("[BikeBoost] networkView is not mine, but invoke StartBoost");
            return;
        }

        if (type == TypeNormal || type == TypeItem) {
            _velocity = _normalVelocity;
        } else if (type == TypeRamp ) {
            _velocity = _rampVelocity;
        }

        //if (_isBoosting) {
            //StopBoost();
        //}

        StartCoroutine(VelocityOverwrite());
        StartCoroutine(BoostCoroutine());
    }

    [RPC]
    void RPCBoostStarted() {
        soundEffect1.Play();
        if (boostParticleEmitter!=null) {
            boostParticleEmitter.Play(true);
        }
        _trailRenderer.enabled = true;
    }

    [RPC]
    void RPCBoostFinished() {
        if (boostParticleEmitter!=null) {
            boostParticleEmitter.Stop();
        }
        _trailRenderer.enabled = false;
    }
}
