using Microsoft.AspNetCore.Mvc;

namespace NZ_Walk2.MvcController
{
    public class BaseController : Controller
    {
        protected async Task SetBaseInfo()
        {
            List<string> baseInfo = new List<string> {
                "Region",
                "Walk",
                "Login"
            };

            ViewBag.baseInfo = baseInfo;
        }
    }
}
