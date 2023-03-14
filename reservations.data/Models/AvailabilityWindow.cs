using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace Reservations.Data.Models
{
    /// <summary>
    /// Represents an availability window for a provider's schedule.
    /// </summary>
    public class AvailabilityWindow
    {
        /// <summary>
        /// The unique identifier for the availability window.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// The foreign key for the associated provider schedule.
        /// </summary>
        [Required]
        public string ProviderId { get; set; }

        /// <summary>
        /// The start of the availability window in UTC.
        /// </summary>
        [Required]
        public DateTime Start { get; set; }

        /// <summary>
        /// The end of the availability window in UTC.
        /// </summary>
        [Required]
        public DateTime End { get; set; }

        /// <summary>
        /// The timestamp when the availability window record was created.
        /// </summary>
        [Required]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// The collection of associated reservations.
        /// </summary>
        public virtual ICollection<Reservation> Reservations { get; set; } = new HashSet<Reservation>();
    }
}
