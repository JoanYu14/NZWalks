using Microsoft.AspNetCore.Mvc;

namespace NZ_Walk2.MvcController
{
    public class HomeController : BaseController
    {
        [HttpGet]
        public async Task<IActionResult> index()
        {
            await base.SetBaseInfo();
            // 返回視圖，並傳遞所有準備好的資料
            return await Task.Run(() => View());
        }
    }
}
