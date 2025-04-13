using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;

namespace NZ_Walk2.ViewComponents
{
    public class Footer:ViewComponent
    {
        public async Task<IViewComponentResult>InvokeAsync()
        {
            return await Task.Run(() => View());
        }
    }
}
