#nullable enable
using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SharpIpp.Tests;

public class StreamConverter : JsonConverter<Stream>
{
    public override bool HandleNull => false;

    public override Stream Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }

    public override void Write(Utf8JsonWriter writer, Stream value, JsonSerializerOptions options)
    {
        writer.WriteStringValue($"STREAM. Length {value.Length}");
    }
}
