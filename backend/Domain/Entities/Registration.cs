
namespace Domain.Entities
{
    public class Registration
    {
        public int Id { get; set; }
        public int BatchId { get; set; }
        public BatchOperation? Batch { get; set; }
        public int GrantorId { get; set; }
        public Grantor? Grantor { get; set; }

        public int SpgId { get; set; }
        public Spg? Spg { get; set; }

        public string Vin { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public string Duration { get; set; } = string.Empty;
    }
}
