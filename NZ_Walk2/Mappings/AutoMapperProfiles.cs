using AutoMapper; // 引入 AutoMapper 函式庫，AutoMapper 提供了自動映射物件屬性的功能
using Microsoft.Identity.Client; // 這行似乎沒有在此段程式碼中使用，可能是多餘的
using NZ_Walk.Server.Models.Domain; // 引入應用程式中的 Domain 模型命名空間，這裡包含了 Region 類型
using NZ_Walk.Server.Models.DTO; // 引入應用程式中的 DTO（資料傳輸物件）命名空間，這裡包含了 RegionDto 類型

namespace NZ_Walk.Server.Mappings // 定義映射配置的命名空間
{
    // 定義一個自訂的 AutoMapper 配置類，繼承自 AutoMapper 的 Profile 類型
    public class AutoMapperProfiles : Profile
    {
        // 在這個建構子中進行映射配置
        public AutoMapperProfiles()
        {
            // 使用 CreateMap 方法來定義物件類型之間的映射規則
            // 這裡將 Region 類型和 RegionDto 類型進行映射
            // ReverseMap() 使得映射雙向進行，讓 Region 可以映射為 RegionDto，反之亦然
            CreateMap<Region, RegionDto>().ReverseMap();

            // CreateMap 方法用於定義兩個類型之間的映射關係
            // 第一個參數是來源類型 (AddRegionRequestDto)，第二個參數是目標類型 (Region)
            CreateMap<AddRegionRequestDto, Region>()
                // ReverseMap() 使得映射關係是雙向的，即可以從 AddRegionRequestDto 映射到 Region，也可以從 Region 映射回 AddRegionRequestDto
                .ReverseMap();

            // CreateMap 方法用於定義兩個類型之間的映射關係
            // 第一個參數是來源類型 (UpdateRegionRequestDto)，第二個參數是目標類型 (Region)
            CreateMap<UpdateRegionRequestDto, Region>()
                // ReverseMap() 使得映射關係是雙向的，即可以從 UpdateRegionRequestDto 映射到 Region，也可以從 Region 映射回 UpdateRegionRequestDto
                .ReverseMap();

            // CreateMap 方法用於定義兩個類型之間的映射關係
            // 第一個參數是來源類型 (AddWalkRequestDto)，第二個參數是目標類型 (Walk)
            CreateMap<AddWalkRequestDto, Walk>()
                // ReverseMap() 使得映射關係是雙向的，即可以從 AddWalkRequestDto 映射到 Walk，也可以從 Walk 映射回 AddWalkRequestDto
                .ReverseMap();

            // CreateMap 方法用於定義兩個類型之間的映射關係
            // 第一個參數是來源類型 (Walk)，第二個參數是目標類型 (WalkDto)
            CreateMap<Walk, WalkDto>()
                // ReverseMap() 使得映射關係是雙向的，即可以從 Walk 映射到 WalkDto，也可以從 WalkDto 映射回 Walk
                .ReverseMap();


            CreateMap<Difficulty, DifficultyDto>().ReverseMap();
            CreateMap<UpdateWalkRequestDto, Walk>().ReverseMap();
        }
    }
}
