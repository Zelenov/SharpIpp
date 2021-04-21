# SharpIpp
![SharpIpp Icon][SharpIpp.icon]
</br>
C# implementation of (Internet Printing Protocol/1.1)[https://tools.ietf.org/html/rfc2911]. It can print! And do other stuff with any printer, conencted via Internet.

`SharpIpp` is still in development. Currently available operations are
* (Print-Job)[https://tools.ietf.org/html/rfc2911#section-3.2]
* (Get-Printer-Attributes)[https://tools.ietf.org/html/rfc2911#section-3.2.5]

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
var request = new PrintJobRequest {PrinterUri = printerUri, Document = stream };
var response = await client.PrintJobAsync(request);
```

### Get-Printer-Attributes
Returns printer info (state, version, etc)
```csharp
var printerUri = new Uri("ipp://192.168.0.1:631");
var request = new GetPrinterAttributesRequest {PrinterUri = printerUri };
var response = await client.GetPrinterAttributesAsync(request);
```

[SharpIpp.icon]: ipp64.png "SharpIpp Icon"
[SharpIpp.nuget]: https://www.nuget.org/packages/SharpIpp