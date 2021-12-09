using CapaEntidad.PasarelaPago;
using CapaPresentacion.Common;
using System.Configuration;

namespace CapaPresentacion.Api
{
    public class AuthServicesApi
    {
        public static AuthDto AuthTokenCatalogoPago(Authetication authDto)
        {
            var response = new AuthDto();
            var _restServicesApi = new RestServicesApi();
            var auth = new Authetication
            {
                password = authDto.password.ToString(),
                username = authDto.username.ToString(),
                usuarioWeb = authDto.usuarioWeb.Trim().ToString(),
            };
            var response_token = _restServicesApi.Authentication<Authetication, ResponseDto>(
                auth, ConfigurationManager.AppSettings[ConstantsCommon.EndPointCatalogoPago.EndPointCatalogoPagoAuth], null, "POST", "Auth");

            if (!string.IsNullOrEmpty(response_token.ToString()))
            {
                if (!string.IsNullOrEmpty(response_token.response.Data))
                {
                    Settings.ACCESS_TOKEN_API_CATALOGO_PAGO = response_token.response.Data;
                }
            }
            else
            {
                response.Estado = "-1";
                response.Mensaje = "Error de comunicación con el método SetCardToken";  
            }
            return response;
        }
    }
}