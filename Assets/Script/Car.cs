using UnityEngine;
using System.Collections;

public class Car : MonoBehaviour {

    public float speed = 10;

	// Use this for initialization
	void Start () {
        speed += Utility.random.Next(4);
	}
	
	// Update is called once per frame
	void Update () {
        float step = speed * Time.deltaTime;
        Vector3 position = transform.position;
        position.z += step;
        transform.position = position;
	}
}
