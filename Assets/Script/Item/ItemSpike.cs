using UnityEngine;
using System.Collections;

public class ItemSpike : AbstractItem {

    public ItemSpike() {
        _typeId = ItemType.Spike; 
        _name = "Spike";
    }

    public override void Use(Bike bike) {
        Vector3 position = bike.transform.position;
        position.x = Track.GetLocationX(Track.GetIndex(position.x));
        position.y += 0.5f;
        position.z -= 2.0f;

        GameObject clone;
        if (Network.isClient || Network.isServer) {
            clone = (GameObject)Network.Instantiate(Resources.Load("Spike"), position, Quaternion.identity, 0);
        } else {
            clone = (GameObject)GameObject.Instantiate(Resources.Load("Spike"), position, Quaternion.identity);
        }
    }

    public static AbstractItem Create() {
        return new ItemSpike();
    }

}
