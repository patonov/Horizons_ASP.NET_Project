using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static Horizons.Common.ValidationConstants.DestinationValidationConstants;

namespace Horizons.Data.Models
{
    public class Destination
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(DestinationNameMaxLength)]
        public string Name { get; set; } = null!;

        [Required]
        [MaxLength(250)]
        public string Description { get; set; } = null!;

        public string? ImageUrl { get; set; } = null!;

        [Required]
        public string PublisherId { get; set; } = null!;

        [Required]
        public IdentityUser Publisher { get; set; } = null!;

        public DateTime PublishedOn { get; set; }

        public int TerrainId { get; set; }

        [Required]
        [ForeignKey(nameof(TerrainId))]
        public Terrain Terrain { get; set; } = null!;

        public bool IsDeleted { get; set; } = false;

        [Required]
        public ICollection<UserDestination> UsersDestinations { get; set; } = new HashSet<UserDestination>();

    }
}
