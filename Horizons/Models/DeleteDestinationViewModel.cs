namespace Horizons.Models
{
    public class DeleteDestinationViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string PublisherId { get; set; } = null!;

        public string Publisher { get; set; } = null!;
    }
}
