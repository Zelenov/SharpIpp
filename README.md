# SharpIpp
![SharpIpp Icon][SharpIpp.icon]
</br>
C# implementation of [Internet Printing Protocol/1.1](https://tools.ietf.org/html/rfc2911).
It can print! And do other stuff with any printer, connected via Internet.

`SharpIpp` is still in development. Currently available operations are
* [Print-Job](https://tools.ietf.org/html/rfc2911#section-3.2)
* [Get-Printer-Attributes](https://tools.ietf.org/html/rfc2911#section-3.2.5)
* [Get-Job-Attributes](https://tools.ietf.org/html/rfc2911#section-3.3.4)

## Installation
Available on [nuget][SharpIpp.nuget]
```bash
PM> Install-Package SharpIpp
```

## Operations

### Print-Job
Prints file (pdf or txt). Returns new job info
```csharp
await using var stream = File.Open(@"c:\file.pdf", FileMode.Open);
var printerUri = new Uri("ipp://192.168.0.1:631");
var request = new PrintJobRequest
{
    PrinterUri = printer,
    Document = stream,
    JobName = "Test Job",
    IppAttributeFidelity = false,
    DocumentName = "Document Name",
    DocumentFormat = "application/octet-stream",
    DocumentNaturalLanguage = "en",
    MultipleDocumentHandling = MultipleDocumentHandling.SeparateDocumentsCollatedCopies,
    Copies = 1,
    Finishings = Finishings.None,
    PageRanges = new[] {new Range(1, 1)},
    Sides = Sides.OneSided,
    NumberUp = 1,
    OrientationRequested = Orientation.Portrait,
    PrinterResolution = new Resolution(600, 600, ResolutionUnit.DotsPerInch),
    PrintQuality = PrintQuality.Normal
};
var response = await client.PrintJobAsync(request);
```

### Get-Printer-Attributes
Returns printer info (state, version, etc)
```csharp
var printerUri = new Uri("ipp://192.168.0.1:631");
var request = new GetPrinterAttributesRequest {PrinterUri = printerUri };
var response = await client.GetPrinterAttributesAsync(request);
```

### Get-Job-Attributes
Returns job info (state, dates, pages)
```csharp
var printerUri = new Uri("ipp://192.168.0.1:631");
var request = new GetJobAttributesRequest {PrinterUri = printer, JobId = 1};
var response = await client.GetJobAttributesAsync(request);
```

[SharpIpp.icon]: ipp64.png "SharpIpp Icon"
[SharpIpp.nuget]: https://www.nuget.org/packages/SharpIpp