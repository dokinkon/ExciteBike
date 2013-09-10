using UnityEngine;
using System.Collections;
using System;

public class Bike : MonoBehaviour {
	
	public float maxSpeedZ = 1;
	public float maxSpeedX = 10;
	public float jumpSpeed = 20;
	public Transform lookAt;
	public BikeWheel frontWheel;
	public BikeWheel rearWheel;
	
	public bool controlByNPC;
	public bool lookAtSnapGround = true;
	public float lookAtOffset = 7;
	
	public GameObject blobShadowPrefab;
    public BikeEngine engine;
	
    BikeCrash _bikeCrash;
	private bool _isEngineStarted;
    private BikeSteer _bikeSteer;
    public BikeSteer steer {
        get { return _bikeSteer; }
    }
    private BikePitch _pitch;
    public BikePitch pitch {
        get { return _pitch; }
    }

    public bool isFlying {
        get {
            return !frontWheel.IsTouchingTheRoad() && !rearWheel.IsTouchingTheRoad();
        }

    }

	private GameObject _blobShadow;
    private BikeBoost _boost;
	private bool _shouldSlowdown;
	private bool _shouldJump;
	private int _boostLevel;
    public float boostMaxSpeed = 30;
	public float boostDuration = 3;
	private float _boostDelay;
	private NetworkPlayer _networkPlayer;
	private Joystick _joystick;
	private RuntimePlatform _runtimePlatform;
    private int _currentTrackIndex;
    private int _targetTrackIndex;
    private bool _isShiftingTrack = false;
    private NetworkPlayer _owner;
	private GameObject _followNode;
	public GameObject followNode {
		get { return _followNode; }
	}

	private GameObject _selfIndicator;

	
	void Awake() {
		_blobShadow = (GameObject)Instantiate (blobShadowPrefab);
	}
	
	void OnEnable() {
		Debug.Log ("[Bike.OnEnable]");
		
	}
	
	void OnDisable() {
		Destroy(_followNode);
		_followNode = null;
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
		rigidbody.centerOfMass = Vector3.zero;
		_runtimePlatform = Application.platform;
	}
	
	void LimitVelocity() {
		
		Vector3 v = rigidbody.velocity;
		
		if ( _boost.isBoosting ) {
			v.z = boostMaxSpeed;
            engine.SetThrottle(1);
            engine.rpm = 4500;
		} else {
			if ( v.z > maxSpeedZ ) {
				v.z = maxSpeedZ;
			}
		}
		
		if ( v.x > maxSpeedX ) 
			v.x = maxSpeedX;
		
		if ( v.x < -maxSpeedX ) 
			v.x = -maxSpeedX;
		
		
		if ( _shouldSlowdown ) {
			if ( v.z > 5 )
				v.z = 5;
			
			if ( v.x > 5 )
				v.x = 5;
			
			if ( v.x < -5 )
				v.x = -5;
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

	void FixedUpdate() {
		if (!_isEngineStarted)
			return;

		if ( _bikeCrash.isCrashed ) {
		} else {
			if ( rearWheel.IsTouchingTheRoad() ) {
				rigidbody.AddRelativeForce(Vector3.forward*engine.GetCurrentPower());
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
	
	void UpdateBlobShadow() {
		if ( !_blobShadow )
			return;
		
		Vector3 p = transform.position;
		p.y += 4;
		_blobShadow.transform.position = p;
	}
	
	
	// Update is called once per frame
	void Update () {
		
		UpdateBlobShadow();
		
		if ( lookAt ) {
			Vector3 p = transform.position;
			p.z += lookAtOffset;
			p.x = 0;
			if (lookAtSnapGround) {
				p.y = 0;
			}
			lookAt.position = p;
		}
		
		_followNode.transform.position = gameObject.transform.position;
	}

	void OnTriggerEnter(Collider other ) {
		if (other.gameObject.tag == "accelerator" ) {
            //StartBoost();
		} else if ( other.gameObject.tag == "slowdown" ) {
			this._shouldSlowdown = true;
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
			this._shouldSlowdown = false;
		}
		
	}
	
	void OnNetworkInstantiate(NetworkMessageInfo info) {
        Debug.Log("[Bike.OnNetworkInstantiate] TimeStamp:" + info.timestamp);
		
		NetworkRigidbody networkRigidbody = (NetworkRigidbody)GetComponent<NetworkRigidbody>();
		BikeControl control = GetComponent<BikeControl>();
		_networkPlayer = info.sender;
		
		_followNode = new GameObject("FollowNode ( " + name + " )");
		
		if ( networkView.isMine ) {
			networkRigidbody.enabled = false;
			control.enabled = true;
			lookAt = GameObject.FindWithTag ("camera_look_at").transform;
			_joystick = (Joystick)GameObject.FindWithTag ("joystick").GetComponent("Joystick");
			_selfIndicator = (GameObject)Instantiate(Resources.Load("self-indicator"));
			_selfIndicator.transform.parent = _followNode.transform;
			_selfIndicator.transform.localPosition = new Vector3(0, 3, 0);
			_selfIndicator.transform.localRotation = Quaternion.identity;
			_selfIndicator.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
			
			Quaternion q = Quaternion.identity;
			q.eulerAngles = new Vector3(90, 0, 0);
			_selfIndicator.transform.localRotation = q;

            networkView.RPC("SetOwner", RPCMode.All, Network.player);
				
		} else {
			networkRigidbody.enabled = true;
			control.enabled = false;
			this.name += "Remote";
			
			// create UserPictureBillboard
			GameObject userPictureBillboardGo = (GameObject)Instantiate(Resources.Load("user-picture-billboard"));
			userPictureBillboardGo.transform.parent = _followNode.transform;
			userPictureBillboardGo.transform.localPosition = new Vector3(0, 4, 0);
			userPictureBillboardGo.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
			userPictureBillboardGo.transform.localRotation = Quaternion.identity;
			//userPictureBillboardGo.renderer.material.mainTexture = playerInfo.profilePicture;
            // No Shadow
			userPictureBillboardGo.layer = 8;
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
        } else {
            rigidbody.MovePosition ( rigidbody.position + direction * stepAbs );
        }
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
}
