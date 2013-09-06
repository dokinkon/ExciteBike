using UnityEngine;
using System.Collections;
using System;

public class Bike : MonoBehaviour {
	
	public float horsePower = 30;
	public float maxSpeedZ = 1;
	public float maxSpeedX = 10;
	public float jumpSpeed = 20;
	public Transform lookAt;
	public BikeWheel frontWheel;
	public BikeWheel rearWheel;
	
	public bool controlByNPC;
	public bool lookAtSnapGround = true;
	public float lookAtOffset = 7;
    public float enginePitchMax = 0.7f;
    public float enginePitchMin = 0.5f;
	
	public GameObject blobShadowPrefab;
    public ParticleEmitter boostParticleEmitter;
    public Engine engine;
    public Animation bikeAnimation;
	
	private bool _isCrashed;
	private bool _isEngineStarted;
    private BikeSteer _bikeSteer;
	private bool _shouldTiltUp;
	private bool _shouldTiltDown;
	private GameObject _blobShadow;
	private bool _shouldBoost;
	private bool _shouldSlowdown;
	private bool _shouldJump;
	private int _boostLevel;
    public float boostMaxSpeed = 30;
	public float boostDuration = 3;
	private float _boostDelay;
    private float _throttle = 0;
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

    private float _steerAxis = 0.0f;
	
	private GameObject _selfIndicator;

    private AnimationState _viberationAnimState;
    private AnimationState _leanLeftAnimState;
    private AnimationState _leanRightAnimState;
	
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

    public void SetThrottle(float t) {
        _throttle = t;
    }
	
	public void StopEngine() {
		_isEngineStarted = false;
        engine.isStarted = false;
	}
	
	public void TiltUp() {
		_shouldTiltDown = false;
		_shouldTiltUp = true;
	}
	
	public void TiltDown() {
		_shouldTiltUp = false;
		_shouldTiltDown = true;
	}
	
	public void ResetTilt() {
		_shouldTiltUp = false;
		_shouldTiltDown = false;
	}
	
	
	
	// Use this for initialization
	void Start () {
        _bikeSteer = GetComponent<BikeSteer>();
		rigidbody.centerOfMass = Vector3.zero;
    	rigidbody.maxAngularVelocity = 3;
		_runtimePlatform = Application.platform;
        _viberationAnimState = bikeAnimation["BikeViberation"];
        _leanLeftAnimState = bikeAnimation["BikeTiltLeft"];
        _leanRightAnimState = bikeAnimation["BikeTiltRight"];
        _leanLeftAnimState.layer = 10;
        _leanRightAnimState.layer = 10;
        //_leanLeftAnimState.blendMode = AnimationBlendMode.Additive;
        //_leanRightAnimState.blendMode = AnimationBlendMode.Additive;
        _leanLeftAnimState.wrapMode = WrapMode.ClampForever;
        _leanRightAnimState.wrapMode = WrapMode.ClampForever;
        _leanLeftAnimState.enabled = true;
        _leanRightAnimState.enabled = true;
        _leanLeftAnimState.weight = 1.0f;
        _leanRightAnimState.weight = 1.0f;
        _leanLeftAnimState.normalizedTime = 0;
        _leanRightAnimState.normalizedTime = 0;
        
        _viberationAnimState.wrapMode = WrapMode.Loop;
        bikeAnimation.Play("BikeViberation");
	}
	
	void FixedUpdateTilt() {

        

		Vector3 v = rigidbody.angularVelocity;
		if ( _shouldTiltUp ) {

            //rigidbody.centerOfMass = new Vector3( 0, 0.28f, -0.79f);

            float target = 300;
            float current = rigidbody.rotation.eulerAngles.x;
            float dist = target - current;

            Debug.Log("curr:" + current + " dist:" + dist);
            v.x = dist * 0.1f;

		} else if ( _shouldTiltDown ) {
			v.x = 2;
		} 
		rigidbody.angularVelocity = v;
	}
	
