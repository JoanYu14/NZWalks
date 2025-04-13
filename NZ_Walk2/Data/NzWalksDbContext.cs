// 匯入 Entity Framework Core 的核心功能
using Microsoft.EntityFrameworkCore;

// 匯入 Domain 模型：Difficulty, Region, Walk
using NZ_Walk.Server.Models.Domain;
using NZ_Walk2.Models.Domain;

namespace NZ_Walk.Server.Data
{
    // 自訂 DbContext 類別，繼承自 Entity Framework Core 的 DbContext
    public class NzWalksDbContext : DbContext
    {
        // 建構函式：接收 DbContext 的選項並傳給基底類別 DbContext
        // 使用泛型版本，讓 DI 系統知道要注入什麼型別
        public NzWalksDbContext(DbContextOptions<NzWalksDbContext> DbContextOptions) : base(DbContextOptions)
        {
        }

        // DbSet 代表資料表，每一個 DbSet 對應一個 Domain 模型類別
        // 這裡會建立一張名為 Difficulties 的資料表，儲存 Difficulty 模型資料
        public DbSet<Difficulty> Difficulties { get; set; }

        // 建立 Regions 資料表，對應 Region 模型資料
        public DbSet<Region> Regions { get; set; }

        // 建立 Walks 資料表，對應 Walk 模型資料
        public DbSet<Walk> Walks { get; set; }

        public DbSet<Image> Images { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 呼叫基底類別的 OnModelCreating 方法，確保任何 EF Core 的內建行為能正常運作
            base.OnModelCreating(modelBuilder);

            // 建立 Difficulty 的預設資料列表：包含 Easy、Medium、Hard 三種難度
            var difficulties = new List<Difficulty>()
            {
                new Difficulty() {
                    Id = Guid.Parse("70f5a843-15c3-42d9-ac99-a4343b56ca92"),
                    Name = "Easy"
                },
                new Difficulty() {
                    Id = Guid.Parse("0c8bac97-5527-4b0c-8e8a-e88b06bbc409"),
                    Name = "Medium"
                },
                new Difficulty() {
                    Id = Guid.Parse("31996f53-0d32-4892-b51c-622f6b7948f8"),
                    Name = "Hard"
                }
            };

            // 使用 HasData 將 difficulties 播種到資料庫中
            modelBuilder.Entity<Difficulty>().HasData(difficulties);

            // 建立 Region 的預設資料列表，包含數個紐西蘭的地區
            var regions = new List<Region>()
            {
                new Region() {
                    Id = Guid.Parse("d4ebc14b-5c40-41da-ae8f-9e8d666f99f3"),
                    Name = "Auckland",
                    Code = "AKL",
                    RegionImageUrl = "https://images.pexels.com/photos/31439461/pexels-photo-31439461.jpeg?auto=compress&cs=tinysrgb&w=1260&h=750&dpr=2"
                },
                new Region() {
                    Id = Guid.Parse("c068d38a-2d3a-43a4-99ec-25303f74acf1"),
                    Name = "Northland",
                    Code = "NTL",
                    RegionImageUrl = null
                },
                new Region() {
                    Id = Guid.Parse("f9a601d4-3fd5-4d6e-9b74-2209aaccb352"),
                    Name = "Bay Of Plenty",
                    Code = "BOP",
                    RegionImageUrl = null
                },
                new Region() {
                    Id = Guid.Parse("978ec598-fac3-4f8e-98d3-0282a20369f8"),
                    Name = "Wellington",
                    Code = "WGN",
                    RegionImageUrl = "https://images.pexels.com/photos/3538656/pexels-photo-3538656.jpeg?auto=compress&cs=tinysrgb&w=600"
                },
                new Region() {
                    Id = Guid.Parse("c054fafb-b830-4d77-a46d-9abcbe4f2997"),
                    Name = "Nelson",
                    Code = "NSN",
                    RegionImageUrl = "https://images.pexels.com/photos/11130685/pexels-photo-11130685.jpeg?auto=compress&cs=tinysrgb&w=1260&h=750&dpr=2"
                },
                new Region() {
                    // 此筆資料的 Id 和 Wellington 是一樣的，主鍵重複會導致播種失敗
                    Id = Guid.Parse("5d89cc85-e9b3-4388-8598-71a25e659440"),
                    Name = "Southland",
                    Code = "STL",
                    RegionImageUrl = null
                },
            };

            // 使用 HasData 將 regions 播種到資料庫中
            modelBuilder.Entity<Region>().HasData(regions);
        }
    }
}
