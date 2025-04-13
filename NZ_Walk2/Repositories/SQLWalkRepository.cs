// 匯入與資料庫連線的 DbContext 類別（NzWalksDbContext）所屬命名空間
using Microsoft.EntityFrameworkCore;
using NZ_Walk.Server.Data;

// 匯入資料模型 Walk（實體類別），用於新增資料用
using NZ_Walk.Server.Models.Domain;

namespace NZ_Walk.Server.Repositories
{
    // 定義一個公開類別 SQLWalkRepository，實作自 IWalkRepository 介面
    // 負責實作資料存取邏輯，實作內容使用 SQL（實際上是透過 Entity Framework Core 操作資料庫）
    public class SQLWalkRepository : IWalkRepository
    {
        // 建立一個唯讀欄位（readonly）_dbContext，用來儲存注入進來的資料庫內容物件（DbContext）
        // 此欄位將用來與資料庫互動，例如新增資料、查詢、刪除等操作
        private readonly NzWalksDbContext _dbContext;

        // 建構子：在建立 SQLWalkRepository 物件時，會由外部注入 NzWalksDbContext 實例
        public SQLWalkRepository(NzWalksDbContext dbContext)
        {
            // 將注入進來的 dbContext 指派給內部變數 _dbContext，以便在其他方法中使用
            _dbContext = dbContext;

            // 注意：walkRepository 被注入但未使用，通常是不必要的參數，建議移除。
        }

        // 實作 IWalkRepository 中定義的 CreateAsync 方法
        // 此方法會將 Walk 實體新增到資料庫中（非同步方式）
        public async Task<Walk> CreateAsync(Walk walk)
        {
            // 將 walk 實體新增至資料庫的 Walks 資料表（Entity Framework 會追蹤它）
            await _dbContext.Walks.AddAsync(walk);

            // 實際將變更儲存到資料庫中（例如 INSERT INTO）
            await _dbContext.SaveChangesAsync();

            // 回傳剛剛新增成功的 walk 實體（此時已含資料庫產生的主鍵 Id 等欄位）
            return walk;
        }

        // 非同步方法，用來取得所有 Walk 資料，並包含與 Difficulty 和 Region 的關聯資料。
        // 此方法支援根據文字進行篩選、根據欄位排序以及分頁查詢，可用來動態查詢 Walk 資料庫中的資料。
        public async Task<List<Walk>> GetAllAsync(string? filterOn = null, string? filterQuery = null, string? sortBy = null, bool isAscending = true, int pageNumber = 1, int pageSize = 1000)
        {
            // 從資料庫的 Walks 資料表取得 IQueryable 物件
            // 使用 Include("Difficulty") 與 Include("Region") 方法載入 Walk 與 Difficulty、Region 的關聯資料，
            // 這樣可以在一次查詢中取回相關資料，避免延遲載入（Lazy Loading）導致多次查詢資料庫。
            // 呼叫 AsQueryable() 轉型為 IQueryable，使得後續可以根據需求動態加上篩選、排序與分頁條件。
            var walks = _dbContext.Walks
                .Include("Difficulty") // 載入與 Walk 相關聯的 Difficulty 資料
                .Include("Region")     // 載入與 Walk 相關聯的 Region 資料
                .AsQueryable();        // 轉為 IQueryable，以支援後續的 LINQ 查詢操作

            // 1. 文字篩選（Filtering）
            // 檢查是否同時提供有效的篩選欄位 (filterOn) 與查詢值 (filterQuery)。
            if (string.IsNullOrWhiteSpace(filterOn) == false && string.IsNullOrWhiteSpace(filterQuery) == false)
            {
                // 若指定的篩選欄位為 "Name"（忽略大小寫）：
                if (filterOn.Equals("Name", StringComparison.OrdinalIgnoreCase))
                {
                    // 使用模糊比對（Contains）來篩選出名稱中包含 filterQuery 字串的 Walk 資料
                    walks = walks.Where(x => x.Name.Contains(filterQuery));
                }

                // 此處可依需要加入其他欄位的篩選條件
                // 例如，如果 filterOn 等於 "LengthInKm" 則可以加入相應的 Where 條件進行篩選
            }

            // 2. 排序（Sorting）
            // 檢查是否提供了有效的排序欄位 (sortBy)。
            if (string.IsNullOrWhiteSpace(sortBy) == false)
            {
                // 當排序欄位為 "Name"（忽略大小寫）時：
                if (sortBy.Equals("Name", StringComparison.OrdinalIgnoreCase))
                {
                    // 根據 isAscending 參數決定使用遞增或遞減排序
                    walks = isAscending ? walks.OrderBy(x => x.Name) : walks.OrderByDescending(x => x.Name);
                }
                // 當排序欄位為 "Length"（忽略大小寫）時：
                else if (sortBy.Equals("Length", StringComparison.OrdinalIgnoreCase))
                {
                    // 根據 isAscending 參數決定使用遞增或遞減排序，
                    // 這裡針對 Walk 實體的 LengthInKm 欄位進行排序
                    walks = isAscending ? walks.OrderBy(x => x.LengthInKm) : walks.OrderByDescending(x => x.LengthInKm);
                }

                // 可根據其他欄位擴充更多排序條件
            }

            // 3. 分頁（Pagination）
            // 計算需跳過的資料筆數，根據當前頁數與每頁筆數計算： (pageNumber - 1) * pageSize
            var skipResult = (pageNumber - 1) * pageSize;

            // 執行查詢：
            // 使用 Skip() 方法跳過前面的資料，再用 Take() 方法取得指定數量的資料，
            // 最後使用 ToListAsync() 以非同步方式將查詢結果轉換為 List<Walk> 後回傳
            return await walks.Skip(skipResult).Take(pageSize).ToListAsync();
        }



