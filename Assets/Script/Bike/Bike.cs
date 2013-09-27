using UnityEngine;
using System.Collections;
using System;

public class Bike : MonoBehaviour {

    public delegate void VoidDelegate();
    public delegate void BikeDelegate(Bike bike);
    public event BikeDelegate OnStarted;
    public event BikeDelegate OnItemGot;
    public event BikeDelegate OnItemUsed;
    public event BikeDelegate OnRacePositionChanged;

    public GameObject riderBody;
    public GameObject frontWheelCollider;
    public GameObject rearWheelCollider;

    public AudioSource itemBoxSound;

    public int speedInKPH {
        get {
            return (int)Math.Abs(rigidbody.velocity.magnitude * 3.6f);
        }
    }
	
	public float maxSpeedZ = 1;
	public float maxSpeedX = 10;
	public float jumpSpeed = 20;
	//public Transform lookAt;
	public BikeWheel frontWheel;
	public BikeWheel rearWheel;
    public ParticleEmitter hitParticleEmitter;
    
    public bool isLocal = false;
	public bool isNPC = false;

    private int _jumpAvailbles = 2;
    private Vector3 _roadVector = Vector3.forward;

    private AbstractItem _currentItem = null;
    public AbstractItem currentItem {
        get { return _currentItem; }
    }

    private int _racePosition = 0;
    public int racePosition {
        get { return _racePosition; }
        set {
            if (_racePosition == value )
                return;

            _racePosition = value;
            if (OnRacePositionChanged != null) {
                OnRacePositionChanged(this);
            }
        }
    }

    private int _lap = 0;
    public int lap {
        get { return _lap; }
        set { _lap = value; }
    }

    private int _playerIndex = 0;
    public int playerIndex {
        get { return _playerIndex; }
    }
	
    public BikeEngine engine;
	
    BikeCrash _bikeCrash;
    public BikeCrash crash {
        get { return _bikeCrash; }
    }
    private BikeSlowDown _slowdown;
	private bool _isEngineStarted;

    private BikeSteer _bikeSteer;
    public BikeSteer steer {
        get { return _bikeSteer; }
    }

    private BikePitch _pitch;
    public BikePitch pitch {
        get { return _pitch; }
    }

    private BikeBoost _boost;
    public BikeBoost boost {
        get { return _boost;}
    }

    public BikeShape shape;

    private BikeFollow _follow;
    public BikeFollow follow {
        get { return _follow;}
    }

    private GamePlay.ViewController _viewController;

    public bool isFlying {
        get {
            return !frontWheel.IsTouchingTheRoad() && !rearWheel.IsTouchingTheRoad();
        }
    }

    // Max Speed Multipier. We can use this multipier to adjust max speed.
    // This value is useful to adjust race postion.
    private float _maxSpeedMultipier = 1.0f;
    public float maxSpeedMultipier {
        get { return _maxSpeedMultipier; }
        set { _maxSpeedMultipier = value; }
    }

	private bool _shouldJump = false;
    private bool _jumpedInAir = false;
	private NetworkPlayer _networkPlayer;
    private int _currentTrackIndex;
    public int trackIndex {
        get { return _currentTrackIndex; }
    }
    private int _targetTrackIndex;
    private bool _isShiftingTrack = false;
    private NetworkPlayer _owner;
    public NetworkPlayer owner {
        get { return _owner; }
    }

	public NetworkPlayer GetNetworkPlayer() {
		return _networkPlayer;
	}
	
	public void StartEngine() {
		_isEngineStarted = true;
        engine.isStarted = true;
		Debug.Log ("[Bike.StartEngine]");
	}

	public void StopEngine() {
		_isEngineStarted = false;
        engine.isStarted = false;
	}
	
	// Use this for initialization
	void Start () {

        //if (GameManager.debug) {
            //gameObject.AddComponent<BikeDebug>();
        //}

        _bikeSteer = GetComponent<BikeSteer>();
        _pitch = GetComponent<BikePitch>();
        _boost = GetComponent<BikeBoost>();
        _bikeCrash = GetComponent<BikeCrash>();
        _slowdown = GetComponent<BikeSlowDown>();

        if (networkView.isMine) {
            _bikeCrash.enabled = true;
            _boost.enabled = true;
            _bikeSteer.enabled = true;
            _pitch.enabled = true;
            _slowdown.enabled = true;

            BikeHead head = GetComponentInChildren<BikeHead>();
            head.OnHit += _bikeCrash.StartCrash;

        } else {
            _bikeCrash.enabled = false;
            _boost.enabled = false;
            _bikeSteer.enabled = false;
            _pitch.enabled = false;
            _slowdown.enabled = false;
        }
        _currentTrackIndex = Track.GetIndex(transform.position.x);
        GameObject clone = GameObject.FindGameObjectWithTag("view-controller");
        _viewController = clone.GetComponent<GamePlay.ViewController>();

        if (OnStarted!=null) {
            OnStarted(this);
        }
	}

