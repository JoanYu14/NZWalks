// 引入必要的命名空間
using Microsoft.AspNetCore.Http; // 提供對 HTTP 請求的存取（例如 IFormFile）
using Microsoft.AspNetCore.Mvc; // 提供 MVC Controller 類別與屬性
using NZ_Walk2.Models.Domain; // 匯入 Image 網域模型
using NZ_Walk2.Models.DTO; // 匯入圖片上傳的 DTO 類別
using NZ_Walk2.Repositories; // 匯入圖片儲存的 Repository 介面

namespace NZ_Walk2.Controllers
{
    // 設定 API 的路由為 api/Images，其中 [controller] 會自動使用控制器名稱（ImagesController -> Images）
    [Route("api/[controller]")]
    // 指定這是一個 API 控制器，會自動處理 ModelState 驗證與 JSON 回應等功能
    [ApiController]
    public class ImagesController : ControllerBase // 繼承自 ControllerBase，表示不需要 View，只處理 API 請求
    {
        // 宣告圖片儲存的 Repository 介面
        private readonly IImageRepository _imageRepository;

        // 透過建構式注入 IImageRepository 實作（例如 LocalImageRepository）
        public ImagesController(IImageRepository imageRepository)
        {
            _imageRepository = imageRepository;
        }

        // HTTP POST 方法：處理圖片上傳，路由為 api/Images/Upload
        [HttpPost]
        [Route("Upload")]
        public async Task<IActionResult> Upload([FromForm] ImageUploadRequestDto request)
        {
            // 呼叫私有方法進行檔案格式與大小驗證
            ValidateFileUpload(request);

            // 如果驗證通過（ModelState 沒有錯誤）
            if (ModelState.IsValid)
            {
                // 將 DTO 資料轉換為 Image 網域模型
                var imageDomainModel = new Image
                {
                    File = request.File, // 上傳的圖片檔案
                    FileExtension = Path.GetExtension(request.File.FileName), // 檔案副檔名
                    FileSizeInBytes = request.File.Length, // 圖片檔案大小（byte）
                    FileName = request.FileName, // 自訂的圖片名稱
                    FileDescription = request.FileDescription // 圖片描述（可選）
                };

                // 呼叫 Repository 的 Upload 方法，儲存圖片並取得完整資訊（例如 URL）
                await _imageRepository.Upload(imageDomainModel);

                // 回傳成功的 HTTP 200 OK 並附上上傳後的圖片資訊
                return Ok(imageDomainModel);
            }

            // 若驗證失敗，回傳 HTTP 400 Bad Request 並附上錯誤訊息
            return BadRequest(ModelState);
        }

        // 私有方法：檢查檔案副檔名與大小是否符合限制
        private void ValidateFileUpload(ImageUploadRequestDto request)
        {
            // 定義允許的圖片副檔名
            var allowedExtension = new string[] { ".jpg", ".jpeg", ".png" };

            // 取得實際檔案副檔名，並檢查是否不在允許清單中
            if (!allowedExtension.Contains(Path.GetExtension(request.File.FileName)))
            {
                // 若副檔名不被允許，加入 ModelState 錯誤
                ModelState.AddModelError("file", "Unsupported file extension");
            }

            // 若檔案大於 10MB（10485760 bytes），也加入錯誤
            if (request.File.Length > 10485760)
            {
                ModelState.AddModelError("file", "File size more than 10MB");
            }
        }
    }
}
