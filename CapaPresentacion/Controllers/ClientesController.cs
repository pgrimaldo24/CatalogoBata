﻿using CapaDato.Cliente;
using CapaDato.Maestros;
using CapaEntidad.Cliente;
using CapaEntidad.Control;
using CapaEntidad.General;
using CapaEntidad.Maestros;
using CapaEntidad.Menu;
using CapaEntidad.Persona;
using CapaEntidad.Util;
using CapaPresentacion.Bll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CapaPresentacion.Controllers
{
    public class ClientesController : Controller
    {
        // GET: Clientes

        private Dat_Cliente dat_cliente = new Dat_Cliente();
        private string _session_listCliente_private = "_session_listCliente_private";
        private string _sessin_customer = "_sessin_customer";
        public ActionResult Index()
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
                    // ViewBag.lider = dat_cliente.lista_lider();

                    string user_id = ((_usuario.usu_tip_id.Contains("01") || _usuario.usu_tip_id.Contains("02") || _usuario.usu_tip_id.Contains("09")) ? _usuario.usu_id.ToString() : "-1");

                    Session[_session_listCliente_private] = dat_cliente.lista_cliente(user_id);

                    ViewBag.Usuario = _usuario;

                    return View();
                }
                else
                {
                    return RedirectToAction("Error_Roles", "Home");
                }

              
            }
        }

        public FileContentResult ListaClienteExcel()
        {
           
            List<Ent_Cliente_Lista> lista = (List<Ent_Cliente_Lista>)Session[_session_listCliente_private];
            string[] columns = { "dni", "nombres", "correo", "telefono", "direccion", "bas_fecha_cre", "bas_fec_actv" };

            byte[] filecontent = ExcelExportHelper.ExportExcel(lista, "LISTA DE CLIENTES - CATALOGO - BATA", false, columns);
            string nom_excel = "Lista de Clientes";
            return File(filecontent, ExcelExportHelper.ExcelContentType, nom_excel + ".xlsx");
        }

        public ActionResult getListClienteAjax(Ent_jQueryDataTableParams param)
        {

            List<Ent_Cliente_Lista> listcliente = new List<Ent_Cliente_Lista>();
            /*verificar si esta null*/
            if (Session[_session_listCliente_private] == null)
            {
                listcliente = new List<Ent_Cliente_Lista>();
                listcliente = lista(); //datOE.get_lista_atributos();
                if (listcliente == null)
                {
                    listcliente = new List<Ent_Cliente_Lista>();
                }
                Session[_session_listCliente_private] = listcliente;
            }

        
            //Traer registros
            IQueryable<Ent_Cliente_Lista> membercol = ((List<Ent_Cliente_Lista>)(Session[_session_listCliente_private])).AsQueryable();  //lista().AsQueryable();

            //Manejador de filtros
            int totalCount = membercol.Count();
            IEnumerable<Ent_Cliente_Lista> filteredMembers = membercol;

            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredMembers = membercol
                    .Where(m => m.dni.ToUpper().Contains(param.sSearch.ToUpper()) ||
                     m.nombres.ToUpper().Contains(param.sSearch.ToUpper()) ||
                     m.correo.ToUpper().Contains(param.sSearch.ToUpper()) ||
                     m.telefono.ToUpper().Contains(param.sSearch.ToUpper()) ||
                     m.celular.ToUpper().Contains(param.sSearch.ToUpper()) ||
                     m.direccion.ToUpper().Contains(param.sSearch.ToUpper()) ||
                     m.bas_distrito.ToUpper().Contains(param.sSearch.ToUpper()) ||
                     m.tipo.ToUpper().Contains(param.sSearch.ToUpper())
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
                        case 0: filteredMembers = filteredMembers.OrderBy(o => o.tipo); break;
                        case 1: filteredMembers = filteredMembers.OrderBy(o => o.dni); break;
                        case 2: filteredMembers = filteredMembers.OrderBy(o => o.nombres); break;
                        case 3: filteredMembers = filteredMembers.OrderBy(o => o.correo); break;
                        case 7: filteredMembers = filteredMembers.OrderBy(o => o.bas_distrito); break;
                    }
                }
                else
                {
                    switch (sortIdx)
                    {
                        case 0: filteredMembers = filteredMembers.OrderByDescending(o => o.tipo); break;
                        case 1: filteredMembers = filteredMembers.OrderByDescending(o => o.dni); break;
                        case 2: filteredMembers = filteredMembers.OrderByDescending(o => o.nombres); break;
                        case 3: filteredMembers = filteredMembers.OrderByDescending(o => o.correo); break;
                        case 7: filteredMembers = filteredMembers.OrderByDescending(o => o.bas_distrito); break;
                    }
                }
            }
            var displayMembers = filteredMembers
                .Skip(param.iDisplayStart)
                .Take(param.iDisplayLength);
            var result = from a in displayMembers
                         select new
                         {
                             a.tipo,
                             a.dni,
                             a.nombres,
                             a.correo,
                             a.telefono,
                             a.celular,
                             a.direccion,
                             a.bas_fecha_cre,
                             a.bas_fec_actv,
                             a.bas_distrito,
                             a.bas_Tip_Des,
                             a.bas_Agencia_Direccion,
                             a.bas_referencia,
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
 
        public List<Ent_Cliente_Lista> lista()
        {
            Ent_Usuario _usuario = (Ent_Usuario)Session[Ent_Constantes.NameSessionUser];

            string user_id = ((_usuario.usu_tip_id.Contains("01") || _usuario.usu_tip_id.Contains("02") || _usuario.usu_tip_id.Contains("09")) ? _usuario.usu_id.ToString() : "-1");

            List<Ent_Cliente_Lista> listcliente = dat_cliente.lista_cliente(user_id);
            Session[_session_listCliente_private] = listcliente;
            return listcliente;
        }

        public ActionResult ClienteEditar(string estado,string dni)
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

            if (estado==null) return RedirectToAction("Index", "Clientes");
            List<Ent_Cliente_Agencia> lista_agencia = new List<Ent_Cliente_Agencia>();
            ViewBag.lista_agencia = lista_agencia;
            ViewBag.Estado = estado;
            ViewBag.EstadoDes = (estado == "1" ? "Creando nuevo Cliente" : "Modificando Cliente");
            ViewBag.agencia = dat_cliente.lista_agencia();

            ViewBag.despacho=dat_cliente.lista_despacho();

            #region<REGION SI ES QUE EL ESTADO ES IGUAL A 2 Y SE ESTA MODIFICANDO, ENTONCES VAMOS A BUSCAR EN LA LISTA>
                List<Ent_Cliente_Lista> listcliente = (List<Ent_Cliente_Lista>)Session[_session_listCliente_private];
            if (listcliente==null)
            {
                return RedirectToAction("Index", "Clientes");
            }
            else
            {

                if (estado == "2")
                {
                    Ent_Cliente_Lista cliente_editar = listcliente.Where(a => a.dni == dni).ToArray()[0];
                    ViewBag.DataCliente = cliente_editar;
                }               
            }

                #endregion


            ViewBag.Usuario = _usuario;

            Dat_Usuario_Tipo dat_usu_tipo = new Dat_Usuario_Tipo();

            if (estado == "1")
            {
                    ViewBag.UsuTipo = dat_usu_tipo.get_lista_tip_user((_usuario.usu_tip_id == "01" || _usuario.usu_tip_id == "02" || _usuario.usu_tip_id == "09" ? _usuario.usu_tip_id : "-1"), true);
            }
            else
             {
                    ViewBag.UsuTipo = dat_usu_tipo.get_lista_tip_user( "-1", true);
                }
            

            Dat_Documento_Tipo dat_doc_tipo = new Dat_Documento_Tipo();
            ViewBag.DocTipo = dat_doc_tipo.get_lista();

            Dat_Persona_Tipo dat_per_tipo = new Dat_Persona_Tipo();
            ViewBag.PerTipo = dat_per_tipo.get_lista();


            Dat_Lugar dat_lugar = new Dat_Lugar();

            List<Ent_Lugar> combo_dep_prv_dis = dat_lugar.get_lista();

            ViewBag.Dep = combo_departamento(combo_dep_prv_dis);

            List<Ent_Lugar> lista_prov = new List<Ent_Lugar>();

            ViewBag.Prov = lista_prov;

            List<Ent_Lugar> lista_dis = new List<Ent_Lugar>();

            ViewBag.Dis = lista_dis;

            ViewBag.DepProvDis = combo_dep_prv_dis;

            Dat_Combo_Lider cbolider = new Dat_Combo_Lider();            

            ViewBag.Lider = cbolider.lista_lider((_usuario.usu_tip_id == "09")? _usuario.usu_tip_id : "-1",_usuario.usu_id.ToString());

            ViewBag.Asesor = cbolider.lista_asesor();            

            Ent_Cliente cliente = new Ent_Cliente();

            ViewBag.cliente = cliente;

            return View();
            }
        }
        private List<Ent_Lugar> combo_departamento(List<Ent_Lugar> combo_general)
        {
            List<Ent_Lugar> listar = null;
            try
            {
                listar = new List<Ent_Lugar>();
                listar = (from grouping in combo_general.GroupBy(x => new Tuple<string, string>(x.dep_id, x.dep_descripcion))
                          select new Ent_Lugar
                          {
                              dep_id = grouping.Key.Item1,
                              dep_descripcion = grouping.Key.Item2,
                          }).OrderBy(a => a.dep_descripcion).ToList();
            }
            catch
            {


            }
            return listar;
        }

        public ActionResult valida_cliente(string dni,string correo)
        {
            string mensaje = "";
            try
            {

                string lider = "";
                /*si trare valor 0 entonces no hay concidencias*/
               string valida = dat_cliente.valida_cliente(dni, correo,ref lider);
               switch(valida)
                {                   
                    case "1":
                        mensaje = "El Numero de documento ya existe en la red de " + lider +", ingrese otro numero.";
                        break;
                    case "2":
                        mensaje = "El correo existe, ingrese otro correo";
                        break;
                    case "3":
                        mensaje = "Error de conexion";
                        break;
                }


                return Json(new { estado = valida, mensaje = mensaje });
            }
            catch (Exception ex)
            {
                return Json(new { estado = "3", mensaje = ex.Message });
            }


        }
        public ActionResult editar_cliente()
        {
            return RedirectToAction("ClienteEditar", "Clientes", new { estado = "1" });
        }

        public ActionResult GuardarCliente(Ent_Cliente dataArray,int estado)
        {
            try
            {
                Ent_Usuario _usuario = (Ent_Usuario)Session[Ent_Constantes.NameSessionUser];

                string grabar= dat_cliente.grabar_clientes(estado, _usuario.usu_id, dataArray);

                if (grabar.Length==0)
                {
                    return Json(new { estado = "0", mensaje = "Se Guardo con exito." });
                }
                else
                {
                    return Json(new { estado = "1", mensaje = grabar});
                }
                
            }
            catch (Exception exc)
            {

                return Json(new { estado = "1", mensaje = exc.Message });
            }
            
        }

        public ActionResult GeneracionFlete()
        {
            return View();
        }

    }
}