// 引入處理 JWT Token 所需的命名空間
using System.IdentityModel.Tokens.Jwt;         // 提供 JwtSecurityTokenHandler、JwtSecurityToken 等類別
using System.Security.Claims;                  // 提供 Claim 和 ClaimTypes，用來建立使用者聲明資訊
using System.Text;                             // 用於字串編碼（如 Encoding.UTF8）
using Microsoft.AspNetCore.Identity;           // 提供 IdentityUser 類型，代表使用者
using Microsoft.IdentityModel.Tokens;          // 提供 Token 的安全性設定（例如加密金鑰與簽章）

namespace NZ_Walk.Server.Repositories
{
    // TokenRepository 類別實作 ITokenRepository 介面
    public class TokenRepository : ITokenRepository
    {
        // 注入 IConfiguration，從 appsettings.json 取得 JWT 的相關設定（例如金鑰、Issuer、Audience）
        private readonly IConfiguration _configuration;

        public TokenRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // 這是實作介面的方法，用來建立 JWT Token
        public string CreateJWTToken(IdentityUser user, List<string> roles)
        {
            // 建立一個聲明清單，這些聲明（claims）會包含在 Token 中
            var claims = new List<Claim>();

            // 加入使用者 Email 作為一項聲明
            claims.Add(new Claim(ClaimTypes.Email, user.Email));

            // 將所有角色逐一加入聲明清單
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            // 從設定中讀取密鑰（對稱金鑰），並轉換為位元組陣列
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"]));

            // 使用 HMAC-SHA256 加密演算法建立簽章憑證
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // 建立 JWT Token 的物件，包含：
            // - Issuer：Token 的發行者（從設定中取得）
            // - Audience：Token 的使用對象（從設定中取得）
            // - Claims：用戶相關聲明（包含 Email 與角色）
            // - Expiration：Token 的過期時間，這裡設定為 15 分鐘
            // - SigningCredentials：簽章憑證，確保 Token 的完整性與來源
            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],       // 發行者
                _configuration["Jwt:Audience"],     // 接收者
                claims,                              // 聲明清單
                expires: DateTime.Now.AddMinutes(15), // 有效期限
                signingCredentials: credentials       // 簽章憑證
            );

            // 將 JwtSecurityToken 轉為字串（即最終的 JWT Token）
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
