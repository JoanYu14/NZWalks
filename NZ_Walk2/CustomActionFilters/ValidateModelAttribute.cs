// 匯入 ASP.NET Core 中提供 HTTP 請求相關功能的命名空間
using Microsoft.AspNetCore.Mvc;

// 匯入 ASP.NET Core 提供的篩選器基底類別與上下文物件
using Microsoft.AspNetCore.Mvc.Filters;

namespace NZ_Walk.Server.CustomActionFilters
{
    // 自訂一個動作篩選器（Action Filter），名稱為 ValidateModelAttribute
    // 繼承自 ActionFilterAttribute，可在 Controller 的 Action 方法「執行前或後」插入自訂邏輯
    public class ValidateModelAttribute : ActionFilterAttribute
    {
        // 覆寫 OnActionExecuted 方法，這個方法會在 Controller 的 Action 方法執行「之後」被呼叫
        // context 參數提供了當前請求的上下文資訊（例如 ModelState, HttpContext, Result 等）
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            // 檢查目前的 ModelState 是否有效
            // ModelState 是 ASP.NET Core 自動進行模型驗證後的結果，包含是否符合 [Required], [MaxLength] 等驗證條件
            if (context.ModelState.IsValid == false)
            {
                // 如果驗證失敗，則直接回傳一個 HTTP 400 BadRequest 結果，表示前端送來的資料有誤
                // 這會中斷後續的 Controller 方法邏輯，不會繼續執行
                context.Result = new BadRequestResult();

                // 備註：這裡使用 BadRequestResult()，只會回傳狀態碼 400，無法提供具體的錯誤訊息給前端
                // 若要包含錯誤欄位與錯誤訊息（例如「Name 為必填欄位」），建議改用：
                // context.Result = new BadRequestObjectResult(context.ModelState);
            }

            // 若驗證成功（ModelState.Valid），則不會進行任何操作，Controller 的動作會正常執行並回傳結果
        }
    }
}
