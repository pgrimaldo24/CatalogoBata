﻿using CapaDato.Util;
using CapaDato.Persona;
using CapaDato.Pedido;
using CapaEntidad.Control;
using CapaEntidad.Menu;
using CapaEntidad.Util;
using CapaEntidad.Pedido;
using CapaPresentacion.Bll;
using System;
using System.Collections.Generic;

using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Data;
using CapaEntidad.General;
using System.Linq;
using CapaEntidad.Persona;
using Newtonsoft.Json;
using System.Web.SessionState;
using CapaPresentacion.Models.Crystal;
using CapaPresentacion.Data.RptsCrystal;
using CapaDato.Cliente;
using CapaEntidad.Cliente;
using System.IO;
using System.Text;
using System.Collections;

namespace CapaPresentacion.Controllers
{
    public class PedidoController : Controller
    {
        // GET: Funcion
        private Dat_Pedido datPedido = new Dat_Pedido();
        private Dat_Persona datPersona = new Dat_Persona();
        private string _session_listPedido_private = "_session_listPedido_private";
        private string _session_list_NotaCredito = "_session_list_NotaCredito";
        private string _session_list_consignaciones = "_session_list_consignaciones";
        private string _session_list_saldos = "_session_list_saldos";
        private string _session_list_detalle_pedido = "_session_list_detalle_pedido";
        private string _session_nuevo_item_pedido = "_session_nuevo_item_pedido";
        private string _session_Tran_Ofertas = "_session_TranOfertas";
        private string _session_customer = "_session_customer";
        private string _session_lnfo_liquidacion = "_session_lnfo_liquidacion";
        private string _session_notas_persona = "_session_notas_persona";
        private string _session_ListarPedidoEstado = "_session_ListarPedidoEstado";
        private string _session_ListarPicking = "_session_ListarPicking";
        private string _session_ListarPedidoDespacho = "_session_ListarPedidoDespacho";
        private string _session_ListarPedidoDespacho_Excel = "_session_ListarPedidoDespacho_Excel";
        private string _session_ListarPedidoFacturacion = "_session_ListarPedidoFacturacion";
        private string _session_ListarPedidoFacturacion_Excel = "_session_ListarPedidoFacturacion_Excel";
        private string _session_ListarConsultarPedido = "_session_ListarConsultarPedido";
        private string _session_ListarConsultarPedido_Excel = "_session_ListarConsultarPedido_Excel";
        private string _session_ListarManifiesto = "_session_ListarManifiesto";
        private string _session_ListarManifiestoEditar = "_session_ListarManifiestoEditar";

        private Dat_Cliente dat_cliente = new Dat_Cliente();

        private string _carga_inicial_editar = "_carga_inicial_editar";

        public ActionResult CrearEditar(string custId = "" , string liqId = "" , string pedId = "", string liq_tipo_prov="",string liq_tipo_des="",
                                       string liq_agencia="",string liq_agencia_direccion="",string liq_destino="",string liq_direccion="",
                                       string liq_referencia="")
        {
            Session[_session_list_detalle_pedido] = null;
            Session[_session_Tran_Ofertas] = null;
            Session[_session_nuevo_item_pedido] = null;
            Session[_session_customer] = null;
            Session[_session_notas_persona] = null;
            Session[_carga_inicial_editar] = false;
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
                //Boolean valida_rol = true;
                //Basico valida_controller = new Basico();
                //List<Ent_Menu_Items> menu = (List<Ent_Menu_Items>)Session[Ent_Global._session_menu_user];
                //valida_rol = valida_controller.AccesoMenu(menu, this);
                #endregion
                if (custId == "")
                {
                    return RedirectToAction("Lista", "Pedido");
                }
                //if (valida_rol)
                //{
                    string IdPedido = "";
                    string IdCustomer = "";
                    string strLiqId = "";
                    Ent_Persona customer = datPersona.GET_INFO_PERSONA(custId);
                    if (liqId != "")
                    {
                        Session[_carga_inicial_editar] = true;
                        IdPedido = pedId;
                        IdCustomer = custId;
                        strLiqId = liqId;
                        ViewBag.operacion = "Editar";
                        Session[_session_list_detalle_pedido] = listaDetalleLiquidacion(liqId, customer._commission);
                    }
                    else
                    {
                        ViewBag.operacion = "Nuevo";
                    }

                    
                    List<Ent_Pago_NCredito> notas = datPersona.get_nota_credito(custId, strLiqId);

                    Session[_session_notas_persona] = notas;

                    ViewBag.infoProm = customer;
                    ViewBag.CountNotas = notas.Count();
                    Session[_session_customer] = customer;

                    

                    Ent_Pedido_Maestro maestros = datPedido.Listar_Maestros_Pedido(_usuario.usu_id, _usuario.usu_postPago, IdCustomer);


                    List<Ent_Cliente_Despacho> lis_des = dat_cliente.lista_despacho();

                    //var select = lis_des.Select(a => a.desp_cod == "0").ToList();

                    //lis_des.RemoveAt(0);

                    ViewBag.listPromotor = maestros.combo_ListPromotor;
                    ViewBag.listFormaPago = maestros.combo_ListFormaPago;
                    ViewBag.IdLiquidacion = liqId;
                    ViewBag.despacho = lis_des;// dat_cliente.lista_despacho();

                    Ent_Liquidacion oLiquidacion = new Ent_Liquidacion();                   
                                
                    oLiquidacion.liq_Id = strLiqId;
                    oLiquidacion.ped_Id = IdPedido;
                    oLiquidacion.cust_Id = IdCustomer;

                    oLiquidacion.liq_tipo_prov = liq_tipo_prov;
                    oLiquidacion.liq_tipo_des = liq_tipo_des;
                    oLiquidacion.liq_agencia = liq_agencia;
                    oLiquidacion.liq_agencia_direccion = liq_agencia_direccion;
                    oLiquidacion.liq_destino = liq_destino;
                    oLiquidacion.liq_direccion = liq_direccion;
                    oLiquidacion.liq_referencia = liq_referencia;


                    ViewBag.Liqui = oLiquidacion;
                    ViewBag.ActivaMcoPago = (_usuario.usu_mercado_pago)?"1":"0";

                    Session[_session_lnfo_liquidacion] = oLiquidacion;

                    return View("CrearEditar", oLiquidacion);
                //}
                //else
                //{
                //    return RedirectToAction("Login", "Control", new { returnUrl = return_view });
                //}
            }

        }
        public ActionResult VerLiquidacion(string liquidacion)
        {
            GetCRLiquidacion(liquidacion, false);
            return Json(new { estado = 0 });
        }
        public ActionResult VerFactura(string liquidacion, string invoice)
        {
            try
            {
                GetCRInvoice(invoice, liquidacion);
                return Json(new { estado = 0 });
            }
            catch (Exception ex)
            {
                return Json(new { estado = 1 });
            }
            
        }
        public ActionResult PersonaPedido(int basId) {
            
            Ent_Pedido_Persona Persona = new Ent_Pedido_Persona();
            Persona = datPedido.BuscarPersonaPedido(basId);

            return Json(Persona, JsonRequestBehavior.AllowGet);
        }

        public ActionResult listarStr_ArticuloTalla(string codArticulo)
        {                       
            Ent_Usuario _usuario = (Ent_Usuario)Session[Ent_Constantes.NameSessionUser];
            Ent_Persona cust = (Ent_Persona)Session[_session_customer];
            Ent_Articulo_pedido articulo = new Ent_Articulo_pedido();
            List<Ent_Articulo_Tallas> tallas = new List<Ent_Articulo_Tallas>();
            datPedido.listarStr_ArticuloTalla(codArticulo, Convert.ToDecimal(cust.Bas_id), ref articulo, ref tallas, _usuario.usu_id);

            Ent_Order_Dtl newLineOrder = new Ent_Order_Dtl();
            //List<Ent_Order_Dtl> order = new List<Ent_Order_Dtl>();
                        
            Session[_session_nuevo_item_pedido] = articulo;
            
            return Json(new { articulo = articulo, tallas = tallas } , JsonRequestBehavior.AllowGet);
        }     

        public JsonResult GenerarCombo(int Numsp, string Params)
        {
            string strJson = "";
            JsonResult jRespuesta = null;
            var serializer = new JavaScriptSerializer();


            //switch (Numsp)
            //{
            //    case 1:
            //        strJson = datUtil.listarStr_Provincia(Params);
            //        jRespuesta = Json(serializer.Deserialize<List<Ent_Combo>>(strJson), JsonRequestBehavior.AllowGet);
            //        break;
            //    case 2:
            //        String[] substrings = Params.Split('|');
            //        strJson = datUtil.listarStr_Distrito(Params);
            //        jRespuesta = Json(serializer.Deserialize<List<Ent_Combo>>(strJson), JsonRequestBehavior.AllowGet);
            //        break;
            //    default:
            //        Console.WriteLine("Default case");
            //        break;
            //}
            return jRespuesta;
        }

        private static string[] splitString(string _textString, char _character)
        {
            string[] split = null;
            if (!string.IsNullOrEmpty(_textString))
            {
                split = _textString.Split(new Char[] { _character });
            }
            return split;
        }

        public ActionResult NotificationMercadoPago(string status)
        {
            ViewBag.status_notification = status;
            return View();
        }

        public  Boolean fvalidaartcatalogo()
        {
            Boolean validaartcatag = false;
            try
            {
                //List<Order_Dtl> order = (List<Order_Dtl>)(((object)HttpContext.Current.Session[_nSOrder]) != null ? (object)HttpContext.Current.Session[_nSOrder] : new List<Order_Dtl>());
                List<Ent_Order_Dtl> order = (List<Ent_Order_Dtl>)Session[_session_list_detalle_pedido];
                if (order != null)
                {

                    for (Int32 i = 0; i < order.Count; ++i)
                    {
                        if (order[i]._ap_percepcion == "0")
                        {
                            validaartcatag = true;
                            break;
                        }
                    }
                }
                else
                {
                    validaartcatag = false;
                }

            }
            catch
            {
                return false;
            }
            return validaartcatag;
        }
    
        private string LiquidarPedidoMercadoPago()
        {


            return "";
        }

        public ActionResult LiquidarPedido(string tipo_des,string agencia,string destino,string direccion_agencia,string direccion,string referencia,
                                           string liq_tipo_prov,string liq_provincia, decimal _usu = 0, 
                                           decimal _idCust = 0, string _reference = "", decimal _discCommPctg = 0,
                                           decimal _discCommValue = 0, string _shipTo = "", string _specialInstr = "", List<Ent_Order_Dtl> _itemsDetail = null,
                                           decimal _varpercepcion = 0, Int32 _estado = 1, string _ped_id = "", string _liq = "", Int32 _liq_dir = 0,
                                           Int32 _PagPos = 0, string _PagoPostarjeta = "", string _PagoNumConsignacion = "", decimal _PagoTotal = 0,
                                           /*DataTable dtpago = null*/ List<Ent_Documents_Trans> ListPago=null, Boolean _pago_credito = false, Decimal _porc_percepcion = 0, List<Order_Dtl_Temp>
                                           order_dtl_temp = null, string strTipoPago = "N", string _formaPago = "",string liq_directo="", string ped_directo = "")
        {


            var oJRespuesta = new JsonResponse();
            string[] noOrder;
            Ent_Usuario _usuario = (Ent_Usuario)Session[Ent_Constantes.NameSessionUser];
            Ent_Persona cust = (Ent_Persona)Session[_session_customer];
            List<Ent_Order_Dtl> order = (List<Ent_Order_Dtl>)Session[_session_list_detalle_pedido];
            Ent_Order_Hdr header = getTotals();
            order_dtl_temp = new List<Order_Dtl_Temp>();

            if (fvalidaartcatalogo())
            {
                cust._commission = 0;
                //comision_customer = 0;
                cust._percepcion = 0;
            }


            int index = 0;
            foreach (Ent_Order_Dtl item in order)
            {
                index++;
                order_dtl_temp.Add(new Order_Dtl_Temp()
                {
                    items = index,
                    articulo = item._code,
                    cantidad = item._qty,
                    premId = null,
                    premio = "",
                    talla = item._size
                });
            }
             
            /*Armamos los documentos de pago (nota de credito) utilizados en la liquidacion*/
            DataTable dtpago = new DataTable();
            dtpago.Columns.Add("Liq_Id", typeof(string));
            dtpago.Columns.Add("Doc_Tra_Id", typeof(string));
            dtpago.Columns.Add("Monto", typeof(Double));

            
            if (Session[_session_notas_persona] == null)
            {
                List<Ent_Pago_NCredito> _listNC = new List<Ent_Pago_NCredito>();
                Session[_session_notas_persona] = _listNC;
            }
            List<Ent_Pago_NCredito> listNC = new List<Ent_Pago_NCredito>();
            listNC = (List<Ent_Pago_NCredito>)Session[_session_notas_persona];


            if (listNC != null)
            {

                foreach (Ent_Pago_NCredito dTx in listNC.Where(n => n.Consumido == true).ToList())
                {
                    dtpago.Rows.Add("", dTx.Rhv_return_nro, dTx.Importe);
                }
            }

            if (_formaPago.Equals("MercadoPago"))
            {
                var liqMp = new Ent_Liquidacion();
                liqMp = (Ent_Liquidacion)Session[_session_lnfo_liquidacion];
               

                if (string.IsNullOrEmpty(liqMp.liq_Id))
                {
                    //insert
                    noOrder = datPedido.Gua_Mod_Liquidacion(tipo_des, agencia, destino, direccion_agencia, direccion, referencia,
                    liq_tipo_prov, liq_provincia, _usuario.usu_id, Convert.ToDecimal(cust.Bas_id), string.Empty, cust._commission, 0,
                    string.Empty, string.Empty, order, header._percepcion, _estado, _ped_id, _liq, _liq_dir, _PagPos, _PagoPostarjeta,
                    _PagoNumConsignacion, _PagoTotal, dtpago, _pago_credito, cust._percepcion, order_dtl_temp, cust._vartipopago, _formaPago);

                    /*en este caso para que grabe el mismo numero de pedido que se genero*/
                    if ((noOrder[0]).ToString() != "-1")
                    {
                        liqMp.liq_Id = (noOrder[0]).ToString();
                        liqMp.ped_Id = (noOrder[1]).ToString();
                        Session[_session_lnfo_liquidacion] = liqMp;
                    }

                }
                else
                {
                    //update
                    noOrder = datPedido.Gua_Mod_Liquidacion(tipo_des, agencia, destino, direccion_agencia, direccion, referencia,
                    liq_tipo_prov, liq_provincia, _usuario.usu_id, Convert.ToDecimal(cust.Bas_id), string.Empty, cust._commission, 0, 
                    string.Empty, string.Empty, order, header._percepcion, 2, liqMp.ped_Id, liqMp.liq_Id, _liq_dir, _PagPos, _PagoPostarjeta, 
                    _PagoNumConsignacion, _PagoTotal, dtpago, _pago_credito, cust._percepcion, order_dtl_temp, cust._vartipopago, _formaPago);
                }
            }
            else
            {
                Ent_Liquidacion liq = (Ent_Liquidacion)Session[_session_lnfo_liquidacion];
                /*fin de los documentos de pago*/
                if (string.IsNullOrEmpty(liq.liq_Id))
                {
                    noOrder = datPedido.Gua_Mod_Liquidacion(tipo_des, agencia, destino, direccion_agencia, direccion, referencia,
                    liq_tipo_prov, liq_provincia, _usuario.usu_id, Convert.ToDecimal(cust.Bas_id), string.Empty, cust._commission, 0,
                    string.Empty, string.Empty, order, header._percepcion, _estado, _ped_id, _liq, _liq_dir, _PagPos, _PagoPostarjeta, 
                    _PagoNumConsignacion, _PagoTotal, dtpago, _pago_credito, cust._percepcion, order_dtl_temp, cust._vartipopago, _formaPago);
                }
                else
                {
                    noOrder = datPedido.Gua_Mod_Liquidacion(tipo_des, agencia, destino, direccion_agencia, direccion, referencia,
                    liq_tipo_prov, liq_provincia, _usuario.usu_id, Convert.ToDecimal(cust.Bas_id), string.Empty, cust._commission, 0,
                    string.Empty, string.Empty, order, header._percepcion, 2, liq.ped_Id, liq.liq_Id, _liq_dir, _PagPos, _PagoPostarjeta, 
                    _PagoNumConsignacion, _PagoTotal, dtpago, _pago_credito, cust._percepcion, order_dtl_temp, cust._vartipopago, _formaPago);
                }
            }
             

            if ((noOrder[0]).ToString() != "-1")
            {
                oJRespuesta.Message = (noOrder[0]).ToString();
                oJRespuesta.Data = true;
                oJRespuesta.Success = true;
                oJRespuesta.Products = order;
                GetCRLiquidacion((noOrder[0]).ToString());
            }
            else
            {

                oJRespuesta.Message = (noOrder[1]).ToString();
                oJRespuesta.Data = false;
                oJRespuesta.Success = false;

            }

            return Json(oJRespuesta, JsonRequestBehavior.AllowGet);
            
        }
           
