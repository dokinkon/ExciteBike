using UnityEngine;
using System.Collections;
using System.Text;
using System;

public struct Utility {

    private static System.Random _random = new System.Random();
    public static System.Random random {
        get { return _random; }
    }

	public static string RandomString(int size) {
        StringBuilder builder = new StringBuilder();
        char ch;
        for (int i = 0; i < size; i++)
        {
            ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * _random.NextDouble() + 65)));                 
            builder.Append(ch);
        }

        return builder.ToString();
    }

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
