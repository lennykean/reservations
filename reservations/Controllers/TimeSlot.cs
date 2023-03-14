namespace Reservations.Controllers
{
    public class TimeSlot
    {
        public DateTime Start { get; internal set; }
        public DateTime End { get; internal set; }
        public int AvailabilityWindowId { get; internal set; }
    }
}