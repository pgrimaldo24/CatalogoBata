﻿using CapaDato.Logistica;
using CapaEntidad.Articulo;
using CapaEntidad.Control;
using CapaEntidad.General;
using CapaEntidad.Logistica;
using CapaEntidad.Menu;
using CapaEntidad.Util;
using CapaPresentacion.Bll;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CapaPresentacion.Controllers
{
    public class LogisticaController : Controller
    {       
        private Dat_Despacho  dat_despacho = new Dat_Despacho();

        #region<REGION DE CONSULTA DE DESPACHO>
        private string _session_listDespacho_private = "_session_listDespacho_private";       

        // GET: Logistica
        public ActionResult ListaDespacho()
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
                    List<Ent_Tipo_Despacho> lista_tipo = new List<Ent_Tipo_Despacho>();
                    Ent_Tipo_Despacho tip = new Ent_Tipo_Despacho();
                    tip.tip_des_cod = "L";
                    tip.tip_des_nom = "Lima-Callao";
                    lista_tipo.Add(tip);

                    tip = new Ent_Tipo_Despacho();
                    tip.tip_des_cod = "P";
                    tip.tip_des_nom = "Provincia";
                    lista_tipo.Add(tip);

                    ViewBag.Tipo = lista_tipo;

                    return View();
                }
                else
                {
                    return RedirectToAction("Error_Roles", "Home");
                }
                
            }

        }      
        public List<Ent_Lista_Despacho> lista(string fecha_ini,string fecha_fin,string tipo_des)
        {            
            List<Ent_Lista_Despacho> listdespacho = dat_despacho.get_lista_despacho(Convert.ToDateTime(fecha_ini), Convert.ToDateTime(fecha_fin), tipo_des);            
            Session[_session_listDespacho_private] = listdespacho;
            return listdespacho;
        }
        public ActionResult getListDespachoAjax(Ent_jQueryDataTableParams param, string actualizar,string fecha_ini,string fecha_fin,string tipo_des)
        {

            List<Ent_Lista_Despacho> listdespacho = new List<Ent_Lista_Despacho>();

            if (!String.IsNullOrEmpty(actualizar))
            {
                listdespacho = lista(fecha_ini, fecha_fin, tipo_des);
                //listAtributos = datOE.get_lista_atributos();
                Session[_session_listDespacho_private] = listdespacho;
            }

                /*verificar si esta null*/
            if (Session[_session_listDespacho_private] == null)
            {
             
                listdespacho = new List<Ent_Lista_Despacho>();
             
                Session[_session_listDespacho_private] = listdespacho;
            }


            //Traer registros
            IQueryable<Ent_Lista_Despacho> membercol = ((List<Ent_Lista_Despacho>)(Session[_session_listDespacho_private])).AsQueryable();  //lista().AsQueryable();

            //Manejador de filtros
            int totalCount = membercol.Count();
            IEnumerable<Ent_Lista_Despacho> filteredMembers = membercol;

            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredMembers = membercol
                    .Where(m => m.desp_nrodoc.ToUpper().Contains(param.sSearch.ToUpper()) || m.desp_tipo_descripcion.ToUpper().Contains(param.sSearch.ToUpper()));
            }
            //Manejador de orden
            var sortIdx = Convert.ToInt32(Request["iSortCol_0"]);

            if (param.iSortingCols > 0)
            {
                if (Request["sSortDir_0"].ToString() == "asc")
                {
                    switch (sortIdx)
                    {
                        case 0: filteredMembers = filteredMembers.OrderBy(o => o.desp_nrodoc); break;
                        case 1: filteredMembers = filteredMembers.OrderBy(o => o.desp_tipo_descripcion); break;
                        case 2: filteredMembers = filteredMembers.OrderBy(o => Convert.ToDateTime(o.desp_fechacre)); break;
                        case 3: filteredMembers = filteredMembers.OrderBy(o => o.totalparesenviado); break;
                        case 4: filteredMembers = filteredMembers.OrderBy(o => o.estado); break;
                        
                    }
                }
                else
                {
                    switch (sortIdx)
                    {
                        case 0: filteredMembers = filteredMembers.OrderByDescending(o => o.desp_nrodoc); break;
                        case 1: filteredMembers = filteredMembers.OrderByDescending(o => o.desp_tipo_descripcion); break;
                        case 2: filteredMembers = filteredMembers.OrderByDescending(o => Convert.ToDateTime(o.desp_fechacre)); break;
                        case 3: filteredMembers = filteredMembers.OrderByDescending(o => o.totalparesenviado); break;
                        case 4: filteredMembers = filteredMembers.OrderByDescending(o => o.estado); break;                        
                    }
                }
            }
            var displayMembers = filteredMembers
                .Skip(param.iDisplayStart)
                .Take(param.iDisplayLength);


            var result = from a in displayMembers
                         select new
                         {
                             a.desp_nrodoc,
                             a.desp_descripcion,
                             a.desp_fechacre,
                             a.totalparesenviado,
                             a.estado,  
                             a.desp_id,   
                             a.desp_tipo_descripcion,
                             a.desp_tipo                     
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
        #endregion
        #region<REGION DE DESPACHO DE ALMACEN ACCION>
        private string _session_listDespacho_almacen_cab_private = "_session_listDespacho_almacen_cab_private";
        private string _session_listDespacho_almacen_liq_private = "_session_listDespacho_almacen_liq_private";
        private string _session_tipo_despacho = "_session_tipo_despacho"; /*session para verificar que tipo de despacho es P=PROVINCIA O L=LIMA- CALLAO*/
        private string _session_Listar_Servicio = "_session_Listar_Servicio";
        public ActionResult DespachoAlmacen(Int32 estado=0,Int32 estado_edicion=0/*si es 1 entonces no se puede editar*/ ,decimal desp_id=0,
                                            string tipo_des=""/*P=PROVINCIA O L=LIMA - CALLAO*/)
        {

            if (estado==0)
            {
                return RedirectToAction("ListaDespacho", "Logistica");
            }

            Session[_session_tipo_despacho] = tipo_des;
            ViewBag.TipoDes =(tipo_des=="P")? "Provincia":"LIMA - CALLAO";
            Session[_session_listDespacho_almacen_cab_private] = null;
            Session[_session_listDespacho_almacen_liq_private] = null;

            Session[_session_listDespacho_private] = null;
            Session[_session_listDespacho_almacen_cab_editar_private] = null;
            Session[_session_listDespacho_almacen_det_editar_private] = null;
            Session[_session_listDespacho_almacen_det_get_private] = null;
            Session[_session_despacho_almacen_excel] = null;

            List<Ent_Lista_Rotulo> lista_inicio_rotulo= new List<Ent_Lista_Rotulo>();
            Ent_Lista_Rotulo obj_rot = new Ent_Lista_Rotulo();        
            lista_inicio_rotulo.Add(obj_rot);

            ViewBag.Lista = lista_inicio_rotulo;

            List<Ent_Despacho_Almacen_Update> desp_upd_lista = new List<Ent_Despacho_Almacen_Update>();

            Ent_Despacho_Almacen_Update desp_upd = new Ent_Despacho_Almacen_Update();

            ViewBag.ListaDespachoUpd = desp_upd_lista;
            ViewBag.DespachoUpd = desp_upd;
            ViewBag.estado = estado;
            ViewBag.desp_id = desp_id;
            ViewBag.estado_edicion = estado_edicion;

            return View();
        }
        public ActionResult DespachoAlmacenLima(Int32 estado = 0, Int32 estado_edicion = 0/*si es 1 entonces no se puede editar*/ , decimal desp_id = 0,
                                            string tipo_des = ""/*P=PROVINCIA O L=LIMA - CALLAO*/)
        {

            if (estado == 0)
            {
                return RedirectToAction("ListaDespacho", "Logistica");
            }

            Session[_session_tipo_despacho] = tipo_des;
            ViewBag.TipoDes = (tipo_des == "P") ? "Provincia" : "LIMA - CALLAO";
            Session[_session_listDespacho_almacen_cab_private] = null;
            Session[_session_listDespacho_almacen_liq_private] = null;

            Session[_session_listDespacho_private] = null;
            Session[_session_listDespacho_almacen_cab_editar_private] = null;
            Session[_session_listDespacho_almacen_det_editar_private] = null;
            Session[_session_listDespacho_almacen_det_get_private] = null;
            Session[_session_despacho_almacen_excel] = null;

            List<Ent_Lista_Rotulo> lista_inicio_rotulo = new List<Ent_Lista_Rotulo>();
            Ent_Lista_Rotulo obj_rot = new Ent_Lista_Rotulo();
            lista_inicio_rotulo.Add(obj_rot);

            ViewBag.Lista = lista_inicio_rotulo;

            List<Ent_Despacho_Almacen_Update> desp_upd_lista = new List<Ent_Despacho_Almacen_Update>();

            Ent_Despacho_Almacen_Update desp_upd = new Ent_Despacho_Almacen_Update();

            ViewBag.ListaDespachoUpd = desp_upd_lista;
            ViewBag.DespachoUpd = desp_upd;
            ViewBag.estado = estado;
            ViewBag.desp_id = desp_id;
            ViewBag.estado_edicion = estado_edicion;

            List<Ent_Despacho_Delivery> _ListarServico = new List<Ent_Despacho_Delivery>
            {
                new Ent_Despacho_Delivery() { Codigo ="-1",Descripcion="--Seleccione--" }
            };

            ViewBag.Servicio = _ListarServico.Concat(dat_despacho.Listar_Servicio());
            Session[_session_Listar_Servicio] = dat_despacho.Listar_Servicio().ToList();

            return View();
        }

        public Ent_Despacho_Almacen lista_despacho_almacen(string tipo_des, string fecha_ini, string fecha_fin)
        {
            Ent_Usuario _usuario = (Ent_Usuario)Session[Ent_Constantes.NameSessionUser];

            Ent_Despacho_Almacen listdespacho = dat_despacho.get_despacho_almacen(tipo_des, Convert.ToDateTime(fecha_ini), Convert.ToDateTime(fecha_fin), _usuario.usu_id);
            Session[_session_listDespacho_almacen_cab_private] = listdespacho.despacho_cab;
            Session[_session_listDespacho_almacen_liq_private] = listdespacho.despacho_liq;
            return listdespacho;
        }

        public ActionResult getListDespachoAlmacenAjax(Ent_jQueryDataTableParams param, string actualizar, string fecha_ini, string fecha_fin, string tipo_des,Boolean agregar=false)
        {

            //List<Ent_Lista_Despacho> listdespacho = new List<Ent_Lista_Despacho>();

            Ent_Despacho_Almacen listdespacho = new Ent_Despacho_Almacen();

            if (!String.IsNullOrEmpty(actualizar))
            {
                listdespacho = lista_despacho_almacen(tipo_des, fecha_ini, fecha_fin);
                //listAtributos = datOE.get_lista_atributos();
                Session[_session_listDespacho_almacen_cab_private] = listdespacho.despacho_cab;
                Session[_session_listDespacho_almacen_liq_private] = listdespacho.despacho_liq;
            }

            /*verificar si esta null*/
            if (Session[_session_listDespacho_almacen_cab_private] == null)
            {

                listdespacho = new Ent_Despacho_Almacen();

                List<Ent_Despacho_Almacen_Cab> lista1 = new List<Ent_Despacho_Almacen_Cab>();
                List<Ent_Despacho_Almacen_Liquidacion> lista2 = new List<Ent_Despacho_Almacen_Liquidacion>();
                listdespacho.despacho_cab = lista1;
                listdespacho.despacho_liq = lista2;

                Session[_session_listDespacho_almacen_cab_private] = listdespacho.despacho_cab;
                Session[_session_listDespacho_almacen_liq_private] = listdespacho.despacho_liq;
            }


            //Traer registros
            IQueryable<Ent_Despacho_Almacen_Cab> membercol = ((List<Ent_Despacho_Almacen_Cab>)(Session[_session_listDespacho_almacen_cab_private])).AsQueryable();  //lista().AsQueryable();

            //Manejador de filtros
            int totalCount = membercol.Count();
            IEnumerable<Ent_Despacho_Almacen_Cab> filteredMembers = membercol;

            if (!agregar)
            {
                if (!string.IsNullOrEmpty(param.sSearch))
                {
                    filteredMembers = membercol
                        .Where(m => m.Asesor.ToUpper().Contains(param.sSearch.ToUpper()) ||
                            m.Asesor.ToUpper().Contains(param.sSearch.ToUpper()) ||
                            m.NombreLider.ToUpper().Contains(param.sSearch.ToUpper()) ||
                            m.Promotor.ToUpper().Contains(param.sSearch.ToUpper())
                        );
                }
            }

           
            if (agregar)
            {
                filteredMembers = membercol
                    .Where(m => m.agregar==true
                    );
            }
            //Manejador de orden
            //var sortIdx = Convert.ToInt32(Request["iSortCol_0"]);

                //if (param.iSortingCols > 0)
                //{
                //    if (Request["sSortDir_0"].ToString() == "asc")
                //    {
                //        switch (sortIdx)
                //        {
                //            case 0: filteredMembers = filteredMembers.OrderBy(o => o.desp_nrodoc); break;
                //            case 1: filteredMembers = filteredMembers.OrderBy(o => o.desp_descripcion); break;
                //            case 2: filteredMembers = filteredMembers.OrderBy(o => Convert.ToDateTime(o.desp_fechacre)); break;
                //            case 3: filteredMembers = filteredMembers.OrderBy(o => o.totalparesenviado); break;
                //            case 4: filteredMembers = filteredMembers.OrderBy(o => o.estado); break;

                //        }
                //    }
                //    else
                //    {
                //        switch (sortIdx)
                //        {
                //            case 0: filteredMembers = filteredMembers.OrderByDescending(o => o.desp_nrodoc); break;
                //            case 1: filteredMembers = filteredMembers.OrderByDescending(o => o.desp_descripcion); break;
                //            case 2: filteredMembers = filteredMembers.OrderByDescending(o => Convert.ToDateTime(o.desp_fechacre)); break;
                //            case 3: filteredMembers = filteredMembers.OrderByDescending(o => o.totalparesenviado); break;
                //            case 4: filteredMembers = filteredMembers.OrderByDescending(o => o.estado); break;
                //        }
                //    }
                //}
                var displayMembers = filteredMembers
                .Skip(param.iDisplayStart)
                .Take(param.iDisplayLength);


            var result = from a in displayMembers
                         select new
                         {
                             a.Asesor,
                             a.area_id,
                             a.NombreLider,
                             a.Promotor,
                             a.Rotulo,
                             a.rotulo_courier,
                             a.Agencia,
                             a.Destino,
                             a.Pedido,
                             a.TotalPares,
                             a.TotalCatalogo,
                             a.TotalPremio,
                             a.TotalCantidad,
                             a.TotalVenta,
                             a.Igv,
                             a.McaFlete,
                             a.Flete,
                             a.Lid_Prom, 
                             a.observacion,
                             a.detalle,
                             a.Distrito,
                             a.Direccion,
                             a.Referencia,
                             a.Celular,
                             a.Delivery                        
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

        public ActionResult getRotulo_promotor(string idlider)
        {
            string mensaje = "";
            string estado = "0";
            List<Ent_Lista_Rotulo> lista_rotulo = null;
            try
            {
                lista_rotulo= dat_despacho.listar_rotulo_x_lider(idlider);
                if (lista_rotulo.Count==0)
                {
                    estado = "1";
                    mensaje = "No hay filas para mostrar";
                }
                else
                {
                   
                    estado = "0";
                }
                
            }
            catch (Exception exc)
            {
                estado = "1";
                mensaje = exc.Message;
                lista_rotulo = new List<Ent_Lista_Rotulo>();
            }
            return Json(new { estado = estado, mensaje = mensaje, info = lista_rotulo });
        }


        public ActionResult UpdateAgregarChk(Int32 estado_accion/*Esta de accion 1 o 2 1=nuevo,2=edicion*/,
                                       List<Ent_Despacho_Almacen_Update> desp_lista_upd)
        {
            string mensaje = "";
            string estado = "0";
            try
            {

                if (estado_accion == 1 || estado_accion == 3)
                {
                    List<Ent_Despacho_Almacen_Cab> despacho_edit_lista = (List<Ent_Despacho_Almacen_Cab>)Session[_session_listDespacho_almacen_cab_private];
                    
                    List<Ent_Despacho_Almacen_Cab> lista_despa = new List<Ent_Despacho_Almacen_Cab>();

                    foreach (var it in despacho_edit_lista)
                    {
                        lista_despa.Add(it);
                    }

                    foreach (var fila in lista_despa)
                    {
                        var index_upd = despacho_edit_lista.FindIndex(item => item.Lid_Prom == fila.Lid_Prom);
                        var obj_upd = despacho_edit_lista.Where(e => e.Lid_Prom == fila.Lid_Prom).ToList();

                        var str = desp_lista_upd.Where(u => u.strLid_Prom == fila.Lid_Prom).ToList();

                        if (str.Count > 0)
                        {
                            obj_upd[0].agregar = str[0].BolAgregar;                            
                            despacho_edit_lista[index_upd] = obj_upd[0];// despacho_edit;
                        }
                    }

                    Session[_session_listDespacho_almacen_cab_private] = despacho_edit_lista;

                    estado = "0";
                }
                
            }
            catch (Exception exc)
            {

                estado = "1";
                mensaje = exc.Message;
            }
            return Json(new { estado = estado, mensaje = mensaje });
        }


        public ActionResult UpdateRotulo(string lid_prom,string rotulo,Int32 estado_accion/*Esta de accion 1 o 2 1=nuevo,2=edicion*/,
                                        List<Ent_Despacho_Almacen_Update> desp_lista_upd /*string agencia,string destino,string observacion,string detalle,string flete*/)
        {
            string mensaje = "";
            string estado = "0";
            try
            {

                if (estado_accion==1 || estado_accion == 3)
                {
                    List<Ent_Despacho_Almacen_Cab> despacho_edit_lista = (List<Ent_Despacho_Almacen_Cab>)Session[_session_listDespacho_almacen_cab_private];

                    var index = despacho_edit_lista.FindIndex(item => item.Lid_Prom == lid_prom);

                    var obj = despacho_edit_lista.Where(e => e.Lid_Prom == lid_prom).ToList();

                    obj[0].Rotulo = rotulo;
                   

                    Ent_Despacho_Almacen_Cab despacho_edit = new Ent_Despacho_Almacen_Cab();

                    foreach (Ent_Despacho_Almacen_Cab item in obj)
                    {
                        despacho_edit = item;
                    }

                    despacho_edit_lista[index] = despacho_edit;

                    List<Ent_Despacho_Almacen_Cab> lista_despa = new List<Ent_Despacho_Almacen_Cab>();

                    foreach (var it in despacho_edit_lista)
                    {
                        lista_despa.Add(it);
                    }
                    
                    foreach (var fila in lista_despa)
                    {
                        var index_upd = despacho_edit_lista.FindIndex(item => item.Lid_Prom == fila.Lid_Prom);
                        var obj_upd = despacho_edit_lista.Where(e => e.Lid_Prom == fila.Lid_Prom).ToList();

                        var str = desp_lista_upd.Where(u => u.strLid_Prom == fila.Lid_Prom).ToList();

                        if (str.Count>0)
                        { 
                            obj_upd[0].Agencia = str[0].strAgencia;
                            obj_upd[0].Destino = str[0].strDestino;                        
                            obj_upd[0].observacion = ((str[0].strObs == null) ? "" : str[0].strObs);
                            obj_upd[0].detalle = ((str[0].strDetalle == null) ? "" : str[0].strDetalle);
                            obj_upd[0].Flete = str[0].strMcaFlete;                   
                            despacho_edit_lista[index_upd] = obj_upd[0];// despacho_edit;
                        }
                    }

                    Session[_session_listDespacho_almacen_cab_private] = despacho_edit_lista;

                    estado = "0";
                }
                else
                {
                    List<Ent_Despacho_Almacen_Cab_Update> despacho_edit_lista = (List<Ent_Despacho_Almacen_Cab_Update>)Session[_session_listDespacho_almacen_cab_editar_private];

                    var index = despacho_edit_lista.FindIndex(item => item.Lid_Prom == lid_prom);

                    var obj = despacho_edit_lista.Where(e => e.Lid_Prom == lid_prom).ToList();

                    obj[0].Rotulo = rotulo;
                    
                    Ent_Despacho_Almacen_Cab_Update despacho_edit = new Ent_Despacho_Almacen_Cab_Update();

                    foreach (Ent_Despacho_Almacen_Cab_Update item in obj)
                    {
                        despacho_edit = item;
                    }

                    despacho_edit_lista[index] = despacho_edit;

                    List<Ent_Despacho_Almacen_Cab_Update> lista_despa = new List<Ent_Despacho_Almacen_Cab_Update>();

                    foreach (var it in despacho_edit_lista)
                    {
                        lista_despa.Add(it);
                    }

                    foreach (var fila in lista_despa)
                    {
                        var index_upd = despacho_edit_lista.FindIndex(item => item.Lid_Prom == fila.Lid_Prom);
                        var obj_upd = despacho_edit_lista.Where(e => e.Lid_Prom == fila.Lid_Prom).ToList();

                        var str = desp_lista_upd.Where(u => u.strLid_Prom == fila.Lid_Prom).ToList();
                        if (str.Count > 0)
                        {
                            obj_upd[0].Agencia = str[0].strAgencia;
                            obj_upd[0].Destino = str[0].strDestino;
                            obj_upd[0].Observacion = ((str[0].strObs == null) ? "" : str[0].strObs);
                            obj_upd[0].Detalle = ((str[0].strDetalle == null) ? "" : str[0].strDetalle);
                            obj_upd[0].CobroFlete = str[0].strMcaFlete;


                            despacho_edit_lista[index_upd] = obj_upd[0];// despacho_edit;
                        }
                            

                    }


                    Session[_session_listDespacho_almacen_cab_editar_private] = despacho_edit_lista;

                    estado = "0";
                }

               
            }
            catch (Exception exc)
            {

                estado = "1";
                mensaje = exc.Message;
            }
            return Json(new { estado = estado, mensaje = mensaje});
        }

        private string devolverIdliquidacion(string strIdLider, string strLid_Prom, string pedido)
        {
            string StrlistLiquidacion = "";
            List<Ent_Despacho_Almacen_Liquidacion> list_liqui =(List<Ent_Despacho_Almacen_Liquidacion>) Session[_session_listDespacho_almacen_liq_private];
            try
            {
                foreach(Ent_Despacho_Almacen_Liquidacion item in list_liqui)
                {
                    if (strIdLider ==item.area_id && strLid_Prom ==item.lid_prom && pedido.Length > 0)
                    {

                        string strLiq_Id =item.liq_id.ToString();
                        StrlistLiquidacion += "<row  ";
                        StrlistLiquidacion += " IdLider=¿" + strIdLider + "¿ ";
                        StrlistLiquidacion += " IdDespacho=¿xxyy¿ ";
                        StrlistLiquidacion += " IdLiqui=¿" + strLiq_Id + "¿ ";
                        StrlistLiquidacion += " LidProm=¿" + strLid_Prom + "¿ ";
                        StrlistLiquidacion += "/>";
                    }
                    if (strIdLider ==item.area_id && pedido.Length == 0 && item.bas_tip_des == "02")
                    {
                        string strLiq_Id = item.liq_id.ToString();
                        StrlistLiquidacion += "<row  ";
                        StrlistLiquidacion += " IdLider=¿" + strIdLider + "¿ ";
                        StrlistLiquidacion += " IdDespacho=¿xxyy¿ ";
                        StrlistLiquidacion += " IdLiqui=¿" + strLiq_Id + "¿ ";
                        StrlistLiquidacion += " LidProm=¿" + strLid_Prom + "¿ ";
                        StrlistLiquidacion += "/>";
                    }
                }

            }
            catch (Exception exc)
            {

                throw;
            }

                       
            return StrlistLiquidacion;

        }

        public ActionResult GenerarDespacho(Int32 estado_accion,Int32 iddespacho_upd,List<Ent_Despacho_Almacen_Update> desp_lista_upd)
        {
            string mensaje = "";
            string estado = "0";
            Int32 iddespacho = 0;
            try
            {
                string strDataDetalle = "";
                string strLiqLiderDespacho = "";
                Int32 id_despacho = iddespacho_upd;
                foreach (Ent_Despacho_Almacen_Update obj in desp_lista_upd)
                {
                    if (obj.strPromotor == null) obj.strPromotor = "";
                    if (obj.strPedidos == null) obj.strPedidos = "";
                    if (obj.strLid_Prom == null) obj.strLid_Prom = "";

                    strDataDetalle += "<row  ";
                    strDataDetalle += " IdLider=¿" + obj.strIdLider + "¿ ";
                    strDataDetalle += " IdDetalle=¿" + obj.strIdDetalle + "¿ ";
                    strDataDetalle += " Lider=¿" + obj.strLider + "¿ ";
                    strDataDetalle += " Rotulo=¿" + obj.strRotulo + "¿ ";
                    strDataDetalle += " RotuloCourier=¿" + obj.strRotuloCourier + "¿ ";
                    strDataDetalle += " McaCourier=¿" + obj.strMcaCourier + "¿ ";
                    strDataDetalle += " Pares=¿" + obj.strPares + "¿ ";
                    strDataDetalle += " Catalogo=¿" + obj.strCatalogo + "¿ ";
                    strDataDetalle += " Premio=¿" + obj.strPremio + "¿ ";
                    strDataDetalle += " Destino=¿" + obj.strDestino + "¿ ";
                    strDataDetalle += " Agencia=¿" + obj.strAgencia + "¿ ";
                    strDataDetalle += " Monto=¿" + obj.strMonto + "¿ ";
                    strDataDetalle += " Obs=¿" + obj.strObs + "¿ ";
                    strDataDetalle += " Det=¿" + obj.strDetalle + "¿ ";
                    strDataDetalle += " McaFlete=¿" + obj.strMcaFlete + "¿ ";



                    strDataDetalle += " Promotor=¿" + obj.strPromotor + "¿ ";
                    strDataDetalle += " Pedidos=¿" + obj.strPedidos + "¿ ";
                    strDataDetalle += " LidProm=¿" + obj.strLid_Prom + "¿ ";

                    strDataDetalle += " Distrito=¿" + obj.strDistrito + "¿ ";
                    strDataDetalle += " Direccion=¿" + obj.strDireccion + "¿ ";
                    strDataDetalle += " Referencia=¿" + obj.strReferencia + "¿ ";
                    strDataDetalle += " Celular=¿" + obj.strCelular + "¿ ";
                    strDataDetalle += " Delivery=¿" + obj.strDelivery + "¿ ";

                    strDataDetalle += "/>";

                    strLiqLiderDespacho += devolverIdliquidacion(obj.strIdLider, obj.strLid_Prom, obj.strPedidos);

                }

                Ent_Usuario _usuario = (Ent_Usuario)Session[Ent_Constantes.NameSessionUser];
                string generar = "";

                string tipo_despacho = Session[_session_tipo_despacho].ToString();

                if (estado_accion == 1 || estado_accion == 3)
                {
                    generar = dat_despacho.insertar_despacho(_usuario.usu_id, ref id_despacho, strDataDetalle, strLiqLiderDespacho, "", tipo_despacho);
                }
                else
                {
                    generar = dat_despacho.update_despacho(_usuario.usu_id, iddespacho_upd, strDataDetalle, "");
                }
                    

                if (generar.Length==0)
                {

                    switch(estado_accion)
                    {
                        case 1:
                            iddespacho = id_despacho;
                            estado = "0";
                            mensaje = "Se genero con exito el despacho numero " + iddespacho.ToString();
                            break;
                        case 2:
                            iddespacho = iddespacho_upd;
                            estado = "0";
                            mensaje = "Se actualizo con exito el despacho numero " + iddespacho.ToString();
                            break;
                        case 3:
                            iddespacho = iddespacho_upd;
                            estado = "0";
                            mensaje = "Se Agregaron Items con exito el despacho numero " + iddespacho.ToString();
                            break;
                    }

                    //if (estado_accion == 1)
                    //{
                    //    iddespacho = id_despacho;
                    //    estado = "0";
                    //    mensaje = "Se genero con exito el despacho numero " + iddespacho.ToString();
                    //}
                    //else
                    //{
                    //    iddespacho = iddespacho_upd;
                    //    estado = "0";
                    //    mensaje = "Se actualizo con exito el despacho numero " + iddespacho.ToString();
                    //}
                        


                }else
                {
                    estado = "1";
                    mensaje = generar;
                }

                
            }
            catch (Exception exc)
            {
                estado = "1";
                mensaje = exc.Message;
            }
            return Json(new { estado = estado, mensaje = mensaje, iddespacho = iddespacho });
        }

        #region <REGION DE EDITAR DESPACHO>
        private string _session_listDespacho_almacen_cab_editar_private = "_session_listDespacho_almacen_cab_editar_private";
        private string _session_listDespacho_almacen_det_editar_private = "_session_listDespacho_almacen_det_editar_private";
        private string _session_listDespacho_almacen_det_get_private = "_session_listDespacho_almacen_det_get_private";
        public ActionResult ConsultaDespachoEdit(string iddespacho)
        {
            string mensaje = "";
            string estado = "0";
            Ent_Despacho_Almacen_Editar get_despacho = null;
            try
            {
                get_despacho = get_consulta_despacho(iddespacho);

                Session[_session_listDespacho_almacen_cab_editar_private] = get_despacho.Almacen_Cab_Update;
                Session[_session_listDespacho_almacen_det_editar_private] = get_despacho.Almacen_Det_Update;
                Session[_session_listDespacho_almacen_det_get_private] = get_despacho;



            }
            catch (Exception exc)
            {
                estado = "1";
                mensaje = exc.Message;
            }
            return Json(new { estado = estado, mensaje = mensaje,infocab= get_despacho.Almacen_Cab_Update,infodet= get_despacho.Almacen_Det_Update });
        }
        public Ent_Despacho_Almacen_Editar get_consulta_despacho(string iddespacho)
        {
            Ent_Despacho_Almacen_Editar obj = null;
            try
            {
                obj = dat_despacho.get_despacho_almacen_editar(Convert.ToInt32(iddespacho));
            }
            catch (Exception exc)
            {

                throw exc;
            }
            return obj;
        }
        public ActionResult EliminarItemDespachoEdit(string iddespacho,string lid_prom)
        {
            string mensaje = "";
            string estado = "0";           
            try
            {

                List<Ent_Despacho_Almacen_Cab_Update> despacho_cons =(List<Ent_Despacho_Almacen_Cab_Update>) Session[_session_listDespacho_almacen_cab_editar_private];

                if (despacho_cons.Count==1)
                {
                    estado = "1";
                    mensaje = "Accion rechazada: El despacho debe tener al menos un detalle.";
                }
                else
                {
                    Ent_Usuario _usuario = (Ent_Usuario)Session[Ent_Constantes.NameSessionUser];
                    string eliminar_item = dat_despacho.eliminar_despacho_item(_usuario.usu_id,Convert.ToInt32(iddespacho),Convert.ToInt32(lid_prom));

                    if (eliminar_item.Length == 0)
                    {
                        estado = "0";
                        mensaje = "Item elininado con exito.";
                    }
                    else
                    {
                        estado = "1";
                        mensaje = eliminar_item;
                    }
                  
                }
               

            }
            catch (Exception exc)
            {
                estado = "1";
                mensaje = exc.Message;
            }
            return Json(new { estado = estado, mensaje = mensaje });
        }

        public ActionResult getListDespachoAlmacenEditarAjax(Ent_jQueryDataTableParams param/*, string actualizar, string fecha_ini, string fecha_fin, string tipo_des*/)
        {

            //List<Ent_Lista_Despacho> listdespacho = new List<Ent_Lista_Despacho>();

            List<Ent_Despacho_Almacen_Cab_Update> listdespacho = new List<Ent_Despacho_Almacen_Cab_Update>();

            //if (!String.IsNullOrEmpty(actualizar))
            //{
            //    listdespacho = lista_despacho_almacen(tipo_des, fecha_ini, fecha_fin);
            //    //listAtributos = datOE.get_lista_atributos();
            //    Session[_session_listDespacho_almacen_cab_private] = listdespacho.despacho_cab;
            //    Session[_session_listDespacho_almacen_liq_private] = listdespacho.despacho_liq;
            //}

            /*verificar si esta null*/
            if (Session[_session_listDespacho_almacen_cab_editar_private] == null)
            {

                listdespacho = new List<Ent_Despacho_Almacen_Cab_Update>();

                Session[_session_listDespacho_almacen_cab_editar_private] = listdespacho;
                //Session[_session_listDespacho_almacen_liq_private] = listdespacho.despacho_liq;
            }


            //Traer registros
            IQueryable<Ent_Despacho_Almacen_Cab_Update> membercol = ((List<Ent_Despacho_Almacen_Cab_Update>)(Session[_session_listDespacho_almacen_cab_editar_private])).AsQueryable();  //lista().AsQueryable();

            //Manejador de filtros
            int totalCount = membercol.Count();
            IEnumerable<Ent_Despacho_Almacen_Cab_Update> filteredMembers = membercol;

            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredMembers = membercol
                    .Where(m => m.asesor.ToUpper().Contains(param.sSearch.ToUpper()) ||
                        m.NombreLider.ToUpper().Contains(param.sSearch.ToUpper()) ||
                        m.Promotor.ToUpper().Contains(param.sSearch.ToUpper())
                    );
            }
            //Manejador de orden
            //var sortIdx = Convert.ToInt32(Request["iSortCol_0"]);

            //if (param.iSortingCols > 0)
            //{
            //    if (Request["sSortDir_0"].ToString() == "asc")
            //    {
            //        switch (sortIdx)
            //        {
            //            case 0: filteredMembers = filteredMembers.OrderBy(o => o.desp_nrodoc); break;
            //            case 1: filteredMembers = filteredMembers.OrderBy(o => o.desp_descripcion); break;
            //            case 2: filteredMembers = filteredMembers.OrderBy(o => Convert.ToDateTime(o.desp_fechacre)); break;
            //            case 3: filteredMembers = filteredMembers.OrderBy(o => o.totalparesenviado); break;
            //            case 4: filteredMembers = filteredMembers.OrderBy(o => o.estado); break;

            //        }
            //    }
            //    else
            //    {
            //        switch (sortIdx)
            //        {
            //            case 0: filteredMembers = filteredMembers.OrderByDescending(o => o.desp_nrodoc); break;
            //            case 1: filteredMembers = filteredMembers.OrderByDescending(o => o.desp_descripcion); break;
            //            case 2: filteredMembers = filteredMembers.OrderByDescending(o => Convert.ToDateTime(o.desp_fechacre)); break;
            //            case 3: filteredMembers = filteredMembers.OrderByDescending(o => o.totalparesenviado); break;
            //            case 4: filteredMembers = filteredMembers.OrderByDescending(o => o.estado); break;
            //        }
            //    }
            //}
            var displayMembers = filteredMembers
                .Skip(param.iDisplayStart)
                .Take(param.iDisplayLength);


            var result = from a in displayMembers
                         select new
                         {
                             a.asesor,
                             a.IdLider,
                             a.NombreLider,
                             a.Promotor,
                             a.Rotulo,
                             a.Rotulo_Courier,
                             a.Agencia,
                             a.Destino,
                             a.Pedido,
                             a.TotalPares,
                             a.TotalCatalogo,
                             a.TotalPremio,
                             a.Total_Cantidad,
                             a.TotalVenta,
                             //a.Igv,
                             a.McaFlete,
                             a.CobroFlete,
                             a.Lid_Prom,
                             a.Observacion,
                             a.Detalle,
                             a.Total_Cantidad_Envio,
                             a.Desp_IdDetalle,
                             a.Desp_id,
                             a.Distrito,
                             a.Referencia,
                             a.Direccion,
                             a.Celular,
                             a.Delivery
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
        private void ExportarExcel(Ent_Despacho_Almacen_Editar desp,  string NombreArchivo, string tipo_des = "P" /*P=PROVINCIA ; L=LIMA-CALLAO*/)
        {

            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            String style = style = @"<style> .textmode { mso-number-format:\@; } </script> ";
            Page page = new Page();
            try
            {
                List<Ent_Despacho_Delivery> _ListarServico =  (List<Ent_Despacho_Delivery>)Session[_session_Listar_Servicio];
                List<Ent_Despacho_Almacen_Cab_Update> list_cab = desp.Almacen_Cab_Update;
                List<Ent_Despacho_Almacen_Det_Update> list_det = desp.Almacen_Det_Update;

                String inicio;
                
                Style stylePrueba = new Style();
                stylePrueba.Width = Unit.Pixel(200);
                string strRows = "";
                string strRowsHead = "";
                strRowsHead = strRowsHead + "<tr height=38 >";
               
                var PropertyInfos = list_cab.First().GetType().GetProperties();



                foreach (var col in PropertyInfos)
                {               
                    if (tipo_des == "P")
                    {
                        switch (col.Name.ToUpper())
                        {
                            case "ASESOR":
                            case "NOMBRELIDER":
                            case "PROMOTOR":
                            case "ROTULO":
                            case "AGENCIA":
                            case "DESTINO":
                            case "PEDIDO":
                            case "TOTAL_CANTIDAD":
                            case "TOTAL_CANTIDAD_ENVIO":
                            case "TOTALVENTA":
                            case "COBROFLETE":
                            case "OBSERVACION":
                            case "DETALLE":
                                strRowsHead = strRowsHead + "<td height=38  bgcolor='#969696' width='38'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + col.Name + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</ td > ";
                                break;
                        }
                    }
                    else
                    {
                        switch (col.Name.ToUpper())
                        {
                            case "ASESOR":
                            case "NOMBRELIDER":
                            case "PROMOTOR":
                            case "DNI_PROMOTOR":
                            case "ROTULO":
                            case "DISTRITO":
                            case "DIRECCION":
                            case "REFERENCIA":
                            case "CELULAR":
                            case "PEDIDO":
                            case "TOTAL_CANTIDAD":
                            case "TOTAL_CANTIDAD_ENVIO":
                            case "TOTALVENTA":
                            case "COBROFLETE":
                            case "OBSERVACION":
                            case "DELIVERY":
                                //case "DETALLE":
                                strRowsHead = strRowsHead + "<td height=38  bgcolor='#969696' width='38'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + col.Name + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</ td > ";
                                break;
                        }                 
                    }
                    
                }

                strRowsHead = strRowsHead + "</tr>";

                foreach (var Item in list_cab)
                {
                    strRows = strRows + "<tr height='38' >";                    
                    string strClass = "";

                    if (tipo_des == "P")
                    {
                        strRows = strRows + "<td width='400' " + strClass + " >" + Item.asesor + "</ td > ";
                        strRows = strRows + "<td width='400' " + strClass + " >" + Item.NombreLider + "</ td > ";
                        strRows = strRows + "<td width='400' " + strClass + " >" + Item.Promotor + "</ td > ";
                        strRows = strRows + "<td width='400' " + strClass + " >" + Item.Rotulo + "</ td > ";
                        strRows = strRows + "<td width='400' " + strClass + " >" + Item.Agencia + "</ td > ";
                        strRows = strRows + "<td width='400' " + strClass + " >" + Item.Destino + "</ td > ";
                        strRows = strRows + "<td width='400' " + strClass + " >" + Item.Pedido + "</ td > ";
                        strRows = strRows + "<td width='400' " + strClass + " >" + Item.Total_Cantidad.ToString() + "</ td > ";
                        strRows = strRows + "<td width='400' " + strClass + " >" + Item.Total_Cantidad_Envio.ToString() + "</ td > ";
                        strRows = strRows + "<td width='400' " + strClass + " >" + Item.TotalVenta.ToString() + "</ td > ";
                        strRows = strRows + "<td width='400' " + strClass + " >" + Item.CobroFlete + "</ td > ";
                        strRows = strRows + "<td width='400' " + strClass + " >" + Item.Observacion + "</ td > ";
                        strRows = strRows + "<td width='400' " + strClass + " >" + Item.Detalle + "</ td > ";
                    }
                    else
                    {
                        strRows = strRows + "<td width='400' " + strClass + " >" + Item.asesor + "</ td > ";
                        strRows = strRows + "<td width='400' " + strClass + " >" + Item.NombreLider + "</ td > ";
                        strRows = strRows + "<td width='400' " + strClass + " >" + Item.Promotor + "</ td > ";
                        strRows = strRows + "<td width='400' class='xlxTexto' " + strClass + " >" + Item.Dni_Promotor + "</ td > ";
                        strRows = strRows + "<td width='400' " + strClass + " >" + Item.Rotulo + "</ td > ";
                        strRows = strRows + "<td width='400' " + strClass + " >" + Item.Distrito + "</ td > ";
                        strRows = strRows + "<td width='400' " + strClass + " >" + Item.Direccion + "</ td > ";
                        strRows = strRows + "<td width='400' " + strClass + " >" + Item.Referencia + "</ td > ";
                        strRows = strRows + "<td width='400' " + strClass + " >" + Item.Celular + "</ td > ";
                        strRows = strRows + "<td width='400' " + strClass + " >" + Item.Pedido + "</ td > ";                        
                        strRows = strRows + "<td width='400' " + strClass + " >" + Item.Total_Cantidad.ToString() + "</ td > ";
                        strRows = strRows + "<td width='400' " + strClass + " >" + Item.Total_Cantidad_Envio.ToString() + "</ td > ";
                        strRows = strRows + "<td width='400' " + strClass + " >" + Item.TotalVenta.ToString() + "</ td > ";
                        strRows = strRows + "<td width='400' " + strClass + " >" + Item.CobroFlete + "</ td > ";
                        strRows = strRows + "<td width='400' " + strClass + " >" + Item.Observacion + "</ td > ";
                        strRows = strRows + "<td width='400' " + strClass + " >" + (Item.Delivery == "" ? "" : _ListarServico.Where(x => x.Codigo == Item.Delivery).Select(y => new { Descripcion = y.Descripcion }).ElementAt(0).Descripcion) + "</ td > ";
                        //strRows = strRows + "<td width='400' " + strClass + " >" + Item.Detalle + "</ td > ";
                    }
                    
                    
                    strRows = strRows + "</tr>";
                }


                string desc = "";
                string nrodoc = list_cab[0].Desp_NroDoc.ToString();
                string est = list_cab[0].estado.ToString();
                string fec = list_cab[0].Desp_FechaCre;
                string strTotalPedido = list_det[0].NroPedidos.ToString();
                string strTotalEnviado = list_det[0].NroEnviados.ToString();
                string strTotalCataPedido = list_det[0].CatalogPedidos.ToString();
                string strTotalCataEnviado = list_det[0].CatalogEnviados.ToString(); 
                string strTotalPremioPedido = list_det[0].NroPremio.ToString();
                string strTotalPremioEnviado = list_det[0].PremioEnviados.ToString();
                string strTotalMonto = list_det[0].MontoTotal.ToString();

                string strTable = "<table <Table border='1' bgColor='#ffffff' " +
            "borderColor='#000000' cellSpacing='2' cellPadding='2' " +
            "style='font-size:10.0pt; font-family:Calibri; background:white;'>";
                strTable += "<tr height=38 ><td height=38  bgcolor='#969696' width='38'>Nro. Documento </ td ><td width='400' align='left' >" + nrodoc + "</ td > ";
                strTable += "<td height=38  bgcolor='#969696' width='38'>Fec. Creación. </ td ><td width='400' align='left' colspan='2' >" + fec + "</ td > </tr>";
                strTable += "<tr height=38 ><td height=38  bgcolor='#969696' width='38'>Total Monto. </ td ><td width='400' align='left' >" + strTotalMonto + "</ td > ";
                strTable += "<td height=38  bgcolor='#969696' width='38'>Estado </ td ><td width='400' align='left' colspan='2' >" + est + "</ td ></tr>";
                strTable += "<tr height=38 ><td height=38  bgcolor='#969696' width='38'>Pares Pedido. </ td ><td width='400' align='left' >" + strTotalPedido + "</ td > ";
                strTable += "<td height=38  bgcolor='#969696' width='38'>Pares Enviado </ td ><td width='400' align='left' colspan='2' >" + strTotalEnviado + "</ td ></tr>";
                strTable += "<tr height=38 ><td height=38  bgcolor='#969696' width='38'>Catalogo Facturado </ td ><td width='400' align='left' >" + strTotalCataPedido + "</ td > ";
                strTable += "<td height=38  bgcolor='#969696' width='38'>Catalogo Enviado </ td ><td width='400' align='left' colspan='2' >" + strTotalCataEnviado + "</ td ></tr>";

                strTable += "<tr height=38 ><td height=38  bgcolor='#969696' width='38'>Premio Pedido </ td ><td width='400' align='left' >" + strTotalPremioPedido + "</ td > ";
                strTable += "<td height=38  bgcolor='#969696' width='38'>Premio Enviado </ td ><td width='400' align='left' colspan='2' >" + strTotalPremioEnviado + "</ td ></tr>";

                //strTable += "<tr height=38 ><td height=38  bgcolor='#969696' width='38'>Descripción </ td ><td colspan='4' align='left' >" + desc + "</ td > ";
                strTable += "</tr>";

                strTable += "</table>";

                inicio = "<div> " + strTable +
                "<table <Table border='1' bgColor='#ffffff' " +
                "borderColor='#000000' cellSpacing='2' cellPadding='2' " +
                "style='font-size:10.0pt; font-family:Calibri; background:white;'>" +
                strRowsHead +
                strRows +
                "</table>" +
                "</div>";

                sb.Append(inicio);

                Session[_session_despacho_almacen_excel] = sb.ToString();

            }
            catch (Exception exc)
            {

                throw exc;
            }

           
        }
        private void ExportarExcel_new(List<Ent_Despacho_Almacen_Cab> list_cab,  string NombreArchivo, string tipo_des = "P" /*P=PROVINCIA ; L=LIMA-CALLAO*/)
        {

            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            String style = style = @"<style> .textmode { mso-number-format:\@; } </script> ";
            Page page = new Page();
            try
            {
                //List<Ent_Despacho_Almacen_Cab_Update> list_cab = desp.Almacen_Cab_Update;
                //List<Ent_Despacho_Almacen_Det_Update> list_det = desp.Almacen_Det_Update;
                List<Ent_Despacho_Delivery> _ListarServico = (List<Ent_Despacho_Delivery>)Session[_session_Listar_Servicio];
                String inicio;
              

                Style stylePrueba = new Style();
                stylePrueba.Width = Unit.Pixel(200);
                string strRows = "";
                string strRowsHead = "";
                strRowsHead = strRowsHead + "<tr height=38 >";

                var PropertyInfos = list_cab.First().GetType().GetProperties();
                foreach (var col in PropertyInfos)
                {
                    if (tipo_des=="P")
                    {
                        switch (col.Name.ToUpper())
                        {
                            case "ASESOR":
                            case "NOMBRELIDER":
                            case "PROMOTOR":
                            case "ROTULO":
                            case "AGENCIA":
                            case "DESTINO":
                            case "PEDIDO":
                            case "TOTALCANTIDAD":
                            case "TOTALVENTA":
                            case "FLETE":
                                strRowsHead = strRowsHead + "<td height=38  bgcolor='#969696' width='38'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + col.Name + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</ td > ";
                                break;
                        }
                    }
                    else
                    {
                        switch (col.Name.ToUpper())
                        {
                            case "ASESOR":
                            case "NOMBRELIDER":
                            case "PROMOTOR":
                            case "ROTULO":
                            case "DISTRITO":
                            case "DIRECCION":
                            case "REFERENCIA":
                            case "CELULAR":
                            case "PEDIDO":
                            case "TOTALCANTIDAD":
                            case "TOTALVENTA":
                            case "FLETE":
                            case "DELIVERY":
                                strRowsHead = strRowsHead + "<td height=38  bgcolor='#969696' width='38'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + col.Name + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</ td > ";
                                break;
                        }
                    }
                    
                }

                strRowsHead = strRowsHead + "</tr>";

                foreach (var Item in list_cab)
                {
                    strRows = strRows + "<tr height='38' >";
                    string strClass = "";

                    if (tipo_des == "P")
                    {
                        strRows = strRows + "<td width='400' " + strClass + " >" + Item.Asesor + "</ td > ";
                        strRows = strRows + "<td width='400' " + strClass + " >" + Item.NombreLider + "</ td > ";
                        strRows = strRows + "<td width='400' " + strClass + " >" + Item.Promotor + "</ td > ";
                        strRows = strRows + "<td width='400' " + strClass + " >" + Item.Rotulo + "</ td > ";
                        strRows = strRows + "<td width='400' " + strClass + " >" + Item.Agencia + "</ td > ";
                        strRows = strRows + "<td width='400' " + strClass + " >" + Item.Destino + "</ td > ";
                        strRows = strRows + "<td width='400' " + strClass + " >" + Item.Pedido + "</ td > ";
                        strRows = strRows + "<td width='400' " + strClass + " >" + Item.TotalCantidad.ToString() + "</ td > ";
                        strRows = strRows + "<td width='400' " + strClass + " >" + Item.TotalVenta.ToString() + "</ td > ";
                        strRows = strRows + "<td width='400' " + strClass + " >" + Item.Flete + "</ td > ";
                    }
                    else
                    {
                        strRows = strRows + "<td width='400' " + strClass + " >" + Item.Asesor + "</ td > ";
                        strRows = strRows + "<td width='400' " + strClass + " >" + Item.NombreLider + "</ td > ";
                        strRows = strRows + "<td width='400' " + strClass + " >" + Item.Promotor + "</ td > ";
                        strRows = strRows + "<td width='400' " + strClass + " >" + Item.Rotulo + "</ td > ";
                        strRows = strRows + "<td width='400' " + strClass + " >" + Item.Distrito + "</ td > ";
                        strRows = strRows + "<td width='400' " + strClass + " >" + Item.Direccion + "</ td > ";
                        strRows = strRows + "<td width='400' " + strClass + " >" + Item.Referencia + "</ td > ";
                        strRows = strRows + "<td width='400' " + strClass + " >" + Item.Celular + "</ td > ";
                        strRows = strRows + "<td width='400' " + strClass + " >" + Item.Pedido + "</ td > ";
                        strRows = strRows + "<td width='400' " + strClass + " >" + Item.TotalCantidad.ToString() + "</ td > ";
                        strRows = strRows + "<td width='400' " + strClass + " >" + Item.TotalVenta.ToString() + "</ td > ";
                        strRows = strRows + "<td width='400' " + strClass + " >" + Item.Flete + "</ td > ";
                        strRows = strRows + "<td width='400' " + strClass + " >" + _ListarServico.Where(x => x.Codigo == Item.Delivery).Select(y => new { Descripcion = y.Descripcion }).ElementAt(0).Descripcion + "</ td > ";
                    }
                    
                    strRows = strRows + "</tr>";
                }


                inicio = "<div> " +
                "<table <Table border='1' bgColor='#ffffff' " +
                "borderColor='#000000' cellSpacing='2' cellPadding='2' " +
                "style='font-size:10.0pt; font-family:Calibri; background:white;'>" +
                strRowsHead +
                strRows +
                "</table>" +
                "</div>";

                sb.Append(inicio);

                Session[_session_despacho_almacen_excel] = sb.ToString();

            }
            catch (Exception exc)
            {

                throw exc;
            }


        }

        private string _session_despacho_almacen_excel = "_session_stock_articulo_categoria_excel";
        public ActionResult ListaDespachoExcel()
        {
            string NombreArchivo = "Orden_Despacho";
            String style = style = @"<style> .textmode { mso-number-format:\@; } 
                                .xlxTexto
	                            {padding-top:1px;
	                            padding-right:1px;
	                            padding-left:1px;
	                            mso-ignore:padding;
	                            color:black;
	                            font-size:10.0pt;
	                            font-weight:400;
	                            font-style:normal;
	                            text-decoration:none;
	                            font-family:Calibri, sans-serif;
	                            mso-font-charset:0;
	                            mso-number-format:\@;
	                            text-align:general;
	                            vertical-align:bottom;
	                            border:.5pt solid black;
	                            background:white;
	                            mso-pattern:black none;
	                            white-space:normal;}
                            </style> ";
            try
            {
                Response.Clear();
                Response.Buffer = true;
                Response.ContentType = "application/vnd.ms-excel";
                Response.AddHeader("Content-Disposition", "attachment;filename=" + NombreArchivo + ".xls");
                Response.Charset = "UTF-8";
                Response.ContentEncoding = Encoding.Default;
                Response.Write(style);
                Response.Write(Session[_session_despacho_almacen_excel].ToString());
                Response.End();

            }
            catch 
            {
                
            }
                      
            return Json(new { estado = 0, mensaje = 1 });
        }
        public ActionResult get_exporta_excel(string id="0",string tipo_des="P" /*P=PROVINCIA ; L=LIMA-CALLAO*/)
        {
            string mensaje = "";
            string estado = "0";
            try
            {
                Ent_Despacho_Almacen_Editar despacho_lista = get_consulta_despacho(id);
                ExportarExcel(despacho_lista, "Orden_Despacho", tipo_des);

                mensaje = "Se genero el excel correctamente";
                estado = "1";
               
            }
            catch (Exception exc)
            {
                estado = "0";
                mensaje = exc.Message;
            }

            return Json(new { estado = estado, mensaje = mensaje });
        }

        public ActionResult get_exporta_excel_new(string id = "0",string tipo_des="P")
        {
            string mensaje = "";
            string estado = "0";
            try
            {
                List<Ent_Despacho_Almacen_Cab> despacho_cab=(List<Ent_Despacho_Almacen_Cab>)Session[_session_listDespacho_almacen_cab_private];
                
                ExportarExcel_new(despacho_cab, "Despacho_Pendiente",tipo_des);

                mensaje = "Se genero el excel correctamente";
                estado = "1";

            }
            catch (Exception exc)
            {
                estado = "0";
                mensaje = exc.Message;
            }

            return Json(new { estado = estado, mensaje = mensaje });
        }

        #endregion
        #endregion
        #region<AGRUPAR DESPACHO>

        private string _session_despacho_agrupar = "_session_despacho_agrupar";

        public ActionResult AgruparDespacho()
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
                    Session[_session_despacho_agrupar] = null;
                    return View();
                }
                else
                {
                    return RedirectToAction("Error_Roles", "Home");
                } 
            } 
        }

        public ActionResult BorrarTodoGrupo()
        {
          
            string mensaje = "";                        
            List<Ent_Lista_Despacho> listdesp = new List<Ent_Lista_Despacho>();
            Session[_session_despacho_agrupar] = listdesp;
            return Json(new { estado = 0, mensaje = mensaje });
        }

        public ActionResult GenerarGrupoValida()
        {
            string mensaje = "";
            Int32 estado = 0;
            try
            {
                List<Ent_Lista_Despacho> listdesp = (List<Ent_Lista_Despacho>)Session[_session_despacho_agrupar];

                if (listdesp.Count <= 1)
                {
                    estado = 1;
                    mensaje = "No hay datos para agrupar ,Agregar las ordenes de despacho ó no se puede agrupar solo un despacho";
                }                

            }
            catch
            {

            }

            return Json(new { estado = estado, mensaje = mensaje });
        }


        public ActionResult GenerarGrupo()
        {
            string mensaje = "";
            Int32 estado = 0;
            string orden = "";
            try
            {
                Ent_Usuario _usuario = (Ent_Usuario)Session[Ent_Constantes.NameSessionUser];
                List<Ent_Lista_Despacho> listdesp = (List<Ent_Lista_Despacho>)Session[_session_despacho_agrupar];

                if (listdesp.Count == 0)
                {
                    estado = 1;
                    mensaje = "No hay datos para agrupar , por favor actualiza la pagina";
                }
                else
                {

                    Dat_Despacho dat_despacho = new Dat_Despacho();

                    DataTable dt = new DataTable();
                    dt.Columns.Add("desp_nrodoc",typeof(string));

                    foreach(Ent_Lista_Despacho it in listdesp)
                    {
                        dt.Rows.Add(it.desp_nrodoc);
                    }



                    string error = dat_despacho.generar_despacho_grupo(_usuario.usu_id, dt, ref orden);

                    if (error.Length > 0)
                    {
                        estado = 1;
                        mensaje = error;
                    }
                    else
                    {
                        estado = 0;
                        //orden = orden;
                        Session[_session_despacho_agrupar] = null;
                    }
                    
                }

            }
            catch 
            {
               
            }
                       
            return Json(new { estado = estado, mensaje = mensaje,orden=orden });
        }

        public ActionResult BorrarItemGrupo(string nro_desp)
        {

            string mensaje = "";

            List<Ent_Lista_Despacho> listdesp = (List<Ent_Lista_Despacho>) Session[_session_despacho_agrupar];
           

            List<Ent_Lista_Despacho> item = listdesp.Where(a=>a.desp_nrodoc==nro_desp).ToList();

            listdesp.Remove(item[0]);

            Session[_session_despacho_agrupar] = listdesp;

            return Json(new { estado = 0, mensaje = mensaje });
        }
        public ActionResult getTableDespachoAjax(Ent_jQueryDataTableParams param)
        {
            /*verificar si esta null*/
            if (Session[_session_despacho_agrupar] == null)
            {
                List<Ent_Lista_Despacho> listdesp = new List<Ent_Lista_Despacho>();
                Session[_session_despacho_agrupar] = listdesp;
            }

            //}
            //Traer registros
            IQueryable<Ent_Lista_Despacho> membercol = ((List<Ent_Lista_Despacho>)(Session[_session_despacho_agrupar])).AsQueryable();

            //Manejador de filtros
            int totalCount = membercol.Count();

            IEnumerable<Ent_Lista_Despacho> filteredMembers = membercol;


            //Manejador de orden
            var displayMembers = filteredMembers
                .Skip(param.iDisplayStart)
                .Take(param.iDisplayLength);

            var result = from a in displayMembers
                         select new
                         {
                             a.desp_nrodoc,                             
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

        public ActionResult get_nro_despacho(string nro_despacho)
        {
            Dat_Despacho dat_despacho = new Dat_Despacho();
            List<Ent_Lista_Despacho> listar_despacho = null;
            
            string mensaje = "";
            string estado = "0";            
            try
            {

                if (Session[_session_despacho_agrupar] == null)
                {
                    List<Ent_Lista_Despacho> listar_despacho_new = new List<Ent_Lista_Despacho>();
                    Session[_session_despacho_agrupar] = listar_despacho_new;
                }
                    

                listar_despacho = dat_despacho.buscar_nro_despacho(nro_despacho);

                if (listar_despacho.Count == 0)
                {
                    

                    estado = "0";
                    mensaje = "No hay N° de despacho con el numero ingresado";
                }
                else
                {
                    /*esta condicion es porque el numero de orden esta despachado por almacen*/
                    if (listar_despacho[0].estado == "S")
                    {
                        estado = "0";
                        mensaje = "El numero de despacho ya se encuentra despachado por almacen";
                    }
                    else
                    {
                        /*en este caso vemos que casos*/
                        List<Ent_Lista_Despacho> lista_des_temp = (List<Ent_Lista_Despacho>)Session[_session_despacho_agrupar];
                        /*en este caso si no hay dato en nuestra variable actualizamos nuestra lista*/
                        if (lista_des_temp.Count == 0)
                        {
                            Ent_Lista_Despacho desp = new Ent_Lista_Despacho();
                            desp.desp_nrodoc = listar_despacho[0].desp_nrodoc;
                            desp.estado = listar_despacho[0].estado;
                            desp.desp_tipo= listar_despacho[0].desp_tipo;
                            lista_des_temp.Add(desp);
                            Session[_session_despacho_agrupar] = lista_des_temp;
                            estado = "1";
                            mensaje = "Se Agrego correctamente el N° de despacho";
                        }
                        else
                        {
                            var existe_lista = lista_des_temp.Where(a=>a.desp_nrodoc== listar_despacho[0].desp_nrodoc).ToList();
                            /*en este caso es porque ya se encuentra disponible */
                            if (existe_lista.Count > 0)
                            {
                                estado = "0";
                                mensaje = "El numero de despacho ya se encuentra en la lista para agrupar, ingrese otro numero por favor";
                            }
                            else
                            {
                                var existe_des_tipo = lista_des_temp.Where(a => a.desp_tipo == listar_despacho[0].desp_tipo).ToList();

                                if (existe_des_tipo.Count == 0)
                                {
                                    estado = "0";
                                    mensaje = "El numero de despacho para agrupar no puede combinarse Lima-Callao y Provincia";
                                }
                                else
                                {
                                    Ent_Lista_Despacho desp = new Ent_Lista_Despacho();
                                    desp.desp_nrodoc = listar_despacho[0].desp_nrodoc;
                                    desp.estado = listar_despacho[0].estado;
                                    desp.desp_tipo = listar_despacho[0].desp_tipo;
                                    lista_des_temp.Add(desp);
                                    Session[_session_despacho_agrupar] = lista_des_temp;
                                    estado = "1";
                                    mensaje = "Se Agrego correctamente el N° de despacho";
                                }


                            }

                            /*si el temporal esta lleno verificamos que no se repita de nuevo*/


                        }

                        
                    }
                }

                //Session[_session_stock_x_articulo] = listar_articulo;
                //Session[_session_stock_x_articulo_filtro] = listar_articulo.Where(s => s.id_almacen == "AQ").ToList();
            }
            catch
            {


            }

            return Json(new { estado = estado, mensaje = mensaje });
        }
        #endregion
        #region<GESTION DE STOCK>
        private string _session_ListarGestion_Stock_Excel = "_session_ListarGestion_Stock_Excel";
        private string _session_ListarGestion_Stock_Data = "_session_ListarGestion_Stock_Data";

        public ActionResult ListaLogisticaStockExcel()
        {
            string NombreArchivo = "Logistica_Stock";
            String style = style = @"<style> .textmode { mso-number-format:\@; } 
                                .xlxTexto
	                            {padding-top:1px;
	                            padding-right:1px;
	                            padding-left:1px;
	                            mso-ignore:padding;
	                            color:black;
	                            font-size:10.0pt;
	                            font-weight:400;
	                            font-style:normal;
	                            text-decoration:none;
	                            font-family:Calibri, sans-serif;
	                            mso-font-charset:0;
	                            mso-number-format:\@;
	                            text-align:general;
	                            vertical-align:bottom;
	                            border:.5pt solid black;
	                            background:white;
	                            mso-pattern:black none;
	                            white-space:normal;}
                            </style> ";
            try
            {
                Response.Clear();
                Response.Buffer = true;
                Response.ContentType = "application/vnd.ms-excel";
                Response.AddHeader("Content-Disposition", "attachment;filename=" + NombreArchivo + ".xls");
                Response.Charset = "UTF-8";
                Response.ContentEncoding = Encoding.Default;
                Response.Write(style);
                Response.Write(Session[_session_ListarGestion_Stock_Excel].ToString());
                Response.End();

            }
            catch
            {

            }

            return Json(new { estado = 0, mensaje = 1 });
        }

        public ActionResult gestion_stock()
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
                    Session[_session_ListarGestion_Stock_Data] = null;
                    Session[_session_ListarGestion_Stock_Excel] = null;
                    return View();
                }
                else
                {
                    return RedirectToAction("Error_Roles", "Home");
                }
               
            }
        }
        public DataTable dt_articulo(List<Ent_Articulo_Precio> articulo)
        {
            DataTable dt = null;
            try
            {
                dt = new DataTable();
                dt.Columns.Add("Artciulo",typeof(string));
                foreach(var item in articulo.GroupBy(a=>a.articulo))
                {
                    dt.Rows.Add(item.Key.ToString());
                }
            }
            catch (Exception exc)
            {
                throw exc;
            }
            return dt;
        }
        public ActionResult JsonExcelArticulos_Logistica(string articulos)
        {
            List<Ent_Articulo_Precio> listArtExcel = null;
            Dat_Gestion_Stock dat_gestion = new Dat_Gestion_Stock();
            //Dat_Articulo_Stock stk_articulo = new Dat_Articulo_Stock();
            try
            {
                listArtExcel = new List<Ent_Articulo_Precio>();
                listArtExcel = JsonConvert.DeserializeObject<List<Ent_Articulo_Precio>>(articulos.ToUpper());
                if (listArtExcel.Where(w => String.IsNullOrEmpty(w.articulo)).ToList().Count > 0)
                {
                    
                    Session[_session_ListarGestion_Stock_Data] = new Ent_Gestion_Stock();
                    Session[_session_ListarGestion_Stock_Excel] = new Ent_Gestion_Stock();
                    return Json(new { estado = 0, resultados = "El Archivo no tiene el formato correcto ó hay campos vacios.\nVerifique el archivo." });
                }
                else
                {

                    DataTable dt = dt_articulo(listArtExcel);
                    Session[_session_ListarGestion_Stock_Data] = dat_gestion.get_gestion(dt);// dat_precio.lista_articulo_precio(listArtExcel);
                    return Json(new { estado = 1, resultados = "ok" });

                }
            }
            catch (Exception ex)
            {
                return Json(new { estado = 0, resultados = ex.Message });
            }

        }
        public string get_html_ListarGestion_Stock_str(Ent_Gestion_Stock obj_gestion)
        {
            StringBuilder sb = new StringBuilder();
            //var Lista = _ListarPedidos_Vencidos.ToList();
            try
            {
                sb.Append("<div>");
                sb.Append("<table border='1' cellspacing='0' >");
                sb.Append("<tr><td Colspan='17'></td></tr>");
                sb.Append("<tr><td Colspan='4'></td><td valign='middle' align='center'>Regla</td><td Colspan='12' width='400' valign='middle' align='center' style='vertical-align: middle;font-size: 9.0pt;font-weight: bold;color:#285A8F'>Tallas</tr>");

                List<Ent_Gestion_Stock_Medida> medida = obj_gestion.gestion_medida;
                List<Ent_Gestion_Stock_Detalle> articulo = obj_gestion.gestion_detalle;

                foreach (var med_grup in medida.GroupBy(c=>c.cod_rgmed))
                {
                    string str_html_regla = "";
                    for(Int32 i=0;i<12;++i)
                    {
                        var reg_med = medida.Where(a => a.cod_rgmed == med_grup.Key && Convert.ToInt32(a.reg_med) == i).ToList();
                        
                        if (reg_med.Count==0)
                        {
                            str_html_regla += "<td valign='middle' align='center'></td>";
                        }
                        else
                        {
                            str_html_regla += "<td valign='middle' align='center'>" + reg_med[0].talla + "</td>";
                        }

                        
                    }
                    
                    sb.Append("<tr><td Colspan='4'></td><td valign='middle' align='center'>" + med_grup.Key + "</td>" + str_html_regla + "</tr>");
                }
                sb.Append("<tr><td Colspan='17'></td></tr>");

                sb.Append("<tr><td bgcolor='#C1F7DA' valign='middle' align='center'>Articulo</td><td bgcolor='#C1F7DA' valign='middle' align='center'   >Accion</td><td valign='middle' align='center' bgcolor='#C1F7DA' width='60' >Foto</td><td valign='middle' align='center' bgcolor='#C1F7DA' width='60' >Almacen</td><td Colspan='13'></td><td bgcolor='#C1F7DA' valign='middle' align='center'>Total</td></tr>");

                /*agrupar articulo*/
                foreach (var articulo_grup in articulo.GroupBy(c => c.articulo))
                {
                    string html_almac = "";
                    foreach (var alma in articulo.Where(a => a.articulo == articulo_grup.Key && a.item == 10).GroupBy(c => c.almacen))
                    {
                        html_almac += "<tr><td bgcolor='#C1F7DA' ></td><td bgcolor='#C1F7DA' ></td><td bgcolor='#C1F7DA' ></td><td valign='middle' align='center' bgcolor='#C1F7DA' >" + alma.Key.ToString() + "<td bgcolor='#C1F7DA'></td></td>";
                        string str_html_talla = "";
                        for (Int32 i = 0; i < 12; ++i)
                        {
                            var talla_stock = articulo.Where(a => a.item == 10 && a.almacen == alma.Key && a.articulo == articulo_grup.Key && Convert.ToInt32(a.med_per) == i).ToList();

                            if (talla_stock.Count == 0)
                            {
                                str_html_talla += "<td bgcolor='#C1F7DA' valign='middle' align='center'></td>";
                            }
                            else
                            {
                                str_html_talla += "<td bgcolor='#C1F7DA' valign='middle' align='center'>" + talla_stock[0].stock.ToString() + "</td>";
                            }

                        }

                        var total = articulo.Where(a => a.almacen == alma.Key && a.articulo == articulo_grup.Key).ToList().Sum(b => b.stock);

                        string thtml_total1 = "<td bgcolor='#C1F7DA' valign='middle' align='center'>" + total.ToString() + "</td>";

                        html_almac += str_html_talla + thtml_total1 + "</tr>";
                    }

                    string html_venta = "";

                    var articulo_venta = articulo.Where(a => a.item == 11 && a.articulo == articulo_grup.Key).ToList();
                    if (articulo_venta.Count > 0)
                    {
                        html_venta = "<tr><td Colspan='2' bgcolor='#F9BD27'>" + articulo_venta[0].almacen + "</td><td bgcolor='#F9BD27'></td><td bgcolor='#F9BD27'></td>";                 
                        html_venta += "<td bgcolor='#F9BD27'></td>"; //html_articulo_foto;

                        string str_html_talla_ven = "";
                        for (Int32 i = 0; i < 12; ++i)
                        {
                            var talla_stock = articulo.Where(a => a.item == 11 && a.articulo == articulo_grup.Key && Convert.ToInt32(a.med_per) == i).ToList();

                            if (talla_stock.Count == 0)
                            {
                                str_html_talla_ven += "<td bgcolor='#F9BD27' valign='middle' align='center'></td>";
                            }
                            else
                            {
                                str_html_talla_ven += "<td bgcolor='#F9BD27' valign='middle' align='center'>" + talla_stock[0].stock.ToString() + "</td>";
                            }

                        }


                        var total2_venta = articulo.Where(a => a.item == 11 && a.articulo == articulo_grup.Key).ToList().Sum(b => b.stock);

                        string thtml_total2_venta = "<td bgcolor='#F9BD27' valign='middle' align='center'>" + total2_venta.ToString() + "</td>";

                        html_venta += str_html_talla_ven + thtml_total2_venta + "</tr>";
                    }

                    string html_separado = "";

                    var articulo_separado = articulo.Where(a => a.item == 12 && a.articulo == articulo_grup.Key).ToList();
                    if (articulo_separado.Count > 0)
                    {
                        html_separado = "<tr><td Colspan='2' bgcolor='#ADADAD'>" + articulo_separado[0].almacen + "</td><td bgcolor='#ADADAD'></td><td bgcolor='#ADADAD'></td>";
                        html_separado += "<td bgcolor='#ADADAD'></td>"; //html_articulo_foto;

                        string str_html_talla_separado = "";
                        for (Int32 i = 0; i < 12; ++i)
                        {
                            var talla_stock = articulo.Where(a => a.item == 12 && a.articulo == articulo_grup.Key && Convert.ToInt32(a.med_per) == i).ToList();

                            if (talla_stock.Count == 0)
                            {
                                str_html_talla_separado += "<td bgcolor='#ADADAD' valign='middle' align='center'></td>";
                            }
                            else
                            {
                                str_html_talla_separado += "<td bgcolor='#ADADAD' valign='middle' align='center'>" + talla_stock[0].stock.ToString() + "</td>";
                            }

                        }


                        var total2_separado = articulo.Where(a => a.item == 12 && a.articulo == articulo_grup.Key).ToList().Sum(b => b.stock);

                        string thtml_total2_separado = "<td bgcolor='#ADADAD' valign='middle' align='center'>" + total2_separado.ToString() + "</td>";

                        html_separado += str_html_talla_separado + thtml_total2_separado + "</tr>";
                    }

                    //string html_articulo = "<tr><td> " + articulo_grup.Key.ToString() + " </td><td></td>";
                    string html_articulo = "<tr><td bgcolor='#7FDB86'>Stock 100</td><td bgcolor='#7FDB86'></td><td bgcolor='#7FDB86'></td><td bgcolor='#7FDB86'></td>";

                    var articulo_foto = articulo.Where(a => a.item == 20 && a.articulo == articulo_grup.Key).ToList();

                    string html_articulo_foto = "";

                    if (articulo_foto.Count==0)
                    {
                        html_articulo_foto = "<td></td>";
                    }
                    else
                    { 
                        html_articulo_foto = "<td style='text-align: center ;vertical-align: top;' height = 40><div style='margin: 50 auto; width: 130px'><img WIDTH='55' HEIGHT='30' alt='Logo_FR' src=" + articulo_foto[0].foto + " style = 'margin: 30px 30px 30px 30px; vertical-align:middle; padding-left:20;padding-top:30;top: 50px' /></div></td>";
                    }

                    //html_articulo_foto+= "<td></td><td></td>";

                    html_articulo += "<td bgcolor='#7FDB86'></td>"; //html_articulo_foto;

                    string str_html_talla_cat = "";
                    for (Int32 i = 0; i < 12; ++i)
                    {
                        var talla_stock = articulo.Where(a => a.item == 20 &&  a.articulo == articulo_grup.Key && Convert.ToInt32(a.med_per) == i).ToList();

                        if (talla_stock.Count == 0)
                        {
                            str_html_talla_cat += "<td bgcolor='#7FDB86' valign='middle' align='center'></td>";
                        }
                        else
                        {
                            str_html_talla_cat += "<td bgcolor='#7FDB86' valign='middle' align='center'>" + talla_stock[0].stock.ToString() + "</td>";
                        }

                    }

                    var total2 = articulo.Where(a => a.item == 20 && a.articulo == articulo_grup.Key).ToList().Sum(b => b.stock);

                    string thtml_total2 = "<td bgcolor='#7FDB86' valign='middle' align='center'>" + total2.ToString() + "</td>";

                    html_articulo += str_html_talla_cat + thtml_total2 + "</tr>";

                    string html_tras = "<tr align='center' style='text-align: center;align-content: center;' valign='middle' ><td style='text-align: center;align-content: center;border: 1.5px solid black' valign='middle' align='center'>" + articulo_grup.Key + "<td style='text-align: center;align-content: center;border: 1.5px solid black'>Traspasar</td>" + html_articulo_foto + "<td style='text-align: center;align-content: center;border: 1.5px solid black'></td><td style='text-align: center;align-content: center;border: 1.5px solid black'></td><td style='text-align: center;align-content: center;border: 1.5px solid black'></td><td style='text-align: center;align-content: center;border: 1.5px solid black'></td><td style='text-align: center;align-content: center;border: 1.5px solid black'></td><td style='text-align: center;align-content: center;border: 1.5px solid black'></td><td style='text-align: center;align-content: center;border: 1.5px solid black'></td><td style='text-align: center;align-content: center;border: 1.5px solid black'></td><td style='text-align: center;align-content: center;border: 1.5px solid black'></td><td style='text-align: center;align-content: center;border: 1.5px solid black'></td><td style='text-align: center;align-content: center;border: 1.5px solid black'></td><td style='text-align: center;align-content: center;border: 1.5px solid black'></td><td style='text-align: center;align-content: center;border: 1.5px solid black'></td><td style='text-align: center;align-content: center;border: 1.5px solid black'></td><td style='text-align: center;align-content: center;border: 1.5px solid black'></td></tr>";

                    sb.Append(html_almac);
                    if (articulo_venta.Count>0) sb.Append(html_venta);                    
                    sb.Append(html_articulo);
                    if (articulo_separado.Count > 0) sb.Append(html_separado);
                    sb.Append(html_tras);

                }


                    //sb.Append("<tr><td Colspan='7' valign='middle' align='center' style='vertical-align: middle;font-size: 18.0pt;font-weight: bold;color:#285A8F'>REPORTE DE PEDIDOS VENCIDOS </td></tr>");
                    //sb.Append("<tr><td Colspan='7' valign='middle' align='center' style='vertical-align: middle;font-size: 10.0pt;font-weight: bold;color:#000000'>Rango de : " + String.Format("{0:dd/MM/yyyy}", _Ent.FechaIni) + " hasta " + String.Format("{0:dd/MM/yyyy}", _Ent.FechaFin) + "</td></tr>");//subtitulo
                    //sb.Append("<tr>\n");
                    //sb.Append("<th bgColor='#1E77AB' style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Nro Pedido</font></th>\n");
                    //sb.Append("<th bgColor='#1E77AB' style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Asesor</font></th>\n");
                    //sb.Append("<th bgColor='#1E77AB' style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Directora</font></th>\n");
                    //sb.Append("<th bgColor='#1E77AB' style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Promotor</font></th>\n");
                    //sb.Append("<th bgColor='#1E77AB' style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Fec.Pedido</font></th>\n");
                    //sb.Append("<th bgColor='#1E77AB' style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Fec.Venc.</font></th>\n");
                    //sb.Append("<th bgColor='#1E77AB' style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Pares</font></th>\n");
                    sb.Append("</tr>\n");
                // {0:N2} Separacion miles , {0:F2} solo dos decimales
                //foreach (var item in Lista)
                //{
                //    sb.Append("<tr>\n");
                //    sb.Append("<td align=''>" + item.pedido + "</td>\n");
                //    sb.Append("<td align=''>" + item.asesor + "</td>\n");
                //    sb.Append("<td align=''>" + item.lider + "</td>\n");
                //    sb.Append("<td align=''>" + item.promotor + "</td>\n");
                //    sb.Append("<td align=''>" + item.fechapedido + "</td>\n");
                //    sb.Append("<td align=''>" + item.fechaven + "</td>\n");
                //    sb.Append("<td align='Right'>" + item.pares + "</td>\n");
                //    sb.Append("</tr>\n");
                //}
                sb.Append("</table></div>");
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return sb.ToString();
        }

        public ActionResult get_exporta_LisGestion_Stock_excel()
        {
            JsonResponse objResult = new JsonResponse();
            //Dat_Gestion_Stock dat_gestion = new Dat_Gestion_Stock();
            string cadena = "";
            try
            {
                //Session[_session_ListarGestion_Stock_Excel] = null;

                //Ent_Gestion_Stock obj_gestion = dat_gestion.get_gestion();
                Ent_Gestion_Stock obj_gestion =(Ent_Gestion_Stock) Session[_session_ListarGestion_Stock_Data];

                if (obj_gestion.gestion_detalle.Count == 0)
                {
                    objResult.Success = false;
                    objResult.Message = "No hay filas para exportar";
                }
                else
                {
                    cadena = get_html_ListarGestion_Stock_str(obj_gestion);
                    if (cadena.Length == 0)
                    {
                        objResult.Success = false;
                        objResult.Message = "Error del formato html";
                    }
                    else
                    {
                        objResult.Success = true;
                        objResult.Message = "Se genero el excel correctamente";
                        Session[_session_ListarGestion_Stock_Excel] = cadena;
                    }
                }

                
            }
            catch (Exception ex)
            {
                objResult.Success = false;
                objResult.Message = "No hay filas para exportar";
            }

            var JSON = JsonConvert.SerializeObject(objResult);

            return Json(JSON, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}