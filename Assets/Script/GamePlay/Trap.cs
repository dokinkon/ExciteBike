using UnityEngine;
using System.Collections;

public class Trap : MonoBehaviour {

    public Transform respawn;
    public int respawnTrackIndex = -1;
    private int _lastRespawnTrackIndex = 0;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    IEnumerator MoveBikeToRespawn(Bike bike) {
        bike.engine.isStarted = false;
        yield return new WaitForSeconds(2);
        int trackIndex = respawnTrackIndex;
        if (respawnTrackIndex < 0) {
            _lastRespawnTrackIndex += 1;
            _lastRespawnTrackIndex = _lastRespawnTrackIndex % 4;
            bike.SetPositionTo(respawn.position, _lastRespawnTrackIndex);

        } else {
            bike.SetPositionTo(respawn.position, respawnTrackIndex);
        }
    }

    void OnTriggerEnter(Collider collider) {
        if (collider.gameObject.tag.Contains("player-")) {
            Bike bike = collider.gameObject.GetComponent<Bike>();
            if (bike.owner == Network.player) {
                StartCoroutine(MoveBikeToRespawn(bike));
            }
        }
    }
}
