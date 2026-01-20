package model;

import java.time.Instant;

public class AppointmentInfoRequest {
    public int requestId;
    public int doctorId;
    public int personId;
    public Instant appointmentTime;
    public boolean isNewPatientAppointment;

    public String toJson() {
        return "{"
                + "\"requestId\":" + requestId + ","
                + "\"doctorId\":" + doctorId + ","
                + "\"personId\":" + personId + ","
                + "\"appointmentTime\":\"" + appointmentTime.toString() + "\","
                + "\"isNewPatientAppointment\":" + isNewPatientAppointment
                + "}";
    }
}
