using SISGESAL.Areas.Identity.Data;

namespace SISGESAL.Models.ViewModels
{
    public class UserRoleViewModel
    {
        public ApplicationUser User { get; set; }
        public List<string> Roles { get; set; }
    }
}