	void LimitVelocity() {
		
		Vector3 v = rigidbody.velocity;
		
		if ( _shouldBoost ) {
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

    IEnumerator UpdateCrashTimer(float duration) {
        yield return new WaitForSeconds(duration);
        _isCrashed = false;
        Vector3 p = rigidbody.position;
        rigidbody.rotation = Quaternion.identity;
        rigidbody.velocity = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;
        rigidbody.MovePosition(p + new Vector3(0, 1, 0));
        bikeAnimation.Stop();
    }

	void FixedUpdate() {
		if (!_isEngineStarted)
			return;

        // Test crash
        if ( !_isCrashed) {
            if ( rearWheel.IsTouchingTheRoad() || frontWheel.IsTouchingTheRoad() ) {
                float x = rigidbody.rotation.eulerAngles.x;
                while (x > 180) {
                    x -= 360;
                }
                if ( x > 85 || x < -85) {
                    _isCrashed = true;
                    StartCoroutine(UpdateCrashTimer(3.0f));
                    if (bikeAnimation != null) {
                        //bikeAnimation.Play("BikeCrash");
                    }
                }
            }
        }

		
		if ( _isCrashed ) {
            Vector3 v = rigidbody.angularVelocity;
            Debug.Log("is crashed");
            v.x = 70;
            //rigidbody.AddRelativeTorque(new Vector3(200, 0, 0));
            rigidbody.angularVelocity = v;
		} else {
			if ( rearWheel.IsTouchingTheRoad() ) {
				rigidbody.AddRelativeForce(Vector3.forward*engine.GetCurrentPower());
			}
			
			FixedUpdateTilt();
			
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
	
	void UpdateInputControlWithKeyboard() {
		if ( Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W)) {
			_bikeSteer.TurnLeft();
		} else if ( Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S) ) {
			_bikeSteer.TurnRight();
		} else {
			_bikeSteer.ResetSteer();
		}
		
		if ( Input.GetKey (KeyCode.LeftArrow) || Input.GetKey ( KeyCode.A) ) {
			TiltUp();
		} else if ( Input.GetKey (KeyCode.RightArrow) || Input.GetKey ( KeyCode.D) ) {
			TiltDown();
		} else {
			ResetTilt();
		}	

        if ( Input.GetKey(KeyCode.Space) ) {
            if (_shouldBoost)
                engine.SetThrottle(1);
            else
                engine.SetThrottle(1.0f);
        } else {
            engine.SetThrottle(0);
        }
	}
	
	void UpdateInputControlWithVirtualJoystick() {
		
		if (!_joystick) {
			_joystick = (Joystick)GameObject.FindWithTag("joystick").GetComponent("Joystick");
			if (!_joystick) {
				Debug.LogError("Can't grad joystick");
			}
		}
		
		if ( _joystick ) {
			//Debug.Log ("Joystick.x:" + _joystick.position.x);
			if (_joystick.position.x < -0.7 ) {
				TiltUp();
			} else if (_joystick.position.x > 0.7 ) {
				TiltDown();
			} else {
				ResetTilt();
			}
			
			if (_joystick.position.y < -0.7 ) {
				_bikeSteer.TurnRight();
			} else if (_joystick.position.y > 0.7 ) {
				_bikeSteer.TurnLeft();
			} else {
				_bikeSteer.ResetSteer();
			}
		}

        engine.SetThrottle(1.0f);
	}
	
	void UpdateInputControl() {
		if (!networkView.isMine)
			return;

        if (controlByNPC) {
            engine.SetThrottle(1.0f);
        } else {
            if ( _runtimePlatform == RuntimePlatform.IPhonePlayer ) {
                UpdateInputControlWithVirtualJoystick();
            } else {
                UpdateInputControlWithKeyboard();
            }
        }

        //UpdateSteerAxis();
        _leanLeftAnimState.normalizedTime = -_steerAxis;
        _leanRightAnimState.normalizedTime = _steerAxis;
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
		
		if ( _shouldBoost ) {
			_boostDelay += Time.deltaTime;
			if ( _boostDelay > boostDuration ) {
				_boostDelay = 0;
                StopBoost();
			}
		}
		
		UpdateInputControl();
		_followNode.transform.position = gameObject.transform.position;
	}

    void StartBoost() {
        _shouldBoost = true;
        if (boostParticleEmitter!=null) {
            boostParticleEmitter.emit = true;
        }
    }

    void StopBoost() {
        _shouldBoost = false;
        if (boostParticleEmitter != null) {
            boostParticleEmitter.emit = false;
        }
    }
	
	void OnTriggerEnter(Collider other ) {
		if (other.gameObject.tag == "accelerator" ) {
			//this._shouldBoost = true;
            StartBoost();
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
			
			if ( other.gameObject.tag == "turnleft" ) {
				_bikeSteer.TurnLeft();
			} else if ( other.gameObject.tag == "turnright") {
				_bikeSteer.TurnRight();
			} else if ( other.gameObject.tag == "tiltup" ) {
				this.TiltUp();
			}
		}
	}
	
	void OnTriggerExit(Collider other) {
		
		if ( controlByNPC ) {
			if ( other.gameObject.tag == "turnleft" || other.gameObject.tag == "turnright") {
				_bikeSteer.ResetSteer();
			} else if ( other.gameObject.tag== "tiltup" ) {
				this.ResetTilt();
			}
		}
		
		
		if ( other.gameObject.tag == "slowdown" ) {
			this._shouldSlowdown = false;
		}
		
	}
	
	void OnNetworkInstantiate(NetworkMessageInfo info) {
        Debug.Log("[Bike.OnNetworkInstantiate] TimeStamp:" + info.timestamp);
		
		NetworkRigidbody networkRigidbody = (NetworkRigidbody)GetComponent<NetworkRigidbody>();
		PlayerController playerController = (PlayerController)GetComponent<PlayerController>();
		_networkPlayer = info.sender;
		
		_followNode = new GameObject("FollowNode ( " + name + " )");
		
		if ( networkView.isMine ) {
			networkRigidbody.enabled = false;
			playerController.enabled = true;
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
			playerController.enabled = false;
			this.name += "Remote";
			
			// create UserPictureBillboard
			GameObject userPictureBillboardGo = (GameObject)Instantiate(Resources.Load("user-picture-billboard"));
			userPictureBillboardGo.transform.parent = _followNode.transform;
			userPictureBillboardGo.transform.localPosition = new Vector3(0, 6, 0);
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

    [RPC]
    void SetOwner(NetworkPlayer player) {
        _owner = player;
		PlayerInfo playerInfo = GameManager.Instance.GetPlayerInfo(_owner);
        playerInfo.bike = this;
    }
}
