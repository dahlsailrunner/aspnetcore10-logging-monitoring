using Microsoft.Extensions.Compliance.Classification;

namespace CarvedRock.ServiceDefaults;

// https://learn.microsoft.com/en-us/dotnet/core/extensions/data-redaction
public static class SensitiveDataTypes
{
    public static string Name => "SensitiveDataTypes";

    // NOTE: use different types to trigger different redactors.  Simplify if
    // you'll only use one redactor
    public static DataClassification Private => new(Name, nameof(Private));
    public static DataClassification Public => new(Name, nameof(Public));
    public static DataClassification Personal => new(Name, nameof(Personal));
}

public sealed class PrivateAttribute : DataClassificationAttribute
{
    public PrivateAttribute() : base(SensitiveDataTypes.Private)
    {
    }
}