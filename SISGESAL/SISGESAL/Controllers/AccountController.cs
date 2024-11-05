using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Threading.Tasks;

namespace SISGESAL.Controllers
{
    public class AccountController : Controller
    {
        private readonly IEmailSender _emailSender;

        public AccountController(IEmailSender emailSender)
        {
            _emailSender = emailSender;
        }

        // Acción para enviar el correo de prueba
        public async Task<IActionResult> SendTestEmail()
        {
            await _emailSender.SendEmailAsync("recipient@example.com", "Test Email", "This is a test email.");
            return View();
        }
    }
}
