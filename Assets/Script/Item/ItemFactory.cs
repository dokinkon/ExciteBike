using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemFactory {

    public delegate AbstractItem Creator();
    private Dictionary<int, Creator> _creators = new Dictionary<int, Creator>();

    static ItemFactory _instance = null;
    public static ItemFactory Instance {
        get {
            if (_instance == null) {
                _instance = new ItemFactory();
            }
            return _instance;
        }
    }

    public AbstractItem Create(int typeId) {
        if (_creators.ContainsKey(typeId)) {
            return _creators[typeId]();
        } 

        return null;
    }

    public void RegisterCreator(int typeId, Creator creator) {
        if (_creators.ContainsKey(typeId))
            return;

        _creators.Add(typeId, creator);
    }

    public void InitializeCreators() {
        RegisterCreator(ItemType.HomingMissileX1, ItemMissile.Create);
    }
}
