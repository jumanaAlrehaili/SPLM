namespace Domain.Lookups
{
    public static class RoleLookup
    {
            public static readonly RoleInfo ProjectManager = new(1, "Project Manager");
            public static readonly RoleInfo BusinessAnalyst = new(2, "Business Analyst");
            public static readonly RoleInfo SystemAnalyst = new(3, "System Analyst");
            public static readonly RoleInfo UIUXDesigner = new(4, "UI/UX Designer");
            public static readonly RoleInfo Developer = new(5, "Developer");
            public static readonly RoleInfo QualityAssurance = new(6, "Quality Assurance");
            public static readonly RoleInfo Admin = new(7, "Admin");
    }

        public record RoleInfo(int Id, string Name); 
 }
