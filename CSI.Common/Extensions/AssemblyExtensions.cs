using System;
using System.IO;
using System.Reflection;

namespace CSI.Common.Extensions;

public static class AssemblyExtensions
{
    public static string DirectoryPath(this Assembly assembly)
    {
        var filePath = new Uri(assembly.CodeBase).LocalPath;
        return Path.GetDirectoryName(filePath);
    }
}