// 引入必要的命名空間
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using NZ_Walk.Server.Data;
using NZ_Walk.Server.Models.Domain;

namespace NZ_Walk.Server.Repositories
{
    // 定義 SQLRegionRepository 類別，這是 IRegionRepository 介面的實作
    // 用來處理與資料庫中區域（Region）資料相關的操作
    public class SQLRegionRepository : IRegionRepository
    {
        // 建立一個私有變數 _dbContext 用來儲存注入的 DbContext 實例
        // _dbContext 是與資料庫互動的實際物件，負責資料存取的邏輯
        private readonly NzWalksDbContext _dbContext;

        // 建構函式，使用建構子注入的方式將 DbContext 實例傳遞給 SQLRegionRepository 類別
        // 這樣 _dbContext 可以在方法中使用，處理資料庫中的區域資料
        public SQLRegionRepository(NzWalksDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // 非同步方法，取得所有區域的資料
        // 使用 ToListAsync 方法將結果轉換為 List<Region>
        public async Task<List<Region>> GetAllAsync()
        {
            // 從 _dbContext 取得所有區域資料，並非同步地返回結果
            return await _dbContext.Regions.ToListAsync();
        }

        // 非同步方法，根據區域的 ID 取得特定區域資料
        // 使用 FirstOrDefaultAsync 來尋找符合條件的第一筆資料
        public async Task<Region?> GetByIdAsync(Guid id)
        {
            // 使用 FirstOrDefaultAsync 查找資料，如果找不到會返回 null
            return await _dbContext.Regions.FirstOrDefaultAsync(x => x.Id == id);
        }

        // 非同步方法，創建一個新的區域資料
        // 將新區域資料加入到資料庫中，並儲存變更
        public async Task<Region> CreateAsync(Region region)
        {
            // 使用 AddAsync 方法將新區域資料加入到 Regions 資料表
            await _dbContext.Regions.AddAsync(region);

            // 使用 SaveChangesAsync 儲存變更到資料庫
            await _dbContext.SaveChangesAsync();

            // 返回創建的區域資料（包含資料庫自動生成的 ID 等欄位）
            return region;
        }

        // 非同步方法，根據區域 ID 更新區域資料
        // 如果區域存在，則更新其資料；如果找不到，則返回 null
        public async Task<Region?> UpdateAsync(Guid id, Region region)
        {
            // Step 1：根據 ID 查找資料庫中的區域資料
            var exsitingRegion = await _dbContext.Regions.FirstOrDefaultAsync(x => x.Id == id);

            // Step 2：檢查是否找到資料
            if (exsitingRegion == null)
            {
                // 如果找不到對應的區域資料，則返回 null
                // 可以用來處理 404 Not Found 錯誤，表示前端提供的 ID 不存在
                return null;
            }

            // Step 3：更新區域資料的屬性
            exsitingRegion.Code = region.Code;
            exsitingRegion.Name = region.Name;
            exsitingRegion.RegionImageUrl = region.RegionImageUrl;

            // Step 4：儲存變更到資料庫
            await _dbContext.SaveChangesAsync();

            // Step 5：返回更新後的區域資料
            return exsitingRegion;
        }

        // 非同步方法，根據區域 ID 刪除區域資料
        // 如果找到對應資料，則從資料庫中刪除該區域並儲存變更
        public async Task<Region> DeleteAsync(Guid id)
        {
            // Step 1：根據 ID 查找資料庫中的區域資料
            var exsitingRegion = await _dbContext.Regions.FirstOrDefaultAsync(x => x.Id == id);

            // Step 2：檢查是否找到資料
            if (exsitingRegion == null)
            {
                // 如果找不到對應的區域資料，則返回 null
                // 可以用來處理 404 Not Found 錯誤，表示前端提供的 ID 不存在
                return null;
            }

            // Step 3：刪除該區域資料
            _dbContext.Regions.Remove(exsitingRegion);

            // Step 4：儲存變更到資料庫，這將永久刪除該區域資料
            await _dbContext.SaveChangesAsync();

            // Step 5：返回已刪除的區域資料
            return exsitingRegion;
        }
    }
}
