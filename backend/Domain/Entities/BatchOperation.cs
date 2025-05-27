using Domain.Enums;

namespace Domain.Entities
{
    public class BatchOperation
    {
        public int BatchId { get; set; }
        public int ClientId { get; set; }
        public  string FileName { get; set; } = string.Empty;
        public string FileChecksum { get; set; } = string.Empty;
        public BatchStatus Status { get; set; }
        public DateTime? SubmittedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public int TotalRecords { get; set; }
        public int ValidRecords { get; set; }
        public int InvalidRecords { get; set; }
        public int AddedRecords { get; set; }
        public int UpdatedRecords { get; set; }
        public ICollection<Registration> Registration { get; set; } = new List<Registration>();
    }
}
