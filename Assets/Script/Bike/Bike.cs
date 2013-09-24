using UnityEngine;
using System.Collections;
using System;

public class Bike : MonoBehaviour {

    public GameObject riderBody;
    public GameObject frontWheelCollider;
    public GameObject rearWheelCollider;
	
	public float maxSpeedZ = 1;
	public float maxSpeedX = 10;
	public float jumpSpeed = 20;
	public Transform lookAt;
	public BikeWheel frontWheel;
	public BikeWheel rearWheel;
    public ParticleEmitter hitParticleEmitter;
    
    public bool isLocal = false;
	public bool isNPC = false;
	public bool controlByNPC;
	public bool lookAtSnapGround = true;
	public float lookAtOffset = 7;

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

	private bool _shouldJump;
	private int _boostLevel;
    public float boostMaxSpeed = 30;
	private NetworkPlayer _networkPlayer;
    private int _currentTrackIndex;
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

        if (GameManager.debug) {
            gameObject.AddComponent<BikeDebug>();
        }

        _bikeSteer = GetComponent<BikeSteer>();
        _pitch = GetComponent<BikePitch>();
        _boost = GetComponent<BikeBoost>();
        _bikeCrash = GetComponent<BikeCrash>();
        _slowdown = GetComponent<BikeSlowDown>();
		//rigidbody.centerOfMass = Vector3.zero;

        if (networkView.isMine) {
            _bikeCrash.enabled = true;
            _boost.enabled = true;
            _bikeSteer.enabled = true;
            _pitch.enabled = true;
            _slowdown.enabled = true;
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
	}

    private void AddToViewController() {
        // Add this to ViewController
        GameObject go = GameObject.FindGameObjectWithTag("view-controller");
        if (go!=null) {
            GamePlay.ViewController viewController = go.GetComponent<GamePlay.ViewController>();
            viewController.AddBike(this);
        }
    }
	
	void LimitVelocity() {
		
		Vector3 v = rigidbody.velocity;
        v.z = Math.Min(v.z, maxSpeedZ * _maxSpeedMultipier);
        v.x = Math.Max(Math.Min(v.x, maxSpeedX), -maxSpeedX);
        if ( _slowdown.shouldSlowdown) {
            v.z = Math.Min(Math.Max(v.z, 0), _slowdown.speedLimit);
        }
			
		rigidbody.velocity = v;
	}
	
	void UpdateJump() {
		if (_shouldJump) {
			Vector3 v = rigidbody.velocity;
			v.y = jumpSpeed;
			rigidbody.velocity = v;
			_shouldJump = false;
		}
	}

    private void GenerateItem() {
        if (!isLocal)
            return;

        _viewController.ShowItemButton(1);
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
			if ( rearWheel.IsTouchingTheRoad() ) {
				rigidbody.AddForce(Vector3.forward*engine.GetCurrentPower());
			}
			
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
		LimitVelocity();	
	}

    public void Jump() {
        _shouldJump = true;
    }
	
	void Update () {
		if ( isLocal && lookAt ) {
			Vector3 p = transform.position;
			p.z += lookAtOffset;
			p.x = 0;
			if (lookAtSnapGround) {
				p.y = 0;
			}
			lookAt.position = p;
		}
	}

    void OnCollisionEnter(Collision collision) {

        float dot = Vector3.Dot(Vector3.up, collision.relativeVelocity.normalized);
        if ( Math.Abs(dot) > 0.2f && collision.relativeVelocity.magnitude > 5) {
            hitParticleEmitter.Emit();
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
			lookAt = GameObject.FindWithTag ("camera_look_at").transform;
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

    public void UseItem() {

        Vector3 position = transform.position;
        position.y += 0.5f;
        position.z -= 2.0f;
        Item.Spike.Use(position);


        /*
        Vector3 velocity = rigidbody.velocity;
        velocity.y = 10;
        velocity.z += 20;
        velocity.x = 0;
        Vector3 position = transform.position;
        position.y += 3;
        Item.BombController.Use(position, velocity);
        */

        /*
        Bike targetBike = _viewController.GetBikeWithRacePosition(0);
        if (targetBike==null)
            return;

        Vector3 position = transform.position;
        position.y += 6;
        MissileController.Launch(position, targetBike.transform, follow.transform);
        //GameObject clone = (GameObject)Network.Instantiate(Resources.Load("HomingMissile"), position, Quaternion.identity, 0 );
        //Missile missile = clone.GetComponent<Missile>();
        //missile.SetOwner(Network.player);
        */
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
