using Horizons.Data.Models;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Configuration;

namespace Horizons.Models
{
    public class AddDestinationViewModel
    {
        [Required]
        [StringLength(80, MinimumLength = 3)]
        public string Name { get; set; } = null!;

        [Required]
        [StringLength(250, MinimumLength = 10)]
        public string Description { get; set; } = null!;

        public string? ImageUrl { get; set; } = null!;

        [Required]
        public string PublishedOn { get; set; } = DateTime.Today.ToString("dd-MM-yyyy"); 

        [Required]
        public int TerrainId { get; set; }

        public virtual IEnumerable<TerrainViewModel>? Terrains { get; set; }

    }
}
