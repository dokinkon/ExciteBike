using UnityEngine;
using System.Collections;

public class Coin : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider other ) {
		if (other.gameObject.tag == "Player" ) {
            StartCoroutine(OnTouched());
        }
    }

    IEnumerator OnTouched() {
        collider.enabled = false;
        renderer.enabled = false;
        audio.Play();
        yield return new WaitForSeconds(2);
        Destroy(gameObject);
    }
    
}
