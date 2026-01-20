import model.AppointmentInfo;
import model.AppointmentInfoRequest;
import model.AppointmentRequest;

import java.util.List;

public class Main {
    public static void main(String[] args) throws Exception {
        String baseUrl = "https://scheduling.interviews.brevium.com";
        String token = System.getenv("BREVIUM_TOKEN");
        if (token == null || token.isBlank()) {
            throw new IllegalStateException("Set BREVIUM_TOKEN env var first.");
        }

        ApiClient api = new ApiClient(baseUrl, token);

        api.start();
        System.out.println("Reset via /Start");

        // MUST call once per run
        List<AppointmentInfo> initial = api.getInitialSchedule();
        System.out.println("Initial appointments: " + initial.size());

        Scheduler scheduler = new Scheduler(initial);

        int count = 0;
        while (true) {
            AppointmentRequest req = api.getNextRequest();
            if (req == null) break;

            AppointmentInfoRequest booking = scheduler.schedule(req);
            api.postSchedule(booking);

            count++;
            System.out.printf("Scheduled #%d: requestId=%d personId=%d doctorId=%d time=%s new=%s%n",
                    count, booking.requestId, booking.personId, booking.doctorId,
                    booking.appointmentTime, booking.isNewPatientAppointment);
        }

        api.stop(); // optional
        System.out.println("Done. Scheduled: " + count);
    }
}
