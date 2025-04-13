// 匯入必要的命名空間
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NZ_Walk.Server.CustomActionFilters;
using NZ_Walk.Server.Data;             // 引入自定義的 DbContext 類別
using NZ_Walk.Server.Models.Domain;
using NZ_Walk.Server.Models.DTO;
using NZ_Walk.Server.Repositories;       // 引入定義好的 Region DTO 類別

namespace NZ_Walk.Server.Controllers
{
    // 指定這個 Controller 對應的路由為：api/regions
    // [controller] 會自動被替換成這個類別的名稱（去掉 "Controller" 結尾）
    // 例如如果類別叫做 RegionsController，路由就會變成 api/regions
    [Route("api/[controller]")]

    // 表示這是一個 API Controller
    // 自動啟用 Web API 常用的功能，例如：
    // - 自動回傳 JSON
    // - 自動驗證 ModelState（資料模型驗證失敗會回傳 400）
    // - 不需要手動寫 [FromBody] 等標註也能正確綁定參數
    [ApiController]

    // 啟用授權機制
    // 當使用者呼叫這個 Controller 的 API 時，會強制進行身份驗證
    // 如果沒有提供合法的 JWT 或身份資訊，將回傳 401 Unauthorized
    // [Authorize] (因為要基於Role做驗證，所以不要直接在Routes前做驗證)
    public class RegionsController : ControllerBase
    {
        // 建立一個私有變數 _dbContext 用來儲存注入的 DbContext 實例
        // DbContext 是用來與資料庫進行互動的主要類別，通常會包含資料表的集合與對資料庫進行增刪改查的邏輯
        private readonly NzWalksDbContext _dbContext;

        // 建立一個私有變數 _regionRepository1 用來儲存注入的 IRegionRepository 實例
        // IRegionRepository 是一個介面，負責處理與「區域」相關的資料操作，具體實作會在其他地方提供
        private readonly IRegionRepository _regionRepository;

        // 建立一個私有變數 _mapper 用來儲存注入的 AutoMapper 實例
        // IMapper 是 AutoMapper 提供的核心介面，負責將物件之間的資料轉換（例如從 Domain Model 到 DTO，或從 DTO 到 Domain Model）
        // 控制器會使用這個變數來執行物件映射操作，避免手動轉換資料
        private readonly IMapper _mapper;

        // 建構函式，這是控制器的初始化方法，會在控制器被創建時自動呼叫
        // 使用建構子注入的方式將 NzWalksDbContext 和 IRegionRepository 實例傳遞給控制器
        // 這使得控制器可以依賴這些外部服務（DbContext 和資料庫存取邏輯），並且可以在後續的方法中使用這些實例

        public RegionsController(NzWalksDbContext dbContext, IRegionRepository regionRepository, IMapper mapper)
        {
            // 將注入的 DbContext 實例儲存到私有變數 _dbContext 中
            _dbContext = dbContext;

            // 將注入的 IRegionRepository 實例儲存到私有變數 _regionRepository1 中
            _regionRepository = regionRepository;

            // 將注入的 IMapper 實例儲存到私有變數 _mapper 中
            // IMapper 是 AutoMapper 提供的物件映射工具，可以自動將不同類型的物件進行轉換
            // 例如，將 Domain Model 轉換為 DTO 物件，或將 DTO 物件轉換為 Domain Model，這樣可以避免手動編寫映射邏輯
            _mapper = mapper;
        }

        // 方法一：取得所有 Region 的資料
        // HTTP 方法：GET
        // URL 範例：http://localhost:portNum/api/regions
        [HttpGet] // 標註此方法為 HTTP GET 請求，表示該方法會處理針對 /api/regions 的 GET 請求
        [Authorize(Roles ="Reader, Writer")]
        public async Task<IActionResult> GetAll()
        {
            // Step 1：使用 Entity Framework Core 的非同步方法，從資料庫中查詢所有 Regions 資料
            // _regionRepository.GetAllAsync() 是呼叫已注入的 IRegionRepository 介面的 GetAllAsync 方法
            // GetAllAsync 方法會從資料庫中查詢所有的 Region 資料，並以非同步的方式返回結果
            // 使用 await 來等待資料庫查詢完成，這樣可以讓程式在等待結果的過程中不會阻塞執行緒，提升效能
            var regionsDomain = await _regionRepository.GetAllAsync(); // 取得所有的 Region 資料，並儲存到 regionsDomain 變數中

            // Step 2：使用 AutoMapper 進行物件轉換
            // _mapper.Map<List<RegionDto>>(regionsDomain) 將從資料庫中查詢到的 Region 物件集合 (regionsDomain) 
            // 轉換成對應的 RegionDto 物件集合
            // RegionDto 是一個資料傳輸物件 (DTO)，通常用於將資料從後端傳送到前端，避免直接暴露資料庫中的物件
            // AutoMapper 會自動處理兩者之間的屬性對應，從而避免手動編寫轉換邏輯
            var regionsDto = _mapper.Map<List<RegionDto>>(regionsDomain);

            // Step 3：使用 Ok() 方法回傳 200 OK 狀態碼，並將 DTO 清單以 JSON 格式回傳給前端
            // Ok() 是 ASP.NET Core 提供的內建方法，用來回傳 HTTP 200 狀態碼
            // 回傳資料會包裝在 HTTP 回應中，並以 JSON 格式傳送給前端
            return Ok(regionsDto); // 回傳轉換後的 DTO 清單，並傳遞給前端
        }



