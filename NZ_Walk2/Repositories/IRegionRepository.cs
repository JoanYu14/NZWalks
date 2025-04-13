// 引入資料模型的命名空間，這樣可以使用 Region 類別
using NZ_Walk.Server.Models.Domain;

namespace NZ_Walk.Server.Repositories
{
    // 定義一個介面 IRegionRepository，這個介面包含操作區域（Region）資料的基本方法
    // 通常用來對應到資料庫中儲存區域資料的 CRUD（增刪改查）操作
    public interface IRegionRepository
    {
        // 非同步方法，取得所有區域的資料
        // 返回一個 List<Region>，表示所有區域的列表
        Task<List<Region>> GetAllAsync();

        // 非同步方法，根據區域的 ID 取得特定區域的資料
        // 會返回一個 Region 物件，若找不到對應 ID 的區域，則返回 null
        Task<Region?> GetByIdAsync(Guid id);

        // 非同步方法，創建一個新的區域
        // 傳入一個 Region 物件，該物件包含要創建的區域資料
        // 返回創建後的 Region 物件，通常會包含一個生成的 ID 或者其他資料庫層的變更結果
        Task<Region> CreateAsync(Region region);

        // 非同步方法，根據區域的 ID 更新現有的區域資料
        // 傳入的參數包括一個區域 ID 和一個 Region 物件，Region 物件包含更新後的區域資料
        // 返回更新後的 Region 物件，如果 ID 無效或找不到對應的區域，則返回 null
        Task<Region?> UpdateAsync(Guid id, Region region);

        // 非同步方法，根據區域的 ID 刪除特定的區域資料
        // 傳入區域的 ID，刪除對應的區域資料
        // 返回刪除後的 Region 物件，如果 ID 無效或找不到對應的區域，則返回 null
        Task<Region?> DeleteAsync(Guid id);
    }
}
