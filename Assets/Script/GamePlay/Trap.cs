using UnityEngine;
using System.Collections;

public class Trap : MonoBehaviour {

    public Transform respawn;
    public int respawnTrackIndex = -1;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider collider) {
        if (collider.gameObject.tag == "Player") {
            Bike bike = collider.gameObject.GetComponent<Bike>();
            if (bike.owner == Network.player) {
                bike.SetPositionTo(respawn.position, respawnTrackIndex);
            }

        }
    }
}
