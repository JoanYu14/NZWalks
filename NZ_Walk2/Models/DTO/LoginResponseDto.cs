// 命名空間：用來組織程式碼，這個類別屬於 NZ_Walk.Server.Models.DTO（DTO：資料傳輸物件）
namespace NZ_Walk.Server.Models.DTO
{
    // LoginResponseDto 是登入成功後要回傳給前端的資料模型（Data Transfer Object）
    public class LoginResponseDto
    {
        // JwtToken 屬性：用來儲存登入成功後產生的 JWT Token 字串
        // 前端收到這個 Token 後會儲存在 LocalStorage 或 Headers 中，用來做後續的授權請求
        public string JwtToken { get; set; }
    }
}
