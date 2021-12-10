using CapaDato.Financiera;
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

        public ActionResult PasarelaPago2(string numerodocumento, decimal totalpago, string tipo_des, string referencia,
            string agencia, string destino, string agencia_direccion, string liq_tipo_prov, string liq_provincia)
        {
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
        }

        public ActionResult PasarelaPago(string numerodocumento, decimal totalpago, string tipo_des, string referencia,
            string agencia, string destino, string agencia_direccion, string liq_tipo_prov, string liq_provincia)
        {
            try
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

                #region<Obteniendo el token> 
                var usuario = (Ent_Usuario)Session[Ent_Constantes.NameSessionUser];
                var auth = new Authetication
                {
                    password = ConfigurationManager.AppSettings[ConstantsCommon.Credentials_CatalogoPago.Password],
                    username = ConfigurationManager.AppSettings[ConstantsCommon.Credentials_CatalogoPago.User],
                    usuarioWeb = usuario.usu_login,
                };
                var token = AuthServicesApi.AuthTokenCatalogoPago(auth);
                #endregion

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
            catch (Exception ex)
            { 
                throw ex;
            }
          
        }



        [HttpPost]
        public ActionResult SetCardToken(string token_public, string numeroTarjeta, string codigoSeguridad, int? fechaExpiracionMes, int? fechaExpiracionAnio, string nombreCompletoTitular, string numeroDocumento, string tipoDocumento,
            string os_device, string system_version, int? ram, int? disk_space, string model, int? free_disk_space, bool? feature_flash, bool? can_make_phone_calls, bool? can_send_sms, bool? video_camera_available, int? cpu_count, bool? simulator,
               string device_languaje, string device_idiom, string platform, string device_name, int? device_family, bool? retina_display_capable, bool? feature_camera, string device_model, bool feature_front_camera, string resolution, List<VendorId> vendors = null)
        {
            try
            {
                Ent_Usuario _usuario = (Ent_Usuario)Session[Ent_Constantes.NameSessionUser];



                var _restServicesApi = new RestServicesApi();


                #region<Obteniendo el token>
                var auth = new Authetication
                {
                    password = ConfigurationManager.AppSettings[ConstantsCommon.Credentials_CatalogoPago.Password],
                    username = ConfigurationManager.AppSettings[ConstantsCommon.Credentials_CatalogoPago.User],
                    usuarioWeb = _usuario.usu_login,
                };
                var response_token = _restServicesApi.Authentication<Authetication, ResponseDto>(
                    auth, ConfigurationManager.AppSettings[ConstantsCommon.EndPointCatalogoPago.EndPointCatalogoPagoAuth], null, "POST", "Auth");

                if (response_token != null)
                {
                    if (!string.IsNullOrEmpty(response_token.response.Data))
                    {
                        Settings.ACCESS_TOKEN_API_CATALOGO_PAGO = response_token.response.Data;
                    }
                }
                else
                {
                    return Json(new { estado = "-1", Message = "Error de comunicación con el método SetCardToken" });
                }
                #endregion

                var card = new CardDto(); 
                var vendorobj = new VendorId();
                card.vendor_ids = new List<VendorId>();
                card.TokenPublic = token_public;
                card.numeroTarjeta = numeroTarjeta;
                card.codigoSeguridad = codigoSeguridad;
                card.fechaExpiracionMes = fechaExpiracionMes;
                card.fechaExpiracionAnio = fechaExpiracionAnio;
                card.nombreCompletoTitular = nombreCompletoTitular;
                card.numeroDocumento = numeroDocumento;
                card.tipoDocumento = tipoDocumento;
                card.os_device = os_device;
                card.system_version = system_version;
                card.ram = ram;
                card.disk_space = disk_space;
                card.model = model;
                card.free_disk_space = free_disk_space;
                card.feature_flash = feature_flash;
                card.can_make_phone_calls = can_make_phone_calls;
                card.can_send_sms = can_send_sms;
                card.video_camera_available = video_camera_available;
                card.cpu_count = cpu_count;
                card.simulator = simulator;
                card.device_languaje = device_languaje;
                card.device_idiom = device_idiom;
                card.platform = platform;
                card.device_name = device_name;
                card.device_family = device_family;
                card.retina_display_capable = retina_display_capable;
                card.feature_camera = feature_camera;
                card.device_model = device_model;
                card.feature_front_camera = feature_front_camera;
                card.resolution = resolution;

                foreach (var item in vendors)
                {
                    vendorobj.name = item.name;
                    vendorobj.value = item.value;
                    card.vendor_ids.Add(vendorobj);
                } 

                var cardToken = setClientCard(card);
                var response = _restServicesApi.PostInvoque<CardToken, CardTokenResponseDto>(cardToken,
                    ConfigurationManager.AppSettings[ConstantsCommon.EndPointCatalogoPago.EndPointCatalogoPagoProccessCardToken], Settings.ACCESS_TOKEN_API_CATALOGO_PAGO, ConstantsCommon.TypeRequest.POST, ConstantsCommon.Method.CARDTOKEN);

                if (response == null)
                {
                    return Json(new { estado = ConstantsCommon.CodeType.Error, Message = ConstantsCommon.Resources.ErrorPostInvoque });
                }
                else
                {
                    if (response.response.Data == null)
                    {
                        return Json(new { Status = response.response.Status, Message = ConstantsCommon.Resources.ErrorTarjetaInvalida }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {

                        if (response.response.Data.last_four_digits == null)
                        {
                            return Json(new { Status = response.response.Status, LastFourDigits = response.response.Data.last_four_digits, Message = ConstantsCommon.Resources.ErrorTarjetaInvalida, data = response.response.Data.id }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            return Json(new { Status = response.response.Status, LastFourDigits = response.response.Data.last_four_digits, Message = response.response.Message, data = response.response.Data.id }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { estado = ConstantsCommon.CodeType.Error, Message = ConstantsCommon.Resources.ErrorSetCardToken + ex.Message });
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
            string numeroDocumento, string payment_method_id, string tokenCard, decimal importeTotal, string emailCliente, string numerocelular, string external_reference, List<Ent_Order_Dtl> listProducts, string street_number)
        {
            var jsonResponse = new JsonResponse();
            var mensaje_premio = "";
            try
            {
                var _restServicesApi = new RestServicesApi();
                var request = new PaymentRequest();
                var metadaData = new MetadataDto();
                var response = new Root();
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

                if (installments.Equals("") || installments.Equals(null)) installments = 1;

                foreach (var products in listProducts)
                {
                    item = new AdditionalInfoDto
                    {
                        id = products._code,
                        title = products._artName,
                        description = "Color del producto:" + " " + products._color + " " + "Talla del producto:" + " " + products._size,
                        picture_url = "",
                        category_id = "",
                        quantity = products._qty,
                        unit_price = products._lineTotal
                    };

                    request.additional_info.items.Add(item);
                }

                request.additional_info.payer.first_name = nombreTitular;
                request.additional_info.payer.last_name = apellidoTitular; 
                request.additional_info.payer.phone.area_code = ConstantsCommon.Resources.AREA_CODE_PE;
                request.additional_info.payer.phone.number = numerocelular.ToString(); 
                request.additional_info.payer.address.zip_code = zipCode;
                request.additional_info.payer.address.street_name = street_name;
                request.additional_info.payer.address.street_number = street_number;
                request.metadata = metadaData;
                request.additional_info.shipments.receiver_address.zip_code = zipCode;
                request.additional_info.shipments.receiver_address.state_name = state_name;
                request.additional_info.shipments.receiver_address.city_name = city_name;
                request.additional_info.shipments.receiver_address.street_name = street_name;
                request.additional_info.shipments.receiver_address.street_number = street_number;
                request.binary_mode = true;
                request.capture = true;
                request.external_reference = external_reference.ToString();
                request.installments = installments;
                request.notification_url = "https://posperu.bgr.pe/Ws_Multivende/Api/ApiBataPos/Update_Data_Bata";
                request.payer.email = emailCliente;
                request.payer.identification.type = tipoDocumento;
                request.payer.identification.number = numeroDocumento;
                request.payer.first_name = nombreTitular;
                request.payer.last_name = apellidoTitular;
                request.payment_method_id = payment_method_id;
                request.token = tokenCard;
                request.transaction_amount = importeTotal;

                response = _restServicesApi.PostInvoque<PaymentRequest, Root>(
                    request, ConfigurationManager.AppSettings[ConstantsCommon.EndPointCatalogoPago.EndPointCatalogoPagoProccessPayment], Settings.ACCESS_TOKEN_API_CATALOGO_PAGO, ConstantsCommon.TypeRequest.POST, ConstantsCommon.Method.PROCCESS_PAYMENT);

                var payment = new DataRoot()
                {
                    id = response.response.Data.id,
                    status = response.response.Data.status
                }; 

                jsonResponse.Status = response.response.Status.ToString();
                jsonResponse.Message = response.response.Message.ToString();
                jsonResponse.Success = true;
                jsonResponse.Data = payment;
                 
                #region<Si el pago se realizo correctamente entonces cambiamos los estados>  
                var _usuario = (Ent_Usuario)Session[Ent_Constantes.NameSessionUser];
                var cruce_pago = new Dat_Pago();
                mensaje_premio = cruce_pago.registra_pago_mercado_pago(external_reference, payment.id.ToString(), importeTotal, _usuario.usu_id, jsonResponse.Status);
                #endregion

                return Json(new { Status = jsonResponse.Status, Message = jsonResponse.Message, Success = jsonResponse.Success, Data = jsonResponse.Data, Mensaje_Premio = mensaje_premio }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                jsonResponse.Status = "0";
                jsonResponse.Message = e.Message;
                jsonResponse.Success = false;
                return Json(new { Status = jsonResponse.Status, Message = jsonResponse.Message, Success = jsonResponse.Success }, JsonRequestBehavior.AllowGet);
            } 
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


        private CardToken setClientCard(CardDto card)
        {
            var cardToken = new CardToken();
            cardToken.cardholder = new CardHolderDto();
            cardToken.cardholder.identification = new IndetificationDto();
            cardToken.device = new Device();
            cardToken.device.fingerprint = new Fingerprint();
            cardToken.device.fingerprint.vendor_ids = new List<VendorId>();
            var vendor = new VendorId();
            cardToken.device.fingerprint.vendor_specific_attributes = new VendorSpecificAttributes();
            //card.vendor_ids = new List<VendorId>();

            cardToken.token_public = card.TokenPublic;
            cardToken.card_number = card.numeroTarjeta;
            cardToken.security_code = card.codigoSeguridad;
            cardToken.expiration_month = card.fechaExpiracionMes;
            cardToken.expiration_year = card.fechaExpiracionAnio;
            /*CAMBIAR NOMBRE PARA PRD*/
            cardToken.cardholder.name = "APRO";
            cardToken.cardholder.identification.number = card.numeroDocumento;
            cardToken.cardholder.identification.type = card.tipoDocumento;

            cardToken.device.fingerprint.os = card.os_device;
            cardToken.device.fingerprint.system_version = card.system_version;
            cardToken.device.fingerprint.ram = card.ram;
            cardToken.device.fingerprint.disk_space = card.disk_space;

            foreach (var item in card.vendor_ids)
            {
                vendor.name = string.Empty;
                vendor.value = string.Empty;
                cardToken.device.fingerprint.vendor_ids.Add(vendor);
            }

            cardToken.device.fingerprint.model = card.model;
            cardToken.device.fingerprint.free_disk_space = card.free_disk_space;

            cardToken.device.fingerprint.vendor_specific_attributes.feature_flash = card.feature_flash;
            cardToken.device.fingerprint.vendor_specific_attributes.can_make_phone_calls = card.can_make_phone_calls;
            cardToken.device.fingerprint.vendor_specific_attributes.can_send_sms = card.can_send_sms;
            cardToken.device.fingerprint.vendor_specific_attributes.video_camera_available = card.video_camera_available;
            cardToken.device.fingerprint.vendor_specific_attributes.cpu_count = card.cpu_count;
            cardToken.device.fingerprint.vendor_specific_attributes.simulator = card.simulator;
            cardToken.device.fingerprint.vendor_specific_attributes.device_languaje = card.device_languaje;
            cardToken.device.fingerprint.vendor_specific_attributes.device_idiom = card.device_idiom;
            cardToken.device.fingerprint.vendor_specific_attributes.platform = card.platform;
            cardToken.device.fingerprint.vendor_specific_attributes.device_name = card.device_name;
            cardToken.device.fingerprint.vendor_specific_attributes.device_family = card.device_family;
            cardToken.device.fingerprint.vendor_specific_attributes.retina_display_capable = card.retina_display_capable;
            cardToken.device.fingerprint.vendor_specific_attributes.feature_camera = card.feature_camera;
            cardToken.device.fingerprint.vendor_specific_attributes.device_model = card.device_model;
            cardToken.device.fingerprint.vendor_specific_attributes.feature_front_camera = card.feature_front_camera;
            cardToken.device.fingerprint.resolution = card.resolution;
 
            return cardToken;
        }
        #endregion

    }
}