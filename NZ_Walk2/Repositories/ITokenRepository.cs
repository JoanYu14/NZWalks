// 引入 ASP.NET Core Identity 套件中的 IdentityUser 類別，這是用來表示使用者的標準類別
using Microsoft.AspNetCore.Identity;

namespace NZ_Walk.Server.Repositories
{
    // 定義一個介面 ITokenRepository，負責處理與 JWT Token 相關的操作
    // 在這裡，我們定義了這個介面，並指定了它應該有一個方法來創建 JWT Token。
    public interface ITokenRepository
    {
        // reateJWTToken 方法接受兩個參數：
        // 1. IdentityUser user：表示進行身份驗證的使用者。
        // 2. List<string> roles：包含使用者角色的列表，用於決定使用者的權限。
        //
        // 這個方法返回一個字串（string），這個字串就是生成的 JWT Token。
        // JWT Token 是用來進行身份驗證和授權的，通常在 API 呼叫時提供，確保使用者具有相應的權限。
        string CreateJWTToken(IdentityUser user, List<string> roles);
    }
}
