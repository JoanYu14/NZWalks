using System.Runtime;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NZ_Walk.Server.Data;
using NZ_Walk.Server.Mappings;
using NZ_Walk.Server.Repositories;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using NZ_Walk2.Repositories;
using Microsoft.Extensions.FileProviders;
namespace NZ_Walk.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // 建立 Web 應用程式的建構器，會自動載入 appsettings.json、環境變數、命令列參數等
            var builder = WebApplication.CreateBuilder(args);

            // 註冊服務到 DI 容器（Dependency Injection）

            builder.Services.AddControllers();
            // 將 IHttpContextAccessor 加入 DI 容器，使得其他類別（例如 Service 或 Repository）
            // 可以透過建構子注入方式，取得目前的 HttpContext（請求上下文）
            // 這在需要存取使用者資訊、Session、Headers、Request/Response 等狀況時非常有用
            builder.Services.AddHttpContextAccessor();
            // 加入 MVC Controller 支援，處理 API 請求用

            // 設定 Razor View 支援 Session TempData 提供者
            builder.Services.AddControllersWithViews().AddSessionStateTempDataProvider();

            // 自訂 Razor View 的搜尋路徑
            builder.Services.Configure<RazorViewEngineOptions>(options =>
            {
                options.ViewLocationFormats.Clear(); // 清除預設搜尋路徑
                options.ViewLocationFormats.Add("/Views/{1}/{0}" + RazorViewEngine.ViewExtension); // 控制器專屬
                options.ViewLocationFormats.Add("/Views/Shared/{0}" + RazorViewEngine.ViewExtension); // 共用檢視
            });

            builder.Services.AddHttpContextAccessor(); // 可讓服務層取得目前的 HTTP 請求內容


            builder.Services.AddEndpointsApiExplorer();
            // 用於探索 Endpoints 結構，給 Swagger/OpenAPI 使用

            builder.Services.AddSwaggerGen(options =>
            {
                // 設定 Swagger 文件的基本資訊（標題與版本）
                options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "NZ Walks API",  // API 文件標題
                    Version = "v1"           // 版本資訊
                });

                // 為 Swagger UI 增加 JWT Bearer 驗證的描述
                options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
                {
                    Name = "Authorization",                    // 要在 Header 中輸入的欄位名稱
                    In = ParameterLocation.Header,             // Token 要加在哪裡（這裡是 HTTP Header）
                    Type = SecuritySchemeType.ApiKey,          // 授權類型（這裡使用 API 金鑰模式）
                    Scheme = JwtBearerDefaults.AuthenticationScheme // 使用 Bearer JWT 的標準協定
                });

                // 告訴 Swagger：API 使用了 JWT 驗證，因此需要傳入 Token
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,              // 參考上面定義的 SecurityScheme
                                Id = JwtBearerDefaults.AuthenticationScheme       // ID 要一致，這樣才能連結起來
                            },
                            Scheme = "Oauth2",              // 雖然是 Oauth2 但這裡只是名稱，實際上我們用的是 Bearer Token
                            Name = JwtBearerDefaults.AuthenticationScheme,
                            In = ParameterLocation.Header  // 同樣是透過 HTTP Header 傳入 Token
                        },
                        new List<string>() // 這邊可以指定需要的權限範圍（scopes），我們先留空
                    }
                });
            });

            // 註冊 Swagger 產生器，用於自動產生 API 文件（Swagger UI）

            builder.Services         // ASP.NET Core 的 Service 容器
            .AddDbContext<NzWalksDbContext> // 註冊我們的資料庫上下文
            (options =>
                options.UseSqlServer(       // 指定使用 SQL Server 資料庫
                    builder.Configuration   // 從 appsettings.json 取得連線字串
                        .GetConnectionString("NzWalksConnectionString")
                )
            );

            builder.Services.AddDbContext<NzWalksAuthDbContext>
            (options =>
                options.UseSqlServer(       // 指定使用 SQL Server 資料庫
                    builder.Configuration   // 從 appsettings.json 取得連線字串
                        .GetConnectionString("NzWalksAuthConnectionString")
                )
            );

            // 註冊一個範圍生命週期的服務，當有請求時，會將 IRegionRepository 的實例化為 SQLRegionRepository。
            // 這意味著每當有服務需要 IRegionRepository 介面時，會將 SQLRegionRepository 的實作傳遞給它。
            // `AddScoped` 表示這個服務的生命週期是根據每個請求來管理的。
            // 例如，對於每一個 HTTP 請求，會為這個請求提供一個 SQLRegionRepository 的實例。
            // 當請求結束後，這個實例會被釋放。
            builder.Services.AddScoped<IRegionRepository, SQLRegionRepository>();
            builder.Services.AddScoped<IWalkRepository, SQLWalkRepository>();
            builder.Services.AddScoped<ITokenRepository, TokenRepository>() ;
            builder.Services.AddScoped<IImageRepository, LocalImageRepository>();

            builder.Services.AddAutoMapper(typeof(AutoMapperProfiles));

            // 設定 ASP.NET Core 的身分驗證服務（Identity）
            // 這邊使用 IdentityUser 作為使用者模型，搭配角色（IdentityRole）
            builder.Services.AddIdentityCore<IdentityUser>() // 註冊 Identity 的核心功能（例如註冊、登入、密碼驗證等）
                .AddRoles<IdentityRole>() // 加入角色管理功能（可建立、指派角色）
                .AddTokenProvider<DataProtectorTokenProvider<IdentityUser>>("NZWalks") // 加入 token 產生器，用於重設密碼、Email 驗證等
                .AddEntityFrameworkStores<NzWalksAuthDbContext>() // 設定 Identity 使用哪個 DbContext 來儲存使用者與角色資料
                .AddDefaultTokenProviders(); // 加入內建的 Token 產生器（例如 Email、重設密碼用的 token）


            // 設定 Identity 的驗證規則，例如密碼的強度要求
            builder.Services.Configure<IdentityOptions>(options =>
            {
                // 不強制密碼包含數字
                options.Password.RequireDigit = false;

                // 不強制密碼包含小寫字母
                options.Password.RequireLowercase = false;

                // 不強制密碼包含特殊字元（如 !@#$%^）
                options.Password.RequireNonAlphanumeric = false;

                // 不強制密碼包含大寫字母
                options.Password.RequireUppercase = false;

                // 密碼長度至少 6 個字元
                options.Password.RequiredLength = 6;

                // 密碼中至少要有 1 個獨特字元
                options.Password.RequiredUniqueChars = 1;
            });


            // 設定 ASP.NET Core 的驗證服務，指定使用 JWT Bearer 驗證方式
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)

                // 設定 JWT Bearer 的相關選項
                .AddJwtBearer(options =>

                    // 設定 Token 驗證的參數
                    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                    {
                        // 驗證 Token 的 Issuer（發行者）是否正確
                        ValidateIssuer = true,

                        // 驗證 Token 的 Audience（接收者）是否正確
                        ValidateAudience = true,

                        // 驗證 Token 是否在有效期限內（是否過期）
                        ValidateLifetime = true,

                        // 驗證 Token 的簽章是否正確（是否有被竄改）
                        ValidateIssuerSigningKey = true,

                        // 指定合法的 Token 發行者，從 appsettings.json 中取得設定值
                        ValidIssuer = builder.Configuration["Jwt:Issuer"],

                        // 指定合法的 Token 接收者，從 appsettings.json 中取得設定值
                        ValidAudience = builder.Configuration["Jwt:Audience"],

                        // 指定用來驗證簽章的金鑰，使用 appsettings.json 中的 "Jwt:Key" 值進行 UTF-8 編碼後建立對稱式金鑰
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
                    }
                );




            // 建立 Web 應用實例（會根據上面註冊的服務來建立應用程式）
            var app = builder.Build();

            // 設定中介軟體（Middleware）流程

            app.UseDefaultFiles();
            // 如果沒有指定檔案，預設會尋找 index.html（用於 SPA 前端）

            app.UseStaticFiles();
            // 啟用靜態檔案服務，支援 wwwroot 目錄下的檔案（例如 React/Vue 編譯後的前端）

            // 若處於開發環境，啟用 Swagger UI
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();       // 產生 Swagger JSON
                app.UseSwaggerUI();     // 顯示 Swagger UI 頁面
            }



            app.UseHttpsRedirection();
            // 自動將 HTTP 請求轉為 HTTPS，提高安全性

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}"
            );

            // 啟用身份驗證中介軟體（Authentication Middleware）
            // 這一行會讓 ASP.NET Core 應用程式開始解析傳入的 HTTP 請求中是否包含有效的身份驗證資訊（例如 JWT）
            // 必須放在 `UseAuthorization()` 之前，因為授權（Authorization）需要基於先前驗證的結果
            app.UseAuthentication();

            // 啟用授權中介軟體（Authorization Middleware）
            // 根據身份驗證的結果與設定的授權規則，來決定某個使用者是否有權限存取特定資源或執行某些操作
            // 若控制器或路由上有使用 [Authorize] 屬性，就會依照這裡的設定來判斷是否允許請求繼續執行
            app.UseAuthorization();

            app.UseStaticFiles(new StaticFileOptions
            {
                // FileProvider 設定靜態檔案的實體資料夾來源（伺服器上的實際路徑）
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(Directory.GetCurrentDirectory(), "Images")
                ),

                // RequestPath 是指定當前端請求 URL 包含 /Images 時，要從哪個資料夾抓檔案
                RequestPath = "/Images"
            });


            app.MapControllers();
            // 將標記為 [ApiController] 的 Controller 類別映射為路由端點

            //app.MapFallbackToFile("/index.html");
            // 對於找不到的 API 路徑，回傳前端的 index.html
            // 適用於 SPA 前端 routing，如 Vue、React Router

            app.Run();
            // 啟動 Web 應用程式並開始監聽 HTTP 請求
        }
    }
}
