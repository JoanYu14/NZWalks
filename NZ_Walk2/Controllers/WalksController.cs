// 匯入 AutoMapper 命名空間，用於物件轉換（例如 Domain <-> DTO）
using AutoMapper;

// 匯入 ASP.NET Core 的 HTTP 與 MVC 控制器相關類別
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NZ_Walk.Server.CustomActionFilters;


// 匯入資料庫上下文，用於操作資料庫
using NZ_Walk.Server.Data;

// 匯入 Walk 的 Domain Model（資料庫對應模型）
using NZ_Walk.Server.Models.Domain;

// 匯入 Walk 的 DTO 類別（用來傳輸資料的物件）
using NZ_Walk.Server.Models.DTO;

// 匯入 Walk 的 Repository 接口，進行資料存取操作
using NZ_Walk.Server.Repositories;

namespace NZ_Walk.Server.Controllers
{
    // 設定這個控制器的路由規則為 "api/Walks"，因為 "[controller]" 會自動取代為類別名稱（去掉 "Controller" 後）
    // 同時使用 ApiController 屬性，它會啟用自動模型驗證、路由綁定等功能
    [Route("api/[controller]")]
    [ApiController]
    public class WalksController : ControllerBase
    {
        // 私有欄位 _dbContext（但在此程式碼未使用）
        private readonly NzWalksDbContext _dbContext;

        // 注入的 IWalkRepository 實例，用來操作 Walk 相關的資料庫邏輯（例如新增、查詢等）
        private readonly IWalkRepository _walkRepository;

        // 注入的 AutoMapper 實例，用於自動將 DTO 和 Domain Model 互相轉換
        private readonly IMapper _mapper;

        // 建構子，會由 ASP.NET Core 自動注入所需的相依元件（透過 DI）
        public WalksController(IMapper mapper, IWalkRepository walkRepository)
        {
            // 將注入進來的 Repository 與 Mapper 分別指派給內部變數以便後續使用
            _walkRepository = walkRepository;
            _mapper = mapper;
        }

        // 新增一筆 Walk 資料（HTTP POST 請求）
        // 呼叫端需傳送一個 JSON 格式的 Walk 資料（AddWalkRequestDto）
        [HttpPost]
        [ValidateModel]
        public async Task<IActionResult> Create([FromBody] AddWalkRequestDto addWalkRequestDto)
        {
            // 使用 AutoMapper 將傳入的 DTO（AddWalkRequestDto）轉換成 Walk Domain Model
            var walkDomainModel = _mapper.Map<Walk>(addWalkRequestDto);

            // 使用 Repository 將這筆 Walk 新增至資料庫中（非同步操作）
            await _walkRepository.CreateAsync(walkDomainModel);

            // 再次使用 AutoMapper 將新增後的 Domain Model 轉換成 DTO 回傳給用戶
            // 這樣可以避免將資料庫內部結構直接暴露給 API 使用者
            return Ok(_mapper.Map<WalkDto>(walkDomainModel));
 
        }

        // HTTP GET 方法，用來取得所有 Walk 的資料，並根據查詢參數進行篩選、排序與分頁
        // URL 範例：/api/walks?filterOn=Name&filterQuery=Track&sortBy=Name&isAscending=true&pageNumber=1&pageSize=10
        [HttpGet]
        public async Task<IActionResult> GetAll(
            // 從查詢字串中取得篩選欄位名稱，例如果要依照 Name 進行篩選
            [FromQuery] string? filterOn,
            // 從查詢字串中取得篩選的查詢值，例如 "Track"
            [FromQuery] string? filterQuery,
            // 從查詢字串中取得排序依據的欄位名稱，例如 "Name"
            [FromQuery] string? sortBy,
            // 從查詢字串中取得排序方向，布林值 true 代表遞增排序，false 代表遞減排序
            // 若未指定則為 null，並使用預設值 true
            [FromQuery] bool? isAscending,
            // 從查詢字串中取得當前頁碼，預設為 1（第一頁）
            [FromQuery] int pageNumbe = 1,
            // 從查詢字串中取得每頁資料筆數，預設值為 1000
            [FromQuery] int pageSize = 1000)
        {
            // Step 1：呼叫 Repository 層的方法
            // 傳入所有查詢參數，取得符合條件的 Walk Domain Model 資料
            // _walkRepository.GetAllAsync 根據 filterOn 與 filterQuery 進行文字篩選，
            // 根據 sortBy 與 isAscending 進行排序，
            // 並根據 pageNumbe 與 pageSize 進行分頁處理。
            // 若 isAscending 為 null，則預設使用 true（遞增排序）。
            var walksDomain = await _walkRepository.GetAllAsync(filterOn, filterQuery, sortBy, isAscending ?? true, pageNumbe, pageSize);

            // Step 2：使用 AutoMapper 將 Domain Model 轉換為 DTO（Data Transfer Object）
            // 此舉可以讓回傳給前端的資料僅包含所需欄位，避免直接暴露資料庫實體的完整結構與可能敏感的資訊
            var walksDto = _mapper.Map<List<WalkDto>>(walksDomain);

            // Step 3：使用 Ok() 方法回傳 HTTP 200 OK 狀態碼
            // 將轉換後的 DTO 清單以 JSON 格式封裝在 HTTP 回應中傳回給前端
            return Ok(walksDto);
        }



