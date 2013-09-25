using UnityEngine;
using System.Collections;

public class BikeDebug : MonoBehaviour {

    public int lap;
    private Bike _bike;

    private AbstractItem bombItem;

	// Use this for initialization
	void Start () {
        _bike = GetComponent<Bike>();
	}
	
	// Update is called once per frame
	void Update () {
        lap = _bike.lap;

        if (Input.GetKeyDown(KeyCode.B)) {
            bombItem = ItemFactory.Instance.Create(ItemType.BombX1);
            bombItem.Use(_bike);
        }
	}
}
