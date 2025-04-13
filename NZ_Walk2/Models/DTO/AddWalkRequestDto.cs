using System.ComponentModel.DataAnnotations;

namespace NZ_Walk.Server.Models.DTO
{
    public class AddWalkRequestDto
    {
        // 步道的名稱，例如 "Forest Trail", "Mountain Path" 等
        // [Required]：必填欄位，不可為 null 或空字串
        // [MaxLength(100)]：限制最多輸入 100 個字元，避免儲存過長資料
        [Required(ErrorMessage = "Name 為必填欄位")]
        [MaxLength(100, ErrorMessage = "Name 最大長度為100")]
        public string Name { get; set; }

        // 步道的描述，用來說明此步道的特色，例如風景、難度、歷史背景等
        // 同樣為必填欄位，且最大長度限制為 1000 字元（雖然這裡錯誤設定為 100，應為 1000）
        [Required(ErrorMessage = "Description 為必填欄位")]
        [MaxLength(1000, ErrorMessage = "Description 最大長度為1000")]
        public string Description { get; set; }

        // 步道的長度，單位為公里（km）
        // 為必填欄位，並限定長度必須在 0 到 100 公里之間（合理範圍）
        [Required(ErrorMessage = "LengthInKm 為必填欄位")]
        [Range(0, 100, ErrorMessage = "LengthInKm 必須介於0~100")]
        public double LengthInKm { get; set; }

        // 步道的圖片網址，可選欄位
        // 若提供此欄位，可以用來在前端顯示對應圖片；若無則為 null
        public string? WalkImageUrl { get; set; }

        // 步道的難度等級 ID，對應 Difficulty 表的主鍵 (Guid)
        // 為必填欄位，用來指定此步道的難度類型，例如：簡單、中等、困難
        [Required(ErrorMessage = "DifficultyId 為必填欄位")]
        public Guid DifficultyId { get; set; }

        // 步道所屬的地區 ID，對應 Region 表的主鍵 (Guid)
        // 為必填欄位，用來標記此步道所在的位置區域
        [Required(ErrorMessage = "RegionId 為必填欄位")]
        public Guid RegionId { get; set; }
    }
}
