using CapaDato.PasarelaPago;
using CapaEntidad.Control;
using CapaEntidad.Financiera;
using CapaEntidad.Menu;
using CapaEntidad.PasarelaPago;
using CapaEntidad.Pedido;
using CapaEntidad.Persona;
using CapaEntidad.Util;
using CapaPresentacion.Api;
using CapaPresentacion.Bll;
using CapaPresentacion.Common;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace CapaPresentacion.Controllers
{
    public class PasarelaPagoController : Controller
    {
        // GET: ProcessPayment
        Dat_PasarelaPago pasarelaPagoDao = new Dat_PasarelaPago();
        public ActionResult PasarelaPago(string numerodocumento, decimal totalpago, string tipo_des, string referencia, 
            string agencia, string destino, string agencia_direccion, string liq_tipo_prov, string liq_provincia)
        {
            //if (_usuario.Equals(null)) { return RedirectToAction("Login", "Control", new { returnUrl = return_view }); }
            //else
            //{ 
            //    if (GetValidarPermisosXRol())
            //    {
            var _usuario = (Ent_Usuario)Session[Ent_Constantes.NameSessionUser];
            var actionName = this.ControllerContext.RouteData.GetRequiredString("action");
            var controllerName = this.ControllerContext.RouteData.GetRequiredString("controller");
            var return_view = actionName + "|" + controllerName;
            var token_access = ConfigurationManager.AppSettings[ConstantsCommon.TokenMercadoPago.PUBLIC_KEY_MERCADO_PAGO];

            ViewBag.numerodocumento = numerodocumento.Trim();
            ViewBag.tipo_des = tipo_des;
            ViewBag.referencia = referencia.Trim();
            ViewBag.agencia = agencia.Trim();
            ViewBag.destino = destino.Trim();
            ViewBag.agencia_direccion = agencia_direccion.Trim();

            var person = GetInformacionUsuario(numerodocumento.Trim());
            ViewBag.nombres = person.NombreCompleto.Trim().ToString();
            ViewBag.apellidos = person.ApellidoCompleto.Trim();
            ViewBag.nombreCompletoUsuario = person.NombreCompleto.Trim() + ' ' + person.ApellidoCompleto.Trim();
            ViewBag.direccion = person.Bas_Direccion.Trim();
            ViewBag.tipo_documento = person.TipoDocumento.Trim();
            ViewBag.celular = person.Bas_Celular.Trim();
            ViewBag.email = person.Bas_Correo.Trim();
            ViewBag.departamento = person.Departamento.Trim();
            ViewBag.provincia = person.Provincia.Trim();
            ViewBag.liq_tipo_prov = liq_tipo_prov;
            ViewBag.liq_provincia = liq_provincia;

            if (totalpago.Equals("") || totalpago.Equals(0) || totalpago.Equals(null))
                ViewBag.totalpago = 0;
            else
                ViewBag.totalpago = totalpago;
            ViewBag.mercadoPagoPublicKey = token_access;

            return View();
            //    }
            //    else
            //    {
            //        return RedirectToAction("Error_Roles", "Home");
            //    } 
            //} 
        }

        
           
        [HttpPost] 
        public ActionResult SetCardToken(string token_public, string numeroTarjeta, string codigoSeguridad, int fechaExpiracionMes, int fechaExpiracionAnio, string nombreCompletoTitular, string numeroDocumento, string tipoDocumento)
        {
            try
            {
                var _restServicesApi = new RestServicesApi();  
                var cardToken = new CardToken();
                cardToken.cardholder = new CardHolderDto();
                cardToken.cardholder.identification = new IndetificationDto();
                cardToken.token_public = token_public;
                cardToken.card_number = numeroTarjeta;
                cardToken.security_code = codigoSeguridad;
                cardToken.expiration_month = fechaExpiracionMes;
                cardToken.expiration_year = fechaExpiracionAnio;
                cardToken.cardholder.name = nombreCompletoTitular;
                cardToken.cardholder.identification.number = numeroDocumento;
                cardToken.cardholder.identification.type = tipoDocumento;
                
                var response = _restServicesApi.PostInvoque<CardToken, CardTokenResponseDto>(cardToken,
                    ConfigurationManager.AppSettings[ConstantsCommon.EndPointCatalogoPago.EndPointCatalogoPagoProccessCardToken], Settings.ACCESS_TOKEN_API_CATALOGO_PAGO, "POST", "CardToken"); 
                return Json(new { data = response.response.Data.id }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { estado = "-1", mensaje = "Error de comunicación con el método SetCardToken; " + ex.Message });
            }
          
        }

        public Ent_Persona GetInformacionUsuario(string dni)
        {
            try
            { 
                var info = pasarelaPagoDao.GetInformacionUsuario(dni);
                return info;
            }
            catch (Exception e)
            { 
                throw e;
            }
        }

        [HttpGet]
        public ActionResult GetMasterStatus(string key)
        {
            try
            { 
               var response = pasarelaPagoDao.ListaEstadoMercadoPago(key);
               return Json(new { data = response }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            { 
                throw e;
            }
        }

        [HttpPost]
        public ActionResult PostProccessPayment(string nombreTitular, string apellidoTitular, string zipCode, string street_name, string state_name, string city_name, int installments, string tipoDocumento,
            string numeroDocumento, string payment_method_id, string tokenCard, decimal importeTotal, string emailCliente, string numerocelular, string external_reference , List<Ent_Order_Dtl> listProducts)
        {
            var jsonResponse = new JsonResponse();
            try
            { 
                var _restServicesApi = new RestServicesApi();
                var request = new PaymentRequest();
                var metadaData = new MetadataDto();
                request.additional_info = new AdditionalInfoPaymentDto();
                request.additional_info.payer = new PayerMercadoPagoDto();
                request.additional_info.payer.address = new AddressDto();
                request.additional_info.shipments = new ReceiverAddressDto();
                request.additional_info.shipments.receiver_address = new Receiver_AddressDto();
                request.payer = new PayerRequestDto();
                request.payer.identification = new PayerIdentificationDto();
                request.additional_info.items = new List<AdditionalInfoDto>();
                AdditionalInfoDto item;
                request.additional_info.payer.phone = new PhoneDto();
                request.metadata = new MetadataDto();
                
                foreach (var products in listProducts)
                {
                    item = new AdditionalInfoDto
                    {
                        id = products._code,
                        title = products._artName,
                        description = "Color del producto: " + products._color + "Talla del producto: " + products._size,
                        picture_url = "",
                        category_id = "",
                        quantity = products._qty,
                        unit_price = products._lineTotal
                    };

                    request.additional_info.items.Add(item);
                }

                request.additional_info.payer.first_name = nombreTitular;
                request.additional_info.payer.last_name = apellidoTitular;

                request.additional_info.payer.phone.area_code = "+51";
                request.additional_info.payer.phone.number = numerocelular.ToString();

                request.additional_info.payer.address.zip_code = zipCode;
                request.additional_info.payer.address.street_name = street_name;
                request.additional_info.payer.address.street_number = "";
                request.metadata = metadaData;
                request.additional_info.shipments.receiver_address.zip_code = zipCode;
                request.additional_info.shipments.receiver_address.state_name = state_name;
                request.additional_info.shipments.receiver_address.city_name = city_name;
                request.additional_info.shipments.receiver_address.street_name = street_name;
                request.additional_info.shipments.receiver_address.street_number = "";
                request.binary_mode = true;
                request.capture = true;
                request.external_reference = external_reference.ToString();
                request.installments = installments;
                request.notification_url = "https://prueba.com";
                request.payer.email = emailCliente;
                request.payer.identification.type = tipoDocumento;
                request.payer.identification.number = numeroDocumento;
                request.payer.first_name = nombreTitular;
                request.payer.last_name = apellidoTitular;
                request.payment_method_id = payment_method_id;
                request.token = tokenCard;
                request.transaction_amount = importeTotal;

                var response = _restServicesApi.PostInvoque<PaymentRequest, ResponsePasarelaPago>(
                    request, ConfigurationManager.AppSettings[ConstantsCommon.EndPointCatalogoPago.EndPointCatalogoPagoProccessPayment], Settings.ACCESS_TOKEN_API_CATALOGO_PAGO, "POST", "ProccessPayment");

                var payment = new PaymentResponse()
                {
                    id = response.response.Data.id,
                    status = response.response.Data.status
                };

                jsonResponse.Status = response.response.Status.ToString();
                jsonResponse.Message = response.response.Message.ToString();
                jsonResponse.Success = true;
                jsonResponse.Data = payment;
            }
            catch (Exception e)
            {
                jsonResponse.Status = "0";
                jsonResponse.Message = e.Message;
                jsonResponse.Success = false;
                return Json(new { Status = jsonResponse.Status, Message = jsonResponse.Message , Success = jsonResponse.Success }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { Status = jsonResponse.Status, Message = jsonResponse.Message, Success = jsonResponse.Success , Data = jsonResponse.Data }, JsonRequestBehavior.AllowGet);
        }

 

        #region modal card information
        public ActionResult CardInformationCvv()
        {
            return View();
        }
        #endregion
        
        #region method private 
        private bool GetValidarPermisosXRol()
        {
            var valida_rol = true;
            var valida_controller = new Basico();
            var menu = (List<Ent_Menu_Items>)Session[Ent_Global._session_menu_user];
            valida_rol = valida_controller.AccesoMenu(menu, this);
            return valida_rol;
        }
         
        #endregion

    }
}