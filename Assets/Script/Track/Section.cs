using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider))]
public class Section : MonoBehaviour {

    public delegate void IntDelegate(int i);
    public event IntDelegate OnPlayerEnterSection;

    public int index = 0;

	// Use this for initialization
	void Start () {
        BoxCollider boxCollider = GetComponent<BoxCollider>();
        boxCollider.center = new Vector3(0, 20, 128);
        boxCollider.size = new Vector3(16, 40, 256);
        boxCollider.isTrigger = true;
        gameObject.layer = Layer.Item;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider collider) {
        Debug.Log("[Track.Section.OnTriggerEnter]");
        if (collider.gameObject.tag.Contains("player-")) {
            Bike bike = collider.gameObject.GetComponent<Bike>();
            if (bike.isNPC)
                return;

            if ((bike.owner!=null) && (bike.owner==Network.player)) {
                if (OnPlayerEnterSection != null) {
                    OnPlayerEnterSection(index);
                }
            }
        }
    }
}
