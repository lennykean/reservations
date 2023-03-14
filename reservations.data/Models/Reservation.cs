using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reservations.Data.Models
{
    /// <summary>
    /// Represents a reservation associated with an availability window.
    /// </summary>
    public class Reservation
    {
        /// <summary>
        /// The unique identifier for the reservation.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// The client identifier.
        /// </summary>
        [Required]
        public string ClientId { get; set; }

        /// <summary>
        /// The foreign key for the associated availability window.
        /// </summary>
        [Required]
        public int AvailabilityWindowId { get; set; }

        /// <summary>
        /// The start time of the reservation in UTC.
        /// </summary>
        [Required]
        public DateTime StartTime { get; set; }

        /// <summary>
        /// The end time of the reservation in UTC.
        /// </summary>
        [Required]
        public DateTime EndTime { get; set; }

        /// <summary>
        /// The timestamp when the reservation was confirmed.
        /// </summary>
        public DateTime? ConfirmedAt { get; set; }

        /// <summary>
        /// The timestamp when the reservation was created.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// The navigation property for the associated availability window.
        /// </summary>
        [ForeignKey("AvailabilityWindowId")]
        public virtual AvailabilityWindow AvailabilityWindow { get; set; }
    }
}
