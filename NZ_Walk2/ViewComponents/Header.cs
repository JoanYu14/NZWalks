using Microsoft.AspNetCore.Mvc;

namespace NZ_Walk2.ViewComponents
{
    public class Header:ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return await Task.Run(() => View());
        }
    }
}
