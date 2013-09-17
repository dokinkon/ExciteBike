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
	
    public BikeEngine engine;
	
    BikeCrash _bikeCrash;
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

    public bool isFlying {
        get {
            return !frontWheel.IsTouchingTheRoad() && !rearWheel.IsTouchingTheRoad();
        }

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
	}
	
	void LimitVelocity() {
		
		Vector3 v = rigidbody.velocity;
        v.z = Math.Min(v.z, maxSpeedZ);
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

        //foreach (ContactPoint contact in collision.contacts) {
            //Debug.DrawRay(contact.point, collision.relativeVelocity + contact.point, Color.yellow, 0.5f);
        //}

        float dot = Vector3.Dot(Vector3.up, collision.relativeVelocity.normalized);
        if ( Math.Abs(dot) > 0.2f && collision.relativeVelocity.magnitude > 5) {
            hitParticleEmitter.Emit();
        }
    }

	void OnTriggerEnter(Collider other ) {
		if (other.gameObject.tag == "accelerator" ) {
            //StartBoost();
		} else if ( other.gameObject.tag == "slowdown" ) {
            //Debug.Log("[Bike.OnTriggerEnter] slowdown");
			//this._shouldSlowdown = true;
		} else if ( other.gameObject.tag == "jump") {
			this._shouldJump = true;
		} else if ( other.gameObject.tag == "Finish") {
			StopEngine();
		}
	}

	void OnTriggerStay ( Collider other ) {
		if ( controlByNPC ) {
			
            /*
			if ( other.gameObject.tag == "turnleft" ) {
				_bikeSteer.TurnLeft();
			} else if ( other.gameObject.tag == "turnright") {
				_bikeSteer.TurnRight();
			} else if ( other.gameObject.tag == "tiltup" ) {
				this.TiltUp();
			}
            */
		}
	}
	
	void OnTriggerExit(Collider other) {
		
		if ( controlByNPC ) {
            /*
			if ( other.gameObject.tag == "turnleft" || other.gameObject.tag == "turnright") {
				_bikeSteer.ResetSteer();
			} else if ( other.gameObject.tag== "tiltup" ) {
				this.ResetTilt();
			}
            */
		}
		
		
		if ( other.gameObject.tag == "slowdown" ) {
			//this._shouldSlowdown = false;
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

        AudioListener listener = GetComponent<AudioListener>();
		
		if ( networkView.isMine ) {
            listener.enabled = true;

			networkRigidbody.enabled = false;
			control.enabled = true;
			lookAt = GameObject.FindWithTag ("camera_look_at").transform;
			

            networkView.RPC("SetOwner", RPCMode.All, Network.player);
				
		} else {
            listener.enabled = false;
			networkRigidbody.enabled = true;
			control.enabled = false;
			this.name += "Remote";
			
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
        position.z += 2;
        GameObject clone = (GameObject)Network.Instantiate(Resources.Load("Missile"), position, Quaternion.identity, 0 );
        Missile missile = clone.GetComponent<Missile>();
        missile.SetOwner(Network.player);
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

    public void SetCollisionLayer(int layer) {
        networkView.RPC("SyncConllisionLayer", RPCMode.AllBuffered, layer);
    }

    [RPC]
    void SyncConllisionLayer(int layer) {
        Utility.SetLayerRecursively(gameObject, layer);
    }

    [RPC]
    void SetOwner(NetworkPlayer player) {
        _owner = player;
		PlayerInfo playerInfo = GameManager.Instance.GetPlayerInfo(_owner);
        playerInfo.bike = this;
    }

    [RPC]
    void PlaySoundEffect(int effectType) {

    }

}
