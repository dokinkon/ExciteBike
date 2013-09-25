using UnityEngine;
using System.Collections;

public class ItemSpike : AbstractItem {

    public ItemSpike(bool singleUse, int count) {
        _typeId = ItemType.SpikeX1; 
        _name = "Spike";
        _singleUse = singleUse;
        _count = count;
    }

    public override void Use(Bike bike) {
        if (_count <= 0)
            return;

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
        _count--;
    }

    public static AbstractItem CreateX1() {
        return new ItemSpike(true, 1);
    }

    public static AbstractItem CreateX3() {
        return new ItemSpike(false, 3);
    }
}
