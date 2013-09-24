using UnityEngine;
using System.Collections;

public class ItemMissile : AbstractItem {

    public ItemMissile() {
        _typeId = ItemType.HomingMissileX1; 
        _name = "Homing Missile";
    }

    public override void Use(Bike bike) {
        Bike targetBike = bike.GetAttackTarget();
        if (targetBike==null)
            return;

        Vector3 position = bike.transform.position;
        position.y += 6;
        MissileController.Launch(position, targetBike.transform, bike.follow.transform);
    }

    public static AbstractItem Create() {
        return new ItemMissile();
    }
}
