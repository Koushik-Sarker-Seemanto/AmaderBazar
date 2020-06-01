using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebService.Controllers
{
    public class AdminPanelController: Controller
    {
        public AdminPanelController()
        {
            
        }
        
        [Authorize]
        public IActionResult Index()
        {
            return View();
        }
    }
}