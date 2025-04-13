// 引入 ASP.NET Core 身分驗證與身分管理所需的命名空間
using Microsoft.AspNetCore.Identity; // 提供 Identity 相關類別，例如 IdentityUser、IdentityRole
using Microsoft.AspNetCore.Identity.EntityFrameworkCore; // 提供 Identity 專用的 DbContext 基底類別
using Microsoft.EntityFrameworkCore; // 提供 Entity Framework Core 的功能與 DbContext

namespace NZ_Walk.Server.Data
{
    // 自訂的身分驗證資料庫 DbContext，繼承自 IdentityDbContext，
    // 用來管理 ASP.NET Core Identity 所需的資料表，例如 Users、Roles、UserRoles 等
    public class NzWalksAuthDbContext : IdentityDbContext
    {
        // 建構子：接收 DI 傳入的 DbContextOptions，用來設定資料庫連線與行為
        public NzWalksAuthDbContext(DbContextOptions<NzWalksAuthDbContext> options) : base(options)
        {
            // 呼叫父類別的建構子（IdentityDbContext）以初始化資料庫設定
        }

        // 覆寫 OnModelCreating 方法：在建立資料庫模型時進行自定義設定
        protected override void OnModelCreating(ModelBuilder builder)
        {
            // 呼叫基底類別的設定方法，確保 Identity 預設的資料表與欄位設定被保留
            base.OnModelCreating(builder);

            // 預設要加入資料庫的兩個角色 ID（以 GUID 形式表示）
            var readerRoleId = "01895fa9-e687-40e7-85f7-948f6f0cf5ee";
            var writerRoleId = "cd806113-476d-47aa-81e1-3e7408a6250b";

            // 建立一個角色清單（List<IdentityRole>），包含兩個角色：Reader 與 Writer
            var roles = new List<IdentityRole>
            {
                new IdentityRole
                {
                    Id = readerRoleId,                          // 設定角色的唯一識別碼
                    ConcurrencyStamp = readerRoleId,           // 標示用於處理並行更新（可設為與 Id 相同）
                    Name = "Reader",                            // 角色名稱（大小寫敏感）
                    NormalizedName = "Reader".ToUpper()        // 標準化角色名稱（通常用於比對）
                },
                new IdentityRole
                {
                    Id = writerRoleId,
                    ConcurrencyStamp = writerRoleId,
                    Name = "Writer",
                    NormalizedName = "Writer".ToUpper()
                }
            };

            // 使用 HasData 方法註冊初始資料（Seeding），在執行 Migrations 或 Database Update 時會自動新增到資料庫
            builder.Entity<IdentityRole>().HasData(roles);
        }
    }
}
