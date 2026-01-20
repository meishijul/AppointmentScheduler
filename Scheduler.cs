namespace AppointmentScheduler;

public sealed class Scheduler
{
    // November to December
    private static readonly DateOnly Start = new(2021, 11, 1);
    private static readonly DateOnly End   = new(2021, 12, 31);

    // Schedule
    private readonly Dictionary<int, HashSet<DateTimeOffset>> doctorBooked = new();
    private readonly Dictionary<int, List<DateOnly>> personBookedDates = new();

    public Scheduler(List<AppointmentInfo> initial)
    {
        foreach (var a in initial)
            ApplyBooking(a.DoctorId, a.PersonId, a.AppointmentTime);
    }

    public AppointmentInfoRequest Schedule(AppointmentRequest req)
    {
        var slot = FindSlot(req);
        if (slot == null)
        {
            throw new InvalidOperationException("No available slot found.");
        }

        var doc = slot.Value.doctorId;
        var time = slot.Value.timeUtc;

        return new AppointmentInfoRequest
        {
            RequestId = req.RequestId,
            PersonId = req.PersonId,
            DoctorId = doc,
            AppointmentTime = time,
            IsNewPatientAppointment = req.IsNew
        };
    }

    // Call after successful POST
    public void ApplyBooking(AppointmentInfoRequest booking)
    {
        ApplyBooking(booking.DoctorId, booking.PersonId, booking.AppointmentTime);
    }

    private (int doctorId, DateTimeOffset timeUtc)? FindSlot(AppointmentRequest req)
    {
        List<int> docs;
        if (req.PreferredDocs != null && req.PreferredDocs.Count > 0)
            docs = req.PreferredDocs;
        else
            docs = new List<int> { 1, 2, 3 };

        var preferredDates = PreferredDays.ParsePreferredDates(req.PreferredDays);
        IEnumerable<DateOnly> dates;
        if (preferredDates.Count > 0)
        {
            dates = preferredDates;
        }
        else
        {
            dates = AllWeekdays(Start, End);
        }

        foreach (var day in dates)
        {
            if (day < Start || day > End) continue;
            if (!IsWeekday(day)) continue;

            for (int hour = 8; hour <= 16; hour++)
            {
                // new patients only 3pm and 4pm
                if (req.IsNew && hour is not (15 or 16)) continue;

                var slotUtc = new DateTimeOffset(day.Year, day.Month, day.Day, hour, 0, 0, TimeSpan.Zero);

                // doctor can have only 1 appt per hour
                // patient appts must be >= 7 days apart
                if (!PatientOk(req.PersonId, day)) continue;

                foreach (var doc in docs)
                {
                    if (DoctorFree(doc, slotUtc))
                        return (doc, slotUtc);
                }
            }
        }
        return null;
    }

    private bool DoctorFree(int doctorId, DateTimeOffset slotUtc)
    {
        if (!doctorBooked.TryGetValue(doctorId, out var bookedTimes))
            return true;

        return !bookedTimes.Contains(slotUtc);
    }

    private bool PatientOk(int personId, DateOnly candidateDate)
    {
        if (!personBookedDates.TryGetValue(personId, out var dates)) return true;
        foreach (var d in dates)
            if (Math.Abs(d.DayNumber - candidateDate.DayNumber) < 7) return false;

        return true;
    }

    private void ApplyBooking(int doctorId, int personId, DateTimeOffset timeUtc)
    {
        if (!doctorBooked.TryGetValue(doctorId, out var set))
        {
            set = new HashSet<DateTimeOffset>();
            doctorBooked[doctorId] = set;
        }
        set.Add(timeUtc);

        var date = DateOnly.FromDateTime(timeUtc.UtcDateTime);
        if (!personBookedDates.TryGetValue(personId, out var list))
        {
            list = new List<DateOnly>();
            personBookedDates[personId] = list;
        }
        list.Add(date);
    }

    private static bool IsWeekday(DateOnly d)
    {
        return d.DayOfWeek != DayOfWeek.Saturday &&
            d.DayOfWeek != DayOfWeek.Sunday;
    }
    private static IEnumerable<DateOnly> AllWeekdays(DateOnly start, DateOnly end)
    {
        for (var d = start; d <= end; d = d.AddDays(1))
            if (IsWeekday(d)) yield return d;
    }
}
