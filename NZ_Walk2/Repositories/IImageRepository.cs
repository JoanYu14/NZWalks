// 引入 Image 類別所在的命名空間（Domain 模型），
// 這樣我們才可以在介面中使用 Image 型別
using NZ_Walk2.Models.Domain;

namespace NZ_Walk2.Repositories
{
    // 定義一個介面 IImageRepository，用於抽象化圖片儲存的相關操作，
    // 這是依賴反轉原則（Dependency Inversion Principle）與介面導向程式設計的常見做法，
    // 方便之後進行單元測試或替換實作（例如使用不同的儲存位置：本地、雲端）

    public interface IImageRepository
    {
        // 定義一個非同步方法 Upload，接收一個 Image 物件並回傳上傳後的 Image 物件。
        // 通常這個方法會處理下列事情：
        // 1. 將圖片檔案實際儲存在伺服器或雲端
        // 2. 將圖片相關資訊（檔名、描述、路徑等）寫入資料庫
        // 3. 回傳資料庫中新增後的完整 Image 物件（含 Id 等）

        Task<Image> Upload(Image image);
    }
}
