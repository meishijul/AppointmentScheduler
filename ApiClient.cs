using System.Net;
using System.Text;
using System.Text.Json;

namespace AppointmentScheduler;

public sealed class ApiClient
{
    private readonly HttpClient http;
    private readonly string token;
    private readonly JsonSerializerOptions json = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public ApiClient(string baseUrl, string token)
    {
        http = new HttpClient
        {
            BaseAddress = new Uri(baseUrl)
        };
        this.token = token;
    }

    private string WithToken(string path)
    {
        var t = Uri.EscapeDataString(token);
        return $"{path}?token={t}";
    }

    public async Task StartAsync()
    {
        var resp = await http.PostAsync(WithToken("/api/Scheduling/Start"), null);
        resp.EnsureSuccessStatusCode();
    }

    public async Task StopAsync()
    {
        var resp = await http.PostAsync(WithToken("/api/Scheduling/Stop"), null);
        if (!resp.IsSuccessStatusCode)
            Console.WriteLine($"Stop returned {resp.StatusCode}");
    }

    public async Task<List<AppointmentInfo>> GetInitialScheduleAsync()
    {
        var resp = await http.GetAsync(WithToken("/api/Scheduling/Schedule"));
        resp.EnsureSuccessStatusCode();

        var body = await resp.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<List<AppointmentInfo>>(body, json) ?? new List<AppointmentInfo>();
    }

    public async Task<AppointmentRequest?> GetNextRequestAsync()
    {
        var resp = await http.GetAsync(WithToken("/api/Scheduling/AppointmentRequest"));

        if (resp.StatusCode is HttpStatusCode.NoContent or HttpStatusCode.NotFound)
            return null;

        if (!resp.IsSuccessStatusCode)
            return null;

        var body = await resp.Content.ReadAsStringAsync();
        if (string.IsNullOrWhiteSpace(body))
            return null;

        return JsonSerializer.Deserialize<AppointmentRequest>(body, json);
    }

    public async Task PostScheduleAsync(AppointmentInfoRequest booking)
    {
        var _json = JsonSerializer.Serialize(booking, json);

        var resp = await http.PostAsync(
            WithToken("/api/Scheduling/Schedule"),
            new StringContent(_json, Encoding.UTF8, "application/json")
        );

        if (!resp.IsSuccessStatusCode)
        {
            var body = await resp.Content.ReadAsStringAsync();
            throw new Exception($"POST /Schedule failed: {(int)resp.StatusCode} {resp.StatusCode}\n{body}");
        }
    }
}
