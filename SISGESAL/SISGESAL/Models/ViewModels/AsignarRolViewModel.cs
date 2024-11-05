using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SISGESAL.Models
{
    public class AsignarRolViewModel
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        [Display(Name = "Nombre de Usuario")]
        public string UserName { get; set; }

        [Required]
        [Display(Name = "Rol")]
        public string SelectedRole { get; set; }

        public List<string> Roles { get; set; }
    }
}
