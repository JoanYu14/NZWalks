// 引入資料註解用的命名空間（這裡用於 [NotMapped]）
using System.ComponentModel.DataAnnotations.Schema;

namespace NZ_Walk2.Models.Domain
{
    // 定義 Image 資料模型（Entity），通常對應到資料庫中的一張資料表
    public class Image
    {
        // 圖片的唯一識別碼（Primary Key）
        public Guid Id { get; set; }

        // 上傳的圖片檔案本身，這個欄位不會對應到資料庫欄位
        // [NotMapped] 是 Data Annotations，用來告訴 EF Core 不要將這個屬性映射進資料庫
        [NotMapped]
        public IFormFile File { get; set; }

        // 檔案名稱，例如 "mountain.jpg"
        public string FileName { get; set; }

        // 檔案描述（可選），例如 "一張雪山的照片"
        public string? FileDescription { get; set; }

        // 副檔名，例如 ".jpg"、".png"
        public string FileExtension { get; set; }

        // 檔案大小（單位為位元組），例如 1048576（約 1MB）
        public long FileSizeInBytes { get; set; }

        // 檔案在伺服器上的儲存路徑，例如 "/images/uploads/mountain.jpg"
        public string FilePath { get; set; }
    }
}
