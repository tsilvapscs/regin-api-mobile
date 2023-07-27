using System.Web.Mvc;
using regin_app_mobile.Constante;

namespace regin_api_movel.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";
            ViewBag.Version = Versao.GetVersao();
#if DEBUG
            ViewBag.debug = true;
#else
            ViewBag.debug = false;
#endif

            return View();
        }
    }
}
