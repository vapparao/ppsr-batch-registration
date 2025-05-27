

namespace Application.Features.BatchOperation.Dtos
{
    public class CreateBatchDto
    {
        public int ClientId { get; set; }
        public required string FileName { get; set; }
        public required string FileChecksum { get; set; }
    }
}