    private void AddToViewController() {
        // Add this to ViewController
        GameObject go = GameObject.FindGameObjectWithTag("view-controller");
        if (go!=null) {
            GamePlay.ViewController viewController = go.GetComponent<GamePlay.ViewController>();
            viewController.AddBike(this);
        }
    }
	
	void UpdateJump() {
        if (!_shouldJump)
            return;

        if (isFlying) {
            if ( _jumpedInAir ) {
                _shouldJump = false;
            } else {
                _jumpedInAir = true;
            }
        } else {
            _jumpedInAir = false;
        }

		if (_shouldJump) {
			Vector3 v = rigidbody.velocity;
			v.y = jumpSpeed;
			rigidbody.velocity = v;
			_shouldJump = false;
		}
	}

    void GenerateItem() {
        if (!isLocal && !isNPC)
            return;

        itemBoxSound.Play();

        if (_currentItem!=null)
            return;

        int itemTypeId = Utility.random.Next(8);
        _currentItem = ItemFactory.Instance.Create(itemTypeId);
        if (OnItemGot!=null) {
            OnItemGot(this);
        }
    }

    void OnTriggerEnter(Collider collider) {
        if (collider.tag == "item-box") {
            GenerateItem();
        }
    }

    public void SetPositionTo(Vector3 p, int trackIndex) {
        if (!rigidbody.isKinematic) {
            rigidbody.velocity = Vector3.zero;
            rigidbody.angularVelocity = Vector3.zero;
        }
        rigidbody.rotation = Quaternion.identity;
        engine.isStarted = false;

        p.x = Track.GetLocationX(trackIndex);
        rigidbody.MovePosition(p);
        engine.isStarted = true;

        _currentTrackIndex = trackIndex;
    }

	void FixedUpdate() {

		if ( _bikeCrash.isCrashed ) {
		} else {
            if ( rearWheel.IsTouchingTheRoad() || frontWheel.IsTouchingTheRoad() ) {
                if (!_isShiftingTrack && (_bikeSteer.state == BikeSteerState.Left)) {
                    StartShiftTrack(_currentTrackIndex + 1);
                } else if (!_isShiftingTrack && (_bikeSteer.state == BikeSteerState.Right)) {
                    StartShiftTrack(_currentTrackIndex - 1);
                }
            } 
            UpdateShiftTrack();
			UpdateJump();
		}		

        // Update Velocity
        if (!_bikeCrash.isCrashed && rearWheel.IsTouchingTheRoad()) {
            float v = rigidbody.velocity.magnitude;
            v += Time.fixedDeltaTime * 27;

            if (_slowdown.shouldSlowdown) {
                v = Math.Min(v, 5.0f);
            } else if (_boost.isBoosting) {
                v = Math.Min(v, 30.0f);
            } else {
                v = Math.Min(v, 27);
            }

            rigidbody.velocity = _roadVector * v;
        }
	}

    public void Jump() {
        _shouldJump = true;
    }
	
	void Update () {
	}

    void OnCollisionEnter(Collision collision) {

        float dot = Vector3.Dot(Vector3.up, collision.relativeVelocity.normalized);
        if ( Math.Abs(dot) > 0.2f && collision.relativeVelocity.magnitude > 5) {
            hitParticleEmitter.Emit();
        }
    }

    void OnCollisionStay(Collision collision) {
        if (collision.collider.CompareTag("road")) {
            Vector3 normal = Vector3.zero;
            foreach (ContactPoint contact in collision.contacts ) {
                normal += contact.normal.normalized;
            }
            normal.x = 0;
            normal = normal.normalized;
            _roadVector = Vector3.Cross(Vector3.right, normal).normalized;
        }
    }

