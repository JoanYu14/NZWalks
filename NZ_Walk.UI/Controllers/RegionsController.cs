// 引入必要的命名空間
using System.Threading.Tasks;                   // 支援非同步操作 async/await
using Microsoft.AspNetCore.Mvc;                 // 提供 MVC 控制器與動作結果類別（例如 Controller、IActionResult）
using NZ_Walk.UI.Models.DTO;                    // 引入 RegionDto，方便將 API 回傳的 JSON 資料反序列化為物件
using System.Net.Http;                          // 使用 HttpClient 的基本功能
using System.Net.Http.Json;                     // 提供 ReadFromJsonAsync 方法以便直接解析 JSON

namespace NZ_Walk.UI.Controllers
{
    // 控制器類別：處理與 "Regions" 相關的畫面或資料請求
    public class RegionsController : Controller
    {
        // 建立私有唯讀欄位用來儲存 IHttpClientFactory 實例
        private readonly IHttpClientFactory _httpClientFactory;

        // 建構函式透過相依性注入（DI）方式注入 IHttpClientFactory 實例
        // IHttpClientFactory 是 ASP.NET Core 推薦的 HttpClient 使用方式，可有效管理連線池與資源
        public RegionsController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // 定義 Index 方法，當使用者瀏覽 /Regions 時會被呼叫
        // 使用 async Task<IActionResult> 代表這是一個非同步的 MVC 動作方法
        public async Task<IActionResult> Index()
        {
            // 宣告一個 List 來存放從 API 拿到的地區資料（DTO）
            List<RegionDto> response = new List<RegionDto>();

            try
            {
                // 透過工廠建立一個 HttpClient 實例（內部可能重用或管理連線）
                var client = _httpClientFactory.CreateClient();

                // 向後端 API 發送 GET 請求，取得地區資料 JSON
                var httpResponseMessage = await client.GetAsync("https://localhost:7207/api/regions");

                // 確保 API 回傳 2xx 狀態碼，否則會丟出例外（Exception）
                httpResponseMessage.EnsureSuccessStatusCode();

                // 解析回傳的 JSON 為 IEnumerable<RegionDto>，並加入 response 清單中
                var data = await httpResponseMessage.Content.ReadFromJsonAsync<IEnumerable<RegionDto>>();
                response.AddRange(data ?? Enumerable.Empty<RegionDto>());
            }
            catch (Exception ex)
            {
                // 可以在這裡紀錄錯誤訊息，避免例外直接中斷應用程式
                // 建議使用 logger：_logger.LogError(ex, "取得地區資料時發生錯誤");
            }

            // 將資料傳給 View（例如 Regions/Index.cshtml），讓前端可以渲染這些資料
            return View(response);
        }
    }
}
