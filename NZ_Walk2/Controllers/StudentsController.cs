// 引入 ASP.NET Core 的 HTTP 功能與 MVC 控制器功能
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace NZ_Walk.Server.Controllers
{
    // 設定路由為 "api/students"，因為 [controller] 會自動取代為類別名稱去除 Controller（即 Students）
    [Route("api/[controller]")]

    // 指定這是一個 API Controller，ASP.NET Core 會幫你自動處理 Model 驗證、推斷輸入來源等功能
    [ApiController]
    public class StudentsController : ControllerBase
    {
        // 設定這個方法會處理 GET 請求到 "api/students"
        [HttpGet]
        public IActionResult GetAllStudents()
        {
            // 模擬一組學生名字的資料（通常這裡會從資料庫或服務層取得）
            string[] studentsName = new string[] { "John", "Kevin", "Jessica", "Peter" };

            // 回傳 HTTP 200 OK 狀態碼，並將學生名字陣列當作 JSON 回傳給前端
            return Ok(studentsName);
        }
    }
}