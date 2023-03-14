using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Reservations.Data;
using Reservations.Data.Models;

namespace Reservations.Controllers
{
    [ApiController]
    [Route("[controller]/{providerId}")]
    public class ReservationController : Controller
    {
        private readonly ReservationsContext _context;

        public ReservationController(ReservationsContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync(string providerId)
        {
            return Ok(await _context.Reservations.Where(r => r.AvailabilityWindow.ProviderId == providerId).ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody] Reservation reservation, string providerId, [FromQuery, BindRequired] string clientId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Check if the reservation is more than 24 hours in the future
            if (reservation.StartTime <= DateTime.UtcNow.AddHours(24))
                return BadRequest("Reservations must be made at least 24 hours in advance.");

            // Check if there are any overlapping reservations for the given provider
            var overlappingReservations = await _context.Reservations
                .Where(r => r.AvailabilityWindow.ProviderId == providerId && !r.Expired &&
                            ((reservation.StartTime <= r.StartTime && reservation.EndTime > r.StartTime) ||
                            (reservation.StartTime < r.EndTime && reservation.EndTime >= r.EndTime) ||
                            (reservation.StartTime > r.StartTime && reservation.EndTime < r.EndTime)))
                .AnyAsync();
            if (overlappingReservations)
                return Conflict("The reservation overlaps with an existing reservation.");

            // Check if the reservation is within an availability window
            var availabilityWindow = await _context.AvailabilityWindow
                .Where(aw => aw.ProviderId == providerId && reservation.StartTime >= aw.Start && reservation.EndTime <= reservation.EndTime)
                .SingleOrDefaultAsync();

            if (availabilityWindow == null)
                return BadRequest("The reservation is not within an availability window.");

            reservation.ClientId = clientId;
            reservation.AvailabilityWindow = availabilityWindow;

            // Save the reservation and return
            _context.Add(reservation);
            await _context.SaveChangesAsync();

            return Ok(reservation);
        }

        [HttpPost("{reservationId}/Confirm")]
        public async Task<IActionResult> Confirm(string providerId, int reservationId, [FromQuery, BindRequired] string clientId)
        {
            var reservation = await _context.Reservations
                .FirstOrDefaultAsync(r => r.Id == reservationId && r.AvailabilityWindow.ProviderId == providerId && r.ClientId == clientId && !r.Expired);

            if (reservation == null)
                return NotFound("Reservation not found.");

            reservation.ConfirmedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Ok(reservation);
        }
    }
}
