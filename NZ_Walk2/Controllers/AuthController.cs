// 匯入 ASP.NET Core 常用的函式庫（例如處理 HTTP 請求、身分驗證、MVC 控制器等）
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;   // Identity 身分管理相關類別
using Microsoft.AspNetCore.Mvc;
using NZ_Walk.Server.Models.DTO;
using NZ_Walk.Server.Repositories;       // 匯入自定義的資料傳輸物件 DTO

namespace NZ_Walk.Server.Controllers
{
    // 設定此 Controller 的路由為：api/Auth
    [Route("api/[controller]")]
    [ApiController]  // 套用 ApiController 屬性，提供模型驗證與自動綁定等功能
    public class AuthController : ControllerBase
    {
        // 宣告一個 Identity 提供的 UserManager，可以用來建立使用者、驗證密碼、加入角色等
        private readonly UserManager<IdentityUser> _userManager;
        // 宣告一個唯讀欄位，用來儲存 TokenRepository 的實例（JWT 產生器）
        private readonly ITokenRepository _tokenRepository;

        // 建構子：當 ASP.NET Core 建立這個控制器時，會自動從 DI 容器中注入以下兩個服務：
        // 1. UserManager<IdentityUser>：提供對 Identity 使用者的管理（如驗證、建立帳號、查詢角色）
        // 2. ITokenRepository：提供 JWT Token 的產生功能（用來發 Token）
        public AuthController(UserManager<IdentityUser> userManager, ITokenRepository tokenRepository)
        {
            // 將注入進來的 UserManager 實例指定給私有欄位，之後就可以在控制器內使用它
            _userManager = userManager;

            // 將注入進來的 TokenRepository 指定給唯讀欄位，讓控制器能夠產生 JWT Token
            _tokenRepository = tokenRepository;
        }

        // API 路由：POST /api/Auth/Register
        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto registerRequestDto)
        {
            // 將前端傳來的註冊資料轉成 IdentityUser 物件（只包含 Email/Username）
            var identityUser = new IdentityUser
            {
                UserName = registerRequestDto.Username, // 使用者名稱
                Email = registerRequestDto.Username     // 設定 Email（跟 Username 相同）
            };

            // 呼叫 Identity 的 CreateAsync 建立新帳號（會自動處理密碼雜湊）
            var identityResult = await _userManager.CreateAsync(identityUser, registerRequestDto.Password);

            // 若帳號建立成功，接著檢查是否有指定角色
            if (identityResult.Succeeded)
            {
                // 若有指定角色（如 "Admin", "User"），將該使用者加入這些角色
                if (registerRequestDto.Roles != null && registerRequestDto.Roles.Any())
                {
                    identityResult = await _userManager.AddToRolesAsync(identityUser, registerRequestDto.Roles);

                    // 加入角色成功後，回傳 200 OK，並給出訊息
                    if (identityResult.Succeeded)
                    {
                        return Ok("使用者註冊成功！請重新登入");
                    }
                }

                // 如果沒指定角色，也可選擇在此回傳成功（可視需求）
                return Ok("使用者註冊成功！");
            }

            // 將錯誤訊息轉為文字陣列回傳給前端
            var errorMessages = identityResult.Errors.Select(e => e.Description).ToList();
            // 若帳號建立或角色加入失敗，回傳錯誤訊息（可再強化為顯示錯誤明細）
             return BadRequest(new { Errors = errorMessages });
        }

        // 接收 POST 請求，路由為 /api/Auth/Login
        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequestDto)
        {
            // 根據傳入的帳號（email）從資料庫中尋找使用者
            var user = await _userManager.FindByEmailAsync(loginRequestDto.Username);

            // 如果找得到使用者（代表帳號存在）
            if (user != null)
            {
                // 使用 UserManager 驗證密碼是否正確
                var checkPasswordResult = await _userManager.CheckPasswordAsync(user, loginRequestDto.Password);

                // 如果密碼正確
                if (checkPasswordResult)
                {
                    // 取得該使用者的角色列表（可能是 Admin、User 等）
                    var roles = await _userManager.GetRolesAsync(user);

                    // 如果角色列表不為 null（雖然實務上不太可能為 null）
                    if (roles != null)
                    {
                        // 呼叫自訂的 TokenRepository，建立 JWT 權杖（Token）
                        var jwtToken = _tokenRepository.CreateJWTToken(user, roles.ToList());

                        // 將 token 包裝成 DTO 回傳給前端
                        var response = new LoginResponseDto
                        {
                            JwtToken = jwtToken
                        };

                        // 傳回 200 OK 並附上 token
                        return Ok(response);
                    }
                }
            }

            // 若帳號不存在或密碼錯誤，傳回 400 Bad Request 與錯誤訊息
            return BadRequest("帳號或密碼錯誤");
        }
    }
}
