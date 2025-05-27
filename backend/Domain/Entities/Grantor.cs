

namespace Domain.Entities
{
    public class Grantor
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string? MiddleNames { get; set; }
        public string LastName { get; set; } = string.Empty;
    }
}
