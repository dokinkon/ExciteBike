using UnityEngine;
using System.Collections;
using System;
//using NetworkRigidbody;

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
	
	private bool _isCrashed;
	private bool _isEngineStarted;
	private bool _turnLeft;
	private bool _turnRight;
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

    public void SetThrottle(float t) {
        _throttle = t;
    }
	
	public void StopEngine() {
		_isEngineStarted = false;
        engine.isStarted = false;
	}
	
	public void TurnLeft() {
		_turnLeft = true;
		_turnRight = false;
	}
	
	public void TurnRight() {
		_turnRight = true;
		_turnLeft = false;
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
	
	public void ResetSteer() {
		_turnLeft = false;
		_turnRight = false;
	}
	
	
	// Use this for initialization
	void Start () {
		rigidbody.centerOfMass = Vector3.zero;
    	rigidbody.maxAngularVelocity = 3;
		_runtimePlatform = Application.platform;
	}
	
	void FixedUpdateTilt() {
		Vector3 v = rigidbody.angularVelocity;
		if ( _shouldTiltUp ) {
			v.x = -2;
		} else if ( _shouldTiltDown ) {
			v.x = 2;
		} 
		rigidbody.angularVelocity = v;
	}
	
	void LimitVelocity() {
		
		Vector3 v = rigidbody.velocity;
		
		if ( _shouldBoost ) {
			v.z = boostMaxSpeed;//maxSpeedZ + 5;
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

    void FixedUpdateSnapTrack() {
        // Snap at 1, 3, 5, 7
        Vector3 p = transform.position;
        float[] tracks = new float[]{-3.0f, -1.0f, 1.0f, 3.0f };
        int trackIndex = 0;
        float minDist = 1000.0f;
        for (int i=0;i<4;i++) {
            float dist = Math.Abs( tracks[i] - p.x);
            if (dist < minDist) {
                minDist = dist;
                trackIndex = i;
            }
        }

        if ( p.x - tracks[trackIndex] > 0.1f ) {
            TurnLeft();
        } else if ( p.x - tracks[trackIndex] < -0.1f ) {
            TurnRight();
        } else {
            ResetSteer();
        }
    }
	
	void FixedUpdate() {
		if (!_isEngineStarted)
			return;
		
		if ( _isCrashed ) {
		} else {
			if ( rearWheel.IsTouchingTheRoad() ) {
				rigidbody.AddRelativeForce(Vector3.forward*engine.GetCurrentPower());
				//rigidbody.AddRelativeForce(Vector3.forward*horsePower*_throttle);
			} else {
                //engine.SetThrottle(0);
            }
			
			FixedUpdateTilt();
			
            if ( rearWheel.IsTouchingTheRoad() || frontWheel.IsTouchingTheRoad() ) {
                if ( _turnLeft ) {
                    rigidbody.AddRelativeForce ( -Vector3.right*horsePower );
                } else if ( _turnRight ) {     
                    rigidbody.AddRelativeForce ( Vector3.right*horsePower );
                } else {
                    Vector3 v = rigidbody.velocity;
                    v.x = 0;
                    rigidbody.velocity = v;
                }
                FixedUpdateSnapTrack();
            } 

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
			TurnLeft();
		} else if ( Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S) ) {
			TurnRight();
		} else {
			ResetSteer();
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
				TurnRight();
			} else if (_joystick.position.y > 0.7 ) {
				TurnLeft();
			} else {
				ResetSteer();
			}
		}
	}
	
	void UpdateInputControl() {
		if (!networkView.isMine)
			return;
		
		if ( _runtimePlatform == RuntimePlatform.IPhonePlayer ) {
			UpdateInputControlWithVirtualJoystick();
		} else {
			UpdateInputControlWithKeyboard();
		}
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
        UpdateEngineSound();
		_followNode.transform.position = gameObject.transform.position;
	}

    void UpdateEngineSound() {

        //float v = rigidbody.velocity.z;
        //engine.rpm = (int)(Math.Abs(v) / maxSpeedZ ) * 1000 + 2000;
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
				this.TurnLeft();
			} else if ( other.gameObject.tag == "turnright") {
				this.TurnRight();
			} else if ( other.gameObject.tag == "tiltup" ) {
				this.TiltUp();
			}
		}
	}
	
	void OnTriggerExit(Collider other) {
		
		if ( controlByNPC ) {
			if ( other.gameObject.tag == "turnleft" || other.gameObject.tag == "turnright") {
				this.ResetSteer();
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
		
		PlayerInfo playerInfo = GameManager.Instance.GetPlayerInfo(networkView.owner);
		
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
			userPictureBillboardGo.renderer.material.mainTexture = playerInfo.profilePicture;
            // No Shadow
			userPictureBillboardGo.layer = 8;
		}
		playerInfo.bike = this;
    }
	
	void OnDestroy() {
		NetworkPlayer p = networkView.owner;
		PlayerInfo playerInfo = GameManager.Instance.GetPlayerInfo(p);
		if (playerInfo!=null ) {
			playerInfo.bike = null;
		}
	}
}
