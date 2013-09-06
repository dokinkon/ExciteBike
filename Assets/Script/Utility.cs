using UnityEngine;
using System.Collections;

public struct Utility {

    public static void SetLayerRecursively(GameObject go, int newLayer) {

        if (null == go) {
            return;
        }

        go.layer = newLayer;

        foreach (Transform child in go.transform)
        {
            if (null == child)
            {
                continue;
            }
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }
}
