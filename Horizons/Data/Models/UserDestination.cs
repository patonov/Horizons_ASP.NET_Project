using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Horizons.Data.Models
{
    [PrimaryKey(nameof(UserId), nameof(DestinationId))]
    public class UserDestination
    {
        [Required]
        public string UserId { get; set; } = null!;

        [Required]
        [ForeignKey(nameof(UserId))]
        public IdentityUser User { get; set; } = null!;

        public int DestinationId { get; set; }

        [Required]
        [ForeignKey(nameof(DestinationId))]
        public Destination Destination { get; set; } = null!;

    }
}