        // 根據 ID 查詢單一 Walk
        // HTTP 方法：GET
        // URL 範例：http://localhost:portNum/api/walks/{id}
        [HttpGet] // 標註此方法為 HTTP GET 請求，表示該方法會處理針對 /api/walks/{id} 的 GET 請求
        [Route("{id:Guid}")] // 設定路由參數 {id} 必須是 Guid 格式，這樣可以確保傳入的參數是正確的 GUID 格式，避免錯誤的參數類型進來
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            // Step 1：根據 id 從資料庫中取得一筆 Region 的資料
            // 這裡使用的是 _walkRepository 介面中的 GetByIdAsync 方法，並傳入 id 作為查詢條件
            // 這個方法會查詢資料庫中的 Walks 資料表，並返回與該 id 相符的 Walk 資料
            // 使用 FirstOrDefaultAsync 是為了能夠處理查無資料時回傳 null 的情況
            var walkDomainModel = await _walkRepository.GetByIdAsync(id);

            // Step 2：檢查資料庫是否有找到符合條件的資料
            // 如果 walkDomainModel 為 null，代表資料庫中沒有符合條件的 Walk，這時會回傳 404 Not Found 給前端
            if (walkDomainModel == null)
            {
                return NotFound(); // 回傳 HTTP 404 Not Found，表示未找到該資源
            }

            // Step 3：回傳狀態碼 200 OK 並將該筆資料回傳給前端（JSON 格式）
            // 使用 AutoMapper 將從資料庫查詢到的 domain model (Region) 轉換為 DTO (RegionDto)
            // 這樣可以避免將資料庫的內部結構直接暴露給前端，並將資料以適合的格式傳遞給前端
            return Ok(_mapper.Map<WalkDto>(walkDomainModel)); // 回傳 HTTP 200 OK，並將轉換後的 RegionDto 物件以 JSON 格式傳送
        }


        // 方法：更新一筆 Walk 資料
        // HTTP 方法：PUT（此方法用來進行完整的更新操作）
        // 路徑範例：http://localhost:portNum/api/walks/{id}
        [HttpPut]
        [Route("{id:Guid}")]  // 限定此路由中的 {id} 參數必須為 Guid 格式，避免錯誤請求進入此方法
        [ValidateModel]
        public async Task<IActionResult> Update(
            [FromRoute] Guid id,                                // 從路由 URL 中取得 id 參數
            [FromBody] UpdateWalkRequestDto updateWalkRequestDto // 從 request body 中取得前端傳來的更新資料
        )
        {
            // Step 1：將前端傳來的更新資料（DTO）轉換為資料庫使用的 Domain Model 物件
            var walkDomainModel = _mapper.Map<Walk>(updateWalkRequestDto);

            // Step 2：根據 id 嘗試從資料庫中找到對應的 Walk 資料
            walkDomainModel = await _walkRepository.UpdateAsync(id, walkDomainModel);

            // Step 3：檢查資料是否找到並更新成功
            // 如果找不到對應的資料，則回傳 HTTP 404 Not Found 狀態碼，表示資料庫中沒有該筆資料
            if (walkDomainModel == null)
            {
                // 回傳 404 狀態碼，表示根據 id 找不到對應的資料
                return NotFound();
            }

            // Step 4：將更新後的 Domain Model 轉換為 DTO 格式
            return Ok(_mapper.Map<WalkDto>(walkDomainModel));
        }


        // 方法：刪除一筆 Walk 資料
        // HTTP 方法：DELETE（用來刪除資源）
        // 範例路徑：http://localhost:portNum/api/walks/{id}
        [HttpDelete]
        [Route("{id:Guid}")]  // 限制 {id} 必須為 Guid 格式，避免接收到錯誤格式的請求
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            // Step1：根據 ID 從資料庫中找出該筆 Walk 資料
            var walkDomainModel = await _walkRepository.DeleteAsync(id);

            // Step2：確認是否有找到該筆資料
            // 檢查從資料庫中刪除的資料是否為 null，如果為 null，表示該筆資料不存在
            if (walkDomainModel == null)
            {
                // 如果找不到，回傳 HTTP 404 Not Found，表示指定的資料不存在
                return NotFound();
            }

            // Step3：回傳刪除成功的資料
            // 若資料成功刪除，將刪除的資料轉換成 DTO 格式
            // DTO 是一種簡化的資料格式，通常包含資料的關鍵資訊，適合傳遞給前端使用
            return Ok(_mapper.Map<WalkDto>(walkDomainModel)); // 回傳 HTTP 200 OK，並附上刪除後的資料（以 RegionDto 格式）
        }

    }
}
