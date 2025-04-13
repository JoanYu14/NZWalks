namespace NZ_Walk.Server.Models.DTO
{
    // WalkDto 是用來傳送或接收「步道（Walk）」資訊的資料傳輸物件（DTO）
    // 與 Domain Model（Walk 類別）不同，DTO 專注在前後端資料的傳遞
    // 它不會包含資料庫邏輯，僅承載必要欄位，用於控制資料暴露與轉換格式
    public class WalkDto
    {
        // 步道的唯一識別碼，使用 Guid 作為主鍵來確保唯一性
        public Guid Id { get; set; }
        // 步道的名稱
        // 例如："Lakefront Walk", "Sunset Trail"
        public string Name { get; set; }

        // 步道的描述
        // 提供該步道的相關細節說明，例如景觀、特色、建議裝備等
        public string Description { get; set; }

        // 步道的長度（單位：公里）
        // 可用來顯示步道的距離給使用者參考
        public double LengthInKm { get; set; }

        // 步道的圖片 URL（網址）
        // 用於前端畫面顯示步道的代表圖片
        // 若無圖片，則可為 null
        public string? WalkImageUrl { get; set; }



        // 導覽屬性（DTO 格式）：地區的相關資訊（非僅有 RegionId）
        // 將 Region 的詳細資料一併包裝在 DTO 中傳給前端
        // 這樣前端就可以同時拿到地區名稱、ID 等資訊
        public RegionDto Region { get; set; }

        // 導覽屬性（DTO 格式）：難度的相關資訊（非僅有 DifficultyId）
        // 將 Difficulty 的詳細資料一併包裝在 DTO 中傳給前端
        // 讓前端可以直接顯示難度名稱，而不是再用 ID 去查詢
        public DifficultyDto Difficulty { get; set; }
    }
}
