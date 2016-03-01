using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public static class EditorUtil
{
    private static void DrawCallButton(object thiz, MethodInfo method)
    {
        if (GUILayout.Button(method.Name))
            method.Invoke(thiz, null);
    }

    public static void DrawButtonList<T>(IEnumerable<T> enumerable, Func<T, string> toName, Action<T> callback)
    {
        foreach (var elem in enumerable)
        {
            if (GUILayout.Button(toName(elem)))
                callback(elem);
        }
    }
}
