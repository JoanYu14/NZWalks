namespace NZ_Walk.Server.Models.DTO
{
    public class DifficultyDto
    {
        //  // 主鍵 ID，用來唯一識別這筆 Difficulty 資料
        //  唯一識別碼，通常對應到資料庫中的主鍵（Primary Key）
        // 使用 Guid 是因為它在多系統整合或雲端環境中更不容易重複
        public Guid Id { get; set; }

        // 難度名稱，例如："Easy", "Medium", "Hard"
        // 沒有設定為 nullable（沒加 ?），所以這個欄位是必填的
        public string Name { get; set; }
    }
}
