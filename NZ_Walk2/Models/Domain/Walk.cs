namespace NZ_Walk.Server.Models.Domain
{
    // 定義 Walk 類別，代表一個步道
    // 步道包含名稱、描述、長度等基本屬性，並且與難度（Difficulty）與地區（Region）關聯
    public class Walk
    {
        // 步道的唯一識別碼，使用 Guid 作為主鍵來確保唯一性
        public Guid Id { get; set; }

        // 步道的名稱，例如 "Forest Trail", "Mountain Path" 等
        public string Name { get; set; }

        // 步道的描述，提供更多有關步道的詳細資訊
        // 例如，描述步道的風景、挑戰性或歷史等
        public string Description { get; set; }

        // 步道的長度，以公里為單位
        public double LengthInKm { get; set; }

        // 步道的圖片 URL，用來顯示圖片
        // 如果步道沒有圖片，則這個屬性為 null
        public string? WalkImageUrl { get; set; }

        // 與 Difficulty 類別的關聯，記錄步道的難度
        // 這裡存的是 Difficulty 的 Guid，作為外鍵與 Difficulty 進行關聯
        public Guid DifficultyId { get; set; }

        // 與 Region 類別的關聯，記錄步道所屬的地區
        // 這裡存的是 Region 的 Guid，作為外鍵與 Region 進行關聯
        public Guid RegionId { get; set; }

        // 以下是Navigation Propertise 導航屬性
        // 實際的 Difficulty 物件，代表步道的難度
        public Difficulty Difficulty { get; set; }
        // 實際的 Region 物件，代表步道所屬的地區
        public Region Region { get; set; }
    }
}
