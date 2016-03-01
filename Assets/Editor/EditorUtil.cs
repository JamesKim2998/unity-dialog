using System;
using System.Collections.Generic;
using UnityEngine;

public static class EditorUtil
{
    public static void DrawButtonList<T>(IEnumerable<T> enumerable, Func<T, string> toName, Action<T> callback)
    {
        foreach (var elem in enumerable)
        {
            if (GUILayout.Button(toName(elem)))
                callback(elem);
        }
    }
}