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

    protected bool _singleUse = true;
    public bool singleUse {
        get { return _singleUse; }
    }

    protected int _count = 0;
    public int count {
        get { return _count; }
    }

    public virtual void Use(Bike bike) {

    }
}
