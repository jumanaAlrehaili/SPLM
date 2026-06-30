namespace Application.Shared.DTOs.Lookup
{
    public record StageLookupDto(int Id, string StageName, int Sequence, bool IsDefault);
    public record StatusLookupDto(int Id, string StatusName);
    public record RoleLookupDto(int Id, string Name);
    public record FeaturePriorityDto (int Id, string Name );
    public record DurationUnitDto(int Id, string Name);
    public record RequestStatusDto(int Id, string Name);
    public record ProjectMembershipStatusDto(int Id, string Name);
    public record ReleaseStageChangeTypeDto(int Id, string Name);
    public record EstimationUnitDto(int Id, string Name);

}
