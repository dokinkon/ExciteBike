using UnityEngine;
using System.Collections;

public class CarManager : MonoBehaviour {

    public GameObject[] carPrefabs;
    public float spawnInterval = 5;

    GamePlay.ViewController _viewController;

	// Use this for initialization
	void Start () {
        GameObject clone = GameObject.FindGameObjectWithTag("view-controller");
        if (clone != null) {
            _viewController = clone.GetComponent<GamePlay.ViewController>();
        }

        if (Network.isServer)
            StartCoroutine(Coroutine());
	}
	
	// Update is called once per frame
	void Update () {

	
	}

    void SpanwCar(int index) {
        if (!Network.isServer) {
            return;
        }

        Bike localBike = _viewController.localBike;
        Vector3 position = localBike.transform.position;
        position.z += 20;
        position.y = 0;
        position.x = Track.GetLocationX(-1);

        GameObject clone = (GameObject)Network.Instantiate(carPrefabs[index], position, Quaternion.identity, 0 );
    }

    IEnumerator Coroutine() {
        while (true) {
            yield return new WaitForSeconds(spawnInterval);
            int count = carPrefabs.Length;
            if (count > 0) {
                int index = Utility.random.Next(count);
                SpanwCar(index);
            }
        }
    }
}
