namespace Horizons.Models
{
    public class IndexDestinationViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string? ImageUrl { get; set; } = null!;
               
        public string Terrain { get; set;} = null!;    
        
        public int FavoritesCount { get; set; }

        public bool IsPublisher { get; set; }

        public bool IsFavorite { get; set; }
    }
}
