// �w�q�R�W�Ŷ��A��ܳo�����O�ݩ� NZ_Walk.UI �M�׽d��
namespace NZ_Walk.UI
{
    // �D�{�����O�A��� ASP.NET Core ���Ϊ��i�J�I
    public class Program
    {
        // Main ��k�O C# ���Ϊ��J�f�A�{���q�o�̶}�l����
        public static void Main(string[] args)
        {
            // �ϥ� WebApplication.CreateBuilder �إ����ε{���غc��
            // �ǤJ args �i�����R�O�C�Ѽơ]�Ҧp����ɳ]�w�����ܼƵ��^
            var builder = WebApplication.CreateBuilder(args);

            // =========================
            // ���U�A�Ȩ� DI �e����
            // =========================

            // �[�J MVC �һݪ�����P�˵��]Controller + View�^
            builder.Services.AddControllersWithViews();

            // �[�J HttpClient �\��A�i�ΨӦV�~�� API �o�e HTTP �ШD
            builder.Services.AddHttpClient();

            // =========================
            // �إ� WebApplication ��ҡ]�غc App�^
            // =========================
            var app = builder.Build();

            // =========================
            // �]�w�����n��]Middleware�^�޽u
            // =========================

            // �p�G�ثe���O�}�o���ҡ]�Ҧp�O�������ҡ^
            if (!app.Environment.IsDevelopment())
            {
                // �ϥο��~�B�z���A��ҥ~�o�ͮɷ|���s�ɦV�� /Home/Error
                app.UseExceptionHandler("/Home/Error");

                // �ϥ� HSTS�]HTTP Strict Transport Security�^
                // �o�O�@�Ӧw���ʱj�ƾ���A�w�]�|�j��ϥ� HTTPS �s�u 30 ��
                app.UseHsts();
            }

            // �۰ʱN HTTP �ШD���s�ɦV�� HTTPS�A���ɦw����
            app.UseHttpsRedirection();

            // �ҥ��R�A�ɮפ䴩�]�Ҧp CSS�BJS�B�Ϥ��^�A�w�]�|Ū�� wwwroot ��Ƨ�
            app.UseStaticFiles();

            // �ҥθ��ѥ\��A���t�ί�ھ� URL �ɦV�����������P�ʧ@
            app.UseRouting();

            // �ҥα��v�����n��]�Ҧp���ҥΤ᪺�v���^
            app.UseAuthorization();

            // =========================
            //  �]�w MVC ���ѳW�h�]Routing�^
            // =========================
            app.MapControllerRoute(
                name: "default", // ���Ѫ��W��
                pattern: "{controller=Home}/{action=Index}/{id?}"
            // �w�]�����Ѯ榡�G���/�ʧ@/�i�諸�Ѽ� id
            // �p�G�S���w URL�A�|�w�]�ɦV HomeController �� Index ��k
            );

            // �Ұ� Web ���ΡA�}�l�����ШD
            app.Run();
        }
    }
}
