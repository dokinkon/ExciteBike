using UnityEngine;
using System.Collections;

public class AbstractItem {

    protected int _typeId = 0;
    public int typeId {
        get { return _typeId; }
    }

    protected string _name = "";
    public string name {
        get { return _name; }
    }

    public virtual void Use(Bike bike) {

    }
}
