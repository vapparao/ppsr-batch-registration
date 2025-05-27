
namespace Application.Features.BatchOperation.Dtos
{
    public class BatchDto
    {
        public int BatchId { get; set; }
        public int ClientId { get; set; }
        public string? FileName { get; set; }
        public string? FileChecksum { get; set; }
        public string? Status { get; set; }
        public DateTime SubmittedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public int TotalRecords { get; set; }
        public int ValidRecords { get; set; }
        public int InvalidRecords { get; set; }
        public int AddedRecords { get; set; }
        public int UpdatedRecords { get; set; }
    }
}
