using System.ComponentModel.DataAnnotations;
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
    }
}
