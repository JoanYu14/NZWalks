using System.ComponentModel.DataAnnotations;  // 引入資料註解命名空間，提供資料驗證功能

namespace NZ_Walk.Server.Models.DTO  // 定義此類別所在的命名空間
{
    public class LoginRequestDto  // 定義一個公共的資料傳輸物件類別，用於封裝用戶登入所需資料
    {
        [Required]  // 驗證此欄位為必填項目
        [DataType(DataType.EmailAddress)]  // 驗證此欄位為有效的電子郵件格式
        public string Username { get; set; }  // 用戶名，通常為電子郵件

        [Required]  // 驗證此欄位為必填項目
        [DataType(DataType.Password)]  // 設定此欄位為密碼類型，將密碼內容隱藏顯示
        public string Password { get; set; }  // 密碼欄位
    }
}
