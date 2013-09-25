using UnityEngine;
using System.Collections;

public class ItemBomb : AbstractItem {

    public ItemBomb (bool singleUse, int count) {
        _typeId = ItemType.BombX1;
        _name = "Bomb";
        _singleUse = singleUse;
        _count = count;
    }

    public override void Use(Bike bike) {
        if (_count <= 0)
            return;

        Vector3 position = bike.transform.position;
        position.y += 3;

        GameObject clone;
        if (Network.isServer || Network.isClient) {
            clone = (GameObject)Network.Instantiate(Resources.Load("Bomb"), position, Quaternion.identity, 0);
        } else {
            clone = (GameObject)GameObject.Instantiate(Resources.Load("Bomb"), position, Quaternion.identity);
        }

        Item.BombController controller = clone.GetComponent<Item.BombController>();
        controller.initVelocity = bike.rigidbody.velocity + new Vector3(0, 10, 20);
        _count--;
    }

    public static AbstractItem CreateX1() {
        return new ItemBomb(true, 1);
    }

    public static AbstractItem CreateX3() {
        return new ItemBomb(false, 3);
    }
}