        // 方法二：根據 ID 查詢單一 Region
        // HTTP 方法：GET
        // URL 範例：http://localhost:portNum/api/regions/{id}
        [HttpGet] // 標註此方法為 HTTP GET 請求，表示該方法會處理針對 /api/regions/{id} 的 GET 請求
        [Route("{id:Guid}")] // 設定路由參數 {id} 必須是 Guid 格式，這樣可以確保傳入的參數是正確的 GUID 格式，避免錯誤的參數類型進來
        [Authorize(Roles = "Reader")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            // Step 1：根據 id 從資料庫中取得一筆 Region 的資料
            // 這裡使用的是 _regionRepository 介面中的 GetByIdAsync 方法，並傳入 id 作為查詢條件
            // 這個方法會查詢資料庫中的 Region 資料表，並返回與該 id 相符的 Region 資料
            // 使用 FirstOrDefaultAsync 是為了能夠處理查無資料時回傳 null 的情況
            var regionDomain = await _regionRepository.GetByIdAsync(id);

            // Step 2：檢查資料庫是否有找到符合條件的資料
            // 如果 regionDomain 為 null，代表資料庫中沒有符合條件的 Region，這時會回傳 404 Not Found 給前端
            if (regionDomain == null)
            {
                return NotFound(); // 回傳 HTTP 404 Not Found，表示未找到該資源
            }

            // Step 3：回傳狀態碼 200 OK 並將該筆資料回傳給前端（JSON 格式）
            // 使用 AutoMapper 將從資料庫查詢到的 domain model (Region) 轉換為 DTO (RegionDto)
            // 這樣可以避免將資料庫的內部結構直接暴露給前端，並將資料以適合的格式傳遞給前端
            return Ok(_mapper.Map<RegionDto>(regionDomain)); // 回傳 HTTP 200 OK，並將轉換後的 RegionDto 物件以 JSON 格式傳送
        }




        // 標示這個方法會處理 HTTP POST 請求
        // 根據 Controller 的路由設定，這個方法的完整路徑會是：/api/regions
        [HttpPost]
        // 使用自訂的模型驗證篩選器 ValidateModelAttribute
        // 這會在 Action 執行前，自動檢查 ModelState 是否有效
        // 若模型驗證失敗（例如欄位缺漏或格式錯誤），則會自動回傳 HTTP 400 錯誤，不會進入這個方法本體
        [ValidateModel]
        // 建立一個新的 Region 資源，並儲存至資料庫
        // 傳入的參數使用 [FromBody] 表示會從 HTTP 請求的 JSON 主體中反序列化成 AddRegionRequestDto
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> Create([FromBody] AddRegionRequestDto addRegionRequestDto)
        {
            // Step 1：將 DTO 轉換為 Domain Model
            // 將前端傳來的 DTO（資料傳輸物件）轉換為內部使用的 Domain Model（資料實體）
            // 原因：實際與資料庫互動的是 Domain Model，而非 DTO
            // 使用 AutoMapper 進行自動對應欄位轉換，避免手動賦值造成程式冗長與易錯
            var regionDomainModel = _mapper.Map<Region>(addRegionRequestDto);

            // Step 2：呼叫 Repository 將資料新增到資料庫
            // _regionRepository 是資料庫存取層的介面，負責與 Entity Framework Core 操作資料表
            // CreateAsync 是一個非同步方法，會將 regionDomainModel 儲存到資料庫中
            // 儲存成功後，會回傳包含資料庫自動產生的主鍵 Id 的 Region 實體
            regionDomainModel = await _regionRepository.CreateAsync(regionDomainModel);

            // Step 3：將 Domain Model 轉換回 DTO，準備回傳給前端
            // 雖然資料庫回傳了 Region 物件，但我們不直接回傳實體物件（避免曝露內部結構）
            // 而是轉換為 RegionDto，這樣可控制回傳的欄位格式與內容
            var regionDto = _mapper.Map<RegionDto>(regionDomainModel);

            // Step 4：回傳 HTTP 201 Created 響應
            // CreatedAtAction 是一個標準 RESTful API 的回應方式
            // 它會回傳：
            // - 狀態碼 201 Created，表示成功創建一筆新資源
            // - Location 標頭，指定該新資源的 URL
            // - 回應主體，包含剛創建的 regionDto 物件
            // nameof(GetById) 表示這個資源可以透過 GET /api/regions/{id} 取得
            // new { id = regionDto.Id } 會動態帶入剛創建的 ID 作為 URL 中的參數
            return CreatedAtAction(nameof(GetById), new { id = regionDto.Id }, regionDto);
        }






