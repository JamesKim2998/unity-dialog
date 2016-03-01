using System.Collections.Generic;
using System.IO;
using System.Linq;

public static class FileSystemHelper
{
    public static List<string> GetListOfFilesWithOutExtension(string directory, string pattern)
    {
        var paths = Directory.GetFiles(directory, pattern);
        return paths.Select<string, string>(Path.GetFileNameWithoutExtension).ToList();
    }
}