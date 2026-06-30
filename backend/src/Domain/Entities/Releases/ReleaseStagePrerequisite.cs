namespace Domain.Entities.Releases
{
    public class ReleaseStagePrerequisite
    {
        public int Id { get; set; }

        public int ReleaseStageId { get; set; }
        public ReleaseStage ReleaseStage { get; set; } = null!;

        public int PrerequisiteReleaseStageId { get; set; }
        public ReleaseStage PrerequisiteReleaseStage { get; set; } = null!;
    }
}
