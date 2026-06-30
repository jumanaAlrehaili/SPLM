namespace Domain.Lookups
{
    public static class LeadRoleLookup
    {
        public static readonly LeadRoleInfo BusinessAnalystLead = new(1, "BA Lead",    StageId: 1, AssigneeRoleId: 2);
        public static readonly LeadRoleInfo SystemAnalystLead   = new(2, "SA Lead",    StageId: 2, AssigneeRoleId: 3);
        public static readonly LeadRoleInfo UIUXDesignerLead    = new(3, "UI/UX Lead", StageId: 3, AssigneeRoleId: 4);
        public static readonly LeadRoleInfo DeveloperLead       = new(4, "Dev Lead",   StageId: 4, AssigneeRoleId: 5);
        public static readonly LeadRoleInfo QualityAssuranceLead = new(5, "QA Lead",   StageId: 5, AssigneeRoleId: 6);
        public static readonly LeadRoleInfo UATLead              = new(6, "UAT Lead",  StageId: 6, AssigneeRoleId: 1);

        public static IEnumerable<LeadRoleInfo> All =>
        [
            BusinessAnalystLead, SystemAnalystLead, UIUXDesignerLead,
            DeveloperLead, QualityAssuranceLead, UATLead
        ];
    }

    public record LeadRoleInfo(int Id, string Name, int StageId, int AssigneeRoleId);
}
