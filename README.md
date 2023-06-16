# SharpIpp
![SharpIpp Icon][SharpIpp.icon]
</br>
C# implementation of [Internet Printing Protocol/1.1](https://tools.ietf.org/html/rfc2911) and some bits of [CUPS 1.0](http://www.cups.org/doc/spec-ipp.html).
It can print! And do other stuff with any printer, connected via Internet.
It can also be used to create IPP server.

## Installation
Available on [nuget][SharpIpp.nuget]
```bash
PM> Install-Package SharpIpp
```

### It prints
```csharp
await using var stream = File.Open(@"c:\file.pdf", FileMode.Open);
var printerUri = new Uri("ipp://192.168.0.1:631");
var request = new PrintJobRequest
{
    PrinterUri = printer,
    Document = stream,
    DocumentAttributes = new DocumentAttributes
    {
        DocumentName = "Document Name",
        DocumentFormat = "application/octet-stream",
        Compression = Compression.None,
        DocumentNaturalLanguage = "en",
    },
    NewJobAttributes = new NewJobAttributes
    {
        Copies = 1,
        MultipleDocumentHandling = MultipleDocumentHandling.SeparateDocumentsCollatedCopies,
        JobName = "Test Job",
        IppAttributeFidelity = false,
        Finishings = Finishings.None,
        PageRanges = new[] { new SharpIpp.Protocol.Models.Range( 1, 1 ) },
        Sides = Sides.OneSided,
        NumberUp = 1,
        OrientationRequested = Orientation.Portrait,
        PrinterResolution = new Resolution( 600, 600, ResolutionUnit.DotsPerInch ),
        PrintQuality = PrintQuality.Normal
    }
};
var response = await client.PrintJobAsync(request);
```

## Supported operations
### IPP 1.1
- [Cancel-Job](https://tools.ietf.org/html/rfc2911#section-3.3.3) — `CancelJobAsync`
- [Create-Job](https://datatracker.ietf.org/doc/html/rfc2911#section-3.2.4) — `CreateJobAsync`
- [Get-Job-Attributes](https://datatracker.ietf.org/doc/html/rfc2911#section-3.3.4) — `GetJobAttributesAsync`
- [Get-Jobs](https://datatracker.ietf.org/doc/html/rfc2911#section-3.2.6) — `GetJobsAsync`
- [Get-Job-Attributes](https://datatracker.ietf.org/doc/html/rfc2911#section-3.3.4) — `GetPrinterAttributesAsync`
- [Hold-Job](https://datatracker.ietf.org/doc/html/rfc2911#section-3.3.5) — `HoldJobAsync`
- [Pause-Printer](https://datatracker.ietf.org/doc/html/rfc2911#section-3.2.7) — `PausePrinterAsync`
- [Print-Job](https://datatracker.ietf.org/doc/html/rfc2911#section-3.2.1) — `PrintJobAsync`
- [Print-URI](https://datatracker.ietf.org/doc/html/rfc2911#section-3.2.2) — `PrintUriAsync`
- [Purge-Jobs](https://tools.ietf.org/html/rfc2911#section-3.2.9) — `PurgeJobsAsync`
- [Release-Job](https://tools.ietf.org/html/rfc2911#section-3.3.6) — `ReleaseJobAsync`
- [Restart-Job](https://tools.ietf.org/html/rfc2911#section-3.3.7) — `RestartJobAsync`
- [Resume-Printer](https://tools.ietf.org/html/rfc2911#section-3.2.8) — `ResumePrinterAsync`
- [Send-Document](https://tools.ietf.org/html/rfc2911#section-3.3.1) — `SendDocumentAsync`
- [Send-URI](https://tools.ietf.org/html/rfc2911#section-3.3.2) — `SendUriAsync`
- [Validate-Job](https://datatracker.ietf.org/doc/html/rfc2911#section-3.2.3) — `ValidateJobAsync`

### CUPS 1.0
- [CUPS-Get-Printers](http://www.cups.org/doc/spec-ipp.html#CUPS_GET_PRINTERS) — `GetCUPSPrintersAsync`

### Custom operations
Use method `CustomOperationAsync` to send fully customizable requests to your printer.

```csharp
var request = new IppRequestMessage
{
    RequestId = 1,
    IppOperation = (IppOperation)0x000A,
    Version = IppVersion.V11
};
request.OperationAttributes.AddRange(
    new []
    {
        new IppAttribute(Tag.Charset, "attributes-charset", "utf-8"),
        new IppAttribute(Tag.NaturalLanguage, "attributes-natural-language", "en"),
        new IppAttribute(Tag.NameWithoutLanguage, "requesting-user-name", "test"),
        new IppAttribute(Tag.Uri, "printer-uri", "ipp://localhost:631"),
        new IppAttribute(Tag.Integer, "job-id", 1),
    });
client.CustomOperationAsync("ipp://localhost:631", request);
```


[SharpIpp.icon]: ipp64.png "SharpIpp Icon"
[SharpIpp.nuget]: https://www.nuget.org/packages/SharpIpp