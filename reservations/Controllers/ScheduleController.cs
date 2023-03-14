using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Reservations.Data;
using Reservations.Data.Models;

namespace Reservations.Controllers
{
    [ApiController]
    [Route("[controller]/{providerId}")]
    public class AvailabilityWindowController : ControllerBase
    {
        private readonly ReservationsContext _context;

        public AvailabilityWindowController(ReservationsContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TimeSlot>>> GetTimeSlotsAsync(string providerId)
        {
            var availabilityWindows = await _context.AvailabilityWindow
                .Include(aw => aw.Reservations)
                .Where(aw => aw.ProviderId == providerId)
                .OrderBy(aw => aw.Start)
                .ToListAsync();

            return Ok(ConvertToTimeSlots(availabilityWindows));
        }

        [HttpPost]
        public async Task<ActionResult<AvailabilityWindow>> PostAsync([FromBody] AvailabilityWindow availabilityWindow, string providerId)
        {
            // Check if there are any overlapping windows
            if (availabilityWindow.Start >= availabilityWindow.End)
                return BadRequest("The availability window must start before it ends.");

            // Check if there are any overlapping windows
            var overlapping = await _context.AvailabilityWindow
                .Where(aw => aw.ProviderId == providerId &&
                            ((availabilityWindow.Start <= aw.Start && availabilityWindow.End > aw.Start) ||
                            (availabilityWindow.Start < aw.End && availabilityWindow.End >= aw.End) ||
                            (availabilityWindow.Start > aw.Start && availabilityWindow.End < aw.End)))
                .AnyAsync();
            if (overlapping)
                return Conflict("The availability window overlaps with another availability window.");

            availabilityWindow.ProviderId = providerId;
            _context.AvailabilityWindow.Add(availabilityWindow);
            await _context.SaveChangesAsync();

            return Ok(availabilityWindow);
        }

        /// <summary>
        /// Converts a list of availability windows into a list of non-overlapping time slots.
        /// </summary>
        private IEnumerable<TimeSlot> ConvertToTimeSlots(IEnumerable<AvailabilityWindow> availabilityWindows)
        {
            // Iterate through each availability window
            foreach (var availabilityWindow in availabilityWindows)
            {
                // Set the current start and end time to the availability window's start and end time
                var currentStartTime = availabilityWindow.Start;
                var currentEndTime = availabilityWindow.End;

                // Retrieve non-expired reservations for the current availability window, ordered by start time
                var reservations = availabilityWindow.Reservations
                    .Where(r => !r.Expired)
                    .OrderBy(r => r.StartTime)
                    .ToList();

                // Iterate through each reservation in the availability window
                foreach (var reservation in reservations)
                {
                    // If the current start time is before the reservation's start time, create a new time slot
                    if (currentStartTime < reservation.StartTime)
                    {
                        yield return new TimeSlot { Start = currentStartTime, End = reservation.StartTime, AvailabilityWindowId = availabilityWindow.Id };
                    }
                    // Update the current start time to the end time of the current reservation
                    currentStartTime = reservation.EndTime;
                }

                // If the current start time is before the current end time, create a new time slot
                if (currentStartTime < currentEndTime)
                {
                    yield return new TimeSlot { Start = currentStartTime, End = currentEndTime, AvailabilityWindowId = availabilityWindow.Id };
                }
            }
        }
    }
}