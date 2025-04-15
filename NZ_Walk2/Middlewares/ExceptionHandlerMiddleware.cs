// 匯入需要的命名空間
using System.Net;  // 提供 HttpStatusCode（HTTP 狀態碼）列舉

namespace NZ_Walk2.Middlewares // 定義命名空間，方便組織管理程式碼結構
{
    // 定義一個自訂的中介軟體類別，用來統一處理整個應用程式中的例外錯誤
    public class ExceptionHandlerMiddleware
    {
        // 宣告用來記錄 Log 的 ILogger 實例，支援依據類別產出對應的 Log 訊息
        private readonly ILogger<ExceptionHandlerMiddleware> _logger;

        // RequestDelegate 是 ASP.NET Core 中用來處理請求的委派（下一個中介軟體或終端邏輯）
        private readonly RequestDelegate _next;

        // 建構函式：透過 DI（相依性注入）取得 Logger 與 RequestDelegate 實例
        public ExceptionHandlerMiddleware(ILogger<ExceptionHandlerMiddleware> logger, RequestDelegate next)
        {
            _logger = logger;
            _next = next;
        }

        // 每一筆 HTTP 請求都會進入這個方法。這是中介軟體的主要邏輯處理函式
        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                // 嘗試呼叫下一個中介軟體，讓請求繼續往後傳遞（正常流程）
                await _next(httpContext);
            }
            catch (Exception ex) // 如果後續中介軟體或控制器發生例外，就會被這裡捕捉
            {
                var errorId = Guid.NewGuid(); // 產生一個唯一識別碼，用來追蹤錯誤（例如支援客服報錯用）

                // 使用 logger 紀錄詳細錯誤，包含例外內容與識別碼（方便後續查詢）
                _logger.LogError(ex, $"{errorId} : {ex.Message}");

                // 設定回應的 HTTP 狀態碼為 500（Internal Server Error）
                httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                // 設定回應的 Content-Type 為 JSON 格式
                httpContext.Response.ContentType = "application/json";

                // 建立一個錯誤訊息物件，傳回給前端
                var error = new
                {
                    Id = errorId, // 傳回錯誤識別碼
                    ErrorMessage = "發生錯誤，我們正在排查問題。" // 友善的錯誤訊息
                };

                // 透過內建方法回傳 JSON 格式的錯誤資訊
                await httpContext.Response.WriteAsJsonAsync(error);
            }
        }
    }
}
