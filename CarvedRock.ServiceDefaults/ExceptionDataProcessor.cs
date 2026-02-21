using System.Collections;
using OpenTelemetry;
using OpenTelemetry.Logs;

public sealed class ExceptionDataProcessor : BaseProcessor<LogRecord>
{
    public override void OnEnd(LogRecord record)
    {
        var ex = record.Exception;
        if (ex is null) return;

        if (ex.Data is null || ex.Data.Count == 0) return;
        var attributes = record.Attributes?.ToList() ?? [];

        foreach (DictionaryEntry entry in ex.Data)
        {
            var key = entry.Key?.ToString();
            if (string.IsNullOrWhiteSpace(key)) continue;

            attributes.Add(new KeyValuePair<string, object?>(
                $"exception.data.{key}",
                entry.Value?.ToString()));
        }

        record.Attributes = attributes;
    }
}