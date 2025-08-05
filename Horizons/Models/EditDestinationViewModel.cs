using System.ComponentModel.DataAnnotations;
using System.Configuration;

namespace Horizons.Models
{
    public class EditDestinationViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(80, MinimumLength = 3)]
        public string Name { get; set; } = null!;

        [Required]
        [StringLength(250, MinimumLength = 10)]
        public string Description { get; set; } = null!;

        [Required]
        [RegexStringValidator(@"^\d{4}-\d{2}-\d{2}$")]
        public string PublishedOn { get; set; } = null!;

        public string? ImageUrl { get; set; }

        public int TerrainId { get; set; }

        [Required]
        public string PublisherId { get; set; } = null!;

        public IEnumerable<TerrainViewModel>? Terrains { get; set; }
    }
}
