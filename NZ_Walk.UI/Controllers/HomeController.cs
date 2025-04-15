// �ޤJ�E�_�������R�W�Ŷ��A�ΨӨ��o�ثe�� RequestId�]�Ҧp���~�����|�ϥΡ^
using System.Diagnostics;

// �ޤJ ASP.NET Core MVC �һݪ���¦�R�W�Ŷ�
using Microsoft.AspNetCore.Mvc;

// �ޤJ�M�פ��w�q���ҫ��A�Ҧp�ΨӪ�ܿ��~��T�� ViewModel
using NZ_Walk.UI.Models;

namespace NZ_Walk.UI.Controllers
{
    // HomeController �~�Ӧ� Controller�A�O MVC ������򥻳��
    public class HomeController : Controller
    {
        // �ŧi�@�Өp�����A�����O ILogger<HomeController>
        // �ΨӰO�� Log�A��K�����P�ʱ�
        private readonly ILogger<HomeController> _logger;

        // �غc�l�G�z�L�غc���`�J�]Constructor Injection�^�N ILogger<HomeController> �ǤJ
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger; // �N�ǤJ�� logger �x�s�ܨp�����
        }

        // GET: /Home/Index
        // ��ϥΪ̳X�ݺ��������ɷ|Ĳ�o�o�Ӱʧ@
        public IActionResult Index()
        {
            // �^�ǹ����� View�]�w�]�O Views/Home/Index.cshtml�^
            return View();
        }

        // GET: /Home/Privacy
        // ��ϥΪ̳X�����p�v�F�������ɷ|����o�Ӱʧ@
        public IActionResult Privacy()
        {
            // �^�ǹ����� View�]�w�]�O Views/Home/Privacy.cshtml�^
            return View();
        }

        // GET: /Home/Error
        // �o�O���~�B�z�����A��t�εo�ͥ��B�z�����~�ɷ|�����o��

        // ResponseCache �ݩʡG�]�w�֨�����
        // Duration = 0�G���֨�
        // Location = None�G���n�֨��b�����m�]�Ҧp Proxy �� Client�^
        // NoStore = true�G�������n�x�s�^��
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            // �إߤ@�� ErrorViewModel �ñN RequestId �]�w�i�h
            // RequestId �O�ΨӰl�ܿ��~�ШD���ߤ@ ID�A���U�󰣿�
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });

            // `Activity.Current?.Id`�G�p�G���ثe�� Activity�]�E�_��T�^�A�N�Υ��� ID
            // `?? HttpContext.TraceIdentifier`�G�p�G�S���A�N�ϥ� ASP.NET Core �w�]�� Trace ID
        }
    }
}
