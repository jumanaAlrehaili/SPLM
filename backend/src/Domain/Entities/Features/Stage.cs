namespace Domain.Entities.Features
{
    public class Stage
    {
        public int Id { get; set; }

        public string StageName { get; set; } = string.Empty;

        public int Sequence { get; set; }

        public bool IsDefault { get; set; } = true;
    }
}