        // 非同步方法，根據Walk的 ID 取得特定Walk的資料
        // 使用 FirstOrDefaultAsync 來尋找符合條件的第一筆資料
        public async Task<Walk?> GetByIdAsync(Guid id)
        {
            // 使用 FirstOrDefaultAsync 查找資料，如果找不到會返回 null
            return await _dbContext.Walks
                .Include("Difficulty") // 加入 Difficulty 導覽屬性，載入與難度相關的資料
                .Include("Region")
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        // 非同步方法，根據Walk ID 更新Walk資料
        // 如果Walk存在，則更新其資料；如果找不到，則返回 null
        public async Task<Walk?> UpdateAsync(Guid id, Walk walk)
        {
            // Step 1：根據 ID 查找資料庫中的區域資料
            var exsitingWalk = await _dbContext.Walks.FirstOrDefaultAsync(x => x.Id == id);

            // Step 2：檢查是否找到資料
            if (exsitingWalk == null)
            {
                // 如果找不到對應的區域資料，則返回 null
                // 可以用來處理 404 Not Found 錯誤，表示前端提供的 ID 不存在
                return null;
            }

            // Step 3：更新區域資料的屬性
            exsitingWalk.Name = walk.Name;
            exsitingWalk.Description = walk.Description;
            exsitingWalk.LengthInKm = walk.LengthInKm;
            exsitingWalk.WalkImageUrl = walk.WalkImageUrl;
            exsitingWalk.DifficultyId = walk.DifficultyId;
            exsitingWalk.RegionId = walk.RegionId;

            // Step 4：儲存變更到資料庫
            await _dbContext.SaveChangesAsync();

            // Step 5：返回更新後的區域資料
            return exsitingWalk;
        }


        // 非同步方法，根據Walk ID 刪除Walk資料
        // 如果找到對應資料，則從資料庫中刪除該Walk並儲存變更
        public async Task<Walk?> DeleteAsync(Guid id)
        {
            // Step 1：根據 ID 查找資料庫中的Walk資料
            var exsitingWalk = await _dbContext.Walks.FirstOrDefaultAsync(x => x.Id == id);

            // Step 2：檢查是否找到資料
            if (exsitingWalk == null)
            {
                // 如果找不到對應的Walk資料，則返回 null
                // 可以用來處理 404 Not Found 錯誤，表示前端提供的 ID 不存在
                return null;
            }

            // Step 3：刪除該Walk資料
            _dbContext.Walks.Remove(exsitingWalk);

            // Step 4：儲存變更到資料庫，這將永久刪除該Walk資料
            await _dbContext.SaveChangesAsync();

            // Step 5：返回已刪除的Walk資料
            return exsitingWalk;
        }
    }
}
