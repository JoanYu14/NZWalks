using System.Runtime;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NZ_Walk.Server.Data;
using NZ_Walk.Server.Mappings;
using NZ_Walk.Server.Repositories;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using NZ_Walk2.Repositories;
using Microsoft.Extensions.FileProviders;
namespace NZ_Walk.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // �إ� Web ���ε{�����غc���A�|�۰ʸ��J appsettings.json�B�����ܼơB�R�O�C�ѼƵ�
            var builder = WebApplication.CreateBuilder(args);

            // ���U�A�Ȩ� DI �e���]Dependency Injection�^

            builder.Services.AddControllers();
            // �N IHttpContextAccessor �[�J DI �e���A�ϱo��L���O�]�Ҧp Service �� Repository�^
            // �i�H�z�L�غc�l�`�J�覡�A���o�ثe�� HttpContext�]�ШD�W�U��^
            // �o�b�ݭn�s���ϥΪ̸�T�BSession�BHeaders�BRequest/Response �����p�ɫD�`����
            builder.Services.AddHttpContextAccessor();
            // �[�J MVC Controller �䴩�A�B�z API �ШD��

            // �]�w Razor View �䴩 Session TempData ���Ѫ�
            builder.Services.AddControllersWithViews().AddSessionStateTempDataProvider();

            // �ۭq Razor View ���j�M���|
            builder.Services.Configure<RazorViewEngineOptions>(options =>
            {
                options.ViewLocationFormats.Clear(); // �M���w�]�j�M���|
                options.ViewLocationFormats.Add("/Views/{1}/{0}" + RazorViewEngine.ViewExtension); // ����M��
                options.ViewLocationFormats.Add("/Views/Shared/{0}" + RazorViewEngine.ViewExtension); // �@���˵�
            });

            builder.Services.AddHttpContextAccessor(); // �i���A�ȼh���o�ثe�� HTTP �ШD���e


            builder.Services.AddEndpointsApiExplorer();
            // �Ω󱴯� Endpoints ���c�A�� Swagger/OpenAPI �ϥ�

            builder.Services.AddSwaggerGen(options =>
            {
                // �]�w Swagger ��󪺰򥻸�T�]���D�P�����^
                options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "NZ Walks API",  // API �����D
                    Version = "v1"           // ������T
                });

                // �� Swagger UI �W�[ JWT Bearer ���Ҫ��y�z
                options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
                {
                    Name = "Authorization",                    // �n�b Header ����J�����W��
                    In = ParameterLocation.Header,             // Token �n�[�b���̡]�o�̬O HTTP Header�^
                    Type = SecuritySchemeType.ApiKey,          // ���v�����]�o�̨ϥ� API ���_�Ҧ��^
                    Scheme = JwtBearerDefaults.AuthenticationScheme // �ϥ� Bearer JWT ���зǨ�w
                });

                // �i�D Swagger�GAPI �ϥΤF JWT ���ҡA�]���ݭn�ǤJ Token
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,              // �ѦҤW���w�q�� SecurityScheme
                                Id = JwtBearerDefaults.AuthenticationScheme       // ID �n�@�P�A�o�ˤ~��s���_��
                            },
                            Scheme = "Oauth2",              // ���M�O Oauth2 ���o�̥u�O�W�١A��ڤW�ڭ̥Ϊ��O Bearer Token
                            Name = JwtBearerDefaults.AuthenticationScheme,
                            In = ParameterLocation.Header  // �P�ˬO�z�L HTTP Header �ǤJ Token
                        },
                        new List<string>() // �o��i�H���w�ݭn���v���d��]scopes�^�A�ڭ̥��d��
                    }
                });
            });

            // ���U Swagger ���;��A�Ω�۰ʲ��� API ���]Swagger UI�^

            builder.Services         // ASP.NET Core �� Service �e��
            .AddDbContext<NzWalksDbContext> // ���U�ڭ̪���Ʈw�W�U��
            (options =>
                options.UseSqlServer(       // ���w�ϥ� SQL Server ��Ʈw
                    builder.Configuration   // �q appsettings.json ���o�s�u�r��
                        .GetConnectionString("NzWalksConnectionString")
                )
            );

            builder.Services.AddDbContext<NzWalksAuthDbContext>
            (options =>
                options.UseSqlServer(       // ���w�ϥ� SQL Server ��Ʈw
                    builder.Configuration   // �q appsettings.json ���o�s�u�r��
                        .GetConnectionString("NzWalksAuthConnectionString")
                )
            );

            // ���U�@�ӽd��ͩR�g�����A�ȡA���ШD�ɡA�|�N IRegionRepository ����ҤƬ� SQLRegionRepository�C
            // �o�N���ۨC���A�Ȼݭn IRegionRepository �����ɡA�|�N SQLRegionRepository ����@�ǻ������C
            // `AddScoped` ��ܳo�ӪA�Ȫ��ͩR�g���O�ھڨC�ӽШD�Ӻ޲z���C
            // �Ҧp�A���C�@�� HTTP �ШD�A�|���o�ӽШD���Ѥ@�� SQLRegionRepository ����ҡC
            // ��ШD������A�o�ӹ�ҷ|�Q����C
            builder.Services.AddScoped<IRegionRepository, SQLRegionRepository>();
            builder.Services.AddScoped<IWalkRepository, SQLWalkRepository>();
            builder.Services.AddScoped<ITokenRepository, TokenRepository>() ;
            builder.Services.AddScoped<IImageRepository, LocalImageRepository>();

            builder.Services.AddAutoMapper(typeof(AutoMapperProfiles));

            // �]�w ASP.NET Core ���������ҪA�ȡ]Identity�^
            // �o��ϥ� IdentityUser �@���ϥΪ̼ҫ��A�f�t����]IdentityRole�^
            builder.Services.AddIdentityCore<IdentityUser>() // ���U Identity ���֤ߥ\��]�Ҧp���U�B�n�J�B�K�X���ҵ��^
                .AddRoles<IdentityRole>() // �[�J����޲z�\��]�i�إߡB��������^
                .AddTokenProvider<DataProtectorTokenProvider<IdentityUser>>("NZWalks") // �[�J token ���;��A�Ω󭫳]�K�X�BEmail ���ҵ�
                .AddEntityFrameworkStores<NzWalksAuthDbContext>() // �]�w Identity �ϥέ��� DbContext ���x�s�ϥΪ̻P������
                .AddDefaultTokenProviders(); // �[�J���ت� Token ���;��]�Ҧp Email�B���]�K�X�Ϊ� token�^


            // �]�w Identity �����ҳW�h�A�Ҧp�K�X���j�׭n�D
            builder.Services.Configure<IdentityOptions>(options =>
            {
                // ���j��K�X�]�t�Ʀr
                options.Password.RequireDigit = false;

                // ���j��K�X�]�t�p�g�r��
                options.Password.RequireLowercase = false;

                // ���j��K�X�]�t�S��r���]�p !@#$%^�^
                options.Password.RequireNonAlphanumeric = false;

                // ���j��K�X�]�t�j�g�r��
                options.Password.RequireUppercase = false;

                // �K�X���צܤ� 6 �Ӧr��
                options.Password.RequiredLength = 6;

                // �K�X���ܤ֭n�� 1 �ӿW�S�r��
                options.Password.RequiredUniqueChars = 1;
            });


            // �]�w ASP.NET Core �����ҪA�ȡA���w�ϥ� JWT Bearer ���Ҥ覡
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)

                // �]�w JWT Bearer �������ﶵ
                .AddJwtBearer(options =>

                    // �]�w Token ���Ҫ��Ѽ�
                    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                    {
                        // ���� Token �� Issuer�]�o��̡^�O�_���T
                        ValidateIssuer = true,

                        // ���� Token �� Audience�]�����̡^�O�_���T
                        ValidateAudience = true,

                        // ���� Token �O�_�b���Ĵ������]�O�_�L���^
                        ValidateLifetime = true,

                        // ���� Token ��ñ���O�_���T�]�O�_���Q«��^
                        ValidateIssuerSigningKey = true,

                        // ���w�X�k�� Token �o��̡A�q appsettings.json �����o�]�w��
                        ValidIssuer = builder.Configuration["Jwt:Issuer"],

                        // ���w�X�k�� Token �����̡A�q appsettings.json �����o�]�w��
                        ValidAudience = builder.Configuration["Jwt:Audience"],

                        // ���w�Ψ�����ñ�������_�A�ϥ� appsettings.json ���� "Jwt:Key" �ȶi�� UTF-8 �s�X��إ߹�٦����_
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
                    }
                );




            // �إ� Web ���ι�ҡ]�|�ھڤW�����U���A�Ȩӫإ����ε{���^
            var app = builder.Build();

            // �]�w�����n��]Middleware�^�y�{

            app.UseDefaultFiles();
            // �p�G�S�����w�ɮסA�w�]�|�M�� index.html�]�Ω� SPA �e�ݡ^

            app.UseStaticFiles();
            // �ҥ��R�A�ɮתA�ȡA�䴩 wwwroot �ؿ��U���ɮס]�Ҧp React/Vue �sĶ�᪺�e�ݡ^

            // �Y�B��}�o���ҡA�ҥ� Swagger UI
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();       // ���� Swagger JSON
                app.UseSwaggerUI();     // ��� Swagger UI ����
            }



            app.UseHttpsRedirection();
            // �۰ʱN HTTP �ШD�ର HTTPS�A�����w����

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}"
            );

            // �ҥΨ������Ҥ����n��]Authentication Middleware�^
            // �o�@��|�� ASP.NET Core ���ε{���}�l�ѪR�ǤJ�� HTTP �ШD���O�_�]�t���Ī��������Ҹ�T�]�Ҧp JWT�^
            // ������b `UseAuthorization()` ���e�A�]�����v�]Authorization�^�ݭn�����e���Ҫ����G
            app.UseAuthentication();

            // �ҥα��v�����n��]Authorization Middleware�^
            // �ھڨ������Ҫ����G�P�]�w�����v�W�h�A�ӨM�w�Y�ӨϥΪ̬O�_���v���s���S�w�귽�ΰ���Y�Ǿާ@
            // �Y����θ��ѤW���ϥ� [Authorize] �ݩʡA�N�|�̷ӳo�̪��]�w�ӧP�_�O�_���\�ШD�~�����
            app.UseAuthorization();

            app.UseStaticFiles(new StaticFileOptions
            {
                // FileProvider �]�w�R�A�ɮת������Ƨ��ӷ��]���A���W����ڸ��|�^
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(Directory.GetCurrentDirectory(), "Images")
                ),

                // RequestPath �O���w��e�ݽШD URL �]�t /Images �ɡA�n�q���Ӹ�Ƨ����ɮ�
                RequestPath = "/Images"
            });


            app.MapControllers();
            // �N�аO�� [ApiController] �� Controller ���O�M�g�����Ѻ��I

            //app.MapFallbackToFile("/index.html");
            // ���䤣�쪺 API ���|�A�^�ǫe�ݪ� index.html
            // �A�Ω� SPA �e�� routing�A�p Vue�BReact Router

            app.Run();
            // �Ұ� Web ���ε{���ö}�l��ť HTTP �ШD
        }
    }
}
