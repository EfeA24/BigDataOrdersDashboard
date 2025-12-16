using Microsoft.AspNetCore.Mvc;

namespace BigDataOrdersDashboard.ViewComponents.NavbarViewComponents
{
    public class _NavbarNotificationComponentPartial : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
