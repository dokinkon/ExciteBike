using UnityEngine;
using System.Collections;

public class SuperStar : MonoBehaviour {

    public int duration = 5;

	// Use this for initialization
	void Start () {
        StartCoroutine(Coroutine());
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    IEnumerator Coroutine() {
        yield return new WaitForSeconds(duration);
        if (Network.isServer || Network.isClient) {
            if (networkView.isMine) {
                Network.Destroy(gameObject);
            }
        } else {
            Destroy(gameObject);
        }
    }
}