        private void GetCRLiquidacion(string liq, bool download = false)
        {
            
                Data_Cr_Aquarella datCrAq = new Data_Cr_Aquarella();
                List<Liquidation> _liqValsReport = new List<Liquidation>();

                DataSet dsLiqInfo = datCrAq.data_reporte_liquidacion(liq);

                if (dsLiqInfo == null)
                    return;

                //DataSet dsLiqDtl =  Liquidation_Dtl.getLiquidationDtl(_user._usv_co, _noLiq);
                DataSet dsLiqDtl = new DataSet();
                dsLiqDtl.Tables.Add(dsLiqInfo.Tables[1].Copy());

                if (dsLiqDtl == null)
                    return;

                DataRow dRow = dsLiqInfo.Tables[0].Rows[0];

                foreach (DataRow dRowDtl in dsLiqDtl.Tables[0].Rows)
                {
                    string vncredito = ""; decimal VtotalcreditoTotal = 0;
                    string vfecha = DateTime.Today.ToString("dd/MM/yyyy");



                    //Bata.Aquarella.BLL.Reports.Liquidation objLiqReport = new BLL.Reports.Liquidation(dRow["ohv_warehouseid"].ToString(), dRow["wav_description"].ToString(),
                    //    dRow["wav_address"].ToString(), dRow["wav_telephones"].ToString(), dRow["ubicationwav"].ToString(), dRow["con_coordinator_id"].ToString(), dRow["bdv_document_no"].ToString(),
                    //    dRow["name"].ToString(), dRow["bdv_address"].ToString(), dRow["bdv_phone"].ToString(), dRow["bdv_movil_phone"].ToString(), dRow["bdv_email"].ToString(),
                    //    dRow["ubicationcustomer"].ToString(), dRow["lhv_liquidation_no"].ToString(), Convert.ToDateTime(dRow["lhd_date"]), Convert.ToDateTime(dRow["lhd_expiration_date"].ToString()),
                    //    dRow["stv_description"].ToString(), Convert.ToDecimal(dRow["lon_disscount"]), Convert.ToDecimal(dRow["tax_rate"]), Convert.ToDecimal(dRow["lhn_tax_rate"]), Convert.ToDecimal(dRow["lhn_handling"]),
                    //    dRowDtl["ldv_article"].ToString(), dRowDtl["brv_description"].ToString(), dRowDtl["cov_description"].ToString(), dRowDtl["arv_name"].ToString(), dRowDtl["ldv_size"].ToString(), Convert.ToDecimal(dRowDtl["ldn_qty"]),
                    //    Convert.ToDecimal(dRowDtl["ldn_sell_price"]), Convert.ToDecimal(dRowDtl["ldn_commission"]), Convert.ToDecimal(dRowDtl["ldn_disscount"]), Convert.ToDecimal(dRow["percepcion"]), Convert.ToDecimal(dRow["porc_percepcion"]),
                    //    Convert.ToDecimal(dRow["ncredito"]), vncredito, Convert.ToDateTime(vfecha), VtotalcreditoTotal, _noLiq, Convert.ToDecimal(dRow["totalop"]));


                    Liquidation objLiqReport = new Liquidation("1", dRow["almacen"].ToString(),
                     dRow["alm_direccion"].ToString(), dRow["Alm_Telefono"].ToString(), "", dRow["Bas_Id"].ToString(), dRow["Bas_Documento"].ToString(),
                     dRow["nombres"].ToString(), dRow["Bas_Direccion"].ToString(), dRow["Bas_Telefono"].ToString(), dRow["Bas_Celular"].ToString(), dRow["Bas_Correo"].ToString(),
                     dRow["ubicacion"].ToString(), dRow["Liq_Id"].ToString(), Convert.ToDateTime(dRow["Liq_FechaIng"]), Convert.ToDateTime(dRow["Liq_Fecha_Expiracion"].ToString()),
                     dRow["estado"].ToString(), 0, Convert.ToDecimal(dRow["igvporc"]), Convert.ToDecimal(dRow["igvmonto"]), 0,
                     dRowDtl["Art_Id"].ToString(), dRowDtl["Mar_Descripcion"].ToString(), dRowDtl["Col_Descripcion"].ToString(), dRowDtl["art_descripcion"].ToString(), dRowDtl["Liq_Det_TalId"].ToString(), Convert.ToDecimal(dRowDtl["Liq_Det_Cantidad"]),
                     Convert.ToDecimal(dRowDtl["Liq_Det_Precio"]), Convert.ToDecimal(dRowDtl["Liq_Det_Comision"]), 0, Convert.ToDecimal(dRow["Percepcionm"]), Convert.ToDecimal(dRow["Percepcionp"]),
                     Convert.ToDecimal(dRow["ncredito"]), vncredito, Convert.ToDateTime(vfecha), VtotalcreditoTotal, liq, Convert.ToDecimal(dRow["totalop"]), Convert.ToDecimal(dRowDtl["Liq_Det_OfertaM"]), dRow["Opg"].ToString());

                    _liqValsReport.Add(objLiqReport);
                }




                List<LiqNcSubinforme>_liqValsSubReport = new List<LiqNcSubinforme>();

                //DataSet dsLiqpagoInfo = Liquidations_Hdr.getpagoncreditoliqui(_noLiq);
                DataSet dsLiqpagoInfo = new DataSet();
                dsLiqpagoInfo.Tables.Add(dsLiqInfo.Tables[2].Copy());

                if (dsLiqpagoInfo == null)
                    return;

                foreach (DataRow dRowDtl in dsLiqpagoInfo.Tables[0].Rows)
                {
                    string vncredito = dRowDtl["ncredito"].ToString();
                    decimal VtotalcreditoTotal = Convert.ToDecimal(dRowDtl["Total"].ToString());
                    DateTime vfecha = Convert.ToDateTime(dRowDtl["fecha"].ToString());

                    LiqNcSubinforme objLiqpagoReport = new LiqNcSubinforme("", vncredito, vfecha, VtotalcreditoTotal);

                    _liqValsSubReport.Add(objLiqpagoReport);
                }

                List<VentaPagoSubInforme> _liqValsPagoSubReport = new List<VentaPagoSubInforme>();
                //DataSet dsLiqpagoformainfo = Liquidations_Hdr.getpagonformaliqui(_noLiq);
                DataSet dsLiqpagoformainfo = new DataSet();
                dsLiqpagoformainfo.Tables.Add(dsLiqInfo.Tables[3].Copy());
                if (dsLiqpagoformainfo == null)
                    return;
                foreach (DataRow drowdtl in dsLiqpagoformainfo.Tables[0].Rows)
                {
                    string vpago = drowdtl["pago"].ToString();
                    string vdocumento = drowdtl["Documento"].ToString();
                    DateTime vfecha = Convert.ToDateTime(drowdtl["fecha"].ToString());
                    Decimal vtotal = Convert.ToDecimal(drowdtl["Total"].ToString());
                    VentaPagoSubInforme objLiqpagoformaReport = new  VentaPagoSubInforme(vpago, vdocumento, vfecha, vtotal);
                    _liqValsPagoSubReport.Add(objLiqpagoformaReport);
                }

            this.HttpContext.Session["ReportName"] = "liquidationReport.rpt";
            this.HttpContext.Session["rptSource"] = _liqValsReport;
            this.HttpContext.Session["rptSource1"] = _liqValsSubReport;
            this.HttpContext.Session["rptSource2"] = _liqValsPagoSubReport;
            this.HttpContext.Session["rptDownload"] = download;
        }
        public void GetCRInvoice(string invoice,string noOrder) 
        {
            
                Data_Cr_Aquarella datCrAq = new Data_Cr_Aquarella();
                DataSet ds_venta = datCrAq.getInvoiceHdr(invoice);
                DataTable invoiceHdr = ds_venta.Tables[0].Copy();
                //DataTable invoiceHdr = Facturacion.getInvoiceHdr(this._user._usv_co, this._noInvoice, this._noOrderUrl);
                if (invoiceHdr.Rows.Count > 0)
                {
                    //DataTable warehouseByPk = new warehouses(this._user._usv_co, invoiceHdr.Rows[0]["stv_warehouse"].ToString()).getWarehouseByPk();
                    string wavDescription = "";
                    string wavAddress = "";
                    string wavPhone = "";
                    string wavUbication = "";
                    //if (warehouseByPk != null && warehouseByPk.Rows.Count > 0)
                    //{
                    wavDescription = invoiceHdr.Rows[0]["almacen"].ToString().ToUpper();
                    wavAddress = invoiceHdr.Rows[0]["alm_direccion"].ToString();
                    wavPhone = invoiceHdr.Rows[0]["Alm_Telefono"].ToString();
                    wavUbication = "";
                    //}
                    string typeresolution = "";

                    //DataTable invoiceDtl = Facturacion.getInvoiceDtl(this._user._usv_co, this._noInvoice);

                    DataTable invoiceDtl = ds_venta.Tables[1].Copy();

                    string str = "";
                    Decimal descuentoGnral = 0;
                    string numeroRemision = "";
                    string destinatario = invoiceHdr.Rows[0]["nombres"].ToString();
                    string cedula = invoiceHdr.Rows[0]["Bas_Documento"].ToString();
                    string ubicacionDestinatario = invoiceHdr.Rows[0]["ubicacion"].ToString();
                    string telefono = invoiceHdr.Rows[0]["Bas_Telefono"].ToString();
                    string trasportadora = invoiceHdr.Rows[0]["Tra_Descripcion"].ToString();
                    string numeroGuia = invoiceHdr.Rows[0]["Tra_Gui_No"].ToString();
                    Decimal porc_percepcion = Convert.ToDecimal(invoiceHdr.Rows[0]["Percepcionp"].ToString());
                    Decimal iva = Convert.ToDecimal(invoiceHdr.Rows[0]["igvmonto"].ToString());
                    Decimal flete = 0;
                    DateTime fechaRemision = Convert.ToDateTime(invoiceHdr.Rows[0]["Ven_Fecha"].ToString());
                    Decimal ncredito = Convert.ToDecimal(invoiceHdr.Rows[0]["ncredito"].ToString());
                    Decimal totalop = Convert.ToDecimal(invoiceHdr.Rows[0]["totalop"].ToString());
                    List<ReporteFacturacion> _invoice = new List<ReporteFacturacion>();

                    foreach (DataRow dataRow in (InternalDataCollectionBase)invoiceDtl.Rows)
                    {
                        string numFactura = dataRow["Ven_Det_Id"].ToString();
                        string esCopia = str;
                        string msgs = "";// invoiceHdr.Rows[0]["imv_text"].ToString();
                        string codigoArticulo = dataRow["Art_Id"].ToString();
                        string nomArticulo = dataRow["art_descripcion"].ToString();
                        Decimal cantidad = Convert.ToDecimal(dataRow["Ven_Det_Cantidad"].ToString());
                        string talla = dataRow["Ven_Det_TalId"].ToString();
                        Decimal precio = Convert.ToDecimal(dataRow["Ven_Det_Precio"].ToString());
                        Decimal valorLinea = Convert.ToDecimal(dataRow["articulo_value"].ToString());
                        Decimal descuentoArticulo = 0;
                        Decimal comisionLineal = Convert.ToDecimal(dataRow["Ven_Det_ComisionM"].ToString());
                        string descripcionArtic = dataRow["Col_Descripcion"].ToString();
                        _invoice.Add(new ReporteFacturacion(destinatario, ubicacionDestinatario, telefono, "", "", cedula, "", noOrder, numFactura, fechaRemision, numeroRemision, "", esCopia, typeresolution, codigoArticulo, nomArticulo, descripcionArtic, cantidad, talla, precio, descuentoArticulo, comisionLineal, valorLinea, iva, flete, numeroGuia, trasportadora, msgs, descuentoGnral, wavDescription, wavAddress, wavPhone, wavUbication, porc_percepcion, ncredito, totalop));
                    }
                this.HttpContext.Session["rptSource"] = _invoice;
                List<LiqNcSubinforme> subin1 = new List<LiqNcSubinforme>();

                    //DataSet dsLiqpagoInfo = Liquidations_Hdr.getpagoncreditoliqui(this._noOrderUrl);
                    DataSet dsLiqpagoInfo = new DataSet();
                    dsLiqpagoInfo.Tables.Add(ds_venta.Tables[2].Copy());

                    if (dsLiqpagoInfo == null)
                        return;

                    foreach (DataRow dRowDtl in dsLiqpagoInfo.Tables[0].Rows)
                    {
                        string vncredito = dRowDtl["ncredito"].ToString();
                        decimal VtotalcreditoTotal = Convert.ToDecimal(dRowDtl["Total"].ToString());
                        DateTime vfecha = Convert.ToDateTime(dRowDtl["fecha"].ToString());

                        LiqNcSubinforme objLiqpagoReport = new LiqNcSubinforme("", vncredito, vfecha, VtotalcreditoTotal);

                        subin1.Add(objLiqpagoReport);
                    }
                this.HttpContext.Session["rptSource1"] = subin1;
                List<VentaPagoSubInforme> subin2 = new List<VentaPagoSubInforme>();
                    //DataSet dsLiqpagoformaInfo = Liquidations_Hdr.getpagonformaliqui(this._noOrderUrl);
                    DataSet dsLiqpagoformaInfo = new DataSet();
                    dsLiqpagoformaInfo.Tables.Add(ds_venta.Tables[3].Copy());
                    if (dsLiqpagoInfo == null)
                        return;

                    foreach (DataRow dRowDtl in dsLiqpagoformaInfo.Tables[0].Rows)
                    {
                        string vpago = dRowDtl["pago"].ToString();
                        string vdocumento = dRowDtl["Documento"].ToString();
                        DateTime vfecha = Convert.ToDateTime(dRowDtl["fecha"].ToString());
                        Decimal vtotal = Convert.ToDecimal(dRowDtl["Total"].ToString());
                        VentaPagoSubInforme objLiqpagoformaReport = new VentaPagoSubInforme(vpago, vdocumento, vfecha, vtotal);
                        subin2.Add(objLiqpagoformaReport);
                    }
                this.HttpContext.Session["rptSource2"] = subin2;
            }
            this.HttpContext.Session["ReportName"] = "InvoiceReport.rpt";
        }
        public ActionResult Lista()
        {
            //ViewBag.status_notification = status;
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
                    Session[_session_listPedido_private] = null;
                    Session[_session_list_NotaCredito] = null;
                    Session[_session_list_consignaciones] = null;
                    Session[_session_list_saldos] = null;
                    Session[_session_ListarPedidosFlete] = null;

                    Ent_Pedido_Maestro maestros = datPedido.Listar_Maestros_Pedido(_usuario.usu_id, _usuario.usu_postPago, "");
                    ViewBag.listPromotor = maestros.combo_ListPromotor;
                    ViewBag.usutipo = _usuario.usu_tip_id.ToString();
                    ViewBag.ActivaMcoPago = (_usuario.usu_mercado_pago) ? "1" : "0";
                    //Ent_Promotor_Maestros maestros = datUtil.ListarEnt_Maestros_Promotor(_usuario.usu_id);
                    //ViewBag.listLider = maestros.combo_ListLider;

                    return View();
                }
                else
                {
                    return RedirectToAction("Error_Roles", "Home");
                }

              
            }
        }        
        public ActionResult GET_INFO_PERSONA_PEDIDO(string codigo)
        {
            try
            {
                Ent_Persona info = datPersona.GET_INFO_PERSONA(codigo);
                string _mensaje = "";
                Ent_Info_Promotor infoGeneralPedidos = datPedido.ListarPedidos(Convert.ToDecimal(codigo),ref _mensaje);
                Session[_session_listPedido_private] = infoGeneralPedidos.liquidacion;
                Session[_session_list_NotaCredito] = infoGeneralPedidos.notaCredito;
                Session[_session_list_consignaciones] = infoGeneralPedidos.consignaciones;
                Session[_session_list_saldos] = infoGeneralPedidos.saldos;

                return Json(new { estado = 0, info = info, mensaje = _mensaje });
            }
            catch (Exception ex)
            {
                return Json(new { estado = 2, mensaje = ex.Message });
            }            
        }

        //public ActionResult ListaNC(string BasId,string LiqId)
        //{
        //    List<Ent_Pago_NCredito> listNotaC = listaNotaCredito(BasId, LiqId);
        //    return View(listNotaC);
        //}
        public JsonResult getListaNC(string BasId, string LiqId)
        {
            List<Ent_Pago_NCredito> listNotaC = listaNotaCredito(BasId, LiqId);

          
            return Json(listNotaC, JsonRequestBehavior.AllowGet);// Json(new { listNotaC = listNotaC }, JsonRequestBehavior.AllowGet);
        }
        public List<Ent_Pago_NCredito> listaNotaCredito(string BasId, string LiqId)
        {
            List<Ent_Pago_NCredito> listNotaC = datPedido.ListarNotaCredito(BasId, LiqId);

            return listNotaC;
        }
        public JsonResult getLiquidacionDetalle(string LiqId)
        {
            List<Ent_Order_Dtl> listLiqDetalle = listaDetalleLiquidacion(LiqId);


            return Json(listLiqDetalle, JsonRequestBehavior.AllowGet);// Json(new { listNotaC = listNotaC }, JsonRequestBehavior.AllowGet);
        }
        public List<Ent_Order_Dtl> listaDetalleLiquidacion(string LiqId , decimal comm = 0)
        {
            List<Ent_Order_Dtl> listLiqDetalle = datPedido.getLiquidacionDetalle(LiqId);
            List<Ent_Order_Dtl> order = new List<Ent_Order_Dtl>();
            foreach (Ent_Order_Dtl item in listLiqDetalle)
            {
                loadOrderDtl(ref order, item, comm);
            }

            return order;
        }
        public ActionResult udateTipoPago(string tipoPago)
        {
            try
            {
                List<Ent_Order_Dtl> pedidoCompleto = (List<Ent_Order_Dtl>)Session[_session_list_detalle_pedido];
                for (int i = 0; i < pedidoCompleto.Count; i++)
                {
                    _addArticle(pedidoCompleto[i], 0, tipoPago, true);
                }              
                return Json(new { estado = 0 });
            }
            catch (Exception ex)
            {
                return Json(new { estado = 1 , mensaje = "Error al cambiar tipo de pago; " + ex.Message  });
            }            
        }
        /** Lista Liquidaciones **/
        public ActionResult getListPedido(Ent_jQueryDataTableParams param)
        {
            /*verificar si esta null*/
            if (Session[_session_listPedido_private] == null)
            {
                List<Ent_Liquidacion>  listPed = new List<Ent_Liquidacion>();
                Session[_session_listPedido_private] = listPed;
            }
            
            //Traer registros
            IQueryable<Ent_Liquidacion> membercol = ((List<Ent_Liquidacion>)(Session[_session_listPedido_private])).AsQueryable();  //lista().AsQueryable();

            //Manejador de filtros
            int totalCount = membercol.Count();
            IEnumerable<Ent_Liquidacion> filteredMembers = membercol;

            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredMembers = membercol
                    .Where(m => m.liq_Id.ToUpper().Contains(param.sSearch.ToUpper()) ||
                     m.liq_Id.ToUpper().Contains(param.sSearch.ToUpper()));
            }
            //Manejador de orden
            var sortIdx = Convert.ToInt32(Request["iSortCol_0"]);

            if (param.iSortingCols > 0)
            {
                if (Request["sSortDir_0"].ToString() == "asc")
                {
                    switch (sortIdx)
                    {
                        case 0: filteredMembers = filteredMembers.OrderBy(o => Convert.ToDecimal(o.liq_Id)); break;
                        case 1: filteredMembers = filteredMembers.OrderBy(o => o.ventaId); break;
                        case 2: filteredMembers = filteredMembers.OrderBy(o => Convert.ToDateTime(o.liq_Fecha)).ThenBy(to => Convert.ToDecimal(to.liq_Id)); break;
                        case 3: filteredMembers = filteredMembers.OrderBy(o => o.Pares); break;
                        case 4: filteredMembers = filteredMembers.OrderBy(o => o.Estado); break;
                        case 5: filteredMembers = filteredMembers.OrderBy(o => o.Ganancia); break;
                        case 6: filteredMembers = filteredMembers.OrderBy(o => o.Subtotal); break;
                        case 7: filteredMembers = filteredMembers.OrderBy(o => o.N_C); break;
                        case 8: filteredMembers = filteredMembers.OrderBy(o => o.Total); break;
                        case 9: filteredMembers = filteredMembers.OrderBy(o => o.Percepcion); break;
                        case 10: filteredMembers = filteredMembers.OrderBy(o => o.TotalPagar); break;
                    }
                }
                else
                {
                    switch (sortIdx)
                    {
                        case 0: filteredMembers = filteredMembers.OrderByDescending(o => Convert.ToDecimal(o.liq_Id)); break;
                        case 1: filteredMembers = filteredMembers.OrderByDescending(o => o.ventaId); break;
                        case 2: filteredMembers = filteredMembers.OrderByDescending(o => Convert.ToDateTime(o.liq_Fecha)).ThenByDescending(to => Convert.ToDecimal(to.liq_Id)); break;
                        case 3: filteredMembers = filteredMembers.OrderByDescending(o => o.Pares); break;
                        case 4: filteredMembers = filteredMembers.OrderByDescending(o => o.Estado); break;
                        case 5: filteredMembers = filteredMembers.OrderByDescending(o => o.Ganancia); break;
                        case 6: filteredMembers = filteredMembers.OrderByDescending(o => o.Subtotal); break;
                        case 7: filteredMembers = filteredMembers.OrderByDescending(o => o.N_C); break;
                        case 8: filteredMembers = filteredMembers.OrderByDescending(o => o.Total); break;
                        case 9: filteredMembers = filteredMembers.OrderByDescending(o => o.Percepcion); break;
                        case 10: filteredMembers = filteredMembers.OrderByDescending(o => o.TotalPagar); break;
                    }
                }
            }
            var displayMembers = filteredMembers
                .Skip(param.iDisplayStart)
                .Take(param.iDisplayLength);
            var result = from a in displayMembers
                         select new
                         {
                             a.liq_Id,
                             a.ventaId,
                             a.liq_Fecha,
                             a.Pares,
                             a.Estado,
                             a.Ganancia,
                             a.Subtotal,
                             a.N_C,
                             a.Total,
                             a.Percepcion,
                             a.TotalPagar,
                             a.ped_Id,
                             a.cust_Id,
                             a.estId,
                             a.liq_opg,
                             a.liq_tipo_prov,
                             a.liq_tipo_des,
                             a.liq_agencia,
                             a.liq_agencia_direccion,
                             a.liq_destino,
                             a.liq_direccion,
                             a.liq_referencia,
                             a.bas_tipo_dis,
                             a.bas_provincia,
                             a.bas_documento
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
        /** Lista Nota de cRedito **/
        public ActionResult getListNotaCredito(Ent_jQueryDataTableParams param)
        {
            /*verificar si esta null*/
            if (Session[_session_list_NotaCredito] == null)
            {
                List<Ent_NotaCredito> listPed = new List<Ent_NotaCredito>();
                Session[_session_list_NotaCredito] = listPed;
            }

            //Traer registros
            IQueryable<Ent_NotaCredito> membercol = ((List<Ent_NotaCredito>)(Session[_session_list_NotaCredito])).AsQueryable();  //lista().AsQueryable();

            //Manejador de filtros
            int totalCount = membercol.Count();
            IEnumerable<Ent_NotaCredito> filteredMembers = membercol;

            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredMembers = membercol
                    .Where(m => m.Not_Numero.ToUpper().Contains(param.sSearch.ToUpper()));
            }
            //Manejador de orden
            var sortIdx = Convert.ToInt32(Request["iSortCol_0"]);

            if (param.iSortingCols > 0)
            {
                if (Request["sSortDir_0"].ToString() == "asc")
                {
                    switch (sortIdx)
                    {
                        case 1: filteredMembers = filteredMembers.OrderBy(o => Convert.ToDateTime(o.Not_Fecha)); break;
                    }
                }
                else
                {
                    switch (sortIdx)
                    {
                        case 1: filteredMembers = filteredMembers.OrderByDescending(o => Convert.ToDateTime(o.Not_Fecha)); break;
                    }
                }
            }
            var displayMembers = filteredMembers
                .Skip(param.iDisplayStart)
                .Take(param.iDisplayLength);
            var result = from a in displayMembers
                         select new
                         {
                             a.Not_Numero,
                             a.Not_Fecha,
                             a.Not_Det_Cantidad,
                             a.Total,
                             a.Not_Id                             
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
        /** Lista Consignaciones **/
        public ActionResult getListConsignaciones(Ent_jQueryDataTableParams param)
        {
            /*verificar si esta null*/
            if (Session[_session_list_consignaciones] == null)
            {
                List<Ent_Consignacioes> listPed = new List<Ent_Consignacioes>();
                Session[_session_list_consignaciones] = listPed;
            }

            //Traer registros
            IQueryable<Ent_Consignacioes> membercol = ((List<Ent_Consignacioes>)(Session[_session_list_consignaciones])).AsQueryable();  //lista().AsQueryable();

            //Manejador de filtros
            int totalCount = membercol.Count();
            IEnumerable<Ent_Consignacioes> filteredMembers = membercol;

            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredMembers = membercol
                    .Where(m => m.Pag_Num_Consignacion.ToUpper().Contains(param.sSearch.ToUpper()));
            }
            //Manejador de orden
            var sortIdx = Convert.ToInt32(Request["iSortCol_0"]);

            if (param.iSortingCols > 0)
            {
                if (Request["sSortDir_0"].ToString() == "asc")
                {
                    switch (sortIdx)
                    {
                        case 2: filteredMembers = filteredMembers.OrderBy(o => o.Pag_Monto); break;
                        case 3: filteredMembers = filteredMembers.OrderBy(o => Convert.ToDateTime(o.Pag_Num_ConsFecha)); break;
                    }
                }
                else
                {
                    switch (sortIdx)
                    {
                        case 2: filteredMembers = filteredMembers.OrderByDescending(o => o.Pag_Monto); break;
                        case 3: filteredMembers = filteredMembers.OrderByDescending(o => Convert.ToDateTime(o.Pag_Num_ConsFecha)); break;
                    }
                }
            }
            var displayMembers = filteredMembers
                .Skip(param.iDisplayStart)
                .Take(param.iDisplayLength);
            var result = from a in displayMembers
                         select new
                         {
                             a.Ban_Descripcion,
                             a.Pag_Num_Consignacion,
                             a.Pag_Monto,
                             a.Pag_Num_ConsFecha,
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
        /** Lista Saldos **/
        public ActionResult getListSaldos(Ent_jQueryDataTableParams param)
        {
            /*verificar si esta null*/
            if (Session[_session_list_saldos] == null)
            {
                List<Ent_Saldos> listPed = new List<Ent_Saldos>();
                Session[_session_list_saldos] = listPed;
            }

            //Traer registros
            IQueryable<Ent_Saldos> membercol = ((List<Ent_Saldos>)(Session[_session_list_saldos])).AsQueryable();  //lista().AsQueryable();

            //Manejador de filtros
            int totalCount = membercol.Count();
            IEnumerable<Ent_Saldos> filteredMembers = membercol;

            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredMembers = membercol
                    .Where(m => m.Descipcion.ToUpper().Contains(param.sSearch.ToUpper()));
            }
            //Manejador de orden
            var sortIdx = Convert.ToInt32(Request["iSortCol_0"]);

            if (param.iSortingCols > 0)
            {
                if (Request["sSortDir_0"].ToString() == "asc")
                {
                    switch (sortIdx)
                    {
                        case 1: filteredMembers = filteredMembers.OrderBy(o => o.Monto); break;
                    }
                }
                else
                {
                    switch (sortIdx)
                    {
                        case 1: filteredMembers = filteredMembers.OrderByDescending(o => o.Monto); break;
                    }
                }
            }
            var displayMembers = filteredMembers
                .Skip(param.iDisplayStart)
                .Take(param.iDisplayLength);
            var result = from a in displayMembers
                         select new
                         {
                             a.Descipcion,
                             a.Monto,
                             a.Percepcion,
                             a.Saldo,
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
        [HttpPost]
        public ActionResult setDetallePedido(List<Ent_Nueva_Linea_Talla> listTallasCantidad = null, bool editar = false, string tipoPago = "005" )
        {
            try
            {
                if (listTallasCantidad == null)
                {
                    return Json(new { estado = 0  , mensaje ="Nada para agregar a la lista."});
                }               
                //Ent_Order_Dtl nuevaLinea = new Ent_Order_Dtl();
                Ent_Articulo_pedido articulo = (Ent_Articulo_pedido)(Session[_session_nuevo_item_pedido]);
                foreach (Ent_Nueva_Linea_Talla item in listTallasCantidad)
                {

                    
                    List<Tran_Ofertas> listOfertas = new List<Tran_Ofertas>();
                    int _idOfe = 0;
                    if (!editar) {
                        /* Session con las ofertas */
                        if (Session[_session_Tran_Ofertas] != null)
                        {
                            listOfertas = (List<Tran_Ofertas>)Session[_session_Tran_Ofertas];
                            _idOfe = listOfertas.Max(m => m.id);
                        }
                        List<Tran_Ofertas> list = new List<Tran_Ofertas>();
                        list = (from Ent_Articulo_Ofertas art in articulo._ofertas
                                select new Tran_Ofertas()
                                {
                                    id = _idOfe + 1,
                                    idArt = articulo.Art_id,
                                    ofe_id = art.Ofe_Id,
                                    max_pares = art.Ofe_MaxPares,
                                    ofe_porc = art.Ofe_Porc,
                                    ofe_tipo = art.Ofe_Tipo,
                                    ofe_artventa = art.Ofe_ArtVenta,
                                    ofe_prioridad = art.Ofe_Prioridad,
                                }).ToList();
                        listOfertas = listOfertas.Union(list).ToList();

                        Session[_session_Tran_Ofertas] = listOfertas;
                        /* Session con las ofertas */
                    }else
                    {
                        if (item.CantTalla == 0)
                        {
                            List<Ent_Order_Dtl> _listActual = (List<Ent_Order_Dtl>)Session[_session_list_detalle_pedido];
                            _listActual.Remove(_listActual.Where(w => w._code == articulo.Art_id && w._size == item.codTalla).FirstOrDefault());
                            continue;
                        }
                    }

                    //line = newLine;
                    Ent_Order_Dtl _nuevo = new Ent_Order_Dtl() {
                        _code = articulo.Art_id,
                        _artName = articulo.Art_Descripcion,
                        _brand = articulo.Mar_Descripcion,
                        //_brandImg = dr["brv_image"].ToString(),
                        _color = articulo.Col_Descripcion,
                        _majorCat = articulo.Cat_Pri_Descripcion,
                        _cat = articulo.Cat_Descripcion,
                        _subcat = articulo.Sca_Descripcion,
                        //_origin = dr["arv_origin"].ToString(),
                        //_originDesc = dr["arv_origin"].ToString().Equals(Constants.IdOriginImported) ? "Artículo importado" : "Artículo nacional",
                        _comm = (int)articulo.Art_Comision,
                        _uriPhoto = articulo.Art_Foto,
                        _price = articulo.Art_Pre_Sin_Igv,
                        //_priceDesc = Convert.ToDecimal(dr["Art_Pre_Sin_Igv"]).ToString(currency),
                        //_dsctoDesc = order._dscto.ToString(currency),
                        _priceigv = articulo.Art_Pre_Con_Igv,
                        //_priceigvDesc = Convert.ToDecimal(dr["Art_Pre_Con_Igv"]).ToString(currency),
                        _ap_percepcion = articulo.Afec_Percepcion.ToString(),
                        _ofe_Tipo = articulo.Ofe_Tipo,
                        _ofe_PrecioPack = articulo.Ofe_ArtVenta,
                        _ofe_id = articulo.Ofe_Id,
                        _ofe_maxpares = articulo.Ofe_MaxPares,
                        _ofe_porc = articulo.Ofe_Porc,
                        _premio = "",
                        _premId = "",
                        _premioDesc = "",
                        id_tran_ofe = _idOfe + 1
                        //nroProms = dtArt.Rows.Count,
                    };
                    _nuevo._size = item.codTalla;
                    _addArticle(_nuevo, item.CantTalla, tipoPago, editar);                    
                }
                return Json(new { estado = 1  });
            }
            catch (Exception ex)
            {
                return Json(new { estado = 0 , mensaje = ex.Message });
            }           
        }
        public void fupdateitemoferta()
        {
            List<Ent_Order_Dtl> orderLines = (List<Ent_Order_Dtl>)(((object)Session[_session_list_detalle_pedido]) != null ? (object)Session[_session_list_detalle_pedido] : new List<Ent_Order_Dtl>()); ;

            orderLines = pre_promocion(orderLines);

            try
            {
                Ent_Persona cust = (Ent_Persona)Session[_session_customer];
                string varTipoPago = cust._vartipopago;
                // se agregó varPagoxOPG.06-06-19
                if (orderLines != null && varTipoPago != "008" && varTipoPago != "OPG")
                {

                    /*formatear las promociones*/
                    for (Int32 c = 0; c < orderLines.Count; ++c)
                    {
                        //verificar las promociones
                        if (orderLines[c]._ofe_id != 0)
                        {
                            orderLines[c]._dscto = 0;
                           // orderLines[c]._dsctoDesc = orderLines[c]._dscto.ToString(_currency);

                            orderLines[c]._lineTotal = Math.Round((orderLines[c]._qty * orderLines[c]._price) - (orderLines[c]._commission) - (orderLines[c]._dscto), 2, MidpointRounding.AwayFromZero);
                            orderLines[c]._lineTotDesc = orderLines[c]._lineTotal;
                           // orderLines[c]._lineTotDesc = orderLines[c]._lineTotal.ToString(_currency);
                        }
                    }
                    /********************/

                    /*si es que tiee oferta POR PORCENTAJE entonces vamos a filtrar*/
                    List<Ent_Order_Dtl> orderLinesOferta_filter = orderLines.Where(c => c._ofe_id != 0 && c._ofe_Tipo == "N").ToList();

                    List<Ent_Order_Dtl> orderLinesOferta_filterPack = orderLines.Where(c => c._ofe_id != 0 && c._ofe_Tipo == "P").ToList();

                    /*si es que tiee oferta POR PORCENTAJE entonces vamos a filtrar*/
                    List<Ent_Order_Dtl> orderLinesOferta_filterCombo = orderLines.Where(c => c._ofe_id != 0 && c._ofe_Tipo == "C").ToList();

                    List<Ent_Order_Dtl> orderLinesOferta_filterMochila = orderLines.Where(c => c._ofe_id != 0 && c._ofe_Tipo == "M").ToList();


                    /*INICIO DE LAS OFERTAS DESCUENTO POR PORCENTAJE*/

                    if (orderLinesOferta_filter.Count > 0)
                    {
                        //var grupo_oferta = orderLines.GroupBy(c => c._ofe_id != 0).ToList();
                        var lista_gr = from item in orderLines
                                       where item._ofe_id != 0
                                       group item by
                                       new
                                       {
                                           ofertaid = item._ofe_id,
                                           ofemaxpar = item._ofe_maxpares,
                                           oferporc = item._ofe_porc,
                                       } into g
                                       select new
                                       {
                                           ofertaid = g.Key.ofertaid,
                                           ofemaxpar = g.Key.ofemaxpar,
                                           oferporc = g.Key.oferporc,

                                       };
                        foreach (var it in lista_gr)
                        {



                            /*capturamos el maximo de pares y por descuento*/
                            Decimal _max_pares = it.ofemaxpar;//  orderLinesOferta_filter[0]._ofe_maxpares;
                            Decimal _por_desc = it.oferporc / 100 /* orderLinesOferta_filter[0]._ofe_porc/100*/;



                            Decimal _total = orderLinesOferta_filter.Where(r => r._ofe_id == it.ofertaid).Sum(x => x._qty);

                            /*ahora capturado el total de pares le hacemos un for para */

                            decimal _res = _total / _max_pares;
                            //if (_max_pares == 1)
                            //{
                            //    _res = 1;
                            //}
                            /*para saber si es un entero es true si no es false decimal*/
                            bool isInt = (int)_res == _res;

                            DataTable dt = new DataTable();
                            dt.Columns.Add("articulo", typeof(string));
                            dt.Columns.Add("talla", typeof(string));
                            dt.Columns.Add("precio", typeof(Decimal));
                            dt.Columns.Add("cantidad", typeof(Decimal));
                            dt.Columns.Add("porc_comision", typeof(Decimal));
                            dt.Columns.Add("descuento", typeof(Decimal));
                            dt.Columns.Add("oferta", typeof(string));


                            foreach (var filas in orderLinesOferta_filter.Where(r => r._ofe_id == it.ofertaid).ToList())
                            {
                                for (Int32 c = 0; c < filas._qty; ++c)
                                {
                                    dt.Rows.Add(filas._code.ToString(),
                                         filas._size.ToString(),
                                         filas._price,
                                         1,
                                         filas._commissionPctg,
                                         0, filas._ofe_id.ToString());


                                }
                            }

                            if (!isInt)
                                _res = Convert.ToInt32((_res) - Convert.ToDecimal(0.1));


                            if (_res != 0)
                            {
                                DataRow[] _filas = dt.Select("len(articulo)>0 and oferta='" + it.ofertaid.ToString() + "'", "precio asc");
                                if (_filas.Length > 0)
                                {
                                    if (_por_desc == 1)
                                    {
                                        _por_desc = 0.5M;
                                        _res = 2;
                                    }

                                    Decimal _des_oferta = 0;
                                    for (Int32 i = 0; i < _res; ++i)
                                    {
                                        string _articulo = _filas[i]["articulo"].ToString();
                                        string _talla = _filas[i]["talla"].ToString();
                                        Decimal _precio = Convert.ToDecimal(_filas[i]["precio"]);
                                        Decimal _com_porc = Convert.ToDecimal(_filas[i]["porc_comision"]);
                                        Decimal _cant = Convert.ToDecimal(_filas[i]["cantidad"]);
                                        decimal _com_mon = Math.Round((_precio * _cant) * _com_porc, 2, MidpointRounding.AwayFromZero);

                                        if (i == 0 & _max_pares > 1)
                                        {
                                            _des_oferta = Math.Round(((_precio * _cant) - _com_mon) * (_por_desc), 2, MidpointRounding.AwayFromZero);
                                        }

                                        if (_max_pares == 1)
                                        {
                                            _des_oferta = Math.Round(((_precio * _cant) - _com_mon) * (_por_desc), 2, MidpointRounding.AwayFromZero);
                                        }

                                        _filas[i]["descuento"] = _des_oferta;
                                    }

                                    for (Int32 i = 0; i < orderLines.Count; ++i)
                                    {
                                        string _articulo = orderLines[i]._code.ToString();
                                        string _talla = orderLines[i]._size.ToString();
                                        string _oferta_id = orderLines[i]._ofe_id.ToString();
                                        foreach (DataRow vfila in _filas)
                                        {
                                            if (_articulo == vfila["articulo"].ToString() && _talla == vfila["talla"].ToString() && _oferta_id == vfila["oferta"].ToString())
                                            {
                                                orderLines[i]._dscto += Convert.ToDecimal(vfila["descuento"]);
                                                //orderLines[i]._dsctoDesc = orderLines[i]._dscto.ToString(_currency);

                                                orderLines[i]._lineTotal = Math.Round((orderLines[i]._qty * orderLines[i]._price) - (orderLines[i]._commission) - (orderLines[i]._dscto), 2, MidpointRounding.AwayFromZero);
                                                orderLines[i]._lineTotDesc = orderLines[i]._lineTotal;
                                                //orderLines[i]._lineTotDesc = orderLines[i]._lineTotal.ToString(_currency);
                                            }
                                        }
                                    }
                                }/******/
                            }
                        }

                    }

                    /*FIN DE LAS OFERTAS DESCUENTO POR PORCENTAJE*/

                    /*INICIO DE LAS OFERTAS PRECIO POR PACK*/

                    if (orderLinesOferta_filterPack.Count > 0)
                    {

                        //var grupo_oferta = orderLines.GroupBy(c => c._ofe_id != 0).ToList();
                        var lista_gr = from item in orderLines
                                       where item._ofe_id != 0
                                       group item by
                                       new
                                       {
                                           ofertaid = item._ofe_id,
                                           ofemaxpar = item._ofe_maxpares,
                                           _ofe_PrecioPack = item._ofe_PrecioPack,
                                       } into g
                                       select new
                                       {
                                           ofertaid = g.Key.ofertaid,
                                           ofemaxpar = g.Key.ofemaxpar,
                                           _ofe_PrecioPack = g.Key._ofe_PrecioPack,

                                       };
                        foreach (var it in lista_gr)
                        {



                            /*capturamos el maximo de pares y por descuento*/
                            Decimal _max_pares = it.ofemaxpar;//  orderLinesOferta_filter[0]._ofe_maxpares;
                            Decimal _por_desc = it._ofe_PrecioPack /* orderLinesOferta_filter[0]._ofe_porc/100*/;

                            Decimal _ofe_PrecioPack = it._ofe_PrecioPack;


                            Decimal _total = orderLinesOferta_filterPack.Where(r => r._ofe_id == it.ofertaid).Sum(x => x._qty);

                            /*ahora capturado el total de pares le hacemos un for para */

                            decimal _res = _total / _max_pares;
                            /*para saber si es un entero es true si no es false decimal*/
                            bool isInt = (int)_res == _res;

                            DataTable dt = new DataTable();
                            dt.Columns.Add("articulo", typeof(string));
                            dt.Columns.Add("talla", typeof(string));
                            dt.Columns.Add("precio", typeof(Decimal));
                            dt.Columns.Add("cantidad", typeof(Decimal));
                            dt.Columns.Add("porc_comision", typeof(Decimal));
                            dt.Columns.Add("descuento", typeof(Decimal));
                            dt.Columns.Add("oferta", typeof(string));


                            foreach (var filas in orderLinesOferta_filterPack.Where(r => r._ofe_id == it.ofertaid).ToList())
                            {
                                for (Int32 c = 0; c < filas._qty; ++c)
                                {
                                    dt.Rows.Add(filas._code.ToString(),
                                         filas._size.ToString(),
                                         filas._price,
                                         1,
                                         filas._commissionPctg,
                                         0, filas._ofe_id.ToString());


                                }
                            }

                            if (!isInt)
                                _res = Convert.ToInt32((_res) - Convert.ToDecimal(0.1));


                            if (_res != 0)
                            {
                                DataRow[] _filas = dt.Select("len(articulo)>0 and oferta='" + it.ofertaid.ToString() + "'", "precio asc");
                                if (_filas.Length > 0)
                                {
                                    if (_por_desc == 1)
                                    {
                                        _por_desc = 0.5M;
                                        _res = 2;
                                    }

                                    Decimal _des_oferta = 0;
                                    Decimal cantColec = 0;
                                    Decimal montoColec = 0;
                                    Decimal residuoCant = 0;
                                    Decimal montoColecResiduo = 0;

                                    int Colect = 0;
                                    for (Int32 i = 0; i < _filas.Length; ++i)
                                    {
                                        Colect++;

                                        string _articulo = _filas[i]["articulo"].ToString();
                                        string _talla = _filas[i]["talla"].ToString();
                                        Decimal _precio = Convert.ToDecimal(_filas[i]["precio"]);
                                        Decimal _com_porc = Convert.ToDecimal(_filas[i]["porc_comision"]);
                                        Decimal _cant = Convert.ToDecimal(_filas[i]["cantidad"]);
                                        cantColec += _cant;

                                        if (cantColec > _max_pares)
                                        {

                                            decimal _tPremios = cantColec / _max_pares;
                                            residuoCant = cantColec - (_max_pares * _tPremios);
                                            _cant = _max_pares * _tPremios;
                                            decimal _com_mon_res = Math.Round((_precio * residuoCant) * _com_porc, 2, MidpointRounding.AwayFromZero);
                                            montoColecResiduo = (_precio * residuoCant) - _com_mon_res;
                                        }

                                        decimal _com_mon = Math.Round((_precio * _cant) * _com_porc, 2, MidpointRounding.AwayFromZero);


                                        montoColec += (_precio * _cant) - _com_mon;

                                        if (cantColec >= _max_pares)
                                        {

                                            Decimal TotalDesc = montoColec - _ofe_PrecioPack;

                                            for (Int32 j = 0; j < Colect; ++j)
                                            {

                                                Decimal _preciopk = Convert.ToDecimal(_filas[i - j]["precio"]);
                                                Decimal _cantpk = Convert.ToDecimal(_filas[i - j]["cantidad"]);

                                                if (_cantpk > _max_pares)
                                                    _cantpk = _cant;

                                                Decimal _com_porcpk = Convert.ToDecimal(_filas[i - j]["porc_comision"]);
                                                decimal _com_monpk = Math.Round((_preciopk * _cantpk) * _com_porcpk, 2, MidpointRounding.AwayFromZero);
                                                Decimal totallineapk = _preciopk - _com_monpk;
                                                _des_oferta = Math.Round((totallineapk / montoColec) * TotalDesc, 2, MidpointRounding.AwayFromZero);

                                                //Decimal descuento = Convert.ToDecimal(_filas[i - j]["descuento"]);
                                                _filas[i - j]["descuento"] = _des_oferta;
                                            }

                                            cantColec = 0;
                                            montoColec = 0;
                                            Colect = 0;

                                        }


                                    }

                                    for (Int32 i = 0; i < orderLines.Count; ++i)
                                    {
                                        string _articulo = orderLines[i]._code.ToString();
                                        string _talla = orderLines[i]._size.ToString();
                                        string _oferta_id = orderLines[i]._ofe_id.ToString();
                                        foreach (DataRow vfila in _filas)
                                        {
                                            if (_articulo == vfila["articulo"].ToString() && _talla == vfila["talla"].ToString() && _oferta_id == vfila["oferta"].ToString())
                                            {
                                                orderLines[i]._dscto += Convert.ToDecimal(vfila["descuento"]);
                                                //orderLines[i]._dsctoDesc = orderLines[i]._dscto.ToString(_currency);

                                                orderLines[i]._lineTotal = Math.Round((orderLines[i]._qty * orderLines[i]._price) - (orderLines[i]._commission) - (orderLines[i]._dscto), 2, MidpointRounding.AwayFromZero);
                                                //orderLines[i]._lineTotDesc = orderLines[i]._lineTotal.ToString(_currency);
                                            }
                                        }
                                    }
                                }
                            }/****/
                        }

                    }


                    /*FIN DE LAS OFERTAS PRECIO POR PACK*/


                    #region Ofertas combos

                    if (orderLinesOferta_filterCombo.Count > 0)
                    {

                        //var grupo_oferta = orderLines.GroupBy(c => c._ofe_id != 0).ToList();
                        var lista_gr = from item in orderLines
                                       where item._ofe_id != 0
                                       group item by
                                       new
                                       {
                                           ofertaid = item._ofe_id,
                                           ofemaxpar = item._ofe_maxpares,
                                           _ofe_PrecioPack = item._ofe_PrecioPack,
                                           // _grupo = item._ofe_porc
                                       } into g
                                       select new
                                       {
                                           ofertaid = g.Key.ofertaid,
                                           ofemaxpar = g.Key.ofemaxpar,
                                           _ofe_PrecioPack = g.Key._ofe_PrecioPack,
                                           //   _grupo = g.Key._grupo    
                                       };
                        foreach (var it in lista_gr)
                        {
                            /*capturamos el maximo de pares y por descuento*/
                            Decimal _max_pares = it.ofemaxpar;//  orderLinesOferta_filter[0]._ofe_maxpares;
                            Decimal _por_desc = it._ofe_PrecioPack /* orderLinesOferta_filter[0]._ofe_porc/100*/;

                            Decimal _ofe_PrecioPack = it._ofe_PrecioPack;

                            Decimal _total1 = orderLinesOferta_filterCombo.Where(r => r._ofe_id == it.ofertaid && r._ofe_porc == 1).Sum(x => x._qty);
                            Decimal _total2 = orderLinesOferta_filterCombo.Where(r => r._ofe_id == it.ofertaid && r._ofe_porc == 2).Sum(x => x._qty);
                            Decimal _total = (new decimal[] { _total1, _total2 }).Min();

                            /*ahora capturado el total de pares le hacemos un for para */

                            decimal _res = _total / _max_pares;
                            /*para saber si es un entero es true si no es false decimal*/

                            if (_total > 0)
                            {
                                DataTable dt = new DataTable();

                                dt.Columns.Add("articulo", typeof(string));
                                dt.Columns.Add("talla", typeof(string));
                                dt.Columns.Add("precio", typeof(Decimal));
                                dt.Columns.Add("cantidad", typeof(Decimal));
                                dt.Columns.Add("porc_comision", typeof(Decimal));
                                dt.Columns.Add("descuento", typeof(Decimal));
                                dt.Columns.Add("oferta", typeof(string));
                                dt.Columns.Add("grupo", typeof(string));
                                dt.Columns.Add("promo", typeof(bool));
                                dt.Columns.Add("index", typeof(int));

                                int _ind = 0;
                                foreach (var filas in orderLinesOferta_filterCombo.Where(r => r._ofe_id == it.ofertaid).ToList())
                                {
                                    for (Int32 c = 0; c < filas._qty; ++c)
                                    {
                                        _ind++;
                                        dt.Rows.Add(filas._code.ToString(),
                                                filas._size.ToString(),
                                                filas._price,
                                                1,
                                                filas._commissionPctg,
                                                0, filas._ofe_id.ToString(), filas._ofe_porc, false, _ind);
                                    }
                                }

                                for (int k = 0; k < _total; k++)
                                {
                                    DataRow[] _filas = new DataRow[] { dt.Select("len(articulo)>0 and grupo=1.0000 and promo = 0", "").FirstOrDefault(),
                                        dt.Select("len(articulo)>0 and grupo=2.0 and promo = 0", "").FirstOrDefault() };

                                    if (_filas.Length > 0)
                                    {
                                        if (_por_desc == 1)
                                        {
                                            _por_desc = 0.5M;
                                            _res = 2;
                                        }

                                        Decimal _des_oferta = 0;
                                        Decimal cantColec = 0;
                                        Decimal montoColec = 0;
                                        Decimal residuoCant = 0;
                                        Decimal montoColecResiduo = 0;

                                        int Colect = 0;
                                        for (Int32 i = 0; i < _filas.Length; ++i)
                                        {
                                            Colect++;

                                            string _articulo = _filas[i]["articulo"].ToString();
                                            string _talla = _filas[i]["talla"].ToString();
                                            Decimal _precio = Convert.ToDecimal(_filas[i]["precio"]);
                                            Decimal _com_porc = Convert.ToDecimal(_filas[i]["porc_comision"]);
                                            Decimal _cant = Convert.ToDecimal(_filas[i]["cantidad"]);
                                            cantColec += _cant;

                                            if (cantColec > _max_pares)
                                            {

                                                decimal _tPremios = cantColec / _max_pares;
                                                residuoCant = cantColec - (_max_pares * _tPremios);
                                                _cant = _max_pares * _tPremios;
                                                decimal _com_mon_res = Math.Round((_precio * residuoCant) * _com_porc, 2, MidpointRounding.AwayFromZero);
                                                montoColecResiduo = (_precio * residuoCant) - _com_mon_res;
                                            }

                                            decimal _com_mon = Math.Round((_precio * _cant) * _com_porc, 2, MidpointRounding.AwayFromZero);


                                            montoColec += (_precio * _cant) - _com_mon;

                                            if (cantColec >= _max_pares)
                                            {

                                                Decimal TotalDesc = montoColec - _ofe_PrecioPack;

                                                for (Int32 j = 0; j < Colect; ++j)
                                                {

                                                    Decimal _preciopk = Convert.ToDecimal(_filas[i - j]["precio"]);
                                                    Decimal _cantpk = Convert.ToDecimal(_filas[i - j]["cantidad"]);

                                                    if (_cantpk > _max_pares)
                                                        _cantpk = _cant;

                                                    Decimal _com_porcpk = Convert.ToDecimal(_filas[i - j]["porc_comision"]);
                                                    decimal _com_monpk = Math.Round((_preciopk * _cantpk) * _com_porcpk, 2, MidpointRounding.AwayFromZero);
                                                    Decimal totallineapk = _preciopk - _com_monpk;
                                                    _des_oferta = Math.Round((totallineapk / montoColec) * TotalDesc, 2, MidpointRounding.AwayFromZero);

                                                    //Decimal descuento = Convert.ToDecimal(_filas[i - j]["descuento"]);
                                                    _filas[i - j]["descuento"] = _des_oferta;
                                                }

                                                cantColec = 0;
                                                montoColec = 0;
                                                Colect = 0;

                                            }
                                            for (int l = 0; l < dt.Rows.Count; l++)
                                            {
                                                if (Convert.ToInt32(dt.Rows[l]["index"]) == Convert.ToDecimal(_filas[i]["index"]))
                                                {
                                                    dt.Rows[l]["promo"] = true;
                                                }
                                            }

                                        }

                                        for (Int32 i = 0; i < orderLines.Count; ++i)
                                        {
                                            string _articulo = orderLines[i]._code.ToString();
                                            string _talla = orderLines[i]._size.ToString();
                                            string _oferta_id = orderLines[i]._ofe_id.ToString();
                                            foreach (DataRow vfila in _filas)
                                            {
                                                if (_articulo == vfila["articulo"].ToString() && _talla == vfila["talla"].ToString() && _oferta_id == vfila["oferta"].ToString())
                                                {
                                                    orderLines[i]._dscto += Convert.ToDecimal(vfila["descuento"]);
                                                    //orderLines[i]._dsctoDesc = orderLines[i]._dscto.ToString(_currency);

                                                    orderLines[i]._lineTotal = Math.Round((orderLines[i]._qty * orderLines[i]._price) - (orderLines[i]._commission) - (orderLines[i]._dscto), 2, MidpointRounding.AwayFromZero);
                                                    //orderLines[i]._lineTotDesc = orderLines[i]._lineTotal.ToString(_currency);
                                                }
                                            }
                                        }
                                    }
                                }
                            }                          
                        }
                    }

                    #endregion

                    #region Mochila
                    if (orderLinesOferta_filterMochila.Count > 0)
                    {

                        //var grupo_oferta = orderLines.GroupBy(c => c._ofe_id != 0).ToList();
                        var lista_gr = from item in orderLines
                                       where item._ofe_id != 0
                                       group item by
                                       new
                                       {
                                           ofertaid = item._ofe_id,
                                           ofemaxpar = item._ofe_maxpares,
                                           //oferporc = item._ofe_porc,
                                       } into g
                                       select new
                                       {
                                           ofertaid = g.Key.ofertaid,
                                           ofemaxpar = g.Key.ofemaxpar,
                                           //oferporc = g.Key.oferporc,

                                       };
                        foreach (var it in lista_gr)
                        {
                            /*capturamos el maximo de pares y por descuento*/
                            Decimal _max_pares = it.ofemaxpar;//  orderLinesOferta_filter[0]._ofe_maxpares;
                            //Decimal _por_desc = it.oferporc / 100 /* orderLinesOferta_filter[0]._ofe_porc/100*/;

                            //Decimal _ofe_PrecioPack = it._ofe_PrecioPack;

                            Decimal _total1 = orderLinesOferta_filterMochila.Where(r => r._ofe_id == it.ofertaid && r._ofe_PrecioPack == 1).Sum(x => x._qty);
                            Decimal _total2 = orderLinesOferta_filterMochila.Where(r => r._ofe_id == it.ofertaid && r._ofe_PrecioPack == 2).Sum(x => x._qty);
                            Decimal _total = (new decimal[] { _total1, _total2 }).Min();

                            /*ahora capturado el total de pares le hacemos un for para */

                            decimal _res = _total / _max_pares;
                            /*para saber si es un entero es true si no es false decimal*/

                            if (_total > 0)
                            {
                                DataTable dt = new DataTable();

                                dt.Columns.Add("articulo", typeof(string));
                                dt.Columns.Add("talla", typeof(string));
                                dt.Columns.Add("precio", typeof(Decimal));
                                dt.Columns.Add("cantidad", typeof(Decimal));
                                dt.Columns.Add("porc_comision", typeof(Decimal));
                                dt.Columns.Add("descuento", typeof(Decimal));
                                dt.Columns.Add("oferta", typeof(string));
                                dt.Columns.Add("grupo", typeof(string));
                                dt.Columns.Add("promo", typeof(bool));
                                dt.Columns.Add("index", typeof(int));
                                dt.Columns.Add("porc_desc", typeof(Decimal));

                                int _ind = 0;
                                foreach (var filas in orderLinesOferta_filterMochila.Where(r => r._ofe_id == it.ofertaid).ToList())
                                {
                                    for (Int32 c = 0; c < filas._qty; ++c)
                                    {
                                        _ind++;
                                        dt.Rows.Add(filas._code.ToString(),
                                                filas._size.ToString(),
                                                filas._price,
                                                1,
                                                filas._commissionPctg,
                                                0, filas._ofe_id.ToString(), filas._ofe_PrecioPack, false, _ind, filas._ofe_porc);
                                    }
                                }

                                for (int k = 0; k < _total; k++)
                                {
                                    DataRow[] _filas = (new DataRow[] { dt.Select("len(articulo)>0 and grupo=1.0000 and promo = 0", "precio asc").FirstOrDefault(),
                                        dt.Select("len(articulo)>0 and grupo=2.0 and promo = 0", "precio asc").FirstOrDefault() }).OrderByDescending(r => r.Field<string>("grupo")).ToArray<DataRow>();

                                    if (_filas.Length > 0)
                                    {
                                        DataRow _dtM = _filas.FirstOrDefault();
                                        Decimal _por_desc = Convert.ToDecimal(_dtM["porc_desc"]) / 100;
                                        if (_por_desc == 1)
                                        {
                                            _por_desc = 0.5M;
                                            _res = 2;
                                        }

                                        Decimal _des_oferta = 0;
                                        for (Int32 i = 0; i < _res; ++i)
                                        {
                                            string _articulo = _filas[i]["articulo"].ToString();
                                            string _talla = _filas[i]["talla"].ToString();
                                            Decimal _precio = Convert.ToDecimal(_filas[i]["precio"]);
                                            Decimal _com_porc = Convert.ToDecimal(_filas[i]["porc_comision"]);
                                            Decimal _cant = Convert.ToDecimal(_filas[i]["cantidad"]);
                                            decimal _com_mon = Math.Round((_precio * _cant) * _com_porc, 2, MidpointRounding.AwayFromZero);

                                            if (i == 0 & _max_pares > 1)
                                            {
                                                _des_oferta = Math.Round(((_precio * _cant) - _com_mon) * (_por_desc), 2, MidpointRounding.AwayFromZero);
                                            }

                                            if (_max_pares == 1)
                                            {
                                                _des_oferta = Math.Round(((_precio * _cant) - _com_mon) * (_por_desc), 2, MidpointRounding.AwayFromZero);
                                            }

                                            _filas[i]["descuento"] = _des_oferta;

                                            for (int l = 0; l < dt.Rows.Count; l++)
                                            {
                                                if (Convert.ToInt32(dt.Rows[l]["index"]) == Convert.ToDecimal(_filas[i]["index"]))
                                                {
                                                    dt.Rows[l]["promo"] = true;
                                                }
                                            }
                                        }

                                        for (Int32 i = 0; i < orderLines.Count; ++i)
                                        {
                                            string _articulo = orderLines[i]._code.ToString();
                                            string _talla = orderLines[i]._size.ToString();
                                            string _oferta_id = orderLines[i]._ofe_id.ToString();
                                            foreach (DataRow vfila in _filas)
                                            {
                                                if (_articulo == vfila["articulo"].ToString() && _talla == vfila["talla"].ToString() && _oferta_id == vfila["oferta"].ToString())
                                                {
                                                    orderLines[i]._dscto += Convert.ToDecimal(vfila["descuento"]);
                                                    //orderLines[i]._dsctoDesc = orderLines[i]._dscto.ToString(_currency);

                                                    orderLines[i]._lineTotal = Math.Round((orderLines[i]._qty * orderLines[i]._price) - (orderLines[i]._commission) - (orderLines[i]._dscto), 2, MidpointRounding.AwayFromZero);
                                                    //orderLines[i]._lineTotDesc = orderLines[i]._lineTotal.ToString(_currency);
                                                }
                                            }
                                        }
                                    }/******/

                                }
                            }
                        }
                    }
                    #endregion




                    //Decimal _max_pares=


                    //for (Int32 i=0;i<orderLines.Count;++i)
                    //{

                    //}
                }

            }
            catch { }            
        }
        public List<Ent_Order_Dtl> pre_promocion(List<Ent_Order_Dtl> orderLines)
        {
            /* promociones */

            List<Tran_Ofertas> listOfertas = new List<Tran_Ofertas>();

            List<int> hechos = new List<int>();
            if (Session[_session_Tran_Ofertas] != null)
            {
                listOfertas = (List<Tran_Ofertas>)Session[_session_Tran_Ofertas];
            }
            listOfertas.Select(a =>
            {
                a.hecho = "";
                return a;
            }).ToList();
            /* get ofertas */
            if (listOfertas.Count == 0 && orderLines.Count > 0)
            {
                Ent_Usuario _usuario = (Ent_Usuario)Session[Ent_Constantes.NameSessionUser];
                int _idOfe = 0;
                foreach (Ent_Order_Dtl item in orderLines)
                {
                    
                    Ent_Persona cust = (Ent_Persona)Session[_session_customer];
                    Ent_Articulo_pedido articulo = new Ent_Articulo_pedido();
                    List<Ent_Articulo_Tallas> tallas = new List<Ent_Articulo_Tallas>();
                    datPedido.listarStr_ArticuloTalla(item._code, Convert.ToDecimal(cust.Bas_id), ref articulo, ref tallas, _usuario.usu_id);
                    if (articulo._ofertas!=null)
                    {
                        if (articulo._ofertas.Count > 0)
                        {
                            _idOfe++;
                            List<Tran_Ofertas> list = new List<Tran_Ofertas>();
                            list = (from Ent_Articulo_Ofertas art in articulo._ofertas
                                    select new Tran_Ofertas()
                                    {
                                        id = _idOfe,
                                        idArt = articulo.Art_id,
                                        ofe_id = art.Ofe_Id,
                                        max_pares = art.Ofe_MaxPares,
                                        ofe_porc = art.Ofe_Porc,
                                        ofe_tipo = art.Ofe_Tipo,
                                        ofe_artventa = art.Ofe_ArtVenta,
                                        ofe_prioridad = art.Ofe_Prioridad,
                                    }).ToList();
                            listOfertas = listOfertas.Union(list).ToList();
                            orderLines.Where(w => w._code == item._code && w._size == item._size).Select(s => { s.id_tran_ofe = _idOfe; return s; }).ToList();
                        }
                    }
                    
                }               
            }

            listOfertas = listOfertas.Where(w => (orderLines.Select(s => s.id_tran_ofe).Distinct().ToArray()).Contains(w.id)).ToList();
            listOfertas = listOfertas.OrderBy(o => o.ofe_prioridad).ToList();//.OrderBy(a => a.ofe_id).ToList();

            foreach (Tran_Ofertas item in listOfertas)
            {
                decimal ofe_id = item.ofe_id;
                decimal max_pares = item.max_pares; // item.max_pares; //deberia ser la suma de max pares en general de toda la lista por item.ofe_id
                int max_ofe = 0; // (int)(listOfertas.Where(w => w.ofe_id == ofe_id).Count() / 2); // item.max_pares; //deberia ser la suma de max pares en general de toda la lista por item.ofe_id
                decimal _max_pares_valida = 0;
                string ofe_tipo = item.ofe_tipo;
                decimal ofe_grupo = item.ofe_artventa;

                if ((new String[] { "M" }).Contains(ofe_tipo))
                {
                    int gru1 = listOfertas.Count(c => c.ofe_id == ofe_id && c.hecho == "" && c.ofe_artventa == 1 && !hechos.Contains(c.id));
                    int gru2 = listOfertas.Count(c => c.ofe_id == ofe_id && c.hecho == "" && c.ofe_artventa == 2 && !hechos.Contains(c.id));
                    max_ofe = (new int[] { gru1, gru2 }).Min();
                    if (max_ofe >= 1)
                    {
                        for (int i = 0; i < max_ofe; i++)
                        {

                            Tran_Ofertas subItem1 = listOfertas.Where(w => w.hecho == "" && w.ofe_id == ofe_id && w.ofe_artventa == 1 && !hechos.Contains(w.id)).Take(1).FirstOrDefault();
                            Tran_Ofertas subItem2 = listOfertas.Where(w => w.hecho == "" && w.ofe_id == ofe_id && w.ofe_artventa == 2 && !hechos.Contains(w.id)).Take(1).FirstOrDefault();

                            orderLines.Where(w => w.id_tran_ofe == subItem1.id).Select(a =>
                            {
                                a._ofe_id = subItem1.ofe_id;
                                a._ofe_maxpares = subItem1.max_pares;
                                a._ofe_porc = subItem1.ofe_porc;
                                a._ofe_Tipo = subItem1.ofe_tipo;
                                a._ofe_PrecioPack = subItem1.ofe_artventa;
                                return a;
                            }).ToList();
                            listOfertas.Where(w => w.id == subItem1.id).Select(a =>
                            {
                                a.hecho = "x";
                                return a;
                            }).ToList();
                            hechos.Add(subItem1.id);

                            orderLines.Where(w => w.id_tran_ofe == subItem2.id).Select(a =>
                            {
                                a._ofe_id = subItem2.ofe_id;
                                a._ofe_maxpares = subItem2.max_pares;
                                a._ofe_porc = subItem2.ofe_porc;
                                a._ofe_Tipo = subItem2.ofe_tipo;
                                a._ofe_PrecioPack = subItem2.ofe_artventa;
                                return a;
                            }).ToList();
                            listOfertas.Where(w => w.id == subItem2.id).Select(a =>
                            {
                                a.hecho = "x";
                                return a;
                            }).ToList();
                            hechos.Add(subItem2.id);
                        }
                    }
                    else
                    {
                        continue;
                    }
                }
                else if ((new String[] { "C" }).Contains(ofe_tipo))
                {
                    int gru1 = listOfertas.Count(c => c.ofe_id == ofe_id && c.hecho == "" && c.ofe_porc == 1 && !hechos.Contains(c.id));
                    int gru2 = listOfertas.Count(c => c.ofe_id == ofe_id && c.hecho == "" && c.ofe_porc == 2 && !hechos.Contains(c.id));
                    max_ofe = (new int[] { gru1, gru2 }).Min();
                    if (max_ofe >= 1)
                    {
                        for (int i = 0; i < max_ofe; i++)
                        {

                            Tran_Ofertas subItem1 = listOfertas.Where(w => w.hecho == "" && w.ofe_id == ofe_id && w.ofe_porc == 1 && !hechos.Contains(w.id)).Take(1).FirstOrDefault();
                            Tran_Ofertas subItem2 = listOfertas.Where(w => w.hecho == "" && w.ofe_id == ofe_id && w.ofe_porc == 2 && !hechos.Contains(w.id)).Take(1).FirstOrDefault();

                            orderLines.Where(w => w.id_tran_ofe == subItem1.id).Select(a =>
                            {
                                a._ofe_id = subItem1.ofe_id;
                                a._ofe_maxpares = subItem1.max_pares;
                                a._ofe_porc = subItem1.ofe_porc;
                                a._ofe_Tipo = subItem1.ofe_tipo;
                                a._ofe_PrecioPack = subItem1.ofe_artventa;
                                return a;
                            }).ToList();
                            listOfertas.Where(w => w.id == subItem1.id).Select(a =>
                            {
                                a.hecho = "x";
                                return a;
                            }).ToList();
                            hechos.Add(subItem1.id);

                            orderLines.Where(w => w.id_tran_ofe == subItem2.id).Select(a =>
                            {
                                a._ofe_id = subItem2.ofe_id;
                                a._ofe_maxpares = subItem2.max_pares;
                                a._ofe_porc = subItem2.ofe_porc;
                                a._ofe_Tipo = subItem2.ofe_tipo;
                                a._ofe_PrecioPack = subItem2.ofe_artventa;
                                return a;
                            }).ToList();
                            listOfertas.Where(w => w.id == subItem2.id).Select(a =>
                            {
                                a.hecho = "x";
                                return a;
                            }).ToList();
                            hechos.Add(subItem2.id);
                        }
                    }
                    else
                    {
                        continue;
                    }
                }
                else
                {
                    foreach (Tran_Ofertas subItem in listOfertas.Where(w => w.hecho == "" && w.ofe_id == ofe_id && !hechos.Contains(w.id)))
                    {
                        int _id_linea = subItem.id;
                        orderLines.Where(w => w.id_tran_ofe == _id_linea).Select(a =>
                        {
                            a._ofe_id = subItem.ofe_id;
                            a._ofe_maxpares = subItem.max_pares;
                            a._ofe_porc = subItem.ofe_porc;
                            a._ofe_Tipo = subItem.ofe_tipo;
                            a._ofe_PrecioPack = subItem.ofe_artventa;
                            //a.prom_menor = (a._qty > 1 ? (listOfertas)  : false);
                            return a;
                        }).ToList();
                        listOfertas.Where(w => w.id == _id_linea).Select(a =>
                        {
                            a.hecho = "x";
                            return a;
                        }).ToList();
                    }
                }
            }

            return orderLines;
            /* promociones */
        }
        public void _addArticle( Ent_Order_Dtl newLine, int qty, string varTipoPago , bool editar = false)//, string tipopago)
        {
            decimal commPercent;
            //decimal ofertporcentaje;
            //decimal ofertamaxpares;
            Ent_Persona cust = (Ent_Persona)Session[_session_customer];
            cust._vartipopago = varTipoPago;
            if (varTipoPago == "004")
            {
                commPercent = cust._commission / 100;
            }
            else
            {
                commPercent = cust._commission / 100;
            }


            if (newLine._ap_percepcion == "0")
            {
                commPercent = 0;
            }


            //este quiere decir que tiene oferta
            //if (newLine._ofe_id!=0)
            //{
            //    ofertporcentaje = newLine._ofe_porc;
            //    ofertamaxpares = newLine._ofe_maxpares;
            //}

            //decimal commPercent = (cust._commission / 100);
            int newQty = 0;

            // Lista de pedido
            List<Ent_Order_Dtl> orderLines = (List<Ent_Order_Dtl>)(((object)Session[_session_list_detalle_pedido]) != null ? (object)Session[_session_list_detalle_pedido] : new List<Ent_Order_Dtl>()); ;


            //verificamos que no exista un premio en la lista
            string codArt = newLine._code;
            string tallArt = newLine._size;

            Ent_Order_Dtl resultLinePrem = null;// orderLines.Where(x => x._code.Equals(codArt) && x._premio.Equals("S")).FirstOrDefault();

            //fin de verificacion


            Ent_Order_Dtl resultLine;
            if (resultLinePrem == null)
            {


                if (orderLines != null)
                    resultLine = orderLines.Where(x => x._code.Equals(newLine._code) && x._size.Equals(newLine._size) && x._premio.Equals(newLine._premio)).FirstOrDefault();
                else
                    resultLine = new Ent_Order_Dtl();

                if (resultLine != null)
                {
                    if (orderLines.Remove(resultLine))
                    {
                        if (editar)
                        {
                            if (qty > 0)
                            qty = qty - resultLine._qty;
                        }
                        newQty = resultLine._qty + qty;
                        resultLine._qty = newQty;
                        resultLine._commission = Math.Round((((resultLine._price * newQty) /*- (resultLine._dscto * newQty)*/) * commPercent) * resultLine._comm, 2, MidpointRounding.AwayFromZero);
                        resultLine._commissionPctg = commPercent;
                        //resultLine._commissionDesc = resultLine._commission.ToString(_currency);
                        int num = 1;

                        //  resultLine._dsctoDesc = newLine._commission.ToString(_currency);
                        //resultLine._commissionigv = Math.Round((((resultLine._priceigv * newQty) - (resultLine._dscto * newQty)) * commPercent) * resultLine._comm, 2, MidpointRounding.AwayFromZero);
                        //resultLine._commissionigvDesc = resultLine._commissionigv.ToString(_currency);
                        resultLine._lineTotal = Math.Round((resultLine._price * newQty) - (resultLine._dscto * newQty) - resultLine._commission, 2, MidpointRounding.AwayFromZero);
                        //resultLine._lineTotDesc = (num * ((resultLine._price * newQty) - (resultLine._dscto * newQty) - resultLine._commission)).ToString(_currency);
                        // resultLine._lineTotDesc = ((resultLine._priceigv * newQty) - (resultLine._dscto * newQty) - resultLine._commissionigv).ToString(_currency);
                        resultLine._lineTotDesc = resultLine._lineTotal;
                        if (varTipoPago == "008" || varTipoPago == "OPG")
                        {
                            resultLine._lineTotal = (resultLine._price * newQty);
                            resultLine._commissionPctg = 0m;
                            //resultLine._commissionDesc = (0).ToString(_currency);
                            resultLine._commission = 0;
                            resultLine._dscto = 0;
                            //resultLine._lineTotal = 0m;
                            resultLine._lineTotDesc = (0);

                            //resultLine._priceigvDesc = (0).ToString(_currency);

                        }
                        orderLines.Add(resultLine);
                    }
                }
                else
                {
                    newLine._qty = qty;
                    newLine._commission = Math.Round((((newLine._price * qty) - (newLine._dscto * qty)) * commPercent) * newLine._comm, 2, MidpointRounding.AwayFromZero);
                  //  newLine._commissionDesc = newLine._commission.ToString(_currency);
                    //newLine._commissionigv = Math.Round((((newLine._priceigv * qty) - (newLine._dscto * qty)) * commPercent) * newLine._comm, 2, MidpointRounding.AwayFromZero);
                    //newLine._commissionigvDesc = newLine._commissionigv.ToString(_currency);
                    newLine._commissionPctg = commPercent;

                    //newLine._dscto =Math.Round (((newLine._price * qty) - newLine._commission) * ((newLine._ofe_porc / 100)));

                    //newLine._dsctoDesc = (((newLine._price * qty) - newLine._commission)*((newLine._ofe_porc/100))).ToString(_currency);
                    int num2 = 1;

                    newLine._lineTotal = Math.Round((newLine._price * qty) - (newLine._dscto * qty) - newLine._commission, 2, MidpointRounding.AwayFromZero);
                    //newLine._lineTotDesc = (num2 * ((newLine._price * qty) - (newLine._dscto * qty) - newLine._commission)).ToString(_currency);
                    //newLine._lineTotDesc = ((newLine._priceigv * qty) - (newLine._dscto * qty) - newLine._commissionigv).ToString(_currency);
                    newLine._lineTotDesc = newLine._lineTotal;
                    if (varTipoPago == "008" || varTipoPago == "OPG")
                    {
                        newLine._lineTotal = (newLine._price * qty);
                        newLine._commissionPctg = 0m;
                      //  newLine._commissionDesc = (0).ToString(_currency);
                        newLine._commission = 0;
                        newLine._dscto = 0;
                        //newLine._lineTotal = 0m;
                        newLine._lineTotDesc = 0;
                        //newLine._priceigvDesc = (0).ToString(_currency);

                    }
                    orderLines.Add(newLine);
                }

            }

            Session[_session_list_detalle_pedido] = orderLines;
        }
        public void loadOrderDtl(ref List<Ent_Order_Dtl> order, Ent_Order_Dtl newLine, decimal custComm)
        {
            //
            Ent_Order_Dtl resultLine = order.Where(x => x._code.Equals(newLine._code) && x._size.Equals(newLine._size)).FirstOrDefault();
            //
            decimal commPercent = (custComm / 100);

            //if (fvalidaartcatalogo())
            //{
            //    commPercent = 0;
            //}

            //recalcular el descuento de catalogo 
            if (!(newLine == null))
            {
                if (newLine._ap_percepcion == "0")
                {
                    commPercent = 0;
                }
            }
            //***************************


            if (resultLine != null)
            {
                if (order.Remove(resultLine))
                {
                    int newQty = resultLine._qty + newLine._qty;
                    resultLine._qty = newQty;
                    resultLine._commission = Math.Round((((resultLine._price * newQty)) * commPercent) * resultLine._comm, 2, MidpointRounding.AwayFromZero);
                    //resultLine._commissionigv = (((resultLine._priceigv * newQty) - (resultLine._dscto * newQty)) * commPercent) * resultLine._comm;
                    resultLine._commissionPctg = commPercent;
                    //resultLine._commissionDesc = resultLine._commission.ToString(_currency);
                    //resultLine._commissionigvDesc = resultLine._commissionigv.ToString(_currency);
                    //resultLine._lineTotal =Math.Round ((resultLine._price * newQty) - (resultLine._dscto * newQty) - resultLine._commission,2,MidpointRounding.AwayFromZero);

                    resultLine._lineTotal = Math.Round((resultLine._price * newQty) - (resultLine._dscto + resultLine._commission), 2, MidpointRounding.AwayFromZero);

                    //resultLine._lineTotDesc = ((resultLine._price * newQty) - (resultLine._dscto * newQty) - resultLine._commission).ToString(_currency);

                    //resultLine._lineTotDesc = resultLine._lineTotal.ToString(_currency);
                    //resultLine._lineTotDesc = ((resultLine._priceigv * newQty) - (resultLine._dscto * newQty) - resultLine._commissionigv).ToString(_currency);
                    resultLine._lineTotDesc = resultLine._lineTotal;

                    order.Add(resultLine);
                }
            }
            else
            {
                newLine._commission = Math.Round((((newLine._price * newLine._qty)) * commPercent) * newLine._comm,2,MidpointRounding.AwayFromZero);
                // newLine._commissionigv = (((newLine._priceigv * newLine._qty) - (newLine._dscto * newLine._qty)) * commPercent) * newLine._comm;
                //newLine._commissionDesc = newLine._commission.ToString(_currency);
                //newLine._commissionigvDesc = newLine._commissionigv.ToString(_currency);
                newLine._commissionPctg = commPercent;
                //newLine._lineTotal =Math.Round( (newLine._price * newLine._qty) - (newLine._dscto * newLine._qty) - newLine._commission,2,MidpointRounding.AwayFromZero);

                newLine._lineTotal = Math.Round((newLine._price * newLine._qty) - (newLine._dscto + newLine._commission), 2, MidpointRounding.AwayFromZero);

                //newLine._lineTotDesc = newLine._lineTotal.ToString(_currency);

                //newLine._lineTotDesc = ((newLine._price * newLine._qty) - (newLine._dscto * newLine._qty) - newLine._commission).ToString(_currency);
                //newLine._lineTotDesc = ((newLine._priceigv * newLine._qty) - (newLine._dscto * newLine._qty) - newLine._commissionigv).ToString(_currency);
                newLine._lineTotDesc = newLine._lineTotDesc;
                order.Add(newLine);
            }
        }
        public Ent_Order_Hdr getTotals(string vnc= "")
        {
            Ent_Order_Hdr orderHdr;


            try
            {
                // Lista de pedido
                List<Ent_Order_Dtl> order = (List<Ent_Order_Dtl>)(((object)Session[_session_list_detalle_pedido]) != null ? (object)Session[_session_list_detalle_pedido] : new List<Ent_Order_Dtl>());

                if (order != null)
                {
                    Ent_Persona cust = (Ent_Persona)Session[_session_customer];
                    decimal taxRate = (cust._taxRate / 100);
                    int totalQty = order.Sum(q => q._qty);
                    decimal subTotal = Math.Round(order.Sum(x => x._lineTotal), 2, MidpointRounding.AwayFromZero);
                    decimal subTotalDesc = subTotal;
                    decimal taxes = Math.Round((order.Sum(x => x._lineTotal)) * taxRate, 2, MidpointRounding.AwayFromZero);
                    decimal taxesDesc = taxes;

                    if (Session[_session_notas_persona] == null)
                    {
                        List<Ent_Pago_NCredito> _listNC = new List<Ent_Pago_NCredito>();
                        Session[_session_notas_persona] = _listNC;
                    }
                    List<Ent_Pago_NCredito> listNC = new List<Ent_Pago_NCredito>();
                    listNC = (List<Ent_Pago_NCredito>)Session[_session_notas_persona];

                    decimal mtoncredito = listNC.Where(w => w.Consumido ==true).Sum(s=>s.Importe);//  0;//Convert.ToDecimal(vnc);
                    //string mtoncreditodesc = mtoncredito.ToString(_currency);

                    //Session[_valor_subtotal] = subTotal + taxes;

                    decimal grandTotal = (subTotal + taxes) - mtoncredito;

                    //si el paso es mayor a la venta
                    if (grandTotal < 0)
                    {
                        grandTotal = 0;
                    }
                    //

                    //string grandTotalDesc = grandTotal.ToString(_currency);

                   //// cust._mtoimporte = (subTotal + taxes);


                    Boolean aplicap = true;

                    //verificar si estos articulos tiene percepcion 0
                    for (Int32 i = 0; i < order.Count; ++i)
                    {
                        string vaplicap = order[i]._ap_percepcion;
                        if (vaplicap == "0")
                        {
                            aplicap = false;
                            break;
                        }
                    }

                    decimal Percepcionrate = (aplicap) ? cust._percepcion / 100 : 0;

                    //decimal Percepcionrate = (cust._percepcion / 100);

                    decimal percepcion = Math.Round(grandTotal * Percepcionrate, 2, MidpointRounding.AwayFromZero);



                    //string percepciondesc = percepcion.ToString(_currency);

                    decimal mtopercepcion = grandTotal + percepcion;
                    //string mtopercepciondesc = mtopercepcion.ToString(_currency);



                    //variable de percepcion*********
                    //cust._mtopercepcion = percepcion;


                    ////Session[_opcional_percepcion] = percepcion;
                    //*******************************

                    decimal _totalOPG = 0.00m;
                    if (cust._vartipopago == "008" || cust._vartipopago == "OPG")
                    {
                        _totalOPG = grandTotal;
                        subTotalDesc = (0);
                        //grandTotalDesc = (0).ToString(_currency);
                        grandTotal = 0m;
                        percepcion = 0m;
                        //percepciondesc = (0).ToString(_currency);
                        mtopercepcion = 0m;
                        //mtopercepciondesc = (0).ToString(_currency);
                        mtoncredito = 0m;
                        //mtoncreditodesc = (0).ToString(_currency);
                        taxesDesc = (0);
                    }


                    orderHdr = new Ent_Order_Hdr
                    {
                        _qtys = totalQty,
                        _subTotalOPG = _totalOPG,
                        _taxes = taxes,
                        _subTotal = subTotal,
                        _subTotalDesc = subTotalDesc,
                        //_grandTotalDesc = grandTotalDesc,
                        _taxesDesc = taxesDesc,
                        _grandTotal = grandTotal,
                        _percepcion = percepcion,
                        //_percepciondesc = percepciondesc,
                        _mtopercepcion = mtopercepcion,
                        //_mtopercepciondesc = mtopercepciondesc,
                        _mtoncredito = mtoncredito,
                        //_mtoncreditodesc = mtoncreditodesc,
                    };
                }
                else
                    orderHdr = new Ent_Order_Hdr();
            }
            catch
            {
                orderHdr = new Ent_Order_Hdr();
            }

            return orderHdr;

        }        
        /** Lista de detalle de pedidos **/
        public ActionResult eliminarFilePedido(string articulo , string size)
        {
            try
            {
                if (Session[_session_list_detalle_pedido] == null)
                {
                    List<Ent_Order_Dtl> listPed = new List<Ent_Order_Dtl>();
                    Session[_session_list_detalle_pedido] = listPed;
                }
                List<Ent_Order_Dtl> _listActual = (List<Ent_Order_Dtl>)Session[_session_list_detalle_pedido];
                foreach (Ent_Order_Dtl item in _listActual.Where(w => w._code == articulo && size.Split(',').Contains(w._size)).ToList())
                {
                    _listActual.Remove(_listActual.Where(w => w._code == articulo && size.Split(',').Contains(w._size)).FirstOrDefault());
                }               
                return Json(new { estado = 0});
            }
            catch (Exception ex)
            {
                return Json(new { estado = 1, mensaje = "Error al eliminar el articulo.\n" + ex.Message });
            }                     
        }
        public ActionResult getListaDetPedido(Ent_jQueryDataTableParams param)
        {
            Ent_Order_Hdr header = new Ent_Order_Hdr();
            /*verificar si esta null*/
            if (Session[_session_list_detalle_pedido] == null)
            {
                List<Ent_Order_Dtl> listPed = new List<Ent_Order_Dtl>();
                Session[_session_list_detalle_pedido] = listPed;
            } else {

                Boolean inicial_editar =Convert.ToBoolean(Session[_carga_inicial_editar]);

                if (!inicial_editar) fupdateitemoferta(); /*para cargar solo los articulos que se guardaron para editar */
                if (inicial_editar) Session[_carga_inicial_editar] = false;

                header = getTotals();
            }
            //Traer registros

            List<Ent_Order_Dtl> pedidoCompleto = (List<Ent_Order_Dtl>)Session[_session_list_detalle_pedido];
            /*
                a._code,
                a._artName,
                a._brand,
                a._color,
                a._size,
                a._qty,
                a._price,
                a._commission,
                a._det_dcto_sigv,
                a._lineTotal,
                a._dscto,
            */

            List<Ent_Order_Dtl> VistaLista =(from fila in pedidoCompleto
                             group fila by new
                             {
                                 _code = fila._code,
                                 _artName = fila._artName,
                                 _brand = fila._brand,
                                 _color = fila._color,
                                 _uriPhoto = fila._uriPhoto,
                             }
                             into g
                             select new Ent_Order_Dtl
                             {
                                 _code = g.Key._code,
                                 _artName = g.Key._artName,
                                 _brand = g.Key._brand,
                                 _color = g.Key._color,
                                 _tallas = pedidoCompleto.Where(w => w._code == g.Key._code).Select(s => s._size).ToArray(),
                                 _qtys = pedidoCompleto.Where(w => w._code == g.Key._code).Select(s => Convert.ToDecimal(s._qty)).ToArray(),
                                 _price = g.Average(a => a._price),
                                 _commission = g.Sum(s => s._commission),
                                 _lineTotal = g.Sum(s => s._lineTotal),
                                 _dscto = g.Sum(s => s._dscto),
                                 _lineTotDesc = g.Sum(s => s._lineTotDesc),
                                 _uriPhoto = g.Key._uriPhoto
                             }).ToList();
            //IQueryable <Ent_Order_Dtl> membercol = ((List<Ent_Order_Dtl>)(Session[_session_list_detalle_pedido])).AsQueryable();  //lista().AsQueryable();

            //List<Ent_Order_Dtl> _vislis = VistaLista.ToList<Ent_Order_Dtl>();

             IQueryable<Ent_Order_Dtl> membercol = VistaLista.AsQueryable();  //lista().AsQueryable();

            //Manejador de filtros
            int totalCount = membercol.Count();
            IEnumerable<Ent_Order_Dtl> filteredMembers = membercol;

            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredMembers = membercol
                    .Where(m => m._code.ToUpper().Contains(param.sSearch.ToUpper()) ||
                     m._artName.ToUpper().Contains(param.sSearch.ToUpper()));
            }
            //Manejador de orden
            var sortIdx = Convert.ToInt32(Request["iSortCol_0"]);

            if (param.iSortingCols > 0)
            {
                if (Request["sSortDir_0"].ToString() == "asc")
                {
                    switch (sortIdx)
                    {
                        //case 0: filteredMembers = filteredMembers.OrderBy(o => Convert.ToDecimal(o.liq_Id)); break;
                        //case 1: filteredMembers = filteredMembers.OrderBy(o => o.ventaId); break;
                        //case 2: filteredMembers = filteredMembers.OrderBy(o => Convert.ToDateTime(o.liq_Fecha)); break;
                        //case 3: filteredMembers = filteredMembers.OrderBy(o => o.Pares); break;
                        //case 4: filteredMembers = filteredMembers.OrderBy(o => o.Estado); break;
                        //case 5: filteredMembers = filteredMembers.OrderBy(o => o.Ganancia); break;
                        //case 6: filteredMembers = filteredMembers.OrderBy(o => o.Subtotal); break;
                        //case 7: filteredMembers = filteredMembers.OrderBy(o => o.N_C); break;
                        //case 8: filteredMembers = filteredMembers.OrderBy(o => o.Total); break;
                        //case 9: filteredMembers = filteredMembers.OrderBy(o => o.Percepcion); break;
                        //case 10: filteredMembers = filteredMembers.OrderBy(o => o.TotalPagar); break;
                    }
                }
                else
                {
                    switch (sortIdx)
                    {
                        //case 0: filteredMembers = filteredMembers.OrderByDescending(o => Convert.ToDecimal(o.liq_Id)); break;
                        //case 1: filteredMembers = filteredMembers.OrderByDescending(o => o.ventaId); break;
                        //case 2: filteredMembers = filteredMembers.OrderByDescending(o => Convert.ToDateTime(o.liq_Fecha)); break;
                        //case 3: filteredMembers = filteredMembers.OrderByDescending(o => o.Pares); break;
                        //case 4: filteredMembers = filteredMembers.OrderByDescending(o => o.Estado); break;
                        //case 5: filteredMembers = filteredMembers.OrderByDescending(o => o.Ganancia); break;
                        //case 6: filteredMembers = filteredMembers.OrderByDescending(o => o.Subtotal); break;
                        //case 7: filteredMembers = filteredMembers.OrderByDescending(o => o.N_C); break;
                        //case 8: filteredMembers = filteredMembers.OrderByDescending(o => o.Total); break;
                        //case 9: filteredMembers = filteredMembers.OrderByDescending(o => o.Percepcion); break;
                        //case 10: filteredMembers = filteredMembers.OrderByDescending(o => o.TotalPagar); break;
                    }
                }
            }
            var displayMembers = filteredMembers
                .Skip(param.iDisplayStart)
                .Take(param.iDisplayLength);
            var result = from a in displayMembers
                         select new
                         {
                             a._code,
                             a._artName,
                             a._brand,
                             a._color,
                             a._size,
                             a._qty,
                             a._price,
                             a._commission,
                             a._det_dcto_sigv,
                             a._lineTotal,
                             a._dscto,
                             a._tallas , 
                             a._qtys,
                             a._lineTotDesc,
                             a._uriPhoto
                         };
            //Se devuelven los resultados por json
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = totalCount,
                iTotalDisplayRecords = filteredMembers.Count(),
                aaData = result,
                header = header
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult getPedidoNC(Ent_jQueryDataTableParams param)
        {
            /*verificar si esta null*/
            if (Session[_session_notas_persona] == null)
            {
                List<Ent_Pago_NCredito> listPed = new List<Ent_Pago_NCredito>();
                Session[_session_notas_persona] = listPed;
            }
            //Traer registros

            IQueryable <Ent_Pago_NCredito> membercol = ((List<Ent_Pago_NCredito>)(Session[_session_notas_persona])).AsQueryable();  //lista().AsQueryable();

            //Manejador de filtros
            int totalCount = membercol.Count();
            IEnumerable<Ent_Pago_NCredito> filteredMembers = membercol;

           
            var displayMembers = filteredMembers
                .Skip(param.iDisplayStart)
                .Take(param.iDisplayLength);
            var result = from a in displayMembers
                         select new
                         {
                             a.Consumido,
                             a.Activado,
                             a.Ncredito,
                             a.Importe,
                             a.Rhv_return_nro,
                             a.Fecha_documento
                         };
            //Se devuelven los resultados por json
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = totalCount,
                iTotalDisplayRecords = filteredMembers.Count(),
                aaData = result,
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult AnularLiquidacion(string liq, string cliente)
        {
            Ent_Usuario _usuario = (Ent_Usuario)Session[Ent_Constantes.NameSessionUser];
            string mensaje = "";
            string mensaje2 = "";
            bool ret = false;           
            ret = datPedido.AnularLiquidacion(liq, _usuario.usu_id, ref mensaje);
            Ent_Info_Promotor infoGeneralPedidos = datPedido.ListarPedidos(Convert.ToDecimal(cliente), ref mensaje2);
            Session[_session_listPedido_private] = infoGeneralPedidos.liquidacion;
            return Json(new { estado = ret ? 0 : 1 , mensaje = mensaje });
        }
        public ActionResult MarcarNC(string[] ncs)
        {
            List<Ent_Pago_NCredito> listNC = new List<Ent_Pago_NCredito>();
            listNC = (List<Ent_Pago_NCredito>)Session[_session_notas_persona];
            listNC.Select(s => { s.Consumido = false; return s; }).ToList();

            Int32 estado = 1;
            string mensaje = "Ninguna nota de credito aplicada";

            if (ncs != null) {
                mensaje = "Nota de credito aplicada.";
                estado = 1;
                listNC.Where(w => ncs.Contains(w.Ncredito)).Select(s => { s.Consumido = true; return s; }).ToList();
            }


            return Json(new { estado = estado, mensaje= mensaje });
        }

        public ActionResult get_valida_pedido()
        {
            string prom = "0";
            try
            {
                List<Ent_Order_Dtl> info = null;
                if (Session[_session_list_detalle_pedido] == null)
                {
                    info = new List<Ent_Order_Dtl>();
                }
                else
                {
                    info = (List<Ent_Order_Dtl>)Session[_session_list_detalle_pedido];
                }

                if (info.Count==0) return Json(new { estado = 0, info = info });


                var validapercep = info.GroupBy(g => g._ap_percepcion).ToList();

                if (validapercep.Count>=2) return Json(new { estado = 0, info = info });

                /*validacion de oferta*/
                DataTable dt = new DataTable();
                dt.Columns.Add("cod_artic", typeof(string));
                dt.Columns.Add("precio", typeof(decimal));
                dt.Columns.Add("cantidad", typeof(decimal));
                string valida_descuento = "0";

                Int32 i = 1;
                foreach (Ent_Order_Dtl item in info)
                {
                    if (item._dscto < 0) valida_descuento = "1";

                    dt.Rows.Add(item._code, item._price, item._qty);
                    i++;
                }

                if (valida_descuento=="1") return Json(new { estado = 0, info = info, valida_descuento = valida_descuento });

                if (dt.Rows.Count>0)
                {
                    Dat_Pedido dat_prom = new Dat_Pedido();
                    Boolean val_promo = dat_prom._return_valida_promo_exists(dt);
                    if (val_promo) return Json(new { estado = 0, info = info, prom = prom });
                }
                Ent_Persona cust = (Ent_Persona)Session[_session_customer];

                if (cust.Bas_id=="0") return Json(new { estado = 0, info = info, prom = prom, user = cust.Bas_id.ToString() });

                string articulo = "";
                string talla = "";
                Ent_Order_Stk_Disponible stk = new Ent_Order_Stk_Disponible();
                if (!(fvalidastock(ref articulo, ref talla)))
                {
                   
                    stk.disponible = "1";
                    stk.articulo = articulo;
                    stk.talla = talla;

                    return Json(new { estado = 0, info = info, prom = prom, user = cust.Bas_id.ToString(), stk= stk });

                }
                else
                {
                    stk.disponible = "0";
                }



                return Json(new { estado = 0, info = info, prom = prom,user=cust.Bas_id.ToString(), stk = stk });
            }
            catch (Exception ex)
            {
                return Json(new { estado = 2, mensaje = ex.Message });
            }
        }

        private Boolean fvalidastock(ref string articulo, ref string talla)
        {

            Dat_Pedido dat_ped = new Dat_Pedido();
            Ent_Liquidacion liq = (Ent_Liquidacion)Session[_session_lnfo_liquidacion];

            Boolean valida = true;
            string estadoliquid = liq.estId;//  (string)Session[_estadoliqui];
            string nroliq = liq.liq_Id; // (string)Session[_nSOrderUrl];

            List<Ent_Order_Dtl> order = (List<Ent_Order_Dtl>)Session[_session_list_detalle_pedido];

            //string.IsNullOrEmpty(liq.liq_Id)

            foreach (Ent_Order_Dtl item in order)
            {

                //Int32 vcantidad = dat_ped.fvalidastock(item._code, item._size, item._qty, (!(string.IsNullOrEmpty(estadoliquid))) ? nroliq : "");
                Int32 vcantidad = dat_ped.fvalidastock(item._code, item._size, item._qty, (!(string.IsNullOrEmpty(liq.liq_Id))) ? nroliq : "");
                if (vcantidad == 0)
                {
                    articulo = item._code;
                    talla = item._size;
                    valida = false;
                    break;
                }

            }
            return valida;
        }

        // [HttpPost]        
        //public ActionResult get_detalle_pedido()
        //{
        //    List<Ent_Order_Dtl> listar = null;
        //    if (Session[_session_list_detalle_pedido]==null)
        //    {
        //        listar = new List<Ent_Order_Dtl>();
        //    }
        //    else
        //    {
        //        listar = (List<Ent_Order_Dtl>) Session[_session_list_detalle_pedido];
        //    }
        //    //return View(listar);
        //    // return listar;
        //    return Json(new
        //    {
        //        sEcho = param.sEcho,
        //        iTotalRecords = totalCount,
        //        iTotalDisplayRecords = filteredMembers.Count(),
        //        aaData = result
        //    }, JsonRequestBehavior.AllowGet);
        //}
        /// <summary>
        /// Buscar pedios segun su estado
        /// </summary>
        /// <returns></returns>
        public ActionResult BuscarPedido()
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
                Session[_session_ListarPedidoEstado] = null;
                return View();
            }
        }
        /// <summary>
        /// getListarPedidoEstado lista de pedidos estaods
        /// </summary>
        /// <param name="param"></param>
        /// <param name="Liq_Id"></param>
        /// <param name="isOkUpdate"></param>
        /// <returns></returns>
        public JsonResult getListarPedidoEstado(Ent_jQueryDataTableParams param, string Liq_Id, bool isOkUpdate)
        {
            Ent_Buscar_Pedido Ent_Buscar_Pedido = new Ent_Buscar_Pedido();

            if (isOkUpdate)
            {
                Ent_Buscar_Pedido.Liq_Id = Liq_Id;
                Session[_session_ListarPedidoEstado] = datPedido.ListarPedidoEstado(Ent_Buscar_Pedido).ToList(); ;
            }

            /*verificar si esta null*/
            if (Session[_session_ListarPedidoEstado] == null)
            {
                List<Ent_Buscar_Pedido> Lista_Buscar_Pedido = new List<Ent_Buscar_Pedido>();
                Session[_session_ListarPedidoEstado] = Lista_Buscar_Pedido;
            }

            IQueryable<Ent_Buscar_Pedido> entDocTrans = ((List<Ent_Buscar_Pedido>)(Session[_session_ListarPedidoEstado])).AsQueryable();

            //Manejador de filtros
            int totalCount = entDocTrans.Count();
            IEnumerable<Ent_Buscar_Pedido> filteredMembers = entDocTrans;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredMembers = entDocTrans
                    .Where(m =>
                        m.lider.ToUpper().Contains(param.sSearch.ToUpper())
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
                        case 0: filteredMembers = filteredMembers.OrderBy(o => o.lider); break;
                    }
                }
                else
                {
                    switch (sortIdx)
                    {
                        case 0: filteredMembers = filteredMembers.OrderByDescending(o => o.lider); break;
                    }
                }
            }

            var Result = filteredMembers
                .Skip(param.iDisplayStart)
                .Take(param.iDisplayLength);

            //Se devuelven los resultados por json
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = totalCount,
                iTotalDisplayRecords = filteredMembers.Count(),
                aaData = Result
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult MarcacionPedido()
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
                    List<Ent_Persona> ListarUsuario = new List<Ent_Persona>();
                    ListarUsuario.Add(new Ent_Persona() { Codigo = -1, Descripcion = "-- Selecione --" });
                    Ent_Persona _EntU = new Ent_Persona();
                    _EntU.Usu_Tip_ID = "07";
                    ViewBag.ListarUsuario = ListarUsuario.Concat(datPersona.Listar_Usuario_tipo(_EntU));

                    return View();
                }
                else
                {
                    return RedirectToAction("Error_Roles", "Home");
                }

                
            }
        }
        /// <summary>
        /// Lista de pedios con marcion y sin marcacion, y sus totales
        /// </summary>
        /// <param name="param"></param>
        /// <param name="isOkUpdate"></param>
        /// <returns></returns>
        public JsonResult getListarPicking(Ent_jQueryDataTableParams param, bool isOkUpdate)
        {
            int NroMarcado = 0;
            int NroNoMarcado = 0;
            int UniMarcado = 0;
            int UniNoMarcado = 0;
            int totCantidad = 0;

            if (isOkUpdate)
            {
                Session[_session_ListarPicking] = datPedido.ListarPicking().ToList();                
            }
            /*verificar si esta null*/
            if (Session[_session_ListarPicking] == null)
            {
                List<Ent_Picking> _Ent_ListarPicking = new List<Ent_Picking>();
                Session[_session_ListarPicking] = _Ent_ListarPicking;
            }

            IQueryable<Ent_Picking> entDocTrans = ((List<Ent_Picking>)(Session[_session_ListarPicking])).AsQueryable();            

            if (entDocTrans.Count() > 0)
            {
                NroMarcado = entDocTrans.Count(a => a.Pin_Employee != -1);
                UniMarcado = entDocTrans.Where(a => a.Pin_Employee != -1).Sum(a => a.Cantidad);
                NroNoMarcado = entDocTrans.Count(a => a.Pin_Employee == -1);
                UniNoMarcado = entDocTrans.Where(a => a.Pin_Employee == -1).Sum(a => a.Cantidad);
                totCantidad = entDocTrans.Sum(a => a.Cantidad);
            }           

            //Manejador de filtros
            int totalCount = entDocTrans.Count();
            IEnumerable<Ent_Picking> filteredMembers = entDocTrans;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredMembers = entDocTrans
                    .Where(m =>
                        m.Liq_Id.ToUpper().Contains(param.sSearch.ToUpper())
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
                        case 0: filteredMembers = filteredMembers.OrderBy(o => o.Liq_Id); break;
                    }
                }
                else
                {
                    switch (sortIdx)
                    {
                        case 0: filteredMembers = filteredMembers.OrderByDescending(o => o.Liq_Id); break;
                    }
                }
            }

            var Result = filteredMembers
                .Skip(param.iDisplayStart)
                .Take(param.iDisplayLength);

            //Se devuelven los resultados por json
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = totalCount,
                iTotalDisplayRecords = filteredMembers.Count(),
                aaData = Result,
                iNroMarcado = NroMarcado,
                iNroNoMarcado = NroNoMarcado,
                iUniMarcado = UniMarcado,
                iUniNoMarcado = UniNoMarcado,
                itotCantidad = totCantidad
            }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Muesta la informacion del pedido y control de tiempo.
        /// </summary>
        /// <param name="Liq_Id"></param>
        /// <returns></returns>
        public ActionResult ajaxGetInfoPicking(string Liq_Id)
        {
            JsonResponse objResult = new JsonResponse();
            Ent_Picking_info _EntData = new Ent_Picking_info();
            Ent_Picking_info _EntResult = new Ent_Picking_info();

            _EntData.Liq_Id = Liq_Id;
            try
            {
                _EntData = datPedido.ListarPickingInfo(_EntData);
                _EntResult.Liq_Id = _EntData.Liq_Id;
                _EntResult.Datedesc = _EntData.Datedesc;
                _EntResult.Datedesclear = _EntData.Datedesclear;
                _EntResult.Nameemployee = _EntData.Nameemployee;
                _EntResult.Pick_Startdesc = _EntData.Pick_Startdesc;
                TimeSpan timeSpan = DateTime.Now - Convert.ToDateTime(_EntData.Pick_Start);
                _EntResult.Pick_Start = _EntData.Pick_Start;
                _EntResult.TiempoCorrido = (timeSpan.Days > 0 ? (string)(" " + timeSpan.Days + " Dias - ") : (string)(" " + timeSpan.Hours + " Horas - " + timeSpan.Minutes + " Min - " + timeSpan.Seconds + " seg."));
                _EntResult.Noliq = _EntData.Noliq;
                _EntResult.Ldn_Qty = _EntData.Ldn_Qty;
                objResult.Data = _EntResult;
                if (objResult.Data == null)
                {
                    objResult.Success = false;
                    objResult.Message = "No hay resultados";
                }else
                {
                    objResult.Success = true;
                }                
            }
            catch (Exception exc)
            {
                objResult.Success = false;
                objResult.Message = "Error al anular";
            }
            var JSON = JsonConvert.SerializeObject(objResult);
            return Json(JSON, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Inicia la marcacion del pedido
        /// </summary>
        /// <param name="Liq_Id"></param>
        /// <param name="Pin_Employee"></param>
        /// <returns></returns>
        public ActionResult startPicking(string Liq_Id,int Pin_Employee)
        {
            bool Result = false;
            JsonResponse objResult = new JsonResponse();
            Ent_Picking _Ent = new Ent_Picking();
            _Ent.Liq_Id = Liq_Id;
            _Ent.Pin_Employee = Pin_Employee;
            try
            {
                Result = datPedido.startPicking(_Ent);
                if (Result)
                {
                    objResult.Success = true;
                    objResult.Message = "Se ha iniciado la marcación para la liquidación No." + Liq_Id;
                }
                else
                {
                    objResult.Success = false;
                    objResult.Message = "Error realizando el iniciado de marcación liq No." + Liq_Id;
                }
            }
            catch (Exception ex)
            {
                objResult.Success = false;
                objResult.Message = "Error realizando el iniciado de marcación liq No." + Liq_Id;
            }

            var JSON = JsonConvert.SerializeObject(objResult);

            return Json(JSON, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Finaliza la marcacion del pedido.
        /// </summary>
        /// <param name="Liq_Id"></param>
        /// <returns></returns>
        public ActionResult endPicking(string Liq_Id)
        {
            bool Result = false;
            JsonResponse objResult = new JsonResponse();
            Ent_Picking _Ent = new Ent_Picking();
            _Ent.Liq_Id = Liq_Id;
            try
            {
                Result = datPedido.endPicking(_Ent);
                if (Result)
                {
                    objResult.Success = true;
                    objResult.Message = "Se ha finalizado la marcación para la liquidación No." + Liq_Id;
                }
                else
                {
                    objResult.Success = false;
                    objResult.Message = "Error realizando la finalización de marcación liq No." + Liq_Id;
                }
            }
            catch (Exception ex)
            {
                objResult.Success = false;
                objResult.Message = "Error realizando la finalización de marcación liq No." + Liq_Id;
            }

            var JSON = JsonConvert.SerializeObject(objResult);

            return Json(JSON, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Descarga archivo segun su tipo, Excel o PDF.
        /// </summary>
        /// <param name="Liq_Id"></param>
        /// <param name="Empleado"></param>
        /// <param name="typeFile"></param>
        /// <returns></returns>
        public ActionResult verPedido(string Liq_Id, string Empleado,string typeFile)
        {
            bool Result = false;
            JsonResponse objResult = new JsonResponse();
            try
            {
                Result = GetArchivo(Liq_Id, Empleado, typeFile);
                if (Result)
                {
                    objResult.Success = true;
                    objResult.Message = "Se generó correctamente el archivo";
                }
                else
                {
                    objResult.Success = false;
                    objResult.Message = "Error al generar el archivo";
                }
            }
            catch (Exception ex)
            {
                objResult.Success = false;
                objResult.Message = "Error al generar el archivo";
            }

            var JSON = JsonConvert.SerializeObject(objResult);

            return Json(JSON, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Metodo que genera los archivos
        /// </summary>
        /// <param name="Liq_Id"></param>
        /// <param name="Empleado"></param>
        /// <param name="typeFile"></param>
        /// <returns></returns>
        public bool GetArchivo(string Liq_Id,string Empleado, string typeFile)
        {
            bool resul = false;
            Data_Cr_Aquarella datCrAq = new Data_Cr_Aquarella();

            try
            {
                DataSet dsLiqInfo = datCrAq.getLiquidationHdrInfo(Liq_Id);
                List<Picking> _liqValsReport = new List<Picking>();
                if (dsLiqInfo == null || dsLiqInfo.Tables[0].Rows.Count == 0)
                    return resul = false;

                DataSet dsPickDtl = datCrAq.getDtlPicking(Liq_Id);

                if (dsPickDtl == null || dsPickDtl.Tables[0].Rows.Count == 0)
                    return resul = false;

                DataRow dRow = dsLiqInfo.Tables[0].Rows[0];

                foreach (DataRow dRowDtl in dsPickDtl.Tables[0].Rows)
                {
                    Picking objPickReport = new Picking("", dRow["almacen"].ToString(),
                        dRow["alm_direccion"].ToString(), dRow["Alm_Telefono"].ToString(), "", dRow["Bas_Id"].ToString(),
                        dRow["Bas_Documento"].ToString(), dRow["nombres"].ToString(), dRow["Bas_Direccion"].ToString(), dRow["Bas_Telefono"].ToString(),
                        dRow["Bas_Celular"].ToString(), dRow["Bas_Correo"].ToString(), dRow["ubicacion"].ToString(), dRow["Liq_Id"].ToString(),
                        dRow["estado"].ToString(), dRowDtl["tdv_article"].ToString(), dRowDtl["brv_description"].ToString(),
                        string.Empty, dRowDtl["arv_name"].ToString(), dRowDtl["tdv_size"].ToString(), Convert.ToDecimal(dRowDtl["tdn_qty"]), dRowDtl["stv_descriptions"].ToString(),
                        dRowDtl["po"].ToString(), Empleado, dRowDtl["instrucciones"].ToString(), dRow["lider"].ToString());

                    _liqValsReport.Add(objPickReport);
                }

                this.HttpContext.Session["ReportName"] = "pickingReport.rpt";
                this.HttpContext.Session["rptSource"] = _liqValsReport;
                this.HttpContext.Session["Liq_Id"] = Liq_Id;
                this.HttpContext.Session["typeFile"] = typeFile;
                resul = true;
            }
            catch (Exception)
            {
                resul = false;
            }
            return resul;
        }
        /// <summary>
        /// Genera archivo en grupo
        /// </summary>
        /// <returns></returns>
        public ActionResult verPedidoGrupo()
        {
            bool Result = false;
            JsonResponse objResult = new JsonResponse();
            try
            {
                Result = PopulateValueCrystalReportI();
                if (Result)
                {
                    objResult.Success = true;
                    objResult.Message = "Se generó correctamente el archivo";
                }
                else
                {
                    objResult.Success = false;
                    objResult.Message = "Error al generar el archivo";
                }
            }
            catch (Exception ex)
            {
                objResult.Success = false;
                objResult.Message = "Error al generar el archivo :" + ex.Message;
            }

            var JSON = JsonConvert.SerializeObject(objResult);

            return Json(JSON, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Descarga pdf, por grupos de pedidos.
        /// </summary>
        /// <returns></returns>
        public bool PopulateValueCrystalReportI()
        {
            bool result = false;
            try
            {
                List<Pedido_Lider_Grupo> _ventaValues = new List<Pedido_Lider_Grupo>();
                DataTable dtventa = datPedido.get_pedido_lidergrupo();

                if (dtventa.Rows.Count > 0)
                {                 

                    foreach (DataRow dataRow in (InternalDataCollectionBase)dtventa.Rows)
                    {
                        string lider = dataRow["lider"].ToString();
                        string lider_documento = dataRow["lider_documento"].ToString();
                        string promotor = dataRow["promotor"].ToString();
                        string promotor_doc = dataRow["promotor_doc"].ToString();
                        string liq_id = dataRow["liq_id"].ToString();
                        Decimal cantidad = Convert.ToDecimal(dataRow["cantidad"].ToString());
                        string lider_direccion = dataRow["lider_direccion"].ToString();
                        _ventaValues.Add(new Pedido_Lider_Grupo(lider, lider_documento, promotor, promotor_doc, liq_id, cantidad, lider_direccion));
                    }

                    this.HttpContext.Session["rptSourceGrp"] = _ventaValues;
                    this.HttpContext.Session["ReportNameGrp"] = "liquidacion_grupo_consulta.rpt";
                    result = true;
                }
                else
                {
                    result = false;
                }                
            }
            catch (Exception)
            {
                result = false;
            }
            return result;
        }
        /// <summary>
        /// Anular Pedido marcados.
        /// </summary>
        /// <returns></returns>
        public ActionResult Anular_Pedido_Marcacion()
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
                    return View();
                }
                else
                {
                    return RedirectToAction("Error_Roles", "Home");
                }

                
            }
        }

        /// <summary>
        /// Lista de pedios con marcion y sin marcacion, y sus totales
        /// </summary>
        /// <param name="param"></param>
        /// <param name="isOkUpdate"></param>
        /// <returns></returns>
        public JsonResult getListarAnularPicking(Ent_jQueryDataTableParams param, bool isOkUpdate)
        {
            int NroMarcado = 0;
            int NroNoMarcado = 0;
            int UniMarcado = 0;
            int UniNoMarcado = 0;
            int totCantidad = 0;
            int totCantPedido = 0;
            if (isOkUpdate)
            {
                Session[_session_ListarPicking] = datPedido.ListarAnularPicking().ToList();
            }
            /*verificar si esta null*/
            if (Session[_session_ListarPicking] == null)
            {
                List<Ent_Picking> _Ent_ListarPicking = new List<Ent_Picking>();
                Session[_session_ListarPicking] = _Ent_ListarPicking;
            }

            IQueryable<Ent_Picking> entDocTrans = ((List<Ent_Picking>)(Session[_session_ListarPicking])).AsQueryable();

            if (entDocTrans.Count() > 0)
            {
                NroMarcado = entDocTrans.Count(a => a.Pin_Employee != -1);
                UniMarcado = entDocTrans.Where(a => a.Pin_Employee != -1).Sum(a => a.Cantidad);
                NroNoMarcado = entDocTrans.Count(a => a.Pin_Employee == -1);
                UniNoMarcado = entDocTrans.Where(a => a.Pin_Employee == -1).Sum(a => a.Cantidad);
                totCantidad = entDocTrans.Sum(a => a.Cantidad);
                totCantPedido = entDocTrans.Count();
            }

            //Manejador de filtros
            int totalCount = entDocTrans.Count();
            IEnumerable<Ent_Picking> filteredMembers = entDocTrans;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredMembers = entDocTrans
                    .Where(m =>
                        m.Liq_Id.ToUpper().Contains(param.sSearch.ToUpper())
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
                        case 0: filteredMembers = filteredMembers.OrderBy(o => o.Liq_Id); break;
                    }
                }
                else
                {
                    switch (sortIdx)
                    {
                        case 0: filteredMembers = filteredMembers.OrderByDescending(o => o.Liq_Id); break;
                    }
                }
            }

            var Result = filteredMembers
                .Skip(param.iDisplayStart)
                .Take(param.iDisplayLength);

            //Se devuelven los resultados por json
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = totalCount,
                iTotalDisplayRecords = filteredMembers.Count(),
                aaData = Result,
                iNroMarcado = NroMarcado,
                iNroNoMarcado = NroNoMarcado,
                iUniMarcado = UniMarcado,
                iUniNoMarcado = UniNoMarcado,
                itotCantidad = totCantidad,
                itotCantPedido = totCantPedido
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Anular la marcacion del pedido
        /// </summary>
        /// <param name="Liq_Id"></param>
        /// <returns></returns>
        public ActionResult AnularPicking(string Liq_Id)
        {
            bool Result = false;
            Ent_Usuario _usuario = (Ent_Usuario)Session[Ent_Constantes.NameSessionUser];
            JsonResponse objResult = new JsonResponse();
            Ent_Picking _Ent = new Ent_Picking();
            _Ent.Liq_Id = Liq_Id;
            _Ent.Usu_Id = _usuario.usu_id;
            try
            {
                Result = datPedido.AnularPicking(_Ent);
                //Result = true;
                if (Result)
                {
                    objResult.Success = true;
                    objResult.Message = "Se Anulo la marcación para la liquidación No." + Liq_Id;
                }
                else
                {
                    objResult.Success = false;
                    objResult.Message = "Error al anular el pedido  No." + Liq_Id;
                }
            }
            catch (Exception ex)
            {
                objResult.Success = false;
                objResult.Message = "Error al anular el pedido  No." + Liq_Id;
            }

            var JSON = JsonConvert.SerializeObject(objResult);

            return Json(JSON, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Vista de liquidacion vs despacho
        /// </summary>
        /// <returns></returns>
        public ActionResult LiquidacionVsDespacho()
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
                    Ent_Pedido_Despacho _Ent_Pedido_Despacho = new Ent_Pedido_Despacho();
                    ViewBag._Ent_Pedido_Despacho = _Ent_Pedido_Despacho;
                    return View();
                }
                else
                {
                    return RedirectToAction("Error_Roles", "Home");
                }

              
            }
        }

        /// <summary>
        /// Lista de liquidacion vs despacho
        /// </summary>
        /// <param name="param"></param>
        /// <param name="FechaInicio"></param>
        /// <param name="FechaFin"></param>
        /// <param name="isOkUpdate"></param>
        /// <param name="isOkchkPSD"></param>
        /// <returns></returns>
        public JsonResult getListaPedDespAjax(Ent_jQueryDataTableParams param, string FechaInicio, string FechaFin, bool isOkUpdate, bool isOkchkPSD)
        {
            Ent_Pedido_Despacho Ent_Pedido_Despacho = new Ent_Pedido_Despacho();
            int totCantParReal = 0, totCantParDes = 0, totCantParSD = 0;
            if (isOkUpdate)
            {
                Ent_Pedido_Despacho.FechaInicio = DateTime.Parse(FechaInicio);
                Ent_Pedido_Despacho.FechaFin = DateTime.Parse(FechaFin);
                Session[_session_ListarPedidoDespacho] = (isOkchkPSD == true ? datPedido.ListarPedidoDespacho(Ent_Pedido_Despacho).Where(x => x.Saldo > 0).ToList() : datPedido.ListarPedidoDespacho(Ent_Pedido_Despacho).ToList());
            }

            /*verificar si esta null*/
            if (Session[_session_ListarPedidoDespacho] == null)
            {
                List<Ent_Pedido_Despacho> _ListarPedidoDespacho = new List<Ent_Pedido_Despacho>();
                Session[_session_ListarPedidoDespacho] = _ListarPedidoDespacho;
            }

            IQueryable<Ent_Pedido_Despacho> entDocTrans = ((List<Ent_Pedido_Despacho>)(Session[_session_ListarPedidoDespacho])).AsQueryable();

            if (entDocTrans.Count() > 0)
            {
                totCantParReal = entDocTrans.Sum(a => a.PedOriginal);
                totCantParDes = entDocTrans.Sum(a => a.Pedi_Despachado);
                totCantParSD = entDocTrans.Sum(a => a.Saldo);
            }
            //Manejador de filtros
            int totalCount = entDocTrans.Count();
            IEnumerable<Ent_Pedido_Despacho> filteredMembers = entDocTrans;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredMembers = entDocTrans.Where(
                        m =>
                         m.Liq.ToUpper().Contains(param.sSearch.ToUpper()) ||
                         m.Ven_Id.ToUpper().Contains(param.sSearch.ToUpper()) ||
                         m.Articulo.ToUpper().Contains(param.sSearch.ToUpper())
                );
            }
            var sortIdx = Convert.ToInt32(Request["iSortCol_0"]);

            if (param.iSortingCols > 0)
            {
                if (Request["sSortDir_0"].ToString() == "asc")
                {
                    switch (sortIdx)
                    {
                        case 0: filteredMembers = filteredMembers.OrderBy(o => o.Liq); break;
                        case 1: filteredMembers = filteredMembers.OrderBy(o => o.Ven_Id); break;
                        case 2: filteredMembers = filteredMembers.OrderBy(o => o.Articulo); break;
                    }
                }
                else
                {
                    switch (sortIdx)
                    {
                        case 0: filteredMembers = filteredMembers.OrderByDescending(o => o.Liq); break;
                        case 1: filteredMembers = filteredMembers.OrderByDescending(o => o.Ven_Id); break;
                        case 2: filteredMembers = filteredMembers.OrderByDescending(o => o.Articulo); break;
                    }
                }
            }

            var Result = filteredMembers
                .Skip(param.iDisplayStart)
                .Take(param.iDisplayLength);

            //Se devuelven los resultados por json
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = totalCount,
                iTotalDisplayRecords = filteredMembers.Count(),
                aaData = Result,
                itotCantParReal = totCantParReal,
                itotCantParDes =totCantParDes,
                itotCantParSD = totCantParSD,
            }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Se arma el reporte en excel
        /// </summary>
        /// <param name="_Ent"></param>
        /// <returns></returns>
        public ActionResult get_exporta_ListaPedDesp_excel(Ent_Pedido_Despacho _Ent)
        {
            JsonResponse objResult = new JsonResponse();
            try
            {
                Session[_session_ListarPedidoDespacho_Excel] = null;
                string cadena = "";
                if (Session[_session_ListarPedidoDespacho] != null)
                {

                    List<Ent_Pedido_Despacho> _ListarPedidoDespacho = (List<Ent_Pedido_Despacho>)Session[_session_ListarPedidoDespacho];
                    if (_ListarPedidoDespacho.Count == 0)
                    {
                        objResult.Success = false;
                        objResult.Message = "No hay filas para exportar";
                        
                    }
                    else
                    {
                        cadena = get_html_ListarPedidoDespacho_str((List<Ent_Pedido_Despacho>)Session[_session_ListarPedidoDespacho], _Ent);
                        if (cadena.Length == 0)
                        {
                            objResult.Success = false;
                            objResult.Message = "Error del formato html";
                        }
                        else
                        {
                            objResult.Success = true;
                            objResult.Message = "Se genero el excel correctamente";
                            Session[_session_ListarPedidoDespacho_Excel] = cadena;
                        }
                    }
                }
                else
                {
                    objResult.Success = false;
                    objResult.Message = "No hay filas para exportar";
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
        /// <summary>
        /// Formato excel
        /// </summary>
        /// <param name="_ListarPedidoDespacho"></param>
        /// <param name="_Ent"></param>
        /// <returns></returns>
        public string get_html_ListarPedidoDespacho_str(List<Ent_Pedido_Despacho> _ListarPedidoDespacho, Ent_Pedido_Despacho _Ent)
        {
            StringBuilder sb = new StringBuilder();
            int totCantParReal = 0, totCantParDes = 0, totCantParSD = 0;
            try
            {
                var Lista = _ListarPedidoDespacho.ToList();
                totCantParReal = Lista.Sum(a => a.PedOriginal);
                totCantParDes = Lista.Sum(a => a.Pedi_Despachado);
                totCantParSD = Lista.Sum(a => a.Saldo);

                sb.Append("<div><table cellspacing='0' style='width: 1000px' rules='all' border='0' style='border-collapse:collapse;'><tr><td Colspan='8'></td></tr><tr><td Colspan='8' valign='middle' align='center' style='vertical-align: middle;font-size: 16.0pt;font-weight: bold;color:#285A8F'>REPORTE DE LIQUIDACION VS DESPACHO</td></tr><tr><td Colspan='8'  valign='middle' align='center' style='font-size: 10.0pt;font-weight: bold;vertical-align: middle'>Desde el " + String.Format("{0:dd/MM/yyyy}", _Ent.FechaInicio) + " hasta el " + String.Format("{0:dd/MM/yyyy}", _Ent.FechaFin) + "</td></tr></table>");

                sb.Append("<table  border='1' bgColor='#ffffff' borderColor='#FFFFFF' cellSpacing='2' cellPadding='2' style='font-size:10.0pt; font-family:Calibri; background:white;width: 1000px'><tr  bgColor='#5799bf'>\n");
                sb.Append("<tr bgColor='#1E77AB'>\n");
                sb.Append("<th style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Nro. Pedido</font></th>\n");
                sb.Append("<th style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Nro. Boleta</font></th>\n");
                sb.Append("<th style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Asesor</font></th>\n");
                sb.Append("<th style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Lider</font></th>\n");
                sb.Append("<th style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Promotor</font></th>\n");
                sb.Append("<th style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Fec. Venta</font></th>\n");
                sb.Append("<th style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Articulo</font></th>\n");
                sb.Append("<th style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Talla</font></th>\n");
                sb.Append("<th style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Pares Real</font></th>\n");
                sb.Append("<th style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Pares Desp.</font></th>\n");
                sb.Append("<th style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Pares S/D</font></th>\n");
                sb.Append("</tr>\n");

                foreach (var item in Lista)
                {
                    sb.Append("<tr>\n");
                    sb.Append("<td align='center'>" + item.Liq + "</td>\n");
                    sb.Append("<td align='center'>" + item.Ven_Id + "</td>\n");
                    sb.Append("<td align='center'>" + item.Asesor + "</td>\n");
                    sb.Append("<td align='center'>" + item.Lider + "</td>\n");
                    sb.Append("<td align='center'>" + item.Promotor + "</td>\n");
                    sb.Append("<td >" + item.Fecha + "</td>\n");
                    sb.Append("<td align='center'>" + item.Articulo + "</td>\n");
                    sb.Append("<td align='center'>" + item.Talla + "</td>\n");
                    sb.Append("<td align='center'>" + item.PedOriginal + "</td>\n");
                    sb.Append("<td align='center'>" + item.Pedi_Despachado + "</td>\n");
                    sb.Append("<td align='center'>" + item.Saldo + "</td>\n");
                    sb.Append("</tr>\n");
                }

                sb.Append("<tfoot>\n");
                sb.Append("<tr bgcolor='#085B8C'>\n");
                sb.Append("<td colspan='7'></td>\n");
                sb.Append("<td style='text-align:left;font-weight:bold;font-size:11.0pt; '><font color='#FFFFFF'>Totales</font></td>\n");
                sb.Append("<td style='text-align:center;font-weight: bold;font-size:11.0pt; '><font color='#FFFFFF'>" + String.Format("{0:n0}", totCantParReal)  + "</font></td>\n");
                sb.Append("<td style='text-align:center;font-weight: bold;font-size:11.0pt; '><font color='#FFFFFF'>" + String.Format("{0:n0}", totCantParDes)  + "</font></td>\n");
                sb.Append("<td style='text-align:center;font-weight: bold;font-size:11.0pt; '><font color='#FFFFFF'>" + String.Format("{0:n0}", totCantParSD) + "</font></td>\n");
                sb.Append("</tr>\n");
                sb.Append("</tfoot>\n");
                sb.Append("</table></div>");
            }
            catch
            {

            }
            return sb.ToString();
        }

        /// <summary>
        /// Exportamos el excel
        /// </summary>
        /// <returns></returns>
        public ActionResult ListaListaPedDespExcel()
        {
            string NombreArchivo = "liquidespacho";
            String style = style = @"<style> .textmode { mso-number-format:\@; } </script> ";
            try
            {
                Response.Clear();
                Response.Buffer = true;
                Response.ContentType = "application/vnd.ms-excel";
                Response.AddHeader("Content-Disposition", "attachment;filename=" + NombreArchivo + ".xls");
                Response.Charset = "UTF-8";
                Response.ContentEncoding = Encoding.Default;
                Response.Write(style);
                Response.Write(Session[_session_ListarPedidoDespacho_Excel].ToString());
                Response.End();
            }
            catch
            {

            }
            return Json(new { estado = 0, mensaje = 1 });
        }

        public ActionResult PedidoFacturacion()
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
        public JsonResult getListarPedidoFacturacionAjax(Ent_jQueryDataTableParams param, bool isOkUpdate,bool isOkEstado, string ddlEstado)
        {
            Ent_Pedido_Facturacion Ent_PedidoFacturacion = new Ent_Pedido_Facturacion();
            int totalpares = 0, Paq_Cantidad = 0;
            Decimal liq_value = 0;
            Session[_session_ListarPedidoFacturacion_Excel] = null;
            object ListaEstado = "";
            if (isOkUpdate)
            {
                Ent_Usuario _usuario = (Ent_Usuario)Session[Ent_Constantes.NameSessionUser];

                //if (_usuario.usu_tip_id=="01")
                //{ }

                Ent_PedidoFacturacion.Are_Id = (_usuario.usu_tip_id == "01")? _usuario.usu_area:"%%";               
                Session[_session_ListarPedidoFacturacion] =  datPedido.ListarPedidoFacturacion(Ent_PedidoFacturacion).ToList();
            }

            /*verificar si esta null*/
            if (Session[_session_ListarPedidoFacturacion] == null)
            {
                List<Ent_Pedido_Facturacion> _ListarPedidoFacturacion = new List<Ent_Pedido_Facturacion>();
                Session[_session_ListarPedidoFacturacion] = _ListarPedidoFacturacion;
            }

            IQueryable<Ent_Pedido_Facturacion> entDocTransEs = ((List<Ent_Pedido_Facturacion>)(Session[_session_ListarPedidoFacturacion])).AsQueryable();
            
            var entDocTrans = (isOkEstado == true ? entDocTransEs.Where(x => x.Liq_Estid == ddlEstado).ToList() : entDocTransEs.ToList());
            Session[_session_ListarPedidoFacturacion_Excel] = entDocTrans;
            //Manejador de filtros
            int totalCount = entDocTrans.Count();
            IEnumerable<Ent_Pedido_Facturacion> filteredMembers = entDocTrans;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredMembers = entDocTrans.Where(
                        m =>
                         m.Liq_Id.ToUpper().Contains(param.sSearch.ToUpper()) ||
                         m.Asesor.ToUpper().Contains(param.sSearch.ToUpper()) ||
                         m.Nombres.ToUpper().Contains(param.sSearch.ToUpper()) ||
                         m.Ubicacion.ToUpper().Contains(param.sSearch.ToUpper()) ||
                         m.Are_Descripcion.ToUpper().Contains(param.sSearch.ToUpper()) ||
                         m.Liq_Estid.ToUpper().Contains(param.sSearch.ToUpper())
                );
            }
            var sortIdx = Convert.ToInt32(Request["iSortCol_0"]);

            if (param.iSortingCols > 0)
            {
                if (Request["sSortDir_0"].ToString() == "asc")
                {
                    switch (sortIdx)
                    {
                        case 0: filteredMembers = filteredMembers.OrderBy(o => o.Asesor); break;
                        case 1: filteredMembers = filteredMembers.OrderBy(o => o.Are_Descripcion); break;
                        case 2: filteredMembers = filteredMembers.OrderBy(o => o.Nombres); break;
                        case 3: filteredMembers = filteredMembers.OrderBy(o => o.Dni_Promotor); break;
                        case 4: filteredMembers = filteredMembers.OrderBy(o => o.Ubicacion); break;
                        case 5: filteredMembers = filteredMembers.OrderBy(o => o.Liq_Id); break;
                        case 6: filteredMembers = filteredMembers.OrderBy(o => o.Liq_Estid); break;
                        case 7: filteredMembers = filteredMembers.OrderBy(o => o.Liq_Fecha); break;
                        case 8: filteredMembers = filteredMembers.OrderBy(o => o.Liq_Fecha_Expiracion); break;
                        case 9: filteredMembers = filteredMembers.OrderBy(o => o.Fecha_Grupo); break;
                        case 10: filteredMembers = filteredMembers.OrderBy(o => o.Totalpares); break;
                        case 11: filteredMembers = filteredMembers.OrderBy(o => o.Paq_Cantidad); break;
                        case 12: filteredMembers = filteredMembers.OrderBy(o => o.Liq_Value); break;
                    }
                }
                else
                {
                    switch (sortIdx)
                    {
                        case  0 : filteredMembers = filteredMembers.OrderByDescending(o => o.Asesor); break;
                        case  1 : filteredMembers = filteredMembers.OrderByDescending(o => o.Are_Descripcion); break;
                        case  2 : filteredMembers = filteredMembers.OrderByDescending(o => o.Nombres); break;
                        case  3 : filteredMembers = filteredMembers.OrderByDescending(o => o.Dni_Promotor); break;
                        case  4 : filteredMembers = filteredMembers.OrderByDescending(o => o.Ubicacion); break;
                        case  5 : filteredMembers = filteredMembers.OrderByDescending(o => o.Liq_Id); break;
                        case  6 : filteredMembers = filteredMembers.OrderByDescending(o => o.Liq_Estid); break;
                        case  7 : filteredMembers = filteredMembers.OrderByDescending(o => o.Liq_Fecha); break;
                        case  8 : filteredMembers = filteredMembers.OrderByDescending(o => o.Liq_Fecha_Expiracion); break;
                        case  9 : filteredMembers = filteredMembers.OrderByDescending(o => o.Fecha_Grupo); break;
                        case  10 : filteredMembers = filteredMembers.OrderByDescending(o => o.Totalpares); break;
                        case  11 : filteredMembers = filteredMembers.OrderByDescending(o => o.Paq_Cantidad); break;
                        case  12 : filteredMembers = filteredMembers.OrderByDescending(o => o.Liq_Value); break;
                    }
                }
            }
          if (entDocTrans.Count() > 0)
            {
                totalpares = filteredMembers.Sum(a => a.Totalpares);
                Paq_Cantidad = filteredMembers.Sum(a => a.Paq_Cantidad);
                liq_value = filteredMembers.Sum(a => a.Liq_Value);
                ListaEstado = entDocTransEs.Select(x => new { Codigo = x.Liq_Estid, Descripcion = x.Liq_Estid }).Distinct().ToList();
            }
            var Result = filteredMembers
                .Skip(param.iDisplayStart)
                .Take(param.iDisplayLength);

            //Se devuelven los resultados por json
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = totalCount,
                iTotalDisplayRecords = filteredMembers.Count(),
                aaData = Result,
                itotalpares = totalpares,
                iPaq_Cantidad = Paq_Cantidad,
                iliq_value = liq_value,
                lListaEstado = ListaEstado
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Se arma el reporte en excel
        /// </summary>
        /// <param name="_Ent"></param>
        /// <returns></returns>
        public ActionResult get_exporta_ListaPedFac_excel(Ent_Pedido_Facturacion _Ent)
        {
            JsonResponse objResult = new JsonResponse();
            try
            {
               //Session[_session_ListarPedidoFactura_Excel] = null;
                string cadena = "";
                if (Session[_session_ListarPedidoFacturacion_Excel] != null)
                {

                    List<Ent_Pedido_Facturacion> _ListarPedidoFacturacion = (List<Ent_Pedido_Facturacion>)Session[_session_ListarPedidoFacturacion_Excel];
                    if (_ListarPedidoFacturacion.Count == 0)
                    {
                        objResult.Success = false;
                        objResult.Message = "No hay filas para exportar";

                    }
                    else
                    {
                        cadena = get_html_ListarPedidoFacturacion_str((List<Ent_Pedido_Facturacion>)Session[_session_ListarPedidoFacturacion_Excel], _Ent);
                        if (cadena.Length == 0)
                        {
                            objResult.Success = false;
                            objResult.Message = "Error del formato html";
                        }
                        else
                        {
                            objResult.Success = true;
                            objResult.Message = "Se genero el excel correctamente";
                            Session[_session_ListarPedidoFacturacion_Excel] = cadena;
                        }
                    }
                }
                else
                {
                    objResult.Success = false;
                    objResult.Message = "No hay filas para exportar";
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

        /// <summary>
        /// Formato excel
        /// </summary>
        /// <param name="_ListarPedidoDespacho"></param>
        /// <param name="_Ent"></param>
        /// <returns></returns>
        public string get_html_ListarPedidoFacturacion_str(List<Ent_Pedido_Facturacion> _ListarPedidoDespacho, Ent_Pedido_Facturacion _Ent)
        {
            StringBuilder sb = new StringBuilder();
            int totalpares = 0, Paq_Cantidad = 0;
            Decimal liq_value = 0;
            try
            {
                var Lista = _ListarPedidoDespacho.ToList();
                totalpares = Lista.Sum(a => a.Totalpares);
                Paq_Cantidad = Lista.Sum(a => a.Paq_Cantidad);
                liq_value = Lista.Sum(a => a.Liq_Value);

                sb.Append("<div><table cellspacing='0' style='width: 1000px' rules='all' border='0' style='border-collapse:collapse;'><tr><td Colspan='13'></td></tr><tr><td Colspan='13' valign='middle' align='center' style='vertical-align: middle;font-size: 16.0pt;font-weight: bold;color:#285A8F'>LIQUIDACION ACTIVAS EN BODEGAS</td></tr></table>");
                sb.Append("<table  border='1' bgColor='#ffffff' borderColor='#FFFFFF' cellSpacing='2' cellPadding='2' style='font-size:10.0pt; font-family:Calibri; background:white;width: 1000px'><tr  bgColor='#5799bf'>\n");
                sb.Append("<tr bgColor='#1E77AB'>\n");
                sb.Append("<th style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Asesor</font></th>\n");
                sb.Append("<th style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Área</font></th>\n");
                sb.Append("<th style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Cliente</font></th>\n");
                sb.Append("<th style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Doc. CLiente</font></th>\n");
                sb.Append("<th style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Ubicación</font></th>\n");
                sb.Append("<th style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Nro. Liq</font></th>\n");
                sb.Append("<th style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Estado</font></th>\n");
                sb.Append("<th style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Fecha</font></th>\n");
                sb.Append("<th style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Expiración</font></th>\n");
                sb.Append("<th style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Liberación</font></th>\n");
                sb.Append("<th style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Unds</font></th>\n");
                sb.Append("<th style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Uds. Emp</font></th>\n");
                sb.Append("<th style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Valor</font></th>\n");
                sb.Append("</tr>\n");

                foreach (var item in Lista)
                {
                    sb.Append("<tr>\n");
                    sb.Append("<td>" + item.Asesor + "</td>\n");
                    sb.Append("<td>" + item.Are_Descripcion + "</td>\n");
                    sb.Append("<td >" + item.Nombres + "</td>\n");
                    sb.Append("<td align='center'>" + item.Dni_Promotor + "</td>\n");
                    sb.Append("<td>" + item.Ubicacion + "</td>\n");
                    sb.Append("<td align='center'>" + item.Liq_Id + "</td>\n");
                    sb.Append("<td align='center'>" + item.Liq_Estid + "</td>\n");
                    sb.Append("<td>" + item.Liq_Fecha + "</td>\n");
                    sb.Append("<td>" + item.Liq_Fecha_Expiracion + "</td>\n");
                    sb.Append("<td>" + item.Fecha_Grupo + "</td>\n");
                    sb.Append("<td align='Right'>" + item.Totalpares + "</td>\n");
                    sb.Append("<td align='Right'>" + item.Paq_Cantidad + "</td>\n");
                    sb.Append("<td align='Right'>" + String.Format("{0:n}", item.Liq_Value) + "</td>\n");
                    sb.Append("</tr>\n");
                }

                sb.Append("<tfoot>\n");
                sb.Append("<tr bgcolor='#085B8C'>\n");
                sb.Append("<td colspan='9'></td>\n");
                sb.Append("<td style='text-align:left;font-weight:bold;font-size:11.0pt; '><font color='#FFFFFF'>Totales</font></td>\n");
                sb.Append("<td style='text-align:Right;font-weight: bold;font-size:11.0pt; '><font color='#FFFFFF'>" + String.Format("{0:n0}", totalpares) + "</font></td>\n");
                sb.Append("<td style='text-align:Right;font-weight: bold;font-size:11.0pt; '><font color='#FFFFFF'>" + String.Format("{0:n0}", Paq_Cantidad) + "</font></td>\n");
                sb.Append("<td style='text-align:Right;font-weight: bold;font-size:11.0pt; '><font color='#FFFFFF'>" + String.Format("{0:n}", liq_value) + "</font></td>\n");
                sb.Append("</tr>\n");
                sb.Append("</tfoot>\n");
                sb.Append("</table></div>");
            }
            catch
            {

            }
            return sb.ToString();
        }

        /// <summary>
        /// Exportamos el excel
        /// </summary>
        /// <returns></returns>
        public ActionResult ListaPedFacExcel()
        {
            string NombreArchivo = "Info_Liq_" + DateTime.Today.ToShortDateString() ;
            String style = style = @"<style> .textmode { mso-number-format:\@; } </script> ";
            try
            {
                Response.Clear();
                Response.Buffer = true;
                Response.ContentType = "application/vnd.ms-excel";
                Response.AddHeader("Content-Disposition", "attachment;filename=" + NombreArchivo + ".xls");
                Response.Charset = "UTF-8";
                Response.ContentEncoding = Encoding.Default;
                Response.Write(style);
                Response.Write(Session[_session_ListarPedidoFacturacion_Excel].ToString());
                Response.End();
            }
            catch
            {

            }
            return Json(new { estado = 0, mensaje = 1 });
        }
        #region <CONSULTA DE PEDIDOS>
        /// <summary>
        /// CONSULTAR PEDIDO POR MEDIO DE DNI O RUC
        /// </summary>
        /// <returns></returns>
        public ActionResult ConsultarPedido()
        {
            Session[_session_ListarConsultarPedido] = null;
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
                    return View();
                }
                else
                {
                    return RedirectToAction("Error_Roles", "Home");
                }
 
            }
        }
        /// <summary>
        /// Lista de consulta de pedidos por DNI o RUC
        /// </summary>
        /// <param name="param"></param>
        /// <param name="isOkUpdate"></param>
        /// <param name="isOkEstado"></param>
        /// <param name="ddlEstado"></param>
        /// <returns></returns>
        public JsonResult getListarConsultaPedido(Ent_jQueryDataTableParams param, bool isOkUpdate, string Bas_Documento)
        {
            Ent_Consultar_Pedido Ent_ConsultarPedido = new Ent_Consultar_Pedido();

            if (isOkUpdate)
            {
                Ent_ConsultarPedido.Bas_Documento = Bas_Documento;
                Session[_session_ListarConsultarPedido] = datPedido.ListarConsultarPedido(Ent_ConsultarPedido).ToList();
            }

            /*verificar si esta null*/
            if (Session[_session_ListarConsultarPedido] == null)
            {
                List<Ent_Consultar_Pedido> _ListarConsultarPedido = new List<Ent_Consultar_Pedido>();
                Session[_session_ListarConsultarPedido] = _ListarConsultarPedido;
            }

            IQueryable<Ent_Consultar_Pedido> entDocTrans = ((List<Ent_Consultar_Pedido>)(Session[_session_ListarConsultarPedido])).AsQueryable();

            //Manejador de filtros
            int totalCount = entDocTrans.Count();
            IEnumerable<Ent_Consultar_Pedido> filteredMembers = entDocTrans;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredMembers = entDocTrans.Where(
                        m =>
                         m.NroDNI.ToUpper().Contains(param.sSearch.ToUpper()) ||
                         m.Cliente.ToUpper().Contains(param.sSearch.ToUpper()) ||
                         m.NroPedido.ToUpper().Contains(param.sSearch.ToUpper()) ||
                         m.Estado.ToUpper().Contains(param.sSearch.ToUpper()) ||
                         m.NroDoc.ToUpper().Contains(param.sSearch.ToUpper()) ||
                         m.NroNC.ToUpper().Contains(param.sSearch.ToUpper()) ||
                         m.NroLiquidacion.ToUpper().Contains(param.sSearch.ToUpper())
                );
            }
            var sortIdx = Convert.ToInt32(Request["iSortCol_0"]);

            if (param.iSortingCols > 0)
            {
                if (Request["sSortDir_0"].ToString() == "asc")
                {
                    switch (sortIdx)
                    {
                        case 0: filteredMembers = filteredMembers.OrderBy(o => o.NroDNI); break;
                    }
                }
                else
                {
                    switch (sortIdx)
                    {
                        case 0: filteredMembers = filteredMembers.OrderByDescending(o => o.NroDNI); break;
                    }
                }
            }

            var Result = filteredMembers
                .Skip(param.iDisplayStart)
                .Take(param.iDisplayLength);

            //Se devuelven los resultados por json
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = totalCount,
                iTotalDisplayRecords = filteredMembers.Count(),
                aaData = Result
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Se arma el reporte en excel de Lista de consulta de pedidos por DNI o RUC
        /// </summary>
        /// <param name="_Ent"></param>
        /// <returns></returns>
        public ActionResult get_exporta_ListaConPedido_excel()
        {
            JsonResponse objResult = new JsonResponse();
            try
            {
                Session[_session_ListarConsultarPedido_Excel] = null;
                string cadena = "";
                if (Session[_session_ListarConsultarPedido] != null)
                {

                    List<Ent_Consultar_Pedido> _ListarConsultarPedido = (List<Ent_Consultar_Pedido>)Session[_session_ListarConsultarPedido];
                    if (_ListarConsultarPedido.Count == 0)
                    {
                        objResult.Success = false;
                        objResult.Message = "No hay filas para exportar";

                    }
                    else
                    {
                        cadena = get_html_ListarConsultarPedido_str((List<Ent_Consultar_Pedido>)Session[_session_ListarConsultarPedido]);
                        if (cadena.Length == 0)
                        {
                            objResult.Success = false;
                            objResult.Message = "Error del formato html";
                        }
                        else
                        {
                            objResult.Success = true;
                            objResult.Message = "Se genero el excel correctamente";
                            Session[_session_ListarPedidoFacturacion_Excel] = cadena;
                        }
                    }
                }
                else
                {
                    objResult.Success = false;
                    objResult.Message = "No hay filas para exportar";
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

        /// <summary>
        /// Formato excel
        /// </summary>
        /// <param name="_ListarPedidoDespacho"></param>
        /// <param name="_Ent"></param>
        /// <returns></returns>
        public string get_html_ListarConsultarPedido_str(List<Ent_Consultar_Pedido> _ListarConsultarPedido)
        {
            StringBuilder sb = new StringBuilder();
            var Lista = _ListarConsultarPedido.ToList();
            var ClienteTitulo = _ListarConsultarPedido.Select(x => new { Cliente = x.Cliente,NroDNI = x.NroDNI }).Distinct();

            try
            {
                sb.Append("<div><table cellspacing='0' style='width: 1000px' rules='all' border='0' style='border-collapse:collapse;'><tr><td Colspan='13'></td></tr><tr><td Colspan='13' valign='middle' align='center' style='vertical-align: middle;font-size: 16.0pt;font-weight: bold;color:#285A8F'>HISTORIAL DE LOS PEDIDOS DEL CLIENTE '" + ClienteTitulo.ElementAt(0).Cliente.ToUpper() + "'</td></tr><tr><td Colspan='13' valign='middle' align='center' style='vertical-align: middle;font-size: 11.0pt;font-weight: bold;color:#000000'> DNI/RUC " + ClienteTitulo.ElementAt(0).NroDNI + "</td></tr></table>");
                sb.Append("<table  border='1' bgColor='#ffffff' borderColor='#FFFFFF' cellSpacing='2' cellPadding='2' style='font-size:10.0pt; font-family:Calibri; background:white;width: 1000px'><tr  bgColor='#5799bf'>\n");
                sb.Append("<tr bgColor='#1E77AB'>\n");
                sb.Append("<th style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Nro. Pedido</font></th>\n");
                sb.Append("<th style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Fec. Pedid</font></th>\n");
                sb.Append("<th style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Total</font></th>\n");
                sb.Append("<th style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Est. Pedido</font></th>\n");
                sb.Append("<th style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Nro. Liquidacion</font></th>\n");
                sb.Append("<th style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Fec. Liquidacion</font></th>\n");
                sb.Append("<th style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Nro. Doc</font></th>\n");
                sb.Append("<th style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Fec. Doc</font></th>\n");
                sb.Append("<th style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Nro. NC</font></th>\n");
                sb.Append("<th style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Fec. NC</font></th>\n");
                sb.Append("<th style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Est. NC</font></th>\n");
                
                sb.Append("</tr>\n");

                foreach (var item in Lista)
                {
                    sb.Append("<td align='Center'>" + item.NroPedido + "</td>\n");
                    sb.Append("<td align='Center'>" + item.FecPedido + "</td>\n");
                    sb.Append("<td>" + item.Total + "</td>\n");
                    sb.Append("<td>" + item.Estado + "</td>\n");
                    sb.Append("<td align='Center'>" + item.NroLiquidacion + "</td>\n");
                    sb.Append("<td align='Center'>" + item.FecLiquidacion + "</td>\n");
                    sb.Append("<td align='Center'>" + item.NroDoc + "</td>\n");
                    sb.Append("<td align='Center'>" + item.FecDoc + "</td>\n");
                    sb.Append("<td align='Center'>" + item.NroNC + "</td>\n");
                    sb.Append("<td align='Center'>" + item.FecNC + "</td>\n");
                    sb.Append("<td align='Right'>" + item.Stv_Description + "</td>\n");
                    sb.Append("</tr>\n");
                }
                sb.Append("</table></div>");
            }
            catch
            {

            }
            return sb.ToString();
        }

        /// <summary>
        /// Exportamos el excel
        /// </summary>
        /// <returns></returns>
        public ActionResult ListaConPedExcel()
        {
            string NombreArchivo = "HistorialPedido";
            String style = style = @"<style> .textmode { mso-number-format:\@; } </script> ";
            try
            {
                Response.Clear();
                Response.Buffer = true;
                Response.ContentType = "application/vnd.ms-excel";
                Response.AddHeader("Content-Disposition", "attachment;filename=" + NombreArchivo + ".xls");
                Response.Charset = "UTF-8";
                Response.ContentEncoding = Encoding.Default;
                Response.Write(style);
                Response.Write(Session[_session_ListarPedidoFacturacion_Excel].ToString());
                Response.End();
            }
            catch
            {

            }
            return Json(new { estado = 0, mensaje = 1 });
        }
        #endregion


        #region <MANIFIESTOS, Creación y modificación de manifiesto>
        /// <summary>
        /// Panle de manifiesto
        /// </summary>
        /// <returns></returns>
        public ActionResult PanelManifiesto() {

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
                    return View();
                }
                else
                {
                    return RedirectToAction("Error_Roles", "Home");
                } 
            }
        }
        /// <summary>
        /// lista de manifiestos
        /// </summary>
        /// <create>Juilliand R. Damian Gomez</create>
        /// <param name="param"></param>
        /// <param name="isOkUpdate"></param>
        /// <param name="FechaInicio"></param>
        /// <param name="FechaFin"></param>
        /// <param name="IdManifiesto"></param>
        /// <returns></returns>
        public JsonResult getListaManifiestospAjax(Ent_jQueryDataTableParams param, bool isOkUpdate, string FechaInicio, string FechaFin,int IdManifiesto)
        {
            Ent_Manifiesto_Pedidos Ent_Manifiesto = new Ent_Manifiesto_Pedidos();

            if (isOkUpdate)
            {
                Ent_Manifiesto.FechaInicio = DateTime.Parse(FechaInicio);
                Ent_Manifiesto.FechaFin = DateTime.Parse(FechaFin);
                Ent_Manifiesto.IdManifiesto = IdManifiesto;
                Session[_session_ListarManifiesto] = datPedido.ListarManifiesto(Ent_Manifiesto).ToList();
            }

            /*verificar si esta null*/
            if (Session[_session_ListarManifiesto] == null)
            {
                List<Ent_Manifiesto_Pedidos> _ListaManifiestos = new List<Ent_Manifiesto_Pedidos>();
                Session[_session_ListarManifiesto] = _ListaManifiestos;
            }

            IQueryable<Ent_Manifiesto_Pedidos> entDocTransEs = ((List<Ent_Manifiesto_Pedidos>)(Session[_session_ListarManifiesto])).AsQueryable();
            var entDocTrans = entDocTransEs.OrderByDescending(x => x.IdManifiesto).ToList();
            //Manejador de filtros
            int totalCount = entDocTrans.Count();
            IEnumerable<Ent_Manifiesto_Pedidos> filteredMembers = entDocTrans;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredMembers = entDocTrans.Where(
                        m =>
                         m.IdManifiesto.ToString().ToUpper().Contains(param.sSearch.ToUpper()) ||
                         m.Fecha_Manifiesto.ToString().ToUpper().Contains(param.sSearch.ToUpper()) ||
                         m.Est_Descripcion.ToString().ToUpper().Contains(param.sSearch.ToUpper()) 

                );
            }
            var sortIdx = Convert.ToInt32(Request["iSortCol_0"]);

            if (param.iSortingCols > 0)
            {
                if (Request["sSortDir_0"].ToString() == "desc")
                {
                    switch (sortIdx)
                    {
                        case 0: filteredMembers = filteredMembers.OrderBy(o => o.IdManifiesto); break;
                        case 1: filteredMembers = filteredMembers.OrderBy(o => o.Fecha_Manifiesto); break;
                        case 2: filteredMembers = filteredMembers.OrderBy(o => o.Est_Descripcion); break;
                    }
                }
                else
                {
                    switch (sortIdx)
                    {
                        case 0: filteredMembers = filteredMembers.OrderByDescending(o => o.IdManifiesto); break;
                        case 1: filteredMembers = filteredMembers.OrderByDescending(o => o.Fecha_Manifiesto); break;
                        case 2: filteredMembers = filteredMembers.OrderByDescending(o => o.Est_Descripcion); break;
                    }
                }
            }

            var Result = filteredMembers
                .Skip(param.iDisplayStart)
                .Take(param.iDisplayLength);

            //Se devuelven los resultados por json
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = totalCount,
                iTotalDisplayRecords = filteredMembers.Count(),
                aaData = Result
            }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Anula el manifiesto del panel principal
        /// </summary>
        /// <create>Juilliand R. Damian Gomez</create>        
        /// <param name="Id"></param>
        /// <returns></returns>
        public ActionResult AnularManifiesto(int Id)
        {
            bool Result = false;
            Ent_Usuario _usuario = (Ent_Usuario)Session[Ent_Constantes.NameSessionUser];
            JsonResponse objResult = new JsonResponse();
            Ent_Manifiesto_Pedidos _Ent = new Ent_Manifiesto_Pedidos();
            _Ent.IdUsuario = _usuario.usu_id;
            _Ent.IdManifiesto = Id;
            try
            {
                Result = datPedido.bAnularManifiesto(_Ent);
                if (Result)
                {
                    objResult.Success = true;
                    objResult.Message = "Se ANULO el manifiesto número " + Id;
                }
                else
                {
                    objResult.Success = false;
                    objResult.Message = "Hubo un problema, no se ANULO el manifiesto número " + Id;
                }
            }
            catch (Exception ex)
            {
                objResult.Success = false;
                objResult.Message = "Hubo un problema, no se ANULO el manifiesto número " + Id;
            }

            var JSON = JsonConvert.SerializeObject(objResult);

            return Json(JSON, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Vista donde se edita el manifiesto.
        /// </summary>
        /// <create>Juilliand R. Damian Gomez</create>        
        /// <param name="Id"></param>
        /// <returns></returns>
        public ActionResult EditarManifiesto(int Id)
        {
            Session[_session_ListarManifiestoEditar] = null;
            Ent_Manifiesto_Editar objModel = new Ent_Manifiesto_Editar();
            objModel.IdManifiesto = Id;
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
                Session[_session_ListarManifiestoEditar] = datPedido.ListarManifiestoEditar(objModel).ToList();
                //List<Ent_Manifiesto_Editar> ListarManifiesto = new List<Ent_Manifiesto_Editar>();
                ViewBag.ListarManifiesto = datPedido.ListarManifiestoEditar(objModel).ToList();
                Ent_Manifiesto_Editar Manifiesto = new Ent_Manifiesto_Editar();
                ViewBag.Manifiesto = Manifiesto;
                return View(objModel);
            }
        }
        /// <summary>
        /// Vista donde se crea un nuevo manifiesto.
        /// </summary>
        /// <create>Juilliand R. Damian Gomez</create>        
        /// <param name="Id"></param>
        /// <returns></returns>
        public ActionResult NuevoManifiesto()
        {
            Session[_session_ListarManifiestoEditar] = null;
            Ent_Manifiesto_Editar objModel = new Ent_Manifiesto_Editar();
            objModel.IdManifiesto = datPedido.Correlativo_Manifiesto();
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

                List<Ent_Manifiesto_Editar> _ListaManifiestos = new List<Ent_Manifiesto_Editar>();
                Session[_session_ListarManifiestoEditar] = _ListaManifiestos;

                Ent_Manifiesto_Editar Manifiesto = new Ent_Manifiesto_Editar();
                ViewBag.Manifiesto = Manifiesto;
                return View(objModel);
            }
        }
        /// <summary>
        /// Lista de los documento del manifieso.
        /// </summary>
        /// <create>Juilliand R. Damian Gomez</create>  
        /// <param name="param"></param>
        /// <param name="isOkUpdate"></param>
        /// <param name="IdManifiesto"></param>        
        /// <returns></returns>
        public JsonResult getListaManifiestosEditAjax(Ent_jQueryDataTableParams param, bool isOkUpdate, int IdManifiesto)
        {
            Ent_Manifiesto_Editar Ent_Manifiesto = new Ent_Manifiesto_Editar();

            //if (isOkUpdate)
            //{
            //    Ent_Manifiesto.IdManifiesto = IdManifiesto;
            //    Session[_session_ListarManifiestoEditar] = datPedido.ListarManifiestoEditar(Ent_Manifiesto).ToList();               
            //}

            /*verificar si esta null*/
            if (Session[_session_ListarManifiestoEditar] == null)
            {
                List<Ent_Manifiesto_Editar> _ListaManifiestos = new List<Ent_Manifiesto_Editar>();
                Session[_session_ListarManifiestoEditar] = _ListaManifiestos;
            }

            IQueryable<Ent_Manifiesto_Editar> entDocTransEs = ((List<Ent_Manifiesto_Editar>)(Session[_session_ListarManifiestoEditar])).AsQueryable();
            var entDocTrans = entDocTransEs.OrderByDescending(x => x.Items).ToList();
            //Manejador de filtros
            int totalCount = entDocTrans.Count();
            IEnumerable<Ent_Manifiesto_Editar> filteredMembers = entDocTrans;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredMembers = entDocTrans.Where(
                        m =>
                         m.Guia.ToUpper().Contains(param.sSearch.ToUpper()) ||
                         m.Doc.ToUpper().Contains(param.sSearch.ToUpper()) ||
                         m.Lider.ToUpper().Contains(param.sSearch.ToUpper())||
                         m.Promotor.ToUpper().Contains(param.sSearch.ToUpper()) 
                );
            }
            var sortIdx = Convert.ToInt32(Request["iSortCol_0"]);

            if (param.iSortingCols > 0)
            {
                if (Request["sSortDir_0"].ToString() == "asc")
                {
                    switch (sortIdx)
                    {
                        case 0: filteredMembers = filteredMembers.OrderBy(o => o.IdManifiesto); break;
                        case 1: filteredMembers = filteredMembers.OrderBy(o => o.Doc); break;
                        case 2: filteredMembers = filteredMembers.OrderBy(o => o.Lider); break;
                        case 3: filteredMembers = filteredMembers.OrderBy(o => o.Promotor); break;
                    }
                }
                else
                {
                    switch (sortIdx)
                    {
                        case 0: filteredMembers = filteredMembers.OrderByDescending(o => o.IdManifiesto); break;
                        case 1: filteredMembers = filteredMembers.OrderByDescending(o => o.Doc); break;
                        case 2: filteredMembers = filteredMembers.OrderByDescending(o => o.Lider); break;
                        case 3: filteredMembers = filteredMembers.OrderByDescending(o => o.Promotor); break;
                    }
                }
            }

            var Result = filteredMembers
                .Skip(param.iDisplayStart)
                .Take(param.iDisplayLength);

            //Se devuelven los resultados por json
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = totalCount,
                iTotalDisplayRecords = filteredMembers.Count(),
                aaData = Result
            }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Metodo que quita un documento del manifiesto
        /// <create>Juilliand R. Damian Gomez</create>  
        /// </summary>
        /// <param name="Doc"></param>
        /// <returns></returns>
        public ActionResult ManifiestosEditAnular(string Doc)
        {
            JsonResponse objResult = new JsonResponse();
            try
            {
                List<Ent_Manifiesto_Editar> ListManifiestoEditar = (List<Ent_Manifiesto_Editar>)Session[_session_ListarManifiestoEditar];
                int index = ListManifiestoEditar.FindIndex((Predicate<Ent_Manifiesto_Editar>)(item => item.Doc == Doc));
                ListManifiestoEditar.RemoveAt(index);
                Session[_session_ListarManifiestoEditar] = ListManifiestoEditar;

                objResult.Success = true;
                objResult.Message = "Se ANULO el docuemnto número : " + Doc;
            }
            catch (Exception ex)
            {
                objResult.Success = false;
                objResult.Message = "Se ANULO el docuemnto número : " + Doc;
            }

            var JSON = JsonConvert.SerializeObject(objResult);
            return Json(JSON, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Metodo que agrega a la lista un documento.
        /// </summary>
        /// <create>Juilliand R. Damian Gomez</create>          
        /// <param name="Doc"></param>
        /// <param name="isOk"></param>
        /// <param name="_Ent"></param>
        /// <returns></returns>
        public ActionResult ManifiestosEditAgregar(string Doc,bool isOk, Ent_Manifiesto_Editar _Ent)
        {
            JsonResponse objResult = new JsonResponse();
            Ent_Manifiesto_Editar entResult = new Ent_Manifiesto_Editar();
            Ent_Manifiesto_Editar entData = new Ent_Manifiesto_Editar();
            List<Ent_Manifiesto_Editar> ListManifiestoEditar = (List<Ent_Manifiesto_Editar>)Session[_session_ListarManifiestoEditar];
            var isExists = ListManifiestoEditar.Where(x => x.Doc == Doc).Count();
            entData.Doc = Doc;
            try
            {
                entData = datPedido.ManifiestoAgregarDoc(entData);
                if (isOk)
                {
                    if (isExists > 0)
                    {
                        objResult.Success = false;
                        objResult.Message = "El Documento " + Doc  + " ya existe en la lista";
                    }
                    else
                    {
                       if(entData.Estado == "0")
                        {
                        
                            entResult.Guia = entData.Guia;
                            entResult.Doc = entData.Doc;
                            entResult.Lider = entData.Lider;
                            entResult.Promotor = entData.Promotor;
                            entResult.Pares = entData.Pares;
                            entResult.Agencia = entData.Agencia;
                            entResult.Destino = entData.Destino;
                            entResult.Items = ListManifiestoEditar.Count() + 1;
                            ListManifiestoEditar.Add(entResult);
                            Session[_session_ListarManifiestoEditar] = ListManifiestoEditar;
                            objResult.Success = true;
                            objResult.Message = entData.Descripcion;

                        }
                        else if (entData.Estado == "1")
                        {
                            objResult.Success = false;
                            objResult.Message = entData.Descripcion;
                        }
                        else if (entData.Estado == "-1")
                        {
                            objResult.Success = false;
                            objResult.Message = entData.Descripcion;
                        }
                    }
                }
                else
                {
                    entResult.Guia = _Ent.Guia;
                    entResult.Doc = _Ent.Doc;
                    entResult.Lider = _Ent.Lider;
                    entResult.Promotor = _Ent.Promotor;
                    entResult.Pares = _Ent.Pares;
                    entResult.Agencia = _Ent.Agencia;
                    entResult.Destino = _Ent.Destino;
                    entResult.Items = _Ent.Items;
                    ListManifiestoEditar.Add(entResult);
                    Session[_session_ListarManifiestoEditar] = ListManifiestoEditar;
                    objResult.Success = true;
                    objResult.Message = "El numero de documento esta disponible...";
                }                   
            }
            catch (Exception ex)
            {
                objResult.Success = false;
                objResult.Message = "Error al agregar documento.";
            }
            var JSON = JsonConvert.SerializeObject(objResult);
            return Json(JSON, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Metodo donde se actualiza el manifiesto.
        /// </summary>
        /// <create>Juilliand R. Damian Gomez</create>  
        /// <param name="IdManifiesto"></param>
        /// <param name="Estado"></param>
        /// <returns></returns>
        public ActionResult ActualizarManifiesto(int IdManifiesto, string Estado)
        {
            JsonResponse objResult = new JsonResponse();
            List<Ent_Manifiesto_Editar> ListManifiestoEditar = (List<Ent_Manifiesto_Editar>)Session[_session_ListarManifiestoEditar];
            Ent_Usuario _usuario = (Ent_Usuario)Session[Ent_Constantes.NameSessionUser];
            Ent_Manifiesto_Editar _Ent = new Ent_Manifiesto_Editar();
            _Ent.IdManifiesto = IdManifiesto;
            _Ent.Estado = Estado;
            _Ent.IdUsuario = _usuario.usu_id;
            int _Id = 0;
            bool Result = false;

            DataTable dtManifiesto = new DataTable();
            dtManifiesto.Columns.Add("Man_Det_VenID", typeof(String));
            dtManifiesto.Columns.Add("Man_Det_Items", typeof(Decimal));
            dtManifiesto.Columns.Add("Man_Det_Lider", typeof(string));
            dtManifiesto.Columns.Add("Man_Det_Promotor", typeof(string));
            dtManifiesto.Columns.Add("Man_Det_Agencia", typeof(string));
            dtManifiesto.Columns.Add("Man_Det_Destino", typeof(string));
            try
            {

                foreach (Ent_Manifiesto_Editar item in ListManifiestoEditar)
                {
                    dtManifiesto.Rows.Add(
                        item.Doc,
                        item.Items,
                        item.Lider,
                        item.Promotor,
                        item.Agencia,
                        item.Destino);
                }

                Result = datPedido.RegistrarManifiesto(_Ent, dtManifiesto, ref _Id);
                if (Result==true)
                {
                    objResult.Success = true;
                    objResult.Message = "Se registro correctamente el manifiesto.";
                }
                else
                {
                    objResult.Success = false;
                    objResult.Message = "Hubo un problerma con la actualizacion, por favor consulte con sistemas";
                }

            }
            catch (Exception)
            {
                objResult.Success = false;
                objResult.Message = "Hubo un problerma con la actualizacion, por favor consulte con sistemas";
                throw;
            }
            
            var JSON = JsonConvert.SerializeObject(objResult);
            return Json(JSON, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Metodo donde se registra un nuevo manifiesto.
        /// </summary>
        /// <create>Juilliand R. Damian Gomez</create>  
        /// <param name="IdManifiesto"></param>
        /// <param name="Estado"></param>
        /// <returns></returns>
        public ActionResult RegistrarManifiesto(int IdManifiesto, string Estado)
        {
            JsonResponse objResult = new JsonResponse();
            List<Ent_Manifiesto_Editar> ListManifiestoEditar = (List<Ent_Manifiesto_Editar>)Session[_session_ListarManifiestoEditar];
            Ent_Usuario _usuario = (Ent_Usuario)Session[Ent_Constantes.NameSessionUser];
            Ent_Manifiesto_Editar _Ent = new Ent_Manifiesto_Editar();
            _Ent.IdManifiesto = IdManifiesto;
            _Ent.Estado = Estado;
            _Ent.IdUsuario = _usuario.usu_id;
            int _Id = 0;
            int _Val = 0;
            string Descripcion = "";
            bool Result = false;

            DataTable dtManifiesto = new DataTable();
            dtManifiesto.Columns.Add("Man_Det_VenID", typeof(String));
            dtManifiesto.Columns.Add("Man_Det_Items", typeof(Decimal));
            dtManifiesto.Columns.Add("Man_Det_Lider", typeof(string));
            dtManifiesto.Columns.Add("Man_Det_Promotor", typeof(string));
            dtManifiesto.Columns.Add("Man_Det_Agencia", typeof(string));
            dtManifiesto.Columns.Add("Man_Det_Destino", typeof(string));
            try
            {

                foreach (Ent_Manifiesto_Editar item in ListManifiestoEditar)
                {
                    dtManifiesto.Rows.Add(
                        item.Doc,
                        item.Items,
                        item.Lider,
                        item.Promotor,
                        item.Agencia,
                        item.Destino);
                }

                _Val = datPedido.Valida_Manifiesto(dtManifiesto, ref Descripcion);
                if (_Val==1)
                {
                    objResult.Success = false;
                    objResult.Message = Descripcion;
                }
                else
                {
                    Result = datPedido.RegistrarManifiesto(_Ent, dtManifiesto, ref _Id);
                    if (Result == true)
                    {
                        objResult.Success = true;
                        objResult.Message = "Se registro correctamente el manifiesto.";
                    }
                    else
                    {
                        objResult.Success = false;
                        objResult.Message = "Hubo un problerma al registrar el manifiesto, por favor consulte con sistemas";
                    }
                }
            }
            catch (Exception)
            {
                objResult.Success = false;
                objResult.Message = "Hubo un problerma al registrar el manifiesto, por favor consulte con sistemas";
                throw;
            }

            var JSON = JsonConvert.SerializeObject(objResult);
            return Json(JSON, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Metodo donde de se crear el archivo.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public ActionResult verManifiesto(int Id)
        {
            bool Result = false;
            JsonResponse objResult = new JsonResponse();
            try
            {
                Result = verManifiestoCrystalReport(Id);
                if (Result)
                {
                    objResult.Success = true;
                    objResult.Message = "Se generó correctamente el archivo";
                }
                else
                {
                    objResult.Success = false;
                    objResult.Message = "Error al generar el archivo";
                }
            }
            catch (Exception ex)
            {
                objResult.Success = false;
                objResult.Message = "Error al generar el archivo :" + ex.Message;
            }

            var JSON = JsonConvert.SerializeObject(objResult);

            return Json(JSON, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Metodo que carga la informacion al archivo de manifiesto.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public bool verManifiestoCrystalReport(int Id)
        {
            bool result = false;
            try
            {
                List<Manifiesto_Reports> _ventaValues = new List<Manifiesto_Reports>();
                Ent_Manifiesto_Editar _Ent = new Ent_Manifiesto_Editar();
                _Ent.IdManifiesto = Id;
                DataSet dsManInfo = datPedido.Reporte_Manifiesto(_Ent);

                if (dsManInfo == null)
                    return false;
                DataSet dsManDtl = new DataSet();
                dsManDtl.Tables.Add(dsManInfo.Tables[1].Copy());

                if (dsManDtl == null)
                    return false;

                DataRow dRow = dsManInfo.Tables[0].Rows[0];

                foreach (DataRow dRowDtl in dsManDtl.Tables[0].Rows)
                {
                    Decimal idman = Convert.ToDecimal(dRow["IdManifiesto"].ToString());
                    DateTime fechaman = Convert.ToDateTime(dRow["Man_Fecha"]);
                    string guiaman = dRowDtl["guia"].ToString();
                    string docman = dRowDtl["doc"].ToString();
                    string liderman = dRowDtl["lider"].ToString();
                    string promotorman = dRowDtl["promotor"].ToString();
                    Int32 paresman = Convert.ToInt32(dRowDtl["pares"]);
                    string agenciaman = dRowDtl["agencia"].ToString();
                    string destinoman = dRowDtl["destino"].ToString();
                    string tipo_despacho = dRowDtl["Desp_Des"].ToString();
                    _ventaValues.Add(new Manifiesto_Reports(idman, fechaman, guiaman, docman, liderman, promotorman, paresman, agenciaman, destinoman, tipo_despacho));
                }            
                this.HttpContext.Session["rptSourceMan"] = _ventaValues;
                this.HttpContext.Session["ReportNameMan"] = "manifiestoReport.rpt";
                result = true;
            }
            catch (Exception)
            {
                result = false;
            }
            return result;
        }

        #endregion

        #region<DESCARGA NOTA DE CREDITO>

        public ActionResult VerNotaCredito(string not_id)
        {
            try
            {
                GetCRNotaCredito(not_id, false);
                return Json(new { estado = 0 });
            }
            catch (Exception ex)
            {
                return Json(new { estado = 1 });
            }

        }

        private void GetCRNotaCredito(string _noReturn, bool download = true)
        {
            List<ReporteDevolucion> articlesReturnedValues;
            Data_Cr_Aquarella datCrAq = new Data_Cr_Aquarella();
            List<ReporteDevolucion> _devValsReport = new List<ReporteDevolucion>();

            DataSet ds_nc = datCrAq.getRetunrHdr(_noReturn);
            DataTable dtReturnHdr = new DataTable();
            dtReturnHdr = ds_nc.Tables[0].Copy();
            articlesReturnedValues = new List<ReporteDevolucion>();
            if (dtReturnHdr.Rows.Count > 0)
            {
                /// Crear el array list con el cual se le va a pasar toda la informacion al archivo de crystal
                

                //
                string idCoordinador = Convert.ToString(dtReturnHdr.Rows[0]["Not_BasId"]);
                //
                string coordEmail = Convert.ToString(dtReturnHdr.Rows[0]["Bas_Correo"]);
                //
                string nameCoord = Convert.ToString(dtReturnHdr.Rows[0]["nombres"]);
                //
                string coordDocument = Convert.ToString(dtReturnHdr.Rows[0]["Documento"]);
                //
                string coordAdress = Convert.ToString(dtReturnHdr.Rows[0]["Bas_Direccion"]);
                //
                string coordPhone = Convert.ToString(dtReturnHdr.Rows[0]["Bas_Telefono"]);
                //
                string coordUbication = Convert.ToString(dtReturnHdr.Rows[0]["ubicacion"]);
                //
                DateTime dateReturn = Convert.ToDateTime(dtReturnHdr.Rows[0]["Not_Fecha"]);
                //
                string rhvTransaction = "";// dtReturnHdr.Rows[0]["rhv_transaction"].ToString();
                //
                string rhnPerson = dtReturnHdr.Rows[0]["Not_BasId"].ToString();
                //
                string rhnEmployee = "";// dtReturnHdr.Rows[0]["rhn_employee"].ToString();
                //PERCEPCION
                Decimal percepcion = 0;//Convert.ToDecimal(dtReturnHdr.Rows[0]["percepcion"]);
                //
                Decimal porc_percepcion = 0;//Convert.ToDecimal(dtReturnHdr.Rows[0]["porc_percepcion"]);

                string _direccionfiscal = "";//dtReturnHdr.Rows[0]["direccionf"].ToString();

                // Id Bodega, para consultar la info de la bodega donde se genera la factura
                //string stvWarehouse = "";// dtReturnHdr.Rows[0]["stv_warehouse"].ToString();
                String mcpConcepto = dtReturnHdr.Rows[0]["Con_Descripcion"].ToString();
                String varNotaCredito = dtReturnHdr.Rows[0]["notaCredito"].ToString();

                decimal varSubTotal = Convert.ToDecimal(dtReturnHdr.Rows[0]["SUBTOTAL"]);
                decimal varIGV = Convert.ToDecimal(dtReturnHdr.Rows[0]["IGV"]);
                decimal varTotal = Convert.ToDecimal(dtReturnHdr.Rows[0]["TOTAL"]);

                ///
                //warehouses objW = new warehouses(_user._usv_co, stvWarehouse);
                //DataTable dtInfoWarehouse = objW.getWarehouseByPk();

                string wavDescription = "";
                string wavAddress = "";
                string wavPhone = "";
                string wavUbication = "";
                //
                //if (dtInfoWarehouse != null && dtInfoWarehouse.Rows.Count > 0)
                //{
                    ///
                wavDescription = dtReturnHdr.Rows[0]["Alm_Descripcion"].ToString().ToUpper();
                wavAddress = dtReturnHdr.Rows[0]["Alm_Direccion"].ToString();
                wavPhone = dtReturnHdr.Rows[0]["Alm_Telefono"].ToString();
                wavUbication = "";// dtReturnHdr.Rows[0]["ubication"].ToString();
                //}

                // Consultar los datos de la persona a quien se le facturo
                //DataTable dtInfoPersonInvoiced = Basic_Data.searchPerson(_user._usv_co, "-1", "", rhnPerson, "", "").Tables[0];

                string facturadoDestinatario = "";
                string facturadoUbicacion = "";
                string facturadoTelefono = "";

                ///
                //if (dtInfoPersonInvoiced != null && dtInfoPersonInvoiced.Rows.Count > 0)
                // {
                    ///
                facturadoDestinatario = dtReturnHdr.Rows[0]["nombres"].ToString();
                facturadoUbicacion = dtReturnHdr.Rows[0]["ubicacion"].ToString();
                facturadoTelefono = dtReturnHdr.Rows[0]["Bas_Telefono"].ToString();
                // }

                // DETALLE DE LA DEVOLUCION
                //DataTable dtReturnDtl = Returns_Dtl.getRetunrDtl(_user._usv_co, _noReturn);
                DataTable dtReturnDtl = new DataTable();
                dtReturnDtl = ds_nc.Tables[1].Copy();

                //
                foreach (DataRow drLine in dtReturnDtl.Rows)
                {
                    //
                    string rhvInvoice = drLine["ven_id"].ToString();
                    //
                    string article = drLine["Not_Det_ArtId"].ToString();
                    //
                    string articleName = drLine["articulo"].ToString();
                    //
                    string articleColor = drLine["color"].ToString();
                    //
                    string size = drLine["Not_Det_TalId"].ToString();
                    //
                    decimal qty = Convert.ToDecimal(drLine["Not_Det_Cantidad"]);
                    //
                    decimal sellPrice = Convert.ToDecimal(drLine["Not_Det_Precio"]);
                    //
                    decimal disscountLin = 0;// Convert.ToDecimal(drLine["rdn_disscount_lin"]);
                    //
                    decimal commision = Convert.ToDecimal(drLine["Not_Det_ComisionM"]);
                    //
                    decimal handling = 0;// Convert.ToDecimal(drLine["rdn_handling"]);
                    //
                    decimal disscountGen = 0;// Convert.ToDecimal(drLine["rdn_disscount_gen"]);
                    //
                    decimal taxes = Convert.ToDecimal(drLine["igvm"]);

                    DateTime FecEmisionFactura = Convert.ToDateTime(drLine["FecEmisionFactura"]);

                    //
                    ReporteDevolucion objAR = new ReporteDevolucion(idCoordinador, coordEmail, nameCoord,
                        coordDocument, coordAdress, coordPhone, coordUbication, facturadoDestinatario,
                        facturadoUbicacion, facturadoTelefono, wavDescription, wavAddress,
                        wavPhone, wavUbication, "1", _noReturn, dateReturn, rhvTransaction,
                        rhnPerson, rhnEmployee, rhvInvoice, article, articleName, articleColor, size, qty, sellPrice, disscountLin,
                        commision, handling, disscountGen, taxes, mcpConcepto, FecEmisionFactura, varNotaCredito, varSubTotal, varIGV, varTotal,percepcion,porc_percepcion,_direccionfiscal);

                    // Adicionar el nuevo objeto al arreglo
                    articlesReturnedValues.Add(objAR);
                }
            }
            this.HttpContext.Session["ReportName"] = "reportArticlesReturnedDev.rpt";
            this.HttpContext.Session["rptSource"] = articlesReturnedValues;            
            this.HttpContext.Session["rptDownload"] = download;
        }
        #endregion

        #region<REGION DE FLETE>

        public string _session_ListarPedidosFlete="_session_ListarPedidosFlete";


        public ActionResult GenerarFlete(List<Ent_Pedido_Flete> pedidos,string monto_flete,string bas_id)
        {
            bool Result = false;
            Ent_Usuario _usuario = (Ent_Usuario)Session[Ent_Constantes.NameSessionUser];
            JsonResponse objResult = new JsonResponse();
            Ent_Picking _Ent = new Ent_Picking();
            //_Ent.Liq_Id = Liq_Id;
            //_Ent.Usu_Id = _usuario.usu_id;
            string mensaje = "";
            try
            {
                string strDataDetalle = "";
                foreach(Ent_Pedido_Flete ped in pedidos)
                {
                    string strIdliq = ped.pedido;
                    strDataDetalle += "<row  ";
                    //strDataDetalle += " IdLider=¿" + strIdLider + "¿ ";
                    strDataDetalle += " IdLiquidacion =¿" + strIdliq + "¿ ";
                    strDataDetalle += " IdFlete=¿xxyy¿ ";
                    strDataDetalle += "/>";
                }

              
                if (strDataDetalle != "")
                {
                    string strmonto = monto_flete;
                    decimal monto = 0;

                    if (strmonto == "")
                        strmonto = "0";

                    monto = Convert.ToDecimal(strmonto);

                    if (monto > 0)
                    {
                        string pedido_flete = "";

                        string strliquidacion = datPedido.generar_liquidacion_flete(Convert.ToInt32(_usuario.usu_id), Convert.ToDecimal(bas_id), strDataDetalle, monto, ref pedido_flete);

                        if (strliquidacion.Length == 0)
                        {
                            objResult.Success = true;
                            objResult.Message = "Se genero el flete con el numero de pedido: " + pedido_flete;
                        }
                        else
                        {
                            objResult.Success = false;
                            objResult.Message = mensaje;
                        }



                    }
                }                
            }
            catch (Exception ex)
            {
                objResult.Success = false;
                //objResult.Message = ex.Message + "Pedido=" + Liq_Id;
            }

            var JSON = JsonConvert.SerializeObject(objResult);

            return Json(JSON, JsonRequestBehavior.AllowGet);
        }
        public JsonResult getListPedidoFleteAjax(Ent_jQueryDataTableParams param, bool isOkUpdate,string bas_id, string FechaInicio, string FechaFin)
        {
            Ent_Pedido_Flete EntFlete = new Ent_Pedido_Flete();
            if (isOkUpdate)
            {

                EntFlete.bas_id = decimal.Parse(bas_id);
                EntFlete.fec_ini = DateTime.Parse(FechaInicio);
                EntFlete.fec_fin = DateTime.Parse(FechaFin);

                List<Ent_Pedido_Flete> _ListarFlete = datPedido.listar_liquidacion_flete(EntFlete).ToList();
                Session[_session_ListarPedidosFlete] = _ListarFlete;
            }

            /*verificar si esta null*/
            if (Session[_session_ListarPedidosFlete] == null)
            {
                List<Ent_Pedido_Flete> _ListarFlete = new List<Ent_Pedido_Flete>();
                Session[_session_ListarPedidosFlete] = _ListarFlete;
            }

            IQueryable<Ent_Pedido_Flete> entDocTrans = ((List<Ent_Pedido_Flete>)(Session[_session_ListarPedidosFlete])).AsQueryable();
            //Manejador de filtros
            int totalCount = entDocTrans.Count();
            IEnumerable<Ent_Pedido_Flete> filteredMembers = entDocTrans;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredMembers = entDocTrans.Where(
                        m =>
                            m.pedido.ToUpper().Contains(param.sSearch.ToUpper())
                );
            }
            var sortIdx = Convert.ToInt32(Request["iSortCol_0"]);

            if (param.iSortingCols > 0)
            {
                if (Request["sSortDir_0"].ToString() == "asc")
                {
                    switch (sortIdx)
                    {
                        //case 0: filteredMembers = filteredMembers.OrderBy(o => o.Asesor); break;
                        //case 1: filteredMembers = filteredMembers.OrderBy(o => o.Lider); break;
                        //case 2: filteredMembers = filteredMembers.OrderBy(o => o.LiderDni); break;
                        //case 3: filteredMembers = filteredMembers.OrderBy(o => o.TotPares); break;
                        //case 4: filteredMembers = filteredMembers.OrderBy(o => o.TotVenta); break;
                        //case 5: filteredMembers = filteredMembers.OrderBy(o => o.PorComision); break;
                        //case 6: filteredMembers = filteredMembers.OrderBy(o => o.Comision); break;
                        //case 7: filteredMembers = filteredMembers.OrderBy(o => o.Bonosnuevas); break;
                        //case 8: filteredMembers = filteredMembers.OrderBy(o => o.SubTotalSinIGV); break;

                    }
                }
                else
                {
                    switch (sortIdx)
                    {
                        //case 0: filteredMembers = filteredMembers.OrderByDescending(o => o.Asesor); break;
                        //case 1: filteredMembers = filteredMembers.OrderByDescending(o => o.Lider); break;
                        //case 2: filteredMembers = filteredMembers.OrderByDescending(o => o.LiderDni); break;
                        //case 3: filteredMembers = filteredMembers.OrderByDescending(o => o.TotPares); break;
                        //case 4: filteredMembers = filteredMembers.OrderByDescending(o => o.TotVenta); break;
                        //case 5: filteredMembers = filteredMembers.OrderByDescending(o => o.PorComision); break;
                        //case 6: filteredMembers = filteredMembers.OrderByDescending(o => o.Comision); break;
                        //case 7: filteredMembers = filteredMembers.OrderByDescending(o => o.Bonosnuevas); break;
                        //case 8: filteredMembers = filteredMembers.OrderByDescending(o => o.SubTotalSinIGV); break;
                    }
                }
            }

            var Result = filteredMembers
                .Skip(param.iDisplayStart)
                .Take(param.iDisplayLength);

            //Se devuelven los resultados por json
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = totalCount,
                iTotalDisplayRecords = filteredMembers.Count(),
                aaData = Result
            }, JsonRequestBehavior.AllowGet);
        }
        #endregion
        public ActionResult envioInfoPasarelaPago(string nombresapellidos, string emailcontacto, string numerodocumento, string ubicacion, decimal? totalpago)
        {  
            return RedirectToAction("PasarelaPago", "PasarelaPago", new {
                nombresapellidos = nombresapellidos,
                emailcontacto = emailcontacto,
                numerodocumento = numerodocumento,
                ubicacion = ubicacion,
                totalpago = totalpago
            });

        }
    }
}