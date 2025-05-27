
namespace Domain.Entities
{
    public class InvalidRecord
    {
        public int InvalidRecordId { get; set; }
        public int BatchId { get; set; }
        public string? RecordData { get; set; }
        public string? ErrorMessages { get; set; }
        public DateTime? CreatedAt { get; set; }

        public virtual BatchOperation? BatchOperation { get; set; }
    }
}
