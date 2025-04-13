using System.ComponentModel.DataAnnotations;

namespace NZ_Walk.Server.Models.DTO
{
    // 這個Dto Class的用處是接收client的update region request
    public class UpdateRegionRequestDto
    {
        // 地區代碼，通常用來唯一標識一個地區
        [Required(ErrorMessage = "Code 為必填欄位")]
        [MinLength(3, ErrorMessage = "Code 的最小長度為 3")]
        [MaxLength(3, ErrorMessage = "Code 的最大長度為 3")]
        public string Code { get; set; }

        // 地區名稱，表示地區的名稱，例如 "Taipei", "Kaohsiung", "Tainan"
        // 這是用來讓使用者辨識地區的資訊
        [Required(ErrorMessage = "Name 為必填欄位")]
        [MaxLength(100, ErrorMessage = "Name 的最大長度為 100")]
        public string Name { get; set; }

        // 地區圖片的 URL，這是可選欄位（使用 nullable string）
        // 若有地區的圖片，可以透過這個 URL 來顯示
        // 如果沒有圖片，則該屬性為 null
        public string? RegionImageUrl { get; set; }
    }
}
