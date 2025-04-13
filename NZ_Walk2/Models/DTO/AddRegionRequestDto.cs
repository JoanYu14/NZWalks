using System.ComponentModel.DataAnnotations;

namespace NZ_Walk.Server.Models.DTO
{
    // 用來表示新增地區（Region）的資料傳輸物件（DTO）
    public class AddRegionRequestDto
    {
        // =============================
        // 地區代碼（Code）屬性說明
        // =============================
        // 這是一個必要欄位（[Required]），不可為 null 或空白
        // 使用者輸入的字串長度必須「剛好」是 3（因為有 MinLength 和 MaxLength 都是 3）
        // 通常這個欄位用來作為地區的唯一代碼，例如："TPA" 代表台北
        [Required(ErrorMessage = "Code 為必填欄位")]
        [MinLength(3, ErrorMessage = "Code 的最小長度為 3")]
        [MaxLength(3, ErrorMessage = "Code 的最大長度為 3")]
        public string Code { get; set; }

        // =============================
        // 地區名稱（Name）屬性說明
        // =============================
        // 這個欄位也是必填（[Required]）
        // 限制名稱最長為 100 字元，避免儲存過長的名稱造成資料庫或顯示異常
        // 此欄位讓使用者可以辨識地區的完整名稱，如："Taipei", "Kaohsiung"
        [Required(ErrorMessage = "Name 為必填欄位")]
        [MaxLength(100, ErrorMessage = "Name 的最大長度為 100")]
        public string Name { get; set; }

        // ==========================================
        // 地區圖片 URL（RegionImageUrl）屬性說明
        // ==========================================
        // 這是「非必要」欄位，因此定義為 nullable（`string?`）
        // 如果有地區的圖片，這裡可以放圖片的 URL，例如："https://example.com/image.jpg"
        // 如果沒有圖片則可以為 null，系統也會正常運作
        public string? RegionImageUrl { get; set; }
    }
}
