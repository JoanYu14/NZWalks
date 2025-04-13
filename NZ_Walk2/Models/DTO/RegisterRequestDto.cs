// 匯入 .NET 提供的資料驗證屬性（例如 [Required], [DataType]）
// 這些屬性可以用來在 Model Binding 階段做自動驗證
using System.ComponentModel.DataAnnotations;

// 設定命名空間為 DTO 資料交換模型的資料夾
// 通常用於接收前端傳入的資料或回傳給前端的資料
namespace NZ_Walk.Server.Models.DTO
{
    // 宣告一個 DTO 類別，專門用來接收註冊帳號時所需的欄位
    public class RegisterRequestDto
    {
        // 使用者帳號，通常是 Email 格式
        // [Required] 表示此欄位不可為 null 或空白，否則模型驗證會失敗
        [Required(ErrorMessage = "使用者名稱（Email）為必填欄位")]

        // 標示這是 Email 地址類型，有助於前端表單顯示正確的輸入類型
        // 注意：這不會驗證 Email 格式，若需驗證格式，應加上 [EmailAddress]
        [DataType(DataType.EmailAddress)]
        public string Username { get; set; }  // 使用者名稱（通常為 Email）

        // 使用者密碼
        [Required(ErrorMessage = "密碼為必填欄位")]

        // 正確的屬性應是 [DataType(DataType.Password)]，這樣才能正確顯示為密碼框
        [DataType(DataType.Password)]
        public string Password { get; set; }  // 使用者密碼

        // 使用者的角色陣列，例如 ["Admin", "User"]
        // 雖然這裡沒加上 [Required]，但如果後端需要強制角色就應加上
        public string[] Roles { get; set; }   // 使用者所屬的角色群組
    }
}
