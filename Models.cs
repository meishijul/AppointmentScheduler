using System.Text.Json;
using System.Text.Json.Serialization;

namespace AppointmentScheduler;

// Swagger AppointmentInfo
public sealed class AppointmentInfo
{
    [JsonPropertyName("doctorId")] public int DoctorId { get; set; }
    [JsonPropertyName("personId")] public int PersonId { get; set; }
    [JsonPropertyName("appointmentTime")] public DateTimeOffset AppointmentTime { get; set; } 
    [JsonPropertyName("isNewPatientAppointment")] public bool IsNewPatientAppointment { get; set; }
}

// Swagger AppointmentInfoRequest
public sealed class AppointmentInfoRequest
{
    [JsonPropertyName("requestId")] public int RequestId { get; set; }
    [JsonPropertyName("doctorId")] public int DoctorId { get; set; }
    [JsonPropertyName("personId")] public int PersonId { get; set; }
    [JsonPropertyName("appointmentTime")] public DateTimeOffset AppointmentTime { get; set; }
    [JsonPropertyName("isNewPatientAppointment")] public bool IsNewPatientAppointment { get; set; }
}

// Swagger AppointmentRequest
public sealed class AppointmentRequest
{
    [JsonPropertyName("requestId")] public int RequestId { get; set; }
    [JsonPropertyName("personId")] public int PersonId { get; set; }
    [JsonPropertyName("preferredDays")] public List<JsonElement>? PreferredDays { get; set; }
    [JsonPropertyName("preferredDocs")] public List<Doctor>? PreferredDocs { get; set; }
    [JsonPropertyName("isNew")] public bool IsNew { get; set; }
}

// Swagger Doctor
public enum Doctor
{
    Doctor1 = 1,
    Doctor2 = 2,
    Doctor3 = 3
}
