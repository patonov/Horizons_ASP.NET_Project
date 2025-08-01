using System.ComponentModel.DataAnnotations;

namespace Horizons.Data.Models
{
    public class Terrain
    {
        public int Id { get; set; }
        
        [Required]
        [MaxLength(20)]
        public string Name { get; set; } = null!;

        [Required]
        public ICollection<Destination> Destinations { get; set; } = new List<Destination>();
    }
}
