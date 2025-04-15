// 定義命名空間，表示這個類別屬於 NZ_Walk.UI 專案範圍
namespace NZ_Walk.UI
{
    // 主程式類別，整個 ASP.NET Core 應用的進入點
    public class Program
    {
        // Main 方法是 C# 應用的入口，程式從這裡開始執行
        public static void Main(string[] args)
        {
            // 使用 WebApplication.CreateBuilder 建立應用程式建構器
            // 傳入 args 可接收命令列參數（例如執行時設定環境變數等）
            var builder = WebApplication.CreateBuilder(args);

            // =========================
            // 註冊服務到 DI 容器中
            // =========================

            // 加入 MVC 所需的控制器與檢視（Controller + View）
            builder.Services.AddControllersWithViews();

            // 加入 HttpClient 功能，可用來向外部 API 發送 HTTP 請求
            builder.Services.AddHttpClient();

            // =========================
            // 建立 WebApplication 實例（建構 App）
            // =========================
            var app = builder.Build();

            // =========================
            // 設定中介軟體（Middleware）管線
            // =========================

            // 如果目前不是開發環境（例如是正式環境）
            if (!app.Environment.IsDevelopment())
            {
                // 使用錯誤處理器，當例外發生時會重新導向到 /Home/Error
                app.UseExceptionHandler("/Home/Error");

                // 使用 HSTS（HTTP Strict Transport Security）
                // 這是一個安全性強化機制，預設會強制使用 HTTPS 連線 30 天
                app.UseHsts();
            }

            // 自動將 HTTP 請求重新導向為 HTTPS，提升安全性
            app.UseHttpsRedirection();

            // 啟用靜態檔案支援（例如 CSS、JS、圖片），預設會讀取 wwwroot 資料夾
            app.UseStaticFiles();

            // 啟用路由功能，讓系統能根據 URL 導向到對應的控制器與動作
            app.UseRouting();

            // 啟用授權中介軟體（例如驗證用戶的權限）
            app.UseAuthorization();

            // =========================
            //  設定 MVC 路由規則（Routing）
            // =========================
            app.MapControllerRoute(
                name: "default", // 路由的名稱
                pattern: "{controller=Home}/{action=Index}/{id?}"
            // 預設的路由格式：控制器/動作/可選的參數 id
            // 如果沒指定 URL，會預設導向 HomeController 的 Index 方法
            );

            // 啟動 Web 應用，開始接收請求
            app.Run();
        }
    }
}
