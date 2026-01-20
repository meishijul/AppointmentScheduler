using System.Text.Json;
namespace AppointmentScheduler;

public static class PreferredDays
{
    public static List<DateOnly> ParsePreferredDates(List<JsonElement>? preferredDays)
    {
        List<DateOnly> preferredDates = new List<DateOnly>();
        if (preferredDays == null || preferredDays.Count == 0)
        {
            return preferredDates;
        }

        foreach (JsonElement dayElement in preferredDays)
        {
            if (dayElement.ValueKind != JsonValueKind.String){continue;}

            string? dayText = dayElement.GetString();
            if (string.IsNullOrWhiteSpace(dayText){continue;}

            // try parsing as a date 
            if (DateOnly.TryParse(dayText, out DateOnly dateOnly))
            {
                preferredDates.Add(dateOnly);
                continue;
            }

            // parse and take the UTC date part
            if (DateTimeOffset.TryParse(dayText, out DateTimeOffset dateTime))
            {
                DateOnly dateFromUtc = DateOnly.FromDateTime(dateTime.UtcDateTime);
                preferredDates.Add(dateFromUtc);
            }
        }
        preferredDates.Sort();
        return preferredDates;
    }
}
