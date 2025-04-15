// 引入診斷相關的命名空間，用來取得目前的 RequestId（例如錯誤頁面會使用）
using System.Diagnostics;

// 引入 ASP.NET Core MVC 所需的基礎命名空間
using Microsoft.AspNetCore.Mvc;

// 引入專案中定義的模型，例如用來表示錯誤資訊的 ViewModel
using NZ_Walk.UI.Models;

namespace NZ_Walk.UI.Controllers
{
    // HomeController 繼承自 Controller，是 MVC 控制器的基本單位
    public class HomeController : Controller
    {
        // 宣告一個私有欄位，類型是 ILogger<HomeController>
        // 用來記錄 Log，方便除錯與監控
        private readonly ILogger<HomeController> _logger;

        // 建構子：透過建構式注入（Constructor Injection）將 ILogger<HomeController> 傳入
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger; // 將傳入的 logger 儲存至私有欄位
        }

        // GET: /Home/Index
        // 當使用者訪問網站首頁時會觸發這個動作
        public IActionResult Index()
        {
            // 回傳對應的 View（預設是 Views/Home/Index.cshtml）
            return View();
        }

        // GET: /Home/Privacy
        // 當使用者訪問隱私權政策頁面時會執行這個動作
        public IActionResult Privacy()
        {
            // 回傳對應的 View（預設是 Views/Home/Privacy.cshtml）
            return View();
        }

        // GET: /Home/Error
        // 這是錯誤處理頁面，當系統發生未處理的錯誤時會跳轉到這裡

        // ResponseCache 屬性：設定快取策略
        // Duration = 0：不快取
        // Location = None：不要快取在任何位置（例如 Proxy 或 Client）
        // NoStore = true：完全不要儲存回應
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            // 建立一個 ErrorViewModel 並將 RequestId 設定進去
            // RequestId 是用來追蹤錯誤請求的唯一 ID，有助於除錯
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });

            // `Activity.Current?.Id`：如果有目前的 Activity（診斷資訊），就用它的 ID
            // `?? HttpContext.TraceIdentifier`：如果沒有，就使用 ASP.NET Core 預設的 Trace ID
        }
    }
}
