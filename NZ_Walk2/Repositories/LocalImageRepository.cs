// 使用必要的命名空間
using NZ_Walk.Server.Data; // 資料庫上下文（DbContext）
using NZ_Walk2.Models.Domain; // Image 實體類別

namespace NZ_Walk2.Repositories
{
    // 本地圖片上傳的 Repository 實作類別
    public class LocalImageRepository : IImageRepository
    {
        // 注入用來取得網站根目錄路徑（例如 wwwroot 資料夾）
        private readonly IWebHostEnvironment _webHostEnvironment;

        // 用來取得目前請求（HttpContext），如 Scheme、Host 等資訊
        private readonly IHttpContextAccessor _httpContextAccessor;

        // 注入 EF Core 的資料庫上下文，用來操作 Images 資料表
        private readonly NzWalksDbContext _dbContext;

        // 建構子注入上述三個服務
        public LocalImageRepository(IWebHostEnvironment webHostEnvironment,
            IHttpContextAccessor httpContextAccessor,
            NzWalksDbContext dbContext)
        {
            _webHostEnvironment = webHostEnvironment;
            _httpContextAccessor = httpContextAccessor;
            _dbContext = dbContext;
        }

        // 上傳圖片的非同步方法，回傳 Image 資料（包含圖片的連結）
        public async Task<Image> Upload(Image image)
        {
            // 組合圖片實際要儲存的本地端完整路徑
            // 例如: C:\project\NZ_Walk\Images\xxx.jpg
            var localFilePath = Path.Combine(
                _webHostEnvironment.ContentRootPath, // 取得網站根目錄
                "Images",                            // 儲存圖片的資料夾
                $"{image.FileName}{image.FileExtension}" // 檔名加副檔名
            );

            // 建立一個檔案串流，準備寫入上傳的圖片檔案
            using var stream = new FileStream(localFilePath, FileMode.Create);

            // 將 IFormFile 的內容寫入本地端檔案系統
            await image.File.CopyToAsync(stream);

            // 組合該圖片的公開 URL 路徑
            // 例如: https://localhost:5001/Images/xxx.jpg
            var urlFilePath = $"{_httpContextAccessor.HttpContext.Request.Scheme}://" +  // http or https
                              $"{_httpContextAccessor.HttpContext.Request.Host}" +       // 網域名稱與埠號
                              $"{_httpContextAccessor.HttpContext.Request.PathBase}" +   // 根路徑（通常為空）
                              $"/Images/{image.FileName}{image.FileExtension}";         // 相對路徑

            // 將圖片連結儲存在 Image 實體物件中
            image.FilePath = urlFilePath;

            // 將圖片資訊寫入資料庫的 Images 資料表
            await _dbContext.Images.AddAsync(image);

            // 提交資料庫變更
            await _dbContext.SaveChangesAsync();

            // 回傳包含完整檔案資訊的 Image 物件
            return image;
        }
    }
}
