using System.IO;
using System.Text.Json;

using NUnit.Framework;

namespace SharpIpp.Tests.Extensions;

public static class Test
{
    public static void AddJsonAttachment<T>(T obj, string name)
    {
        var options = new JsonSerializerOptions { WriteIndented = true, Converters = { new StreamConverter() } };
        var json = JsonSerializer.Serialize(obj, typeof(T), options);
        var filePath = GetFilePath(name);
        File.WriteAllText(filePath, json);
        TestContext.AddTestAttachment(filePath);
    }

    public static void AddBinaryAttachment(Stream stream, string name)
    {
        var filePath = GetFilePath(name);
        using var file = File.OpenWrite(filePath);
        stream.CopyTo(file);
        TestContext.AddTestAttachment(filePath);
    }

    public static void AddBinaryAttachment(byte[] bytes, string name)
    {
        var filePath = GetFilePath(name);
        File.WriteAllBytes(filePath, bytes);
        TestContext.AddTestAttachment(filePath);
    }

    private static string GetFilePath(string name)
    {
        var folder = Path.Combine(TestContext.CurrentContext.WorkDirectory,
            "output",
            TestContext.CurrentContext.Test.Name);

        if (!Directory.Exists(folder))
        {
            Directory.CreateDirectory(folder);
        }

        var filePath = Path.Combine(folder, name);
        return filePath;
    }
}
