﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

    public static List<string> GetListOfFilesWithOutExtension(string directory, string pattern)
    {
        var paths = Directory.GetFiles(directory, pattern);
        return paths.Select<string, string>(Path.GetFileNameWithoutExtension).ToList();
    }
}