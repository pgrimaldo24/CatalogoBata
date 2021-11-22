using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CapaEntidad.Pedido;
using CapaEntidad.Control;
using CapaEntidad.Util;

namespace CapaPresentacion.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();            
        }

        public ActionResult About()
        {
            return View();
        }
                    
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            return View();
        }
        public ActionResult Error_Roles()
        {

            Ent_Usuario _usuario = (Ent_Usuario)Session[Ent_Constantes.NameSessionUser];

            string actionName = this.ControllerContext.RouteData.GetRequiredString("action");
            string controllerName = this.ControllerContext.RouteData.GetRequiredString("controller");
            string return_view = actionName + "|" + controllerName;

            if (_usuario == null)
            {
                return RedirectToAction("Login", "Control", new { returnUrl = return_view });
            }
            else
            {

                return View();
            }
        }
    }
}