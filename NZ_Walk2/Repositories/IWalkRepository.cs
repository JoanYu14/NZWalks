// 引入命名空間，讓這個檔案可以使用 'Walk' 類別（Domain 模型）
using Microsoft.AspNetCore.Mvc;
using NZ_Walk.Server.Models.Domain;

namespace NZ_Walk.Server.Repositories
{
    // 定義一個公開介面（Interface），命名為 IWalkRepository
    // 這個介面屬於資料存取層（Repository Layer），
    // 負責規範 Walk 這個實體模型在資料庫中可進行的操作。
    // 使用介面可以方便進行依賴注入與單元測試（例如：可以使用假資料庫模擬）。
    public interface IWalkRepository
    {
        // 定義一個非同步方法 CreateAsync，用來建立一筆新的 Walk 資料
        // 傳入參數為一個 Walk 實體（walk）
        // 回傳值為 Task<Walk>，表示這是非同步操作，完成後會回傳新增成功的 Walk 資料（可能含有產生的主鍵 Id）
        Task<Walk> CreateAsync(Walk walk);

        // 定義一個非同步方法，取得所有 Walk 資料
        // 回傳值為 Task<List<Walk>>，表示當非同步操作完成後會返回一個 Walk 物件的清單
        // 此方法屬於 Repository 介面，用來從資料庫中查詢 Walk 資料
        //
        // 參數說明：
        //
        // filterOn：可選參數，用來指定依據哪個欄位進行篩選
        //           例如：若要根據 "Name" 或 "LengthInKm" 進行篩選，當此參數不為 null 時，
        //           系統會根據指定欄位應用篩選條件；若為 null，則不會針對特定欄位進行篩選
        //
        // filterQuery：可選參數，表示篩選的查詢值
        //              例如：若想篩選出名稱中包含 "Forest Trail" 的 Walk，則傳入 "Forest Trail"；
        //              若為 null，則不會根據值進行篩選
        //
        // sortBy：可選參數，用來指定根據哪個欄位進行排序
        //         例如："Name" 或 "LengthInKm"；當此參數不為 null 時，系統會根據該欄位進行排序；
        //         若為 null，則不進行任何排序
        //
        // isAscending：可選參數，用來指定排序的方向
        //              布林值 true 表示使用遞增排序，false 表示使用遞減排序；
        //              預設值為 true，即若不指定則使用遞增排序
        //
        // pageNumber：可選參數，用來指定目前要求的頁碼
        //             預設值為 1，表示從第一頁開始返回資料
        //
        // pageSize：可選參數，用來指定每一頁返回多少筆資料
        //           預設值為 1000，表示若不指定則返回最多 1000 筆資料
        //
        // 綜合來說：
        // 當沒有提供任何篩選或排序條件時，方法將回傳資料庫中所有 Walk 的清單；
        // 若提供了篩選條件，則只回傳符合條件的資料；
        // 若提供了排序條件，則回傳的資料會依指定欄位與排序方向進行排序；
        // 此外，分頁參數 pageNumber 與 pageSize 可用於控制返回的資料量，
        // 避免一次性返回過多資料導致效能問題。
        Task<List<Walk>> GetAllAsync(string? filterOn = null,
                                       string? filterQuery = null,
                                       string? sortBy = null,
                                       bool isAscending = true,
                                       int pageNumber = 1,
                                       int pageSize = 1000);



        // 非同步方法，根據Walk的 ID 取得特定Walk的資料
        // 會返回一個 Walk 物件，若找不到對應 ID 的Walk，則返回 null
        Task<Walk?> GetByIdAsync(Guid id);

        // 非同步方法，根據Walk的 ID 更新現有的Walk資料
        // 傳入的參數包括一個Walk ID 和一個 Walk 物件，Walk 物件包含更新後的Walk資料
        // 返回更新後的 Walk 物件，如果 ID 無效或找不到對應的Walk，則返回 null
        Task<Walk?> UpdateAsync(Guid id, Walk walk);

        // 非同步方法，根據Walk的 ID 刪除特定的Walk資料
        // 傳入Walk的 ID，刪除對應的Walk資料
        // 返回刪除後的 Region 物件，如果 ID 無效或找不到對應的Walk，則返回 null
        Task<Walk?> DeleteAsync(Guid id);
    }
}
