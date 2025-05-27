using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.BatchOperation.Dtos
{
    public class CsvRowDto
    {
        public  string? FirstName { get; set; }
        public string? MiddleNames { get; set; }
        public string? LastName { get; set; }
        public string? Vin { get; set; }
        public string? StartDate { get; set; }
        public string? Duration { get; set; }
        public string? Acn { get; set; }
        public string? OrganizationName { get; set; }
        public List<string>? Errors { get; set; }
        public bool IsValid { get; set; }

    }
}
