using UnityEngine;
using System.Collections;

public class ItemBoost : AbstractItem {

    public ItemBoost(bool singleUse, int count) {
        _typeId = ItemType.BoostX1;
        _name = "Boost";
        _singleUse = singleUse;
        _count = count;
    }

    public override void Use(Bike bike) {
        bike.boost.StartBoost();
        _count--;
    }

    public static ItemBoost CreateX1() {
        return new ItemBoost(true, 1);
    }

    public static ItemBoost CreateX3() {
        return new ItemBoost(false, 3);
    }
}
