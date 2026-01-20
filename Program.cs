using AppointmentScheduler;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var token = Environment.GetEnvironmentVariable("BREVIUM_TOKEN");
        if (string.IsNullOrWhiteSpace(token))
        {
            Console.WriteLine("Set BREVIUM_TOKEN");
            return;
        }

        var api = new ApiClient("https://scheduling.interviews.brevium.com", token);
        await api.StartAsync();

        // MUST call only once per run
        var initial = await api.GetInitialScheduleAsync();
        var scheduler = new Scheduler(initial);
        int scheduled = 0;

        while (true)
        {
            var req = await api.GetNextRequestAsync();
            if (req is null) break;

            var booking = scheduler.Schedule(req);

            await api.PostScheduleAsync(booking);
            scheduler.ApplyBooking(booking);

            scheduled++;
            Console.WriteLine($"Scheduled #{scheduled}: req={booking.RequestId}, person={booking.PersonId}, doc={booking.DoctorId}, time={booking.AppointmentTime:O}");
        }
        await api.StopAsync();
        Console.WriteLine($"Done. Scheduled {scheduled} appointments.");
    }
}