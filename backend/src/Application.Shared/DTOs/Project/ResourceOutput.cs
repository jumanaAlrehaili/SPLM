namespace Application.Shared.DTOs.Project
{
    public class ResourceOutput
    {
        public int UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? Email { get; set; }

        public int RoleId { get; set; }
        public string? RoleName { get; set; }

        public int ProjectId { get; set; }
        public string ProjectName { get; set; } = string.Empty;
    }
}
