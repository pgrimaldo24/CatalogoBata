﻿using CapaDato.Control;
using CapaDato.Maestros;
using CapaEntidad.Control;
using CapaEntidad.General;
using CapaEntidad.Maestros;
using CapaEntidad.Menu;
using CapaEntidad.Util;
using CapaPresentacion.Bll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace CapaPresentacion.Controllers
{
    public class UsuariosController : Controller
    {
        // GET: Usuarios
        private Dat_Usuario usuario = new Dat_Usuario();
        private string _session_listusu_private = "session_listusu_private";
        //[Authorize]
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
                    ViewBag.usu_tipo = _usuario.usu_tip_id;
                    return View(lista());
                }
                else
                {
                    return RedirectToAction("Error_Roles", "Home");
                }
            }
        }

        public PartialViewResult ListaUsuario()
        {
            return PartialView(lista());
        }

        public List<Ent_Usuario> lista()
        {
            Ent_Usuario _usuario = (Ent_Usuario)Session[Ent_Constantes.NameSessionUser];
            List<Ent_Usuario> listusuario = usuario.get_lista(false, _usuario.usu_id);
            Session[_session_listusu_private] = listusuario;
            return listusuario;
        }

        public ActionResult getListaUsuarioAjax(Ent_jQueryDataTableParams param, string actualizar)
        {

            List<Ent_Usuario> listusuario = new List<Ent_Usuario>();

            if (!String.IsNullOrEmpty(actualizar))
            {
                listusuario = lista();
                //listAtributos = datOE.get_lista_atributos();
                Session[_session_listusu_private] = listusuario;
            }

            /*verificar si esta null*/
            if (Session[_session_listusu_private] == null)
            {
                listusuario = new List<Ent_Usuario>();
                listusuario = lista(); //datOE.get_lista_atributos();
                if (listusuario == null)
                {
                    listusuario = new List<Ent_Usuario>();
                }
                Session[_session_listusu_private] = listusuario;
            }

            //Traer registros

            IQueryable<Ent_Usuario> membercol = ((List<Ent_Usuario>)(Session[_session_listusu_private])).AsQueryable();  //lista().AsQueryable();

            //Manejador de filtros
            int totalCount = membercol.Count();
            IEnumerable<Ent_Usuario> filteredMembers = membercol;

            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredMembers = membercol
                    .Where(m => m.usu_id.ToString().ToUpper().Contains(param.sSearch.ToUpper()) ||
                     m.usu_nombre.ToString().ToUpper().Contains(param.sSearch.ToUpper()) ||
                     m.usu_login.ToString().ToUpper().Contains(param.sSearch.ToUpper()));
            }
            //Manejador de orden
            var sortIdx = Convert.ToInt32(Request["iSortCol_0"]);
            var sortDirection = Request["sSortDir_0"];
            if (param.iSortingCols > 0)
            {
                if (sortDirection == "asc")
                {
                    if (sortIdx == 0) filteredMembers = filteredMembers.OrderBy(o => o.usu_id);
                    else if (sortIdx == 1) filteredMembers = filteredMembers.OrderBy(o => o.usu_nombre);
                    else if (sortIdx == 2) filteredMembers = filteredMembers.OrderBy(o => o.usu_login);                    
                }
                else
                {
                    if (sortIdx == 0) filteredMembers = filteredMembers.OrderByDescending(o => o.usu_id);
                    else if (sortIdx == 1) filteredMembers = filteredMembers.OrderByDescending(o => o.usu_nombre);
                    else if (sortIdx == 2) filteredMembers = filteredMembers.OrderByDescending(o => o.usu_login);                    
                }
            }

            //Func<Ent_Funcion, DateTime> orderingFunction =
            //    (
            //        m => Convert.ToDateTime(m.FECHA_CREACION)
            //    );
            //var sortDirection = Request["sSortDir_0"];
            //if (sortDirection == "asc")
            //    filteredMembers = filteredMembers.OrderBy(orderingFunction);
            //else
            //    filteredMembers = filteredMembers.OrderByDescending(orderingFunction);
            var displayMembers = filteredMembers
                .Skip(param.iDisplayStart)
                .Take(param.iDisplayLength);
            var result = from a in displayMembers
                         select new
                         {
                             a.usu_id,
                             a.usu_nombre,
                             a.usu_login,
                             a.usu_est_id,
                             a.usu_tip_id,
                             a.usu_tip_nom
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

        //public ActionResult getUsuario(Ent_jQueryDataTableParams param)
        //{

        //    /*verificar si esta null*/
        //    if (Session[_session_listusu_private] == null)
        //    {
        //        List<Ent_Usuario> listusuario = new List<Ent_Usuario>();
        //        Session[_session_listusu_private] = listusuario;
        //    }

        //    //Traer registros
        //    IQueryable<Ent_Usuario> membercol = ((List<Ent_Usuario>)(Session[_session_listusu_private])).AsQueryable();  //lista().AsQueryable();

        //    //Manejador de filtros
        //    int totalCount = membercol.Count();
        //    IEnumerable<Ent_Usuario> filteredMembers = membercol;

        //    if (!string.IsNullOrEmpty(param.sSearch))
        //    {
        //        filteredMembers = membercol
        //            .Where(m => m.usu_nombre.ToUpper().Contains(param.sSearch.ToUpper()) ||
        //             m.usu_login.ToUpper().Contains(param.sSearch.ToUpper()));
        //    }
        //    //Manejador de orden
        //    var sortIdx = Convert.ToInt32(Request["iSortCol_0"]);
        //    Func<Ent_Usuario, string> orderingFunction =
        //    (
        //    m => sortIdx == 0 ? m.usu_nombre :
        //     m.usu_login
        //    );
        //    var sortDirection = Request["sSortDir_0"];
        //    if (sortDirection == "asc")
        //        filteredMembers = filteredMembers.OrderBy(orderingFunction);
        //    else
        //        filteredMembers = filteredMembers.OrderByDescending(orderingFunction);
        //    var displayMembers = filteredMembers
        //        .Skip(param.iDisplayStart)
        //        .Take(param.iDisplayLength);
        //    var result = from a in displayMembers
        //                 select new
        //                 {
        //                     a.usu_id,
        //                     a.usu_nombre,
        //                     a.usu_login,
        //                     a.usu_tip_nom,
        //                     a.usu_est_id
        //                 };
        //    //Se devuelven los resultados por json
        //    return Json(new
        //    {
        //        sEcho = param.sEcho,
        //        iTotalRecords = totalCount,
        //        iTotalDisplayRecords = filteredMembers.Count(),
        //        aaData = result
        //    }, JsonRequestBehavior.AllowGet);
        //}


        public ActionResult Nuevo()
        {
            //Dat_Estado estado = new Dat_Estado();
            //ViewBag.estado = estado.get_lista();

            Dat_Usuario_Tipo usuariotipo = new Dat_Usuario_Tipo();
            ViewBag.usuariotipo = usuariotipo.get_lista();
            return View();
        }
        [HttpPost]
        public ActionResult Nuevo(string nombre, string login, string password, string tipo)
        {
            if (nombre == null) return Json(new { estado = "0" });

            if (tipo=="0" || tipo==null) return Json(new { estado = "-1", desmsg="seleccione el tipo de usuario" });

            Ent_Usuario _usuario = new Ent_Usuario();

            _usuario.usu_nombre = nombre;
            _usuario.usu_login = login;
            _usuario.usu_contraseña = password;
            _usuario.usu_tip_id = tipo;

            Dat_Usuario usuario = new Dat_Usuario();
            usuario.usu = _usuario;



            Boolean _valida_nuevo =usuario.InsertarUsuario();

            if (usuario.existe_login)
            {
                _valida_nuevo = false;
                return Json(new { estado ="-1", desmsg = "El login del usuario ya existe." });
            }
            else
            {
                return Json(new { estado = (_valida_nuevo) ? "1" : "-1", desmsg = (_valida_nuevo) ? "Se actualizo satisfactoriamente." : "Hubo un error al actualizar." });
            }
            
        }

        public ActionResult Edit(int? id)
        {

            List<Ent_Usuario> listusuario = (List<Ent_Usuario>)Session[_session_listusu_private];
            if (id == null || listusuario == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ent_Usuario filafuncion = listusuario.Find(x => x.usu_id == id);

            Dat_Estado estado = new Dat_Estado();
            ViewBag.estado = estado.get_lista();

            Dat_Usuario_Tipo usuariotipo = new Dat_Usuario_Tipo();
            ViewBag.usuariotipo = usuariotipo.get_lista();


            return View(filafuncion);
        }

        [HttpPost]
        public ActionResult Edit(string id, string nombre, string password, string tipo,string estado,string pass)
        {
            if (id == null) return Json(new { estado = 0 });

            Ent_Usuario _usuario = new Ent_Usuario();

            _usuario.usu_id =Convert.ToDecimal(id);
            _usuario.usu_nombre = nombre;
            _usuario.usu_contraseña = password;
            _usuario.usu_tip_id = tipo;
            _usuario.usu_est_id = estado;
                        

            Dat_Usuario usuario = new Dat_Usuario();
            usuario.usu = _usuario;

            Boolean _valida_editar =usuario.EditarUsuario((pass=="1")?true:false);

            return Json(new { estado = (_valida_editar) ? "1" : "-1", desmsg = (_valida_editar) ? "Se actualizo satisfactoriamente." : "Hubo un error al actualizar." });
        }

        public ActionResult Roles(Decimal id)
        {
            List<Ent_Usuario> listusuarios = (List<Ent_Usuario>)Session[_session_listusu_private];
            if (listusuarios == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ent_Usuario filausuario = listusuarios.Find(x => x.usu_id == id);
            ViewBag.usuid = id.ToString();
            ViewBag.usunombre = filausuario.usu_nombre.ToString();

            Dat_Roles roles = new Dat_Roles();
            ViewBag.roles = roles.get_lista();

            return View(lista_usu_rol(id));

        }
        public PartialViewResult ListaRolUsu(Decimal usuid)
        {
            ViewBag.usuid = usuid.ToString();
            return PartialView(lista_usu_rol(usuid));
        }
        public List<Ent_Roles> lista_usu_rol(Decimal id)
        {
            Dat_Usuario_Roles lista = new Dat_Usuario_Roles();

            List<Ent_Roles> list_usu_rol = lista.get_lista(id);

            return list_usu_rol;

        }
        [HttpPost]
        public ActionResult Borrar_Rol(Decimal usu_id, Decimal rol_id)
        {
            Dat_Usuario_Roles _usuario_rol = new Dat_Usuario_Roles();

            Boolean _valida_borrar = _usuario_rol.Eliminar_Rol_Usuario(usu_id, rol_id);

            return Json(new { estado = (_valida_borrar) ? "1" : "-1", desmsg = (_valida_borrar) ? "Se borro correctamente." : "Hubo un error al borrar." });
        }
        [HttpPost]
        public ActionResult Agregar_Rol(Decimal usu_id, Decimal rol_id)
        {

            Dat_Usuario_Roles _usuario_rol = new Dat_Usuario_Roles();

            Boolean _valida_agregar = _usuario_rol.Insertar_Rol_Usuario(usu_id, rol_id);

            return Json(new { estado = (_valida_agregar) ? "1" : "-1", desmsg = (_valida_agregar) ? "Se agrego correctamente." : "Hubo un error al agregar." });
        }

        #region<SEGURIDAD DE USUARIO>
        public ActionResult CambioClave()
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
                    ViewBag.DataUsu = _usuario;

                    return View();
                }
                else
                {
                    return RedirectToAction("Error_Roles", "Home");
                }  
            }
            
        }
        [HttpPost]
        public ActionResult CambiarClave(string password)
        {
            Ent_Usuario _usuario = (Ent_Usuario)Session[Ent_Constantes.NameSessionUser];
            Dat_Usuario usuario = new Dat_Usuario();
            
            Boolean _valida_editar = usuario.update_pass(_usuario.usu_id, password);
            return Json(new { estado = (_valida_editar) ? "1" : "-1", desmsg = (_valida_editar) ? "Se actualizo satisfactoriamente." : "Hubo un error al actualizar." });
        }

        public ActionResult recoveryPassword()
        {
            return View();
        }
        [HttpPost]
        public ActionResult EnviaRecoveryPass(string usuario)
        {           
            Dat_Recov_Pass dat_usuario = new Dat_Recov_Pass();

            Ent_Recov_Pass _recov_pass = dat_usuario.get_recov_pass(usuario, "");
         
            if (_recov_pass == null)
            {
                _recov_pass = new Ent_Recov_Pass();
                _recov_pass.COD_ERROR = "-2";
                _recov_pass.DESCRIPCION_ERROR = "Error en el servidor";
            }

            return Json(new { estado = _recov_pass.COD_ERROR, desmsg =_recov_pass.DESCRIPCION_ERROR });
        }

        public ActionResult Set_recoveryPassword()
        {

            string codigo=Request.Params["codigo"];
          
            if (codigo == null)
            {
                return Redirect("~/Control/Login");              
            }
            else
            {
                ViewBag.str_codigo = codigo.ToString();
                return View();
            }
        }
        [HttpPost]
        public ActionResult RecoveryPass(string codigo,string pass)
        {
            Dat_Recov_Pass dat_usuario = new Dat_Recov_Pass();

            Ent_Recov_Pass _recov_pass = dat_usuario.recovery_pass(pass, codigo);
            //_recov_pass = new Ent_Recov_Pass();
            //_recov_pass.COD_ERROR = "0";
            //_recov_pass.DESCRIPCION_ERROR = "Se generaro la nueva contraseña";

            if (_recov_pass == null)
            {
                _recov_pass = new Ent_Recov_Pass();
                _recov_pass.COD_ERROR = "-2";
                _recov_pass.DESCRIPCION_ERROR = "Error en el servidor";
            }

            return Json(new { estado = _recov_pass.COD_ERROR, desmsg = _recov_pass.DESCRIPCION_ERROR });
        }

        #endregion
        #region<CAMBIAR PASSWRORD>

        [HttpPost]
        public ActionResult UpdatePassword(string id,  string password, /*string tipo,*/  string pasedit, string login, string logedit, string estado)
        {

            Ent_Usuario _usuario_acceso = (Ent_Usuario)Session[Ent_Constantes.NameSessionUser];

            if (id == null) return Json(new { estado = 0 });

            Ent_Usuario _usuario = new Ent_Usuario();

            _usuario.usu_id = Convert.ToDecimal(id);            
            _usuario.usu_contraseña = password;
            _usuario.update_pass = (pasedit == "1" ? true : false);

            _usuario.usu_login = login;
            _usuario.update_login = (logedit == "1" ? true : false);
            //_usuario.usu_tip_id = tipo;
            _usuario.usu_est_id = estado;
            _usuario.usu_id_update = _usuario_acceso.usu_id;


            Dat_Usuario usuario = new Dat_Usuario();
            usuario.usu = _usuario;

            Boolean existe_user = false;
            string estado_upd = "";
            string mensaje = "";

            string _valida_editar = usuario.EditarPassControl(ref existe_user);

            if (_valida_editar.Length==0)
            {

                if (existe_user)
                {
                    estado_upd = "-1";
                    mensaje = "El Login ya existe en la base de datos.";
                }
                else
                {
                    estado_upd = "1";
                    mensaje = "Se actualizo satisfactoriamente.";
                }

            }
            else
            {
                estado_upd = "-1";
                mensaje = _valida_editar;
            }

            return Json(new { estado = estado_upd, desmsg = mensaje });
        }

        public ActionResult UpdatePassword(Decimal id)
        {

            //List<Ent_Usuario> listusuario = usuario.get_lista();
            //Session[_session_listusu_private] = listusuario;

            List<Ent_Usuario> listusuario = (List<Ent_Usuario>)Session[_session_listusu_private];
            if (id == null || listusuario == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ent_Usuario filausuario = listusuario.Find(x => x.usu_id == id);

            Ent_Estado_Modulo _Ent = new Ent_Estado_Modulo();
            _Ent.Est_Mod_Id = 0;
            Dat_Estado datEstado = new Dat_Estado();
            var ListarEstados = datEstado.ListarEstadoModulo(_Ent).Where(a => a.Codigo != "E").ToList();
            ViewBag.ListarEstados = ListarEstados;

            Ent_Usuario _usuario = (Ent_Usuario)Session[Ent_Constantes.NameSessionUser];
            Dat_Usuario_Tipo usuariotipo = new Dat_Usuario_Tipo();
            if (_usuario.usu_tip_id=="04")
            {
                ViewBag.usuariotipo = usuariotipo.get_lista();
            }
            else
            {
                ViewBag.usuariotipo = usuariotipo.get_lista().Where(a=>a.usu_tip_id==filausuario.usu_tip_id);
            }

            
            
           

            return View(filausuario);



            ////List<Ent_Usuario> listusuarios = (List<Ent_Usuario>)Session[_session_listusu_private];
            ////if (listusuarios == null)
            ////{
            ////    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            ////}
            ////Ent_Usuario filausuario = listusuarios.Find(x => x.usu_id == id);
            ////ViewBag.usuid = id.ToString();
            ////ViewBag.usunombre = filausuario.usu_nombre.ToString();

            ////Dat_Roles roles = new Dat_Roles();
            ////ViewBag.roles = roles.get_lista();

            //return View();

            ////return View(lista_usu_rol(id));

        }
        #endregion

    }
}