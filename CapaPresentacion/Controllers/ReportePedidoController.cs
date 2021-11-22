using CapaDato.Pedido;
using CapaEntidad.Control;
using CapaEntidad.General;
using CapaEntidad.Menu;
using CapaEntidad.Pedido;
using CapaEntidad.Util;
using CapaPresentacion.Bll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CapaPresentacion.Controllers
{
    public class ReportePedidoController : Controller
    {
        private string _session_listPedidoSeparado_private = "_session_listPedidoSeparado_private";
        private Dat_Pedido_Separado  dat_pedido = new Dat_Pedido_Separado();
        // GET: ReportePedido
        public ActionResult PedidoSeparado()
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
                #region<VALIDACION DE ROLES DE USUARIO>
                Boolean valida_rol = true;
                Basico valida_controller = new Basico();
                List<Ent_Menu_Items> menu = (List<Ent_Menu_Items>)Session[Ent_Global._session_menu_user];
                valida_rol = valida_controller.AccesoMenu(menu, this);
                #endregion

                if (valida_rol)
                {
                    Session[_session_listPedidoSeparado_private] = lista();

                    return View();
                }
                else
                {
                    return RedirectToAction("Error_Roles", "Home");
                }
                
            }
        }
        public List<Ent_Pedido_Separado> lista()
        {
            Ent_Usuario _usuario = (Ent_Usuario)Session[Ent_Constantes.NameSessionUser];

            string user_id = ((_usuario.usu_tip_id.Contains("01") || _usuario.usu_tip_id.Contains("02") || _usuario.usu_tip_id.Contains("09")) ? _usuario.usu_id.ToString() : "-1");

            List<Ent_Pedido_Separado> listpedido = dat_pedido.lista(user_id);

            ViewBag.tcantidad = listpedido.Sum(c=>c.tcantidad);
            ViewBag.tsoles = listpedido.Sum(c => c.subtotal);

            Session[_session_listPedidoSeparado_private] = listpedido;
            return listpedido;
        }
        public ActionResult getListPedSepAjax(Ent_jQueryDataTableParams param, string actualizar)
        {

            List<Ent_Pedido_Separado> listpedido = new List<Ent_Pedido_Separado>();

            if (!String.IsNullOrEmpty(actualizar))
            {
                listpedido = lista();
                //listAtributos = datOE.get_lista_atributos();
                Session[_session_listPedidoSeparado_private] = listpedido;
            }

            /*verificar si esta null*/
            if (Session[_session_listPedidoSeparado_private] == null)
            {
                listpedido = new List<Ent_Pedido_Separado>();
                listpedido = lista(); //datOE.get_lista_atributos();
                if (listpedido == null)
                {
                    listpedido = new List<Ent_Pedido_Separado>();
                }
                Session[_session_listPedidoSeparado_private] = listpedido;
            }


            //Traer registros
            IQueryable<Ent_Pedido_Separado> membercol = ((List<Ent_Pedido_Separado>)(Session[_session_listPedidoSeparado_private])).AsQueryable();  //lista().AsQueryable();

            //Manejador de filtros
            int totalCount = membercol.Count();
            IEnumerable<Ent_Pedido_Separado> filteredMembers = membercol;

            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredMembers = membercol
                    .Where(m => m.asesor.ToUpper().Contains(param.sSearch.ToUpper()) ||
                     m.lider.ToUpper().Contains(param.sSearch.ToUpper()) ||
                     m.pedido.ToUpper().Contains(param.sSearch.ToUpper()) ||
                     m.promotor.ToUpper().Contains(param.sSearch.ToUpper()) 
                     );
            }
            //Manejador de orden
            var sortIdx = Convert.ToInt32(Request["iSortCol_0"]);

            if (param.iSortingCols > 0)
            {
                if (Request["sSortDir_0"].ToString() == "asc")
                {
                    switch (sortIdx)
                    {
                        case 0: filteredMembers = filteredMembers.OrderBy(o => o.asesor); break;
                        case 1: filteredMembers = filteredMembers.OrderBy(o => o.lider); break;
                        case 2: filteredMembers = filteredMembers.OrderBy(o => o.promotor); break;
                        case 3: filteredMembers = filteredMembers.OrderBy(o => o.pedido); break;
                        case 4: filteredMembers = filteredMembers.OrderBy(o => Convert.ToInt32(o.tcantidad)); break;
                        case 5: filteredMembers = filteredMembers.OrderBy(o => Convert.ToInt32(o.subtotal)); break;

                        case 6: filteredMembers = filteredMembers.OrderBy(o => Convert.ToDateTime(o.fecha_ing)); break;
                        case 7: filteredMembers = filteredMembers.OrderBy(o => Convert.ToDateTime(o.fecha_cad)); break;

                        case 8: filteredMembers = filteredMembers.OrderBy(o => Convert.ToInt32(o.dias_pedido)); break;
                        case 9: filteredMembers = filteredMembers.OrderBy(o => o.ubicacion); break;
                    }
                }
                else
                {
                    switch (sortIdx)
                    {
                        case 0: filteredMembers = filteredMembers.OrderByDescending(o => o.asesor); break;
                        case 1: filteredMembers = filteredMembers.OrderByDescending(o => o.lider); break;
                        case 2: filteredMembers = filteredMembers.OrderByDescending(o => o.promotor); break;
                        case 3: filteredMembers = filteredMembers.OrderByDescending(o => o.pedido); break;
                        case 4: filteredMembers = filteredMembers.OrderByDescending(o => Convert.ToInt32(o.tcantidad)); break;
                        case 5: filteredMembers = filteredMembers.OrderByDescending(o => Convert.ToInt32(o.subtotal)); break;
                        case 6: filteredMembers = filteredMembers.OrderByDescending(o => Convert.ToDateTime(o.fecha_ing)); break;
                        case 7: filteredMembers = filteredMembers.OrderByDescending(o => Convert.ToDateTime(o.fecha_cad)); break;
                        case 8: filteredMembers = filteredMembers.OrderByDescending(o =>Convert.ToInt32(o.dias_pedido)); break;
                        case 9: filteredMembers = filteredMembers.OrderByDescending(o => o.ubicacion); break;
                    }
                }
            }
            var displayMembers = filteredMembers
                .Skip(param.iDisplayStart)
                .Take(param.iDisplayLength);


            var result = from a in displayMembers
                         select new
                         {
                             a.asesor,
                             a.lider,
                             a.promotor,
                             a.pedido,
                             a.tcantidad,
                             a.fecha_ing,
                             a.fecha_cad,
                             a.dias_pedido,
                             a.telefono,
                             a.celular,
                             a.ubicacion, 
                             a.subtotal,                            
                         };
            //Se devuelven los resultados por json
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = totalCount,
                iTotalDisplayRecords = filteredMembers.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }
        public FileContentResult ListaPedSepExcel()
        {

            List<Ent_Pedido_Separado> lista = (List<Ent_Pedido_Separado>)Session[_session_listPedidoSeparado_private];
            string[] columns = { "asesor", "lider", "promotor", "pedido", "tcantidad", "subtotal", "fecha_ing", "fecha_cad", "fecha_cad", "dias_pedido","celular","telefono","ubicacion" };

            byte[] filecontent = ExcelExportHelper.ExportExcel(lista, "LISTA DE PEDIDOS SEPARADO - CATALOGO - BATA", false, columns);
            string nom_excel = "Lista de Clientes";
            return File(filecontent, ExcelExportHelper.ExcelContentType, nom_excel + ".xlsx");
        }

        public ActionResult AnularLiquidacion(string liq)
        {
            string mensaje = "";
            Ent_Usuario _usuario = (Ent_Usuario)Session[Ent_Constantes.NameSessionUser];
            bool ret = true;// false;
            Dat_Pedido datPedido = new Dat_Pedido();
            ret = datPedido.AnularLiquidacion(liq, _usuario.usu_id, ref mensaje);            
            return Json(new { estado = ret ? 0 : 1, mensaje = mensaje });
        }
    }
}