	void OnNetworkInstantiate(NetworkMessageInfo info) {
        Debug.Log("[Bike.OnNetworkInstantiate] TimeStamp:" + info.timestamp);
		
		NetworkRigidbody networkRigidbody = (NetworkRigidbody)GetComponent<NetworkRigidbody>();
		BikeControl control = GetComponent<BikeControl>();
		_networkPlayer = info.sender;
		
        GameObject followClone = (GameObject)Instantiate(Resources.Load("BikeFollow"));
        _follow = followClone.GetComponent<BikeFollow>();
        follow.SetBikeTransform(transform);

        BikeBoost bikeBoost = GetComponent<BikeBoost>();
        bikeBoost.boostParticleEmitter = _follow.boostParticleEmitter;

		if ( networkView.isMine ) {
			networkRigidbody.enabled = false;
			control.enabled = true;
            networkView.RPC("SetOwner", RPCMode.All, Network.player);
				
		} else {
			networkRigidbody.enabled = true;
			control.enabled = false;
			this.name += "Remote";
            _follow.selfIndicator.SetActive(false);
			
			// create UserPictureBillboard
			//GameObject userPictureBillboardGo = (GameObject)Instantiate(Resources.Load("user-picture-billboard"));
			//userPictureBillboardGo.transform.parent = _followNode.transform;
			//userPictureBillboardGo.transform.localPosition = new Vector3(0, 4, 0);
			//userPictureBillboardGo.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
			//userPictureBillboardGo.transform.localRotation = Quaternion.identity;
			//userPictureBillboardGo.renderer.material.mainTexture = playerInfo.profilePicture;
            // No Shadow
			//userPictureBillboardGo.layer = 8;
		}
    }

    public Bike GetAttackTarget() {
        return _viewController.GetBikeWithRacePosition(0);
    }

    public void UseItem() {

        if (_currentItem!=null) {
            _currentItem.Use(this);
            
            if (OnItemUsed!=null) {
                OnItemUsed(this);
            }

            if (_currentItem.count <= 0 || _currentItem.singleUse)
                _currentItem = null;
        }
    }
	
	void OnDestroy() {
        if (!GameManager.isShutingDown) {
            NetworkPlayer p = networkView.owner;
            PlayerInfo playerInfo = GameManager.Instance.GetPlayerInfo(p);
            if (playerInfo!=null ) {
                playerInfo.bike = null;
            }
        }
	}

    private void StartShiftTrack(int targetTrackIndex) {
        if (_isShiftingTrack)
            return;

        //steerParticleEmitter.emit = true;

        targetTrackIndex = Math.Min(Math.Max(0, targetTrackIndex), 3);
        if ( targetTrackIndex == _currentTrackIndex )
            return;

        _isShiftingTrack = true;
        _targetTrackIndex = targetTrackIndex;
    }

    private void UpdateShiftTrack() {
        if (!_isShiftingTrack)
            return;

        float speed = 10.0f;

        Vector3 direction;
        float targetLocationX = Track.GetLocationX(_targetTrackIndex);
        if ( rigidbody.position.x > targetLocationX ) {
            direction = new Vector3(-1, 0, 0);
        } else if ( rigidbody.position.x < targetLocationX ) {
            direction = new Vector3(1, 0, 0);
        } else {
            direction = Vector3.zero;
        }

        float distanceAbs = Math.Abs(targetLocationX - rigidbody.position.x);
        float stepAbs = Math.Abs(speed * Time.fixedDeltaTime);

        if ( stepAbs >= distanceAbs ) {
            rigidbody.MovePosition ( rigidbody.position + direction * distanceAbs );
            _isShiftingTrack = false;
            _currentTrackIndex = _targetTrackIndex;
            _bikeSteer.ResetSteer();
            //steerParticleEmitter.emit = false;

        } else {
            rigidbody.MovePosition ( rigidbody.position + direction * stepAbs );
        }
    }

    void OnRaceStarted() {

    }

    public void SetTrackIndex(int trackIndex) {
        networkView.RPC("SyncTrackIndex", RPCMode.AllBuffered, trackIndex);
    }

    [RPC]
    void SyncTrackIndex(int trackIndex) {
        gameObject.layer = Layer.BikeSensor0 + trackIndex;
        riderBody.layer = Layer.Bike0 + trackIndex;
        Utility.SetLayerRecursively(frontWheelCollider, Layer.Bike0 + trackIndex);
        Utility.SetLayerRecursively(rearWheelCollider, Layer.Bike0 + trackIndex);
        shape.SetColor(Track.GetColor(trackIndex));
        gameObject.tag = "player-" + trackIndex.ToString();
        _playerIndex = trackIndex;
        AddToViewController();
    }

    [RPC]
    void SetOwner(NetworkPlayer player) {
        _owner = player;
        if ( !isNPC ) {
            PlayerInfo playerInfo = GameManager.Instance.GetPlayerInfo(_owner);
            playerInfo.bike = this;

        }
    }
}
