// 定義命名空間，通常用來區分不同模組或專案架構中的分類
namespace NZ_Walk.UI.Models.DTO
{
    // 宣告一個公開的類別 RegionDto，代表「地區（Region）」的資料格式
    public class RegionDto
    {
        // 唯一識別碼，用來唯一識別每個 Region
        public Guid Id { get; set; }

        // 地區代碼，例如：AKL、WGN 等
        public string Code { get; set; }

        // 地區名稱，例如：Auckland、Wellington 等
        public string Name { get; set; }

        // 地區圖片的網址，可能是用來顯示地圖或代表圖像
        // 使用 string? 表示這個屬性可以是 null（可選的欄位）
        public string? RegionImageUrl { get; set; }
    }
}
