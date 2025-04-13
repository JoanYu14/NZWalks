namespace NZ_Walk2.ViewModels
{
    public class MenuModel
    {
        // 菜單項目的標題，通常顯示在菜單的文字
        public string Title { get; set; }

        // 菜單項目的 URL，指向菜單項目對應的網頁路徑
        public string Url { get; set; }

        // 子菜單列表，包含當前菜單項目的下屬子菜單
        // 默認初始化為空的列表，避免空參照錯誤
        public List<MenuModel> Childs { get; set; } = new List<MenuModel>();
    }
}
