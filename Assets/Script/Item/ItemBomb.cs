using UnityEngine;
using System.Collections;

public class ItemBomb : AbstractItem {

    public ItemBomb () {
        _typeId = ItemType.BombX1;
        _name = "Bomb";
    }

    public override void Use(Bike bike) {
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
    }

    public static AbstractItem Create() {
        return new ItemBomb();
    }
}
