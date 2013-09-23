using UnityEngine;
using System.Collections;

public class MissileController : MonoBehaviour {

    public ParticleSystem explosionEffect;
    public float maxDistance = 200;
    public float speed = 10.0f;
    private Item.Missile _missile;
    private Vector3 _initPosition;
    private Transform _target;

    private AutonomousVehicle _vehicle;

    // Steers
    private SteerForPoint _steerForPoint;
    private SteerForForward _steerForForward;


    private VehicleLookAtOverride _lookAtOverride;
	// Use this for initialization
	void Start () {
        _initPosition = transform.position;
        _vehicle = GetComponent<AutonomousVehicle>();
        _lookAtOverride = GetComponent<VehicleLookAtOverride>();
        _steerForPoint = GetComponent<SteerForPoint>();
        _steerForForward = GetComponent<SteerForForward>();
        _missile = GetComponentInChildren<Item.Missile>();

        if (networkView.isMine) {
            _vehicle.CanMove = false;
            StartCoroutine(DelayMovingMissile());
        } else {
            _vehicle.enabled = false;
            _lookAtOverride.enabled = false;
            _steerForPoint.enabled = false;
        }

	}

    IEnumerator DelayMovingMissile () {
        _vehicle.CanMove = false;
        yield return new WaitForSeconds(1);
        _vehicle.CanMove = true;
        transform.parent = null;
        _steerForForward.Weight = 0;
        _steerForPoint.Weight = 1;
    }

    IEnumerator MovingForwardSeconds(float seconds) {
        _steerForForward.Weight = 1;
        _steerForPoint.Weight = 0;
        yield return new WaitForSeconds(seconds);
        _steerForForward.Weight = 0;
        _steerForPoint.Weight = 1;
    }

	// Update is called once per frame
	void Update () {
        if (!networkView.isMine)
            return;

        if (_target != null) {
            _steerForPoint.TargetPoint = _target.position;
        } else {
            _steerForPoint.TargetPoint = new Vector3(1, 0, 1000);
        }


        if (Vector3.Distance(_initPosition, transform.position) > maxDistance) {
            Destroy(gameObject);
            return;
        }
	}

    private void Explosion() {
        _missile.Hide();
        explosionEffect.Play();
        audio.Play();
        if (networkView.isMine) {
            StartCoroutine(DelayDestroy());
        }
    }

    private IEnumerator DelayDestroy() {
        yield return new WaitForSeconds(1);
        Network.Destroy(gameObject);
    }

    void OnTriggerEnter(Collider collider) {
        Debug.Log("[Missile.OnTriggerEnter] tag:" + collider.tag);
        if ( collider.tag.Contains("player") || collider.tag == "road") {
            Explosion();
        }
    }

    void OnNetworkInstantiate(NetworkMessageInfo info) {
        if (networkView.isMine) {
        }
    }

    public static void Launch(Vector3 position, Transform target, Transform hook) {

        GameObject clone = null;
        
        if (Network.isServer || Network.isClient) {
            clone = (GameObject)Network.Instantiate(Resources.Load("HomingMissile"), 
                position,
                Quaternion.identity,
                0);

        } else {
            clone = (GameObject)Instantiate(Resources.Load("HomingMissile"), 
                position,
                Quaternion.identity);
        }

        if (clone != null) {
            MissileController controller = clone.GetComponent<MissileController>();
            controller._target = target;

            if (hook!=null) {
                clone.transform.parent = hook;
            }
        }
    }
}
