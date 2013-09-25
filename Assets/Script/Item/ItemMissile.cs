using UnityEngine;
using System.Collections;

public class ItemMissile : AbstractItem {

    public ItemMissile(bool singleUse, int count) {
        _typeId = ItemType.HomingMissileX1; 
        _name = "Homing Missile";
        _singleUse = singleUse;
        _count = count;
    }

    public override void Use(Bike bike) {
        if (_count <= 0)
            return;
        Bike targetBike = bike.GetAttackTarget();
        if (targetBike==null)
            return;

        Vector3 position = bike.transform.position;
        position.y += 6;
        MissileController.Launch(position, targetBike.transform, bike.follow.transform);
        _count--;
    }

    public static AbstractItem CreateX1() {
        return new ItemMissile(true, 1);
    }

    public static AbstractItem CreateX3() {
        return new ItemMissile(false, 3);
    }
}
