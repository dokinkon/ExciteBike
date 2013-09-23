using UnityEngine;
using System.Collections;

public class ItemBox : MonoBehaviour {

    private bool _isHiding = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    IEnumerator HideAWhile(float seconds) {
        renderer.enabled = false;
        _isHiding = true;
        yield return new WaitForSeconds(seconds);
        _isHiding = false;
        renderer.enabled = true;
    }

    void OnTriggerEnter(Collider collider) {
        if (_isHiding)
            return;

        if (collider.tag.Contains("player")) {
            StartCoroutine(HideAWhile(2));
        }
    }
}
