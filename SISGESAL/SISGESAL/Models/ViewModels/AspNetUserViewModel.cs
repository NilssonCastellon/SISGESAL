namespace SISGESAL.Models.ViewModels
{
    public class AspNetUserViewModel
    {
        public string Id { get; set; } = null!;

        public string? UserName { get; set; }

        public string? Email { get; set; }

        public bool EmailConfirmed { get; set; }

        public bool LockoutEnabled { get; set; }

        public int AccessFailedCount { get; set; }
    }
}