        // 方法：更新一筆 Region 資料
        // HTTP 方法：PUT（此方法用來進行完整的更新操作）
        // 路徑範例：http://localhost:portNum/api/regions/{id}
        [HttpPut]
        [Route("{id:Guid}")]  // 限定此路由中的 {id} 參數必須為 Guid 格式，避免錯誤請求進入此方法
        [ValidateModel]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> Update(
            [FromRoute] Guid id,                                // 從路由 URL 中取得 id 參數
            [FromBody] UpdateRegionRequestDto updateRegionRequestDto // 從 request body 中取得前端傳來的更新資料
        )
        {
        // 檢查模型是否通過驗證（例如 DTO 上的 [Required], [MaxLength] 等 DataAnnotations 驗證屬性）

            // Step 1：將前端傳來的更新資料（DTO）轉換為資料庫使用的 Domain Model 物件
            // _mapper.Map<Region>(updateRegionRequestDto) 是將前端傳來的 UpdateRegionRequestDto 物件
            // 轉換成對應的 Region 物件。這個物件是與資料庫交互的 Domain Model，包含需要更新的欄位。
            var regionDomainModel = _mapper.Map<Region>(updateRegionRequestDto);

            // Step 2：根據 id 嘗試從資料庫中找到對應的 Region 資料
            // 使用 _regionRepository.UpdateAsync(id, regionDomainModel) 方法更新資料庫中的 Region 資料
            // 此方法會根據傳入的 id 查找資料，並將更新後的資料儲存回資料庫
            // 使用非同步方式（await）來執行此操作，避免阻塞主執行緒
            regionDomainModel = await _regionRepository.UpdateAsync(id, regionDomainModel);

            // Step 3：檢查資料是否找到並更新成功
            // 如果找不到對應的資料，則回傳 HTTP 404 Not Found 狀態碼，表示資料庫中沒有該筆資料
            if (regionDomainModel == null)
            {
                // 回傳 404 狀態碼，表示根據 id 找不到對應的資料
                return NotFound();
            }

            // Step 4：將更新後的 Domain Model 轉換為 DTO 格式
            // 使用 _mapper.Map<RegionDto>(regionDomainModel) 將更新後的 Region 物件轉換成 DTO，
            // 這樣前端只會接收到簡化過的資料，避免直接暴露資料庫的結構和內部實作
            return Ok(_mapper.Map<RegionDto>(regionDomainModel));
        }


        // 方法：刪除一筆 Region 資料
        // HTTP 方法：DELETE（用來刪除資源）
        // 範例路徑：http://localhost:portNum/api/regions/{id}
        [HttpDelete]
        [Route("{id:Guid}")]  // 限制 {id} 必須為 Guid 格式，避免接收到錯誤格式的請求
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            // Step1：根據 ID 從資料庫中找出該筆 Region 資料
            // 使用 _regionRepository.DeleteAsync(id) 來刪除資料
            // DeleteAsync 方法會從資料庫中查找並刪除對應 ID 的 Region 資料，若該資料不存在則返回 null
            var regionDomainModel = await _regionRepository.DeleteAsync(id);

            // Step2：確認是否有找到該筆資料
            // 檢查從資料庫中刪除的資料是否為 null，如果為 null，表示該筆資料不存在
            if (regionDomainModel == null)
            {
                // 如果找不到，回傳 HTTP 404 Not Found，表示指定的資料不存在
                return NotFound();
            }

            // Step3：回傳刪除成功的資料
            // 若資料成功刪除，將刪除的資料轉換成 DTO 格式
            // DTO 是一種簡化的資料格式，通常包含資料的關鍵資訊，適合傳遞給前端使用
            return Ok(_mapper.Map<RegionDto>(regionDomainModel)); // 回傳 HTTP 200 OK，並附上刪除後的資料（以 RegionDto 格式）
        }
    }
}