﻿namespace NZ_Walk.Server.Models.Domain
{
    // 定義一個名為 Region 的類別，代表「地區」，可以是步道所在的區域、地區代碼等
    public class Region
    {
        // 唯一識別碼，用來唯一識別一個 Region 實體
        // 這個 Id 對應到資料庫中的主鍵（Primary Key）
        public Guid Id { get; set; }

        // 地區代碼，通常用來唯一標識一個地區
        // 這個代碼可以是數字、字母或其組合，依據需求來決定
        public string Code { get; set; }

        // 地區名稱，表示地區的名稱，例如 "Taipei", "Kaohsiung", "Tainan"
        // 這是用來讓使用者辨識地區的資訊
        public string Name { get; set; }

        // 地區圖片的 URL，這是可選欄位（使用 nullable string）
        // 若有地區的圖片，可以透過這個 URL 來顯示
        // 如果沒有圖片，則該屬性為 null
        public string? RegionImageUrl { get; set; }
    }
}
