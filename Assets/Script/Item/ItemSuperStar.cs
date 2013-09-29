using UnityEngine;
using System.Collections;

public class ItemSuperStar : AbstractItem {
    public ItemSuperStar() {
        _singleUse = true;
        _count = 1;
        _name = "Super Star";
    }

    public override void Use(Bike bike) {
        GameObject clone;
        if (Network.isServer || Network.isClient) {
            clone = (GameObject)Network.Instantiate(Resources.Load("SuperStar"), Vector3.zero, Quaternion.identity, 0);
        } else {
            clone = (GameObject)GameObject.Instantiate(Resources.Load("SuperStar"));

        }

        clone.transform.parent = bike.transform;
        clone.transform.localPosition = Vector3.zero;
        clone.transform.localRotation = Quaternion.identity;
        clone.transform.localScale    = Vector3.one;
    }

    public static ItemSuperStar Create() {
        return new ItemSuperStar();
    }
}
