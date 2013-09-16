using UnityEngine;
using System.Collections;

public class Missile : MonoBehaviour {

    public float maxDistance = 200;
    public float flyHeight = 0.8f;
    public float speed = 10.0f;
    private Vector3 _initPosition;
    private NetworkPlayer _owner;

	// Use this for initialization
	void Start () {
        _initPosition = transform.position;
	}

    void FixedUpdate() {

        RaycastHit hitInfo;
        Vector3 position = rigidbody.position;
        if (Physics.Raycast(rigidbody.position, Vector3.down, out hitInfo, 3)) {
            if ( hitInfo.collider.gameObject.tag == "road" ) {
                Vector3 normal = hitInfo.normal;
                normal.x = 0;
                normal = normal.normalized;
                position.y = hitInfo.point.y + flyHeight;
                Quaternion rotation = Quaternion.FromToRotation(Vector3.up, normal);
                rigidbody.MoveRotation(rotation);
            }
        }

        position.z += Time.fixedDeltaTime * speed;
        rigidbody.MovePosition(position);
    }
	
	// Update is called once per frame
	void Update () {
        if (Vector3.Distance(_initPosition, transform.position) > maxDistance) {
            Destroy(gameObject);
            return;
        }
	}

    void OnTriggerEnter(Collider collider) {
        if ( collider.gameObject.tag == "Player" ) {
            if (_owner != Network.player) {
                Destroy(gameObject);
            }
        }
    }

    public void SetOwner(NetworkPlayer player) {
        networkView.RPC("SyncOwner", RPCMode.All, player);
        _owner = player;
    }


    [RPC]
    void SyncOwner(NetworkPlayer player) {
        _owner = player;
    }



}
