using Microsoft.AspNetCore.Identity;

namespace SISGESAL.Models.ViewModels
{
    public class AspNetRoleViewModel : IdentityRole
    {
        public string Id { get; set; }
        public string? Name { get; set; }
        public string? NormalizedName { get; set; }
        public string? ConcurrencyStamp { get; set; }
        public IEnumerable<AspNetRoleClaim> AspNetRoleClaims { get; set; } = new List<AspNetRoleClaim>();
        public IEnumerable<AspNetUser> Users { get; set; } = new List<AspNetUser>();
    }
}

