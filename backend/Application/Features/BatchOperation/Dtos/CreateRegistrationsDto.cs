using System;


namespace Application.Features.BatchOperation.Dtos
{
    public class CreateRegistrationsDto
    {
        public int BatchId { get; set; }
        public  string? FileChecksum { get; set; }
        public  List<CsvRowDto>? Rows { get; set; }
    }
}